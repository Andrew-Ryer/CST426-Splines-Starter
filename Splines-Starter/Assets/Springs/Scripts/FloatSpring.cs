using UnityEngine;

[System.Serializable]
public class FloatSpring
{
    public float equilibrium;
    public float frequencyHz = 1f;
    public float dampingRatio = 0.1f;

    public float Value => _value;
    public float Velocity => _velocity;

    float _velocity;
    float _value;
    float _stepTime;

    const float FixedStep = 1f / 120f;

    public void Update(float deltaTime)
    {
        // TODO: Turn frequencyHz and dampingRatio into a stiffness and a damping, as in
        // TestBetterSpring. This spring has a mass of 1. Treat negative settings as 0.
        float omega = 2f * Mathf.PI * frequencyHz;
        float stiffness = omega * omega;

        float critDamping = 2f * Mathf.Sqrt(stiffness);
        float dampingCoefficient = critDamping * dampingRatio;

        // TODO: Add the restoring and damping forces and take one semi-implicit Euler step
        // toward equilibrium, updating _velocity and then _value.
        float restoringForce = -stiffness * (_value - equilibrium);
        float dampingForce = -dampingCoefficient * _velocity;
        
        float acceleration = (restoringForce + dampingForce);
        _velocity += acceleration * deltaTime;
        _value += _velocity * deltaTime;
    }

    // Steps the spring in fixed 1/120 s chunks, however long the frame was. Call this once a frame.
    public void Advance(float deltaTime)
    {
        _stepTime += deltaTime;
        while (_stepTime >= FixedStep)
        {
            Update(FixedStep);
            _stepTime -= FixedStep;
        }
    }

    public void AddVelocity(float amount)
    {
        _velocity += amount;
    }

    public void SetValue(float value)
    {
        _value = value;
        _velocity = 0f;
    }
}
