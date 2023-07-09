using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class CameraScript : NetworkBehaviour
{
    public float sensX , sensY;
    public Transform Orientation;
    float xRot, yRot;
    public GameObject cameraHolder;
    public Vector3 offset;

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Update() {
        if(!IsOwner) return;
        cameraHolder.transform.position = transform.position + offset;
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRot += mouseX;
        xRot -= mouseY;

        xRot = Mathf.Clamp(xRot , -90f, 90f);

        cameraHolder.GetComponentInChildren<Transform>().rotation = Quaternion.Euler(xRot, yRot, 0);
        Orientation.rotation = Quaternion.Euler(0, yRot, 0);
        if(Input.GetKeyDown(KeyCode.Escape)){
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if(Input.GetKeyDown(KeyCode.Mouse0)){
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

    }

    

}
