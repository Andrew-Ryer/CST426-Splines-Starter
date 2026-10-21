using TMPro;
using UnityEngine;

/*
   A bubble that grows and shrinks with a bounce as you drag the slider.

   No new spring here: the slider is your JuicySlider from scene 3, and it already has one. Its value
   is the spring's bouncing position (see the JuicySlider header), so anything you drive with value
   bounces too. This scene only works once JuicySlider does: if the bubble jumps instead of bouncing,
   look there.
*/
public class ScaleSliderDemo : MonoBehaviour
{
    public JuicySlider scaleSlider;
    public RectTransform bubbleRoot;
    public TMP_Text scaleValueText;
    public TMP_Text dampingValueText;
    public TMP_Text frequencyValueText;

    void Update()
    {
        // TODO: Read scaleSlider.value and scale bubbleRoot by it in x and y. Keep z at 1.
        // Use value, not scaleSlider.spring.equilibrium: equilibrium is the target, so the bubble
        // would jump there with no bounce.

        // TODO: Show the scale, scaleSlider.spring.dampingRatio and scaleSlider.spring.frequencyHz in
        // the three readouts. ToString("0.0") keeps a number short.
    }
}
