using UnityEngine;
using UnityEngine.EventSystems; // Required namespace

public class ClickDetection : MonoBehaviour, IPointerClickHandler
{
    // This automatically triggers when the object's 2D collider is clicked
    public void OnPointerClick(PointerEventData eventData)
    {
    }
}
