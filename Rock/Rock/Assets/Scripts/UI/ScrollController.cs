using UnityEngine;
using UnityEngine.UI;

public class ScrollController : MonoBehaviour
{
    public ScrollRect scrollRect;
    public float scrollStep = 0.1f; // wie stark gescrollt werden soll (0�1)

    public void ScrollUp()
    {
        scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition + scrollStep);
    }

    public void ScrollDown()
    {
        scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition - scrollStep);
    }
}