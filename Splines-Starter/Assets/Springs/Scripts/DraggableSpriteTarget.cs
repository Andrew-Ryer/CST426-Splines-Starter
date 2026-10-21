using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableSpriteTarget : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public Slider targetSlider;
    public RectTransform trackRect;

    public void OnPointerDown(PointerEventData eventData)
    {
        SetSliderFromPointer(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        SetSliderFromPointer(eventData);
    }

    void SetSliderFromPointer(PointerEventData eventData)
    {
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                trackRect,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint))
        {
            return;
        }

        float normalizedX = (localPoint.x - trackRect.rect.xMin) / trackRect.rect.width;
        targetSlider.value = Mathf.Lerp(targetSlider.minValue, targetSlider.maxValue, normalizedX);
    }
}
