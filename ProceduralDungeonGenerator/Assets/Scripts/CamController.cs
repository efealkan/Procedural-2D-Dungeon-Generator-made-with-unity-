using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    [SerializeField] private float zoomOutMin = 5;
    [SerializeField] private float zoomOutMax = 40;
    [SerializeField] private float zoomSpeed = 15;
    
    private Vector3 touchPos;
    private Vector3 camInitialPos;
    private float camInitialZoom;
    
    private void Start()
    {
        camInitialPos = Camera.main.transform.position;
        camInitialZoom = Camera.main.orthographicSize;
    }

    private void Update()
    {
        CamControl();
    }

    private void CamControl()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Camera.main.transform.position = camInitialPos;
            Camera.main.orthographicSize = camInitialZoom;
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        
        if (Input.GetMouseButton(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero);
            if (hit && (hit.collider.tag == "Pixel" || hit.collider.tag == "ColorSwatchGroup" || 
                        hit.collider.tag == "ColorSwatch")) return;

            Vector3 direction = touchPos - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            Camera.main.transform.position += direction;
        }

        Zoom(Input.GetAxis("Mouse ScrollWheel"));
    }

    private void Zoom(float increment)
    {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment * zoomSpeed, zoomOutMin, zoomOutMax*10);
    }
}
