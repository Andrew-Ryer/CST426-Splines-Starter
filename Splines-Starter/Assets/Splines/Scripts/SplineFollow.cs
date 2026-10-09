using UnityEngine;

/*
 * SplineFollow rides a SplinePath. Each frame it moves along the path,
 * finds u, places itself on the curve, and faces the target or the tangent.
 */

public class SplineFollow : MonoBehaviour
{
    // For polish
    [SerializeField] public bool useEasing = false;
    [Range(0.01f, 0.5f)] public float easeFraction = 0.25f;
    float _elapsedTime;
    
    public SplinePath path;
    public Transform target;
    public float speed = 2.5f; // Positive world units per second in the completed exercise.
    public bool travelByDistance = true;
    public bool faceTarget = true;

    float _distance;
    float _u;

    void Update()
    {
        if (travelByDistance)
        {
            // TODO: Advance distance by speed over the frame and look up u for that distance.
            // Stop at TotalLength.
            
            // _distance = Mathf.Min(_distance + speed * Time.deltaTime, path.TotalLength);
            //
            // _u = path.ParameterAtDistance(_distance);

            if (useEasing)
            {
                float length = path.TotalLength;
                float cruiseSpeed = Mathf.Max(0f, speed);
                float e = Mathf.Clamp(easeFraction, 0.01f, 0.5f);

                if (length > 0f && cruiseSpeed > 0f)
                {
                    float tripDuration = length / (cruiseSpeed * (1f - e));

                    _elapsedTime = Mathf.Min(_elapsedTime + Time.deltaTime, tripDuration);

                    float progress = _elapsedTime / tripDuration;
                    float distanceFraction;

                    if (progress < e)
                    {
                        //ease in
                        distanceFraction = progress * progress / (2f * e * (1f - e));
                    }
                    else if (progress <= 1f - e)
                    {
                        //constant speed.
                        distanceFraction =
                            (progress - e * 0.5f) / (1f - e);
                    }
                    else
                    {
                        //ease out
                        float remaining = 1f - progress;

                        distanceFraction = 1f - remaining * remaining / (2f * e * (1f - e));
                    }

                    _distance = Mathf.Clamp01(distanceFraction) * length;
                }

                _u = path.ParameterAtDistance(_distance);
            }
            else
            {
                _distance = Mathf.Min(_distance + speed * Time.deltaTime, path.TotalLength);
                
                _u = path.ParameterAtDistance(_distance);
            }

        }
        else
        {
            // TODO: Advance u in equal steps, paced so the trip takes as long as the distance
            // trip at the same speed. Stop at SegmentCount.
            
            float uSpeed = path.SegmentCount * speed / path.TotalLength;

            _u = Mathf.Min(_u + uSpeed * Time.deltaTime, path.SegmentCount);
        }

        // TODO: Place this object at the path point for u. Replay should return it to the start.
        
        transform.position = path.SamplePoint(_u);

        // TODO: Look at the target if faceTarget is on, otherwise along the path tangent.
        // Use world up so the horizon stays level.
        
        Vector3 forward = faceTarget ? target.position - transform.position : path.SampleTangent(_u);

        if (forward.sqrMagnitude > 0f)
        {
            transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
        }
    }

    public void Restart()
    {
        _distance = 0f;
        _u = 0f;
        
        _elapsedTime = 0f;
    }
}
