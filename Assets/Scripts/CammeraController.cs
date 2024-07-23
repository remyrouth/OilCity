using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CammeraController : MonoBehaviour
{

    public Transform target;

    public float sensitivity = 500f;
   
    float positionX = 0;
    float positionY = 0;
    public float zoomSensitivty = 0.5f;
    public float zoomMax = 10f;
    public float zoomMin = 1f;
    public bool invert = false;
    [SerializeField]
    private float horizontalMax;
    [SerializeField]
    private float horizontalMin;
    [SerializeField]
    private float verticalMax;
    [SerializeField]
    private float verticalMin;
    [SerializeField]
    private float FoV = 10;
    private Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize FoV to current camera size
        FoV = Camera.main.orthographicSize;
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        HandleCameraMovement();
        AdjustCameraBounds();
        ClampPosition();
    }

    private void HandleCameraMovement()
    {
        if (invert)
        {
            if (Input.GetMouseButton(1))
            {
                positionX += Input.GetAxis("Mouse X") * -1 * sensitivity * Time.deltaTime;
                positionY += Input.GetAxis("Mouse Y") * -1 * sensitivity * Time.deltaTime;
            }
        }
        else
        {
            if (Input.GetMouseButton(1))
            {
                positionX += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
                positionY += Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
            }
        }

        Vector3 targetPosition = new Vector3(positionX, positionY, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, 0.0125f);

        cameraZoom();
    }

    private void cameraZoom()
    {
        if (Camera.main.orthographicSize < zoomMax && Input.mouseScrollDelta.y < 0)
        {
            FoV += zoomSensitivty;
        }

        if (Camera.main.orthographicSize > zoomMin && Input.mouseScrollDelta.y > 0)
        {
            FoV -= zoomSensitivty;
        }

        FoV = Mathf.Clamp(FoV, zoomMin, zoomMax);
    }

    private void AdjustCameraBounds()
    {
        if (target == null) return;

        // Calculate the size of the target object
        Bounds targetBounds = target.GetComponent<Renderer>().bounds;

        // Calculate the diagonal of the object
        float objectDiagonal = Vector3.Distance(targetBounds.min, targetBounds.max);

        // Adjust the camera bounds based on zoom level and object size
        float halfHeight = FoV;
        float halfWidth = halfHeight * mainCamera.aspect;

        Vector3 objectSize = targetBounds.size;

        // Calculate min and max based on the object bounds and FoV
        horizontalMax = target.position.x + (objectSize.x / 2) + halfWidth;
        horizontalMin = target.position.x - (objectSize.x / 2) - halfWidth;
        verticalMax = target.position.y + (objectSize.y / 2) + halfHeight;
        verticalMin = target.position.y - (objectSize.y / 2) - halfHeight;
    }

        private void ClampPosition()
    {
        positionX = Mathf.Clamp(positionX, horizontalMin, horizontalMax);
        positionY = Mathf.Clamp(positionY, verticalMin, verticalMax);
    }

    private void LateUpdate()
    {
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, FoV, Time.deltaTime * 20);
    }
}
