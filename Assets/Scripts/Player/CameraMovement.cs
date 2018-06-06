/*
Zoom, Rotate, Move
*/
using UnityEngine;

public class CameraMovement : MonoBehaviour {
    [Header("Movement settings:")]    
    public float dragSpeed;
    private Vector3 dragOrigin;
    private bool canMove;

    [Header("Rotation settings:")]        
    public float rotSpeed;
    private bool canRotate;
    
    [Header("Zoom settings:")]
    public float zoomSpeed;
    public int maxZoom = 40;
    public int minZoom = 2;
    private float tmpZoom, zoomDir;

    private void Start() {
        tmpZoom = Camera.main.orthographicSize;
    }

    void FixedUpdate() {
        // Zoom
        if(zoomDir != Camera.main.orthographicSize) {
            float cameraSize = Camera.main.orthographicSize;
            float newCameraSize = Mathf.Lerp(cameraSize, tmpZoom, Time.deltaTime * zoomSpeed);
            Camera.main.orthographicSize = newCameraSize;
        }

        // Movement & Rotation
        if(canMove){
            if(canRotate)
                Rotate();
            else
                Move();
        }
    }
    private void Update() {
        // Zoom
        if(canRotate){
            zoomDir = -Input.GetAxis("Mouse ScrollWheel");
            if(zoomDir != 0){
                Zoom();
            }
        }

        // Movement
        if (Input.GetMouseButtonDown(1)) {
            canMove = true;
            dragOrigin = Input.mousePosition;
        } else if (Input.GetMouseButtonUp(1)) {
            canMove = false;
        }

        // Rotate
        if (Input.GetKeyDown(KeyCode.LeftControl)) {
            canRotate = true;
        } else if (Input.GetKeyUp(KeyCode.LeftControl)) {
            canRotate = false;
        }
    }

    private void Zoom() {
        bool zoomAtMinimum = tmpZoom < minZoom && zoomDir < 0;
        bool zoomAtMaximum = tmpZoom > maxZoom && zoomDir > 0;
        if( zoomAtMaximum || zoomAtMinimum )
            return;
        tmpZoom += zoomDir * 10;        
    }

    private void Move() {
        Vector3 mousePosition = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        float movementSpeed = dragSpeed * 10 * Time.deltaTime;
        
        Vector3 destination = new Vector3(mousePosition.x * movementSpeed, 0, mousePosition.y * movementSpeed);
        transform.Translate(destination, Space.Self);

        dragOrigin = Input.mousePosition;
    }

    private void Rotate() {
        float diffHorizontal = dragOrigin.x - Input.mousePosition.x;
        if (diffHorizontal != 0) {
            Vector3 add = Vector3.up * Time.deltaTime * rotSpeed * 10;
            transform.localEulerAngles += add * (diffHorizontal < 0 ? 1 : -1);
            dragOrigin = Input.mousePosition;
        } 
    }

}