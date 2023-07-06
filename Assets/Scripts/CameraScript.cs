using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class CameraScript : NetworkBehaviour
{
    public GameObject cameraHolder;
    public Vector3 offset;
    private void Update() {
        if(!IsOwner) return;
        cameraHolder.transform.position = transform.position + offset;
    }
    

}
