using NUnit.Framework;
using UnityEngine;

public class SplineTests
{
    private GameObject _root;
    private SplinePath _path;

    [SetUp]
    public void SetUp()
    {
        _root = new GameObject("Spline fixture");
        _path = _root.AddComponent<SplinePath>();
        Vector3[] points = { new Vector3(4, 3, 7), new Vector3(3, 3, 23), new Vector3(21.5f, 3, 26.5f),
            new Vector3(26.5f, 3, 17.5f), new Vector3(31.5f, 3, 8.5f), new Vector3(39, 3, 2.5f), new Vector3(43.5f, 3, 24) };
        _path.points = new Transform[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            _path.points[i] = new GameObject("P" + i).transform;
            _path.points[i].SetParent(_root.transform);
            _path.points[i].position = points[i];
        }
        _path.BuildDistanceTable();
    }

    [TearDown]
    public void TearDown() => UnityEngine.Object.DestroyImmediate(_root);

    [Test]
    public void SegmentCount_CountsCubicSegments()
    {
        Assert.That(_path.SegmentCount, Is.EqualTo(2));
    }

    [Test]
    public void SamplePoint_LandsOnSharedJoins()
    {
        Assert.That(_path.SamplePoint(0), Is.EqualTo(_path.points[0].position));
        Assert.That(_path.SamplePoint(1), Is.EqualTo(_path.points[3].position));
        Assert.That(_path.SamplePoint(2), Is.EqualTo(_path.points[6].position));
    }

    [Test]
    public void SamplePoint_UsesSegmentIndexPlusLocalT()
    {
        Transform[] p = _path.points;
        Vector3 point = CubicBezierMath.SamplePoint(p[3].position, p[4].position, p[5].position, p[6].position, 0.25f);
        Assert.That(Vector3.Distance(_path.SamplePoint(1.25f), point), Is.LessThan(0.0001f));
    }

    [Test]
    public void SamplePoint_FollowsAMovedControlPoint()
    {
        _path.points[0].position += Vector3.up * 5f;
        Assert.That(_path.SamplePoint(0f), Is.EqualTo(_path.points[0].position));
    }

    [Test]
    public void SamplePoint_EqualStepsInUGiveUnequalWorldSteps()
    {
        float shortest = float.MaxValue;
        float longest = 0f;
        Vector3 previous = _path.SamplePoint(0);
        for (int i = 1; i <= 20; i++)
        {
            Vector3 point = _path.SamplePoint(i / 10f);
            float stepLength = Vector3.Distance(previous, point);
            shortest = Mathf.Min(shortest, stepLength);
            longest = Mathf.Max(longest, stepLength);
            previous = point;
        }
        Assert.That(longest / shortest, Is.GreaterThan(1.5f));
    }

    [Test]
    public void SampleTangent_UsesSegmentIndexPlusLocalT()
    {
        Transform[] p = _path.points;
        Vector3 tangent = CubicBezierMath.SampleTangent(p[3].position, p[4].position, p[5].position, p[6].position, 0.25f);
        Assert.That(Vector3.Distance(_path.SampleTangent(1.25f), tangent), Is.LessThan(0.0001f));
    }

    [Test]
    public void SampleTangent_MatchesTheEndOfThePath()
    {
        Transform[] p = _path.points;
        Vector3 tangent = CubicBezierMath.SampleTangent(p[3].position, p[4].position, p[5].position, p[6].position, 1f);
        Assert.That(Vector3.Distance(_path.SampleTangent(2f), tangent), Is.LessThan(0.0001f));
    }

    [Test]
    public void TotalLength_LiesBetweenStraightLineAndControlPolygon()
    {
        float polygon = 0f;
        for (int i = 1; i < _path.points.Length; i++)
            polygon += Vector3.Distance(_path.points[i - 1].position, _path.points[i].position);
        float straight = Vector3.Distance(_path.points[0].position, _path.points[6].position);
        Assert.That(_path.TotalLength, Is.GreaterThan(straight));
        Assert.That(_path.TotalLength, Is.LessThan(polygon));
    }

    [Test]
    public void BuildDistanceTable_KeepsTheMeasuredLengthUntilRebuilt()
    {
        float length = _path.TotalLength;
        _path.points[0].position += Vector3.up * 5f;
        Assert.That(_path.TotalLength, Is.EqualTo(length));
        _path.BuildDistanceTable();
        Assert.That(_path.TotalLength, Is.Not.EqualTo(length));
    }

    [Test]
    public void ParameterAtDistance_MapsTheStartAndEnd()
    {
        Assert.That(_path.ParameterAtDistance(0), Is.EqualTo(0));
        Assert.That(_path.ParameterAtDistance(_path.TotalLength), Is.EqualTo(2).Within(0.0001f));
    }

    [Test]
    public void ParameterAtDistance_IncreasesAlongThePath()
    {
        float previous = 0f;
        for (int i = 1; i <= 20; i++)
        {
            float u = _path.ParameterAtDistance(_path.TotalLength * i / 20f);
            Assert.That(u, Is.GreaterThan(previous));
            previous = u;
        }
    }

    [Test]
    public void ParameterAtDistance_SpacesPointsEvenly()
    {
        Assert.That(_path.TotalLength, Is.GreaterThan(0f));
        // Short steps, so each chord is close to the arc it spans.
        float step = _path.TotalLength / 100f;
        Vector3 previous = _path.SamplePoint(0);
        for (int i = 1; i <= 100; i++)
        {
            Vector3 point = _path.SamplePoint(_path.ParameterAtDistance(step * i));
            Assert.That(Vector3.Distance(previous, point), Is.EqualTo(step).Within(step * 0.02f));
            previous = point;
        }
    }
}
