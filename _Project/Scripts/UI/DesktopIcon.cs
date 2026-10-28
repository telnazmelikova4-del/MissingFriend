using UnityEngine;
using UnityEngine.EventSystems;

public class DesktopIcon : MonoBehaviour, IPointerClickHandler
{
    // Сюда мы перетащим наше окно в Unity
    public GameObject windowToOpen;

    public void OnPointerClick(PointerEventData eventData)
    {
        // Если окно привязано, включаем его
        if (windowToOpen != null)
        {
            windowToOpen.SetActive(true);
        }
    }
}