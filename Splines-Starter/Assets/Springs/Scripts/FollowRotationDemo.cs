using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
   A head that chases a target along a track and leans as it moves.

   The slider is the target: drag it, or drag the head (DraggableSpriteTarget moves the slider for
   you). It is a plain Slider, so its value jumps. followSpring does the chasing: its value is where
   the head is, and its velocity is how fast the head is moving.

   The lean comes from the velocity, not from the distance to the target. Moving fast leans a lot,
   slowing down straightens up, and once the spring settles its velocity is 0, so the head stands
   upright again.

   Supplied: the red dot and line between the head and the target, their colour (grey once
   settled), and the readouts. ValueToTrackLocalX turns a slider value into an x position on the track.
*/
public class FollowRotationDemo : MonoBehaviour
{
    public Slider targetSlider;
    public RectTransform spriteRoot;
    public RectTransform targetMarker;
    public RectTransform redDot;
    public RectTransform redLine;
    public RectTransform trackRect;
    public TMP_Text angleValueText;
    public TMP_Text dampingValueText;
    public TMP_Text frequencyValueText;
    public FloatSpring followSpring = new() { frequencyHz = 3f, dampingRatio = 0.25f };
    public float rotationDegreesPerVelocity = -12f;
    public bool reverseAngleMapping;
    public Color negativeSideColor = new(1f, 0.08f, 0.04f, 1f);
    public Color positiveSideColor = new(0.1f, 0.45f, 1f, 1f);
    public Color settledColor = new(0.55f, 0.55f, 0.55f, 1f);
    public float settledDistance = 0.01f;
    public float settledVelocity = 0.01f;

    void Start()
    {
        // TODO: Start the follow spring at rest on the slider's value: set followSpring.equilibrium
        // to targetSlider.value, and call followSpring.SetValue(targetSlider.value). Without this
        // the spring starts at 0, and the head flies in from the left on Play.

        UpdateVisuals(targetSlider.value, followSpring.Value, followSpring.Velocity);
    }

    void Update()
    {
        // TODO: Set followSpring.equilibrium to targetSlider.value, then step the spring with
        // followSpring.Advance(Time.deltaTime).

        UpdateVisuals(targetSlider.value, followSpring.Value, followSpring.Velocity);
    }

    // target is where the head is going, follower is where it is, velocity is how fast it moves.
    public void UpdateVisuals(float target, float follower, float velocity)
    {
        float targetX = ValueToTrackLocalX(target);
        float followerX = ValueToTrackLocalX(follower);
        float displacement = follower - target;
        // TODO: Lean by the spring's velocity: angle is velocity * rotationDegreesPerVelocity, times -1
        // when reverseAngleMapping is on. Use velocity, not displacement: a head leaning by distance
        // would stay tilted while it waits off target.
        float angle = 0f;
        bool settled = Mathf.Abs(displacement) <= settledDistance && Mathf.Abs(velocity) <= settledVelocity;
        Color sideColor = settled ? settledColor : displacement < 0f ? negativeSideColor : positiveSideColor;

        // TODO: Move spriteRoot to followerX the same way redDot moves just below: copy its
        // anchoredPosition, change x, and assign it back. Then turn it to angle about z with
        // spriteRoot.localRotation = Quaternion.Euler(0f, 0f, angle).

        Vector2 dotPosition = redDot.anchoredPosition;
        dotPosition.x = followerX;
        redDot.anchoredPosition = dotPosition;

        Vector2 linePosition = redLine.anchoredPosition;
        linePosition.x = (targetX + followerX) * 0.5f;
        redLine.anchoredPosition = linePosition;

        Vector2 lineSize = redLine.sizeDelta;
        lineSize.x = Mathf.Abs(targetX - followerX);
        redLine.sizeDelta = lineSize;
        redLine.localRotation = Quaternion.identity;

        redDot.GetComponent<Image>().color = sideColor;
        redLine.GetComponent<Image>().color = new Color(sideColor.r, sideColor.g, sideColor.b, 0.85f);

        angleValueText.text = angle.ToString("0") + " deg";
        dampingValueText.text = followSpring.dampingRatio.ToString("0.00");
        frequencyValueText.text = followSpring.frequencyHz.ToString("0");
    }

    float ValueToTrackLocalX(float value)
    {
        float range = targetSlider.maxValue - targetSlider.minValue;
        float t = range == 0f ? 0f : (value - targetSlider.minValue) / range;
        return Mathf.LerpUnclamped(trackRect.rect.xMin, trackRect.rect.xMax, t);
    }
}
