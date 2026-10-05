using UnityEngine;

// On-screen controls for the fly-through. The motion itself is SplineFollow.
public class CameraFlyThru : MonoBehaviour
{
    public SplineFollow movement;

    GUIStyle _button;

    void OnGUI()
    {
        float scale = Mathf.Min(Screen.width / 1600f, Screen.height / 900f);
        GUI.matrix = Matrix4x4.Scale(Vector3.one * scale);
        if (_button == null)
            _button = new GUIStyle(GUI.skin.button) { fontSize = 15 };

        if (GUI.Button(new Rect(36, 36, 110, 32), "Replay", _button))
            movement.Restart();
        if (GUI.Button(new Rect(156, 36, 150, 32), movement.faceTarget ? "Aim: bunny" : "Aim: tangent", _button))
            movement.faceTarget = !movement.faceTarget;
    }
}
