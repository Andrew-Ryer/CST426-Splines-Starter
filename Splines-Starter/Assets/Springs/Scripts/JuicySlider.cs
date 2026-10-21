using UnityEngine;
using UnityEngine.UI;

/*
   A Unity Slider whose handle springs to where you click or drag instead of jumping there.

   How Unity's Slider works: it keeps one number, value, and draws the handle and fill from it.
   Every way of changing that number ends in one method, Set(input, sendCallback): dragging,
   clicking the track, the arrow keys, and code that assigns slider.value. Set clamps the number,
   stores it, redraws, and (when sendCallback is true) fires onValueChanged.

   The trick, and why it looks odd: Set is virtual, so the same method name reaches two versions.
     - Everyone else calls Set: the Slider's own drag, click and key handling, and any other code.
       Because we override it, those calls come to our version. It does not move the handle; it
       changes the target behind the scenes, spring.equilibrium.
     - Only our Update calls base.Set, the Slider's original version, with the spring's current
       position. That is what moves the handle and the fill.

   So the Slider's own value is now only what is drawn this frame, and the number the user asked
   for lives in the spring:
     spring.equilibrium  where the user asked to go (the target)
     value               where the handle is drawn right now (the spring's position, bouncing)
*/
public class JuicySlider : Slider
{
    public FloatSpring spring = new() { frequencyHz = 5f, dampingRatio = 0.6f };

    protected override void OnEnable()
    {
        base.OnEnable();

        // Aim the spring at the slider's starting value. The spring itself starts at 0,
        // so on Play the handle springs in from 0.
        spring.equilibrium = value;
    }

    protected override void Update()
    {
        base.Update();

        // Slider Update runs in the Editor but the spring should only be active at runtime
        if (!Application.isPlaying) return;

        // TODO: Step the spring with spring.Advance(Time.deltaTime) and show its value on the slider.
        // base.Set changes the visual location of the slider handle. Make sure sendCallback is off,
        // so listeners only respond to the target value and not the in progress interpolated spring value
        spring.Advance(Time.deltaTime);
        base.Set(spring.Value);
    }

    // Every input comes through here: drag, click, keys, or code assigning slider.value.
    protected override void Set(float input, bool sendCallback = true)
    {
        // base.Set rounds and clamps for us, but the target never goes through it, so do it here.
        float target = wholeNumbers ? Mathf.Round(input) : input;
        target = Mathf.Clamp(target, minValue, maxValue);

        // Outside Play mode nothing steps the spring, so jump straight there like a plain Slider.
        if (!Application.isPlaying)
        {
            base.Set(target, sendCallback);
            return;
        }

        // TODO: Set the spring's new equilibrium. Don't call base.Set here (that moves the handle and
        // should happen in Update). If sendCallback is on and the target changed, invoke onValueChanged
        // with the new target so listeners (e.g. a volume control) know what the value will change to.
        spring.equilibrium = target;

        if (sendCallback)
            onValueChanged.Invoke(spring.equilibrium);
    }
}
