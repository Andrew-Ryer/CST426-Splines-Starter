using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class CameraFlyThruTests
{
    float _captureDeltaTime;
    SplineFollow _follow;

    [SetUp]
    public void SetUp()
    {
        _captureDeltaTime = Time.captureDeltaTime;
        Time.captureDeltaTime = 1f / 60f;
    }

    [TearDown]
    public void TearDown() => Time.captureDeltaTime = _captureDeltaTime;

    IEnumerator LoadFlyThru()
    {
        yield return SceneManager.LoadSceneAsync("Camera Fly-Thru");
        yield return null;
        _follow = Object.FindAnyObjectByType<SplineFollow>();
    }

    [UnityTest]
    public IEnumerator Scene_WiresTheCameraToThePath()
    {
        yield return LoadFlyThru();
        Assert.That(_follow, Is.Not.Null);
        Assert.That(_follow.path.SegmentCount, Is.EqualTo(3));
        Assert.That(_follow.CompareTag("MainCamera"), Is.True);
        Assert.That(Object.FindAnyObjectByType<CameraFlyThru>().movement, Is.SameAs(_follow));
        Assert.That(Object.FindObjectsByType<Camera>(), Has.Length.EqualTo(1));
        Assert.That(_follow.target.IsChildOf(GameObject.Find("Stanford Bunny").transform), Is.True);
        LogAssert.NoUnexpectedReceived();
    }

    [UnityTest]
    public IEnumerator DistanceTravel_MovesAlongThePath()
    {
        yield return LoadFlyThru();
        _follow.travelByDistance = true;
        Vector3 start = _follow.transform.position;
        yield return new WaitForSeconds(0.25f);
        Assert.That(Vector3.Distance(_follow.transform.position, start), Is.GreaterThan(0.1f));
        LogAssert.NoUnexpectedReceived();
    }

    [UnityTest]
    public IEnumerator DistanceTravel_StopsAtTheEnd()
    {
        yield return LoadFlyThru();
        _follow.travelByDistance = true;
        _follow.speed = 1000f;
        yield return new WaitForSeconds(0.25f);
        Vector3 end = _follow.path.points[_follow.path.points.Length - 1].position;
        Assert.That(Vector3.Distance(_follow.transform.position, end), Is.LessThan(0.001f));
        yield return null;
        Assert.That(Vector3.Distance(_follow.transform.position, end), Is.LessThan(0.001f));
        LogAssert.NoUnexpectedReceived();
    }

    [UnityTest]
    public IEnumerator Replay_ReturnsToTheStart()
    {
        yield return LoadFlyThru();
        _follow.travelByDistance = true;
        _follow.speed = 1000f;
        yield return new WaitForSeconds(0.25f);
        Vector3 end = _follow.path.points[_follow.path.points.Length - 1].position;
        Assert.That(Vector3.Distance(_follow.transform.position, end), Is.LessThan(0.001f),
            "Replay is checked after distance travel has reached the end.");
        _follow.speed = 0f;
        _follow.Restart();
        yield return null;
        Assert.That(Vector3.Distance(_follow.transform.position, _follow.path.points[0].position), Is.LessThan(0.001f));
        LogAssert.NoUnexpectedReceived();
    }

    [UnityTest]
    public IEnumerator EqualUTravel_ReachesTheQuarterPoint()
    {
        yield return LoadFlyThru();
        Assert.That(_follow.path.TotalLength, Is.GreaterThan(0f));
        _follow.travelByDistance = false;
        _follow.speed = _follow.path.TotalLength / 2f;
        _follow.Restart();
        for (int i = 0; i < 30; i++)
            yield return null;

        Transform[] p = _follow.path.points;
        Vector3 quarterTrip = CubicBezierMath.SamplePoint(p[0].position, p[1].position, p[2].position, p[3].position, 0.75f);
        Assert.That(Vector3.Distance(_follow.transform.position, quarterTrip), Is.LessThan(0.01f));
        LogAssert.NoUnexpectedReceived();
    }

    [UnityTest]
    public IEnumerator TravelModes_TakeTheSameTime()
    {
        yield return LoadFlyThru();
        Assert.That(_follow.path.TotalLength, Is.GreaterThan(0f));
        _follow.speed = _follow.path.TotalLength / 2f;
        Vector3 end = _follow.path.points[9].position;
        int[] tripFrames = new int[2];

        for (int mode = 0; mode < 2; mode++)
        {
            _follow.travelByDistance = mode == 1;
            _follow.Restart();
            int frames = 0;
            do
            {
                yield return null;
                frames++;
            } while (Vector3.Distance(_follow.transform.position, end) > 0.001f && frames < 180);

            Assert.That(frames, Is.InRange(119, 121), "Each trip should take two seconds of simulated time.");
            tripFrames[mode] = frames;
        }

        Assert.That(tripFrames[0], Is.EqualTo(tripFrames[1]).Within(1));
        LogAssert.NoUnexpectedReceived();
    }

    [UnityTest]
    public IEnumerator FaceTarget_LooksAtTheBunny()
    {
        yield return LoadFlyThru();
        _follow.travelByDistance = true;
        _follow.faceTarget = true;
        Vector3 start = _follow.transform.position;
        yield return new WaitForSeconds(0.25f);
        Assert.That(Vector3.Distance(_follow.transform.position, start), Is.GreaterThan(0.1f));
        Vector3 toTarget = (_follow.target.position - _follow.transform.position).normalized;
        Assert.That(Vector3.Dot(_follow.transform.forward, toTarget), Is.GreaterThan(0.999f));
        Assert.That(Mathf.Abs(_follow.transform.right.y), Is.LessThan(0.001f));
        LogAssert.NoUnexpectedReceived();
    }

    [UnityTest]
    public IEnumerator TangentAim_LooksAlongThePath()
    {
        yield return LoadFlyThru();
        Assert.That(_follow.path.TotalLength, Is.GreaterThan(0f));
        _follow.travelByDistance = false;
        _follow.faceTarget = false;
        _follow.speed = _follow.path.TotalLength / 2f;
        _follow.Restart();
        for (int i = 0; i < 30; i++)
            yield return null;

        Transform[] p = _follow.path.points;
        Vector3 tangent = CubicBezierMath.SampleTangent(p[0].position, p[1].position,
            p[2].position, p[3].position, 0.75f).normalized;
        Assert.That(Vector3.Dot(_follow.transform.forward, tangent), Is.GreaterThan(0.999f));
        Assert.That(Mathf.Abs(_follow.transform.right.y), Is.LessThan(0.001f));
        LogAssert.NoUnexpectedReceived();
    }

    [UnityTest]
    public IEnumerator Aim_SwitchesWithoutMoving()
    {
        yield return LoadFlyThru();
        Assert.That(_follow.path.TotalLength, Is.GreaterThan(0f));
        _follow.travelByDistance = false;
        _follow.faceTarget = false;
        _follow.speed = _follow.path.TotalLength / 2f;
        _follow.Restart();
        for (int i = 0; i < 30; i++)
            yield return null;

        Transform[] p = _follow.path.points;
        Vector3 tangent = CubicBezierMath.SampleTangent(p[0].position, p[1].position,
            p[2].position, p[3].position, 0.75f).normalized;
        _follow.speed = 0f;
        Vector3 position = _follow.transform.position;
        _follow.faceTarget = true;
        yield return null;
        Assert.That(Vector3.Distance(_follow.transform.position, position), Is.LessThan(0.001f));
        Vector3 toTarget = (_follow.target.position - position).normalized;
        Assert.That(Vector3.Dot(_follow.transform.forward, toTarget), Is.GreaterThan(0.999f));
        _follow.faceTarget = false;
        yield return null;
        Assert.That(Vector3.Distance(_follow.transform.position, position), Is.LessThan(0.001f));
        Assert.That(Vector3.Dot(_follow.transform.forward, tangent), Is.GreaterThan(0.999f));
        LogAssert.NoUnexpectedReceived();
    }
}
