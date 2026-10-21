using UnityEngine;
using UnityEngine.InputSystem;

/*
   A 3D button that springs down when you press it and bounces back up when you let go.

   One spring drives the whole press. Its value means "how far down the button is": 0 is fully
   up, 1 is fully down. Pressing moves the spring's target (equilibrium) to 1 and letting go moves
   it back to 0. The spring chases the target, and UpdateButtonVisuals turns that one number into
   a height and a squash.

   On the way, the spring goes past 0 and 1. That overshoot is the bounce, so it is not clamped.

   Nudge kicks the spring by adding velocity without moving the target, so the button wobbles and
   settles back where it was. The scene's Nudge button calls it.
*/
public class JuicyButton : MonoBehaviour
{
    [Header("Scene References")]
    public Transform buttonRoot;
    public Transform buttonCap;

    [Header("Button Shape")]
    public float downHeight = -1f;
    public float upHeight;
    public Vector3 downScale = Vector3.one;
    public Vector3 upScale = Vector3.one;
    
    [Header("Spring Config")]
    public FloatSpring buttonSpring = new() { frequencyHz = 5f, dampingRatio = 0.6f };
    public float nudgeAmount = 5f; 
    
    bool _isButtonDown;

    void Update()
    {
        // Let go anywhere and the button comes back up, even off the button.
        if (_isButtonDown && !Mouse.current.leftButton.isPressed)
            _isButtonDown = false;

        // A press only counts if the click lands on the button.
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            _isButtonDown = Physics.Raycast(ray, out RaycastHit hitInfo) && hitInfo.transform == buttonCap;
        }

        // TODO: Set the spring's equilibrium to 1 while the button is down and 0 when it is up,
        // then step it with buttonSpring.Advance. Applying this to transforms happens in UpdateButtonVisuals
        buttonSpring.equilibrium = (_isButtonDown) ? 1f : 0f;
        buttonSpring.Advance(Time.deltaTime);

        UpdateButtonVisuals();
    }

    void UpdateButtonVisuals()
    {
        // TODO: Use the spring's value (0 up, 1 down) to move buttonCap from upHeight
        // toward downHeight, and to scale buttonRoot (the whole button) from upScale toward downScale.
        // Watch out: Don't clamp the value! When it goes past 0 or 1, that overshoot is the bounce.
        // Mathf.Lerp and Vector3.Lerp clamp to 0..1, so use their LerpUnclamped versions.
        Vector3 newCapPosition = buttonCap.localPosition;
        newCapPosition.y = Mathf.LerpUnclamped(upHeight, downHeight, buttonSpring.Value);
        buttonCap.localPosition = newCapPosition;
        
        buttonRoot.localScale = Vector3.LerpUnclamped(upScale, downScale, buttonSpring.Value);
    }

    // Velocity, not a new target: the button wobbles and settles where it was.
    public void Nudge(float amount)
    {
       buttonSpring.AddVelocity(nudgeAmount);
    }
}
