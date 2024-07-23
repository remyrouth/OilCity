using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CammeraController : MonoBehaviour
{
    public float sensitivity = 500f;
    public float horizontalMax = 10f;
    public float horizontalMin = 0.0f;
   // public float horizontalZoomMax = 15f;
    public float verticalMax = 10f;
    public float verticalMin = 0.0f; 
    //public float verticalZoomMax
    float positionX = 0;
    float positionY = 0;
    public float zoomSensitivty = 0.5f;
    public float zoomMax = 10f;
    public float zoomMin = 1f;
    public bool invert = false;
    private float FoV = 10;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Use player prefs for updating inversion 
        if (invert)
        {
            if (Input.GetMouseButton(1))
            {
                positionX += Input.GetAxis("Mouse X") *  -1 * sensitivity * Time.deltaTime;

            }

            if (Input.GetMouseButton(1))
            {
                positionY += Input.GetAxis("Mouse Y") * -1 * sensitivity * Time.deltaTime;

            }

        } else

        {
            if (Input.GetMouseButton(1))
            {
                positionX += Input.GetAxis("Mouse X") * 1 * sensitivity * Time.deltaTime;

            }

            if (Input.GetMouseButton(1))
            {
                positionY += Input.GetAxis("Mouse Y") * 1 * sensitivity * Time.deltaTime;

            }
        }


        Vector3 cache = new Vector3(positionX, positionY, 0);

        transform.position = Vector3.Lerp(transform.position, cache, 0.0125f);

        cameraZoom();

        FoV = Mathf.Clamp(FoV, zoomMin, zoomMax);

        positionX = Mathf.Clamp(positionX, horizontalMin, horizontalMax);

        positionY = Mathf.Clamp(positionY, verticalMin, verticalMax);
    }
    

    void cameraZoom()
    {
       if (Camera.main.orthographicSize < zoomMax && Input.mouseScrollDelta.y < 0)
        {
           
            FoV += zoomSensitivty;

           if(sensitivity < 500)
            {
                sensitivity += 30;
            }

            if (horizontalMax < 15)
            {
               // horizontalMax += ;
            }
        }

       if (Camera.main.orthographicSize > zoomMin && Input.mouseScrollDelta.y > 0)
        {
            FoV -= zoomSensitivty;

            if (sensitivity > 50)
            {
                sensitivity -= 30;
            }
        }

    }

    private void LateUpdate()
    {
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, FoV, Time.deltaTime * 20);
    }
}
