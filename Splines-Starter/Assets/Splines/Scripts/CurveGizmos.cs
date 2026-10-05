using System;
using UnityEngine;

/*
 * CurveGizmos draws control-point markers, the control polygon, and a sampled curve.
 * The callback receives a normalized parameter in [0, 1]. SplinePath adapts that
 * range to its whole-path u so the drawing includes every segment.
 */

public static class CurveGizmos
{
    public static void Draw(int numSamples, Func<float, Vector3> samplePoint, params Transform[] controlPoints)
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < controlPoints.Length - 1; i++)
            Gizmos.DrawLine(controlPoints[i].position, controlPoints[i + 1].position);

        Gizmos.color = Color.white;
        for (int i = 0; i < controlPoints.Length; i++)
            Gizmos.DrawWireSphere(controlPoints[i].position, 0.1f);

        Vector3 lastSample = samplePoint(0f);
        for (int i = 1; i < numSamples; i++)
        {
            Vector3 sample = samplePoint((float)i / (numSamples - 1));
            Gizmos.DrawLine(lastSample, sample);
            lastSample = sample;
        }
    }
}
