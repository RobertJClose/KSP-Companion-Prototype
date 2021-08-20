using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AddMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    bool isMouseOver = true;
    public int parentAddButtonIndexInTimeline;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && !isMouseOver)
        {
            Destroy(gameObject);
        }
    }
}
