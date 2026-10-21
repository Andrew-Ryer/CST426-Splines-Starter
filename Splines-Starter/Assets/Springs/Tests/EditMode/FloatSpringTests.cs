using NUnit.Framework;
using UnityEngine;

// These check FloatSpring's math, not its feel. Every step is a fixed 1/60 s.
public class FloatSpringTests
{
    const float Dt = 1f / 60f;

    static FloatSpring MakeSpring(float frequencyHz, float dampingRatio, float equilibrium, float start)
    {
        var spring = new FloatSpring { frequencyHz = frequencyHz, dampingRatio = dampingRatio, equilibrium = equilibrium };
        spring.SetValue(start);
        return spring;
    }

    [Test]
    public void OneStepUpdatesVelocityThenValue()
    {
        var spring = MakeSpring(1f, 0f, 0f, 1f);
        spring.Update(Dt);

        float omega = 2f * Mathf.PI;
        float expectedVelocity = -omega * omega * Dt;
        Assert.AreEqual(expectedVelocity, spring.Velocity, 1e-4f, "velocity after one step");
        Assert.AreEqual(1f + expectedVelocity * Dt, spring.Value, 1e-5f, "value moves with the new velocity");
    }

    [Test]
    public void SettlesAtEquilibrium()
    {
        var spring = MakeSpring(2f, 1f, 3f, 0f);
        for (int i = 0; i < 600; i++)
            spring.Update(Dt);

        Assert.AreEqual(3f, spring.Value, 1e-3f);
        Assert.AreEqual(0f, spring.Velocity, 1e-2f);
    }

    [Test]
    public void CriticalDampingDoesNotOvershoot()
    {
        var spring = MakeSpring(1f, 1f, 1f, 0f);
        float highest = float.MinValue;
        for (int i = 0; i < 300; i++)
        {
            spring.Update(Dt);
            highest = Mathf.Max(highest, spring.Value);
        }

        Assert.LessOrEqual(highest, 1.001f);
        Assert.Greater(highest, 0.99f, "it still reaches equilibrium");
    }

    [Test]
    public void LowDampingOvershoots()
    {
        var spring = MakeSpring(1f, 0.25f, 1f, 0f);
        float highest = float.MinValue;
        for (int i = 0; i < 300; i++)
        {
            spring.Update(Dt);
            highest = Mathf.Max(highest, spring.Value);
        }

        Assert.Greater(highest, 1.1f);
    }

    [Test]
    public void NoDampingKeepsItsSwing()
    {
        var spring = MakeSpring(1f, 0f, 0f, 1f);
        float highest = float.MinValue;
        float lowest = float.MaxValue;
        for (int i = 0; i < 600; i++)
        {
            spring.Update(Dt);
            if (i >= 480)
            {
                highest = Mathf.Max(highest, spring.Value);
                lowest = Mathf.Min(lowest, spring.Value);
            }
        }

        Assert.Greater(highest, 0.9f, "it does not die out");
        Assert.Less(lowest, -0.9f, "it still swings past equilibrium");
        Assert.Less(Mathf.Max(highest, -lowest), 1.1f, "it does not grow");
    }

    [Test]
    public void AddVelocityPushesItThatWay()
    {
        var spring = MakeSpring(1f, 0.5f, 0f, 0f);
        spring.AddVelocity(2f);
        spring.Update(Dt);

        Assert.Greater(spring.Value, 0f);
    }
}
