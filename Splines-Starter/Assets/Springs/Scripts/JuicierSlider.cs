using UnityEngine;
using UnityEngine.InputSystem;

/*
   A 3D slider handle you drag along a track. Let go and it springs to the nearest slot.

   Two springs, each with its own job:
     handleSpring  where the handle sits along the track, in x, from -travelHalfWidth to travelHalfWidth
                   (0 is the middle of the track)
     pressSpring   how far the handle is pressed in: 0 up, 1 down, like JuicyButton

   While you hold the mouse, the handle spring's target follows the pointer. When you let go, the
   target moves to the nearest slot and the spring carries the handle there, overshooting a little.
   The code only ever changes the springs' targets (equilibrium). UpdateHandleVisuals then turns
   the springs' values into positions.

   This script sits on the handle (slider-button), so moving transform slides the whole handle,
   base and button, along the track. handleTransform is its Button, the top part that dips when pressed.

   Supplied: when it counts as pressed (UpdatePressedState), where the pointer is along the track
   (PointerXOnTrack), and which slot is closest to a spot on the track (NearestSlot).
*/
public class JuicierSlider : MonoBehaviour
{
    [Header("Scene References")]
    public Transform handleTransform;

    [Header("Slider Shape")]
    public float travelHalfWidth = 6f;
    public int slotCount = 5;

    [Header("Press Shape")]
    public float pressDepth = -0.04f;

    [Header("Spring Config")]
    public FloatSpring handleSpring = new() { frequencyHz = 3f, dampingRatio = 0.35f };
    public FloatSpring pressSpring = new() { frequencyHz = 5f, dampingRatio = 0.6f };

    bool _isPressed;
    Vector3 _restSliderPosition;
    Vector3 _restHandlePosition;

    int MarkerCount => Mathf.Max(1, slotCount);
    float SlotSpacing => MarkerCount == 1 ? 0f : travelHalfWidth * 2f / (MarkerCount - 1);

    void Awake()
    {
        // Remember where everything starts. The springs move things as offsets from these.
        _restSliderPosition = transform.localPosition;
        _restHandlePosition = handleTransform.localPosition;
    }

    void Update()
    {
        UpdatePressedState();

        // TODO: While pressed (_isPressed), set handleSpring.equilibrium to the pointer's spot on
        // the track, PointerXOnTrack(). Keep it on the track with
        // Mathf.Clamp(..., -travelHalfWidth, travelHalfWidth).

        // TODO: Set pressSpring.equilibrium to 1 while pressed and 0 otherwise.

        // TODO: Step both springs with handleSpring.Advance(Time.deltaTime) and
        // pressSpring.Advance(Time.deltaTime). Don't move anything here (that happens in
        // UpdateHandleVisuals).

        UpdateHandleVisuals();
    }

    void UpdatePressedState()
    {
        // Let go anywhere and the press ends, even off the slider.
        if (_isPressed && !Mouse.current.leftButton.isPressed)
        {
            // TODO: On release, set handleSpring.equilibrium to the nearest slot,
            // NearestSlot(handleSpring.equilibrium). The spring carries the handle there.
            // Watch out: change the target, not the spring's value. SetValue would teleport the handle.
            _isPressed = false;
        }

        // A press only counts if the click lands on the slider or one of its parts.
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            _isPressed = Physics.Raycast(ray, out RaycastHit hitInfo)
                         && (hitInfo.transform == transform || hitInfo.transform.IsChildOf(transform));
        }
    }

    void UpdateHandleVisuals()
    {
        // TODO: Slide the handle: start from _restSliderPosition, add handleSpring.Value to its x,
        // and assign the result to transform.localPosition.

        // TODO: Dip the top: start from _restHandlePosition, add pressDepth * pressSpring.Value to
        // its y, and assign the result to handleTransform.localPosition.

        // Watch out: start from the rest positions every frame. Adding to the current position
        // would add the offset again each frame, and the handle would drift away.
    }

    // Where the pointer is along the track, in the same units as handleSpring (0 is the middle).
    float PointerXOnTrack()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane trackPlane = new(Vector3.up, transform.position);

        if (!trackPlane.Raycast(ray, out float distance))
            return handleSpring.equilibrium;

        Vector3 pointerPosition = ray.GetPoint(distance);

        if (transform.parent == null)
            return pointerPosition.x - _restSliderPosition.x;

        return transform.parent.InverseTransformPoint(pointerPosition).x - _restSliderPosition.x;
    }

    // The slot closest to x. Slots are evenly spaced from -travelHalfWidth to travelHalfWidth.
    float NearestSlot(float x)
    {
        if (MarkerCount == 1)
            return 0f;

        int nearestSlot = Mathf.RoundToInt((x + travelHalfWidth) / SlotSpacing);
        float snapped = -travelHalfWidth + nearestSlot * SlotSpacing;
        return Mathf.Clamp(snapped, -travelHalfWidth, travelHalfWidth);
    }
}
