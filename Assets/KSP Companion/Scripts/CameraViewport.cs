using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class CameraViewport : MonoBehaviour, IDragHandler, IScrollHandler
{
    [SerializeField]
    CameraController mainCameraController;

    public void OnDrag(PointerEventData eventData)
    {
        if (Mouse.current.rightButton.isPressed)
            mainCameraController.RotateCamera(eventData.delta);
    }

    public void OnScroll(PointerEventData eventData)
    {
        mainCameraController.ZoomCamera(eventData.scrollDelta.y);
    }
}
