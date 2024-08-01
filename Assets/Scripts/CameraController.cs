using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : Singleton<CameraController>
{
    public float sensitivity = 500f;
    public float zoomSensitivty = 0.5f;
    public float zoomMax = 10f;
    public float zoomMin = 1f;
    [SerializeField] private bool invert;
    public bool Invert
    {
        get => invert;
        set => invert = value;
    }

    private Vector2Int bottomLeftCorner => Vector2Int.zero;
    private Vector2Int upperRightCorner => new Vector2Int(BoardManager.MAP_SIZE_X, BoardManager.MAP_SIZE_Y);

    private float _targetZoom;
    public float TargetZoom
    {
        get => _targetZoom;
        set => _targetZoom = value;
    }

    private Vector3 _targetPosition;
    public Vector3 TargetPosition
    {
        get => _targetPosition;
        set => _targetPosition = value;
    }

    private Camera _cam;

    private void Start()
    {
        _cam = GetComponent<Camera>();
        _targetZoom = _cam.orthographicSize;
        _targetPosition = _cam.transform.position;
    }

    private void Update()
    {
        HandleCameraMovement();
        HandleCameraZoom();
        AdjustCameraBounds();
    }
    private void HandleCameraMovement()
    {
        if (Input.GetMouseButton(1))
        {
            var delta = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);
            delta *= _targetZoom / zoomMax * sensitivity;
            if (invert)
                delta *= -1;
            _targetPosition += delta;

            _targetPosition.y = Mathf.Clamp(_targetPosition.y, bottomLeftCorner.y, upperRightCorner.y);
            _targetPosition.x = Mathf.Clamp(_targetPosition.x, bottomLeftCorner.x, upperRightCorner.x);
        }
    }

    private void HandleCameraZoom()
    {
        _targetZoom = Mathf.Clamp(_targetZoom - Input.mouseScrollDelta.y, zoomMin, zoomMax);
    }

    private void AdjustCameraBounds()
    {
        float screenRatio = (float)Screen.width / Screen.height;
        Vector3 adjustDelta = Vector3.zero;
        if (_targetPosition.x - _cam.orthographicSize * screenRatio < bottomLeftCorner.x)
            adjustDelta.x += 1;
        if (_targetPosition.x + _cam.orthographicSize * screenRatio > upperRightCorner.x)
            adjustDelta.x -= 1;

        if (_targetPosition.y - _cam.orthographicSize < bottomLeftCorner.y)
            adjustDelta.y += 1;
        if (_targetPosition.y + _cam.orthographicSize > upperRightCorner.y)
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
