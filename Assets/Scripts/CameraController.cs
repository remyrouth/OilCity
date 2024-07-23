using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{

    public float sensitivity = 500f;

    public float zoomSensitivty = 0.5f;
    public float zoomMax = 10f;
    public float zoomMin = 1f;
    public bool invert = false;

    [SerializeField]
    private Vector2Int bottomLeftCorner, upperRightCorner;

    private float _targetZoom;
    private Vector3 _targetPosition;
    private Camera _cam;

    void Start()
    {
        // Initialize variables
        _cam = GetComponent<Camera>();
        _targetZoom = _cam.orthographicSize;
        _targetPosition = _cam.transform.position;
    }

    void Update()
    {
        HandleCameraMovement();
        AdjustCameraBounds();
    }
    private void HandleCameraMovement()
    {
        if (Input.GetMouseButton(1))
        {
            _targetPosition += new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);
            if (invert)
                _targetPosition *= -1;
            _targetPosition.y = Mathf.Clamp(_targetPosition.y, bottomLeftCorner.y, upperRightCorner.y);
            _targetPosition.x = Mathf.Clamp(_targetPosition.x, bottomLeftCorner.x, upperRightCorner.x);
        }
        cameraZoom();
    }

    private void cameraZoom()
    {
        _targetZoom = Mathf.Clamp(_targetZoom + Input.mouseScrollDelta.y, zoomMin, zoomMax);
    }

    private void AdjustCameraBounds()
    {
        float screenRatio = 16 / 9; // Screen.width/Screen.height gives 1 ???
        Vector3 adjustDelta = Vector3.zero;
        Debug.Log(screenRatio);
        if (_cam.transform.position.x - _cam.orthographicSize * screenRatio < bottomLeftCorner.x)
            adjustDelta.x += 1;
        if (_cam.transform.position.x + _cam.orthographicSize * screenRatio > upperRightCorner.x)
            adjustDelta.x -= 1;

        if (_cam.transform.position.y - _cam.orthographicSize < bottomLeftCorner.y)
            adjustDelta.y += 1;
        if (_cam.transform.position.y + _cam.orthographicSize > upperRightCorner.y)
            adjustDelta.y -= 1;
        _targetPosition += adjustDelta * Time.deltaTime * 20;

    }


    private void LateUpdate()
    {
        _cam.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, _targetZoom, Time.deltaTime * 20);
        transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * 10);
    }
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Vector3 origin = new();
        Vector3 end = new();
        origin.x = bottomLeftCorner.x;
        origin.y = bottomLeftCorner.y;
        end.x = bottomLeftCorner.x;
        end.y = upperRightCorner.y;
        Gizmos.DrawLine(origin, end);
        origin = end;
        end.x = upperRightCorner.x;
        Gizmos.DrawLine(origin, end);
        origin = end;
        end.y = bottomLeftCorner.y;
        Gizmos.DrawLine(origin, end);
        origin = end;
        end.x = bottomLeftCorner.x;
        Gizmos.DrawLine(origin, end);
    }
#endif
}
