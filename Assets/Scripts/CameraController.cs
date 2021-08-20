using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    Transform kerbin;

    Camera mainCamera;
    RectTransform viewport;
    Canvas mainCanvas;
    Resolution currentResolution;

    Vector3 lookTarget;
    Vector3 cameraOffset;

    public float horizontalLookSensitivity = 1.0f;
    public float verticalLookSensitivity = 1.0f;
    public float maxVerticalLookAngle = 60.0f; // The max vertical rotation around the look target in degrees.
    public float cameraZoomSensitivity = 0.01f;
    public float cameraMinDistance = 2.0f;

    private void Awake()
    {
        // Set the camera viewport
        mainCamera = GetComponent<Camera>();
        viewport = GameObject.FindGameObjectWithTag("3D Scene Viewport").transform as RectTransform;
        mainCanvas = GameObject.FindGameObjectWithTag("Main Canvas").GetComponent<Canvas>();
        currentResolution = Screen.currentResolution;
        StartCoroutine(WaitForCanvasSetup());

        // Set the camera to look at Kerbin
        lookTarget = kerbin.position;
        cameraOffset = transform.position - lookTarget;
    }

    private IEnumerator WaitForCanvasSetup()
    {
        // Suspend setting the camera viewport until the UI Canvas has been set up properly

        yield return new WaitForEndOfFrame();
        SetCameraViewport();
    }

    void SetCameraViewport()
    {
        Rect viewportRect = RectTransformUtility.PixelAdjustRect(viewport, mainCanvas);
        viewportRect.x = 0f;
        viewportRect.y = Screen.height - viewportRect.height;

        mainCamera.pixelRect = viewportRect;
    }

    private void LateUpdate()
    {
        if ((currentResolution.width != Screen.currentResolution.height) || (currentResolution.height != Screen.currentResolution.height))
        {
            currentResolution = Screen.currentResolution;

            SetCameraViewport();
        }

        transform.LookAt(lookTarget, Vector3.forward);
        cameraOffset = transform.position - lookTarget;
    }

    public void SetCameraTarget(Vector3 targetPoint)
    {
        lookTarget = targetPoint;
    }

    public void RotateCamera(Vector2 delta)
    {
        transform.RotateAround(lookTarget, Vector3.forward, delta.x * horizontalLookSensitivity);

        // This float is used to limit the vertical rotation to the max vertical look angle. Limiting not necessary if this 
        // dot product has the same sign as the y-delta. If they have opposite signs, then movement is only allowed if the 
        // angle between the camera-target offset vector and the horizontal plane (xy plane) is less than the max angle.
        float dot = Vector3.Dot(Vector3.forward, cameraOffset);
        float angle = Mathf.Abs(Vector3.Angle(Vector3.forward, cameraOffset) - 90.0f);
        if ((Mathf.Sign(dot) == Mathf.Sign(delta.y)) || angle < maxVerticalLookAngle)
        {
            // The vertical rotation is not limited
            Vector3 verticalLookAxis = Vector3.Cross(Vector3.forward, cameraOffset);
            transform.RotateAround(lookTarget, verticalLookAxis, delta.y * verticalLookSensitivity);
        }
    }

    public void ZoomCamera(float scrollDelta)
    {
        if (scrollDelta < 0.0f || cameraOffset.magnitude > cameraMinDistance)
            transform.position -= cameraOffset.normalized * scrollDelta * cameraZoomSensitivity;
    }
}
