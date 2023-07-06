using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class NetworkPlayerMovement : NetworkBehaviour
{
    public string pName;
    public float speed;
    public Transform objectToSpawn;
    public static NetworkPlayerMovement Instance;


    private void Start() {

        Instance = this;
    }
    

    void Update()
    {
        if(!IsOwner) return;
        Vector3 dir = new Vector3(Input.GetAxisRaw("Horizontal") , 0 , Input.GetAxisRaw("Vertical"));
        transform.position += dir.normalized * speed * Time.deltaTime;
        if(Input.GetKeyDown(KeyCode.Q)) testServerRpc();

    }

    [ServerRpc] // jest jeszcze client rpc   serwer rpc rykonuje sie tylko na serwerze, a client najpierw na serwerze, a pozniej na clientach;
    private void testServerRpc(string message){
        Debug.Log(OwnerClientId + " send a message: " + message);
    }
    [ServerRpc]
    private void testServerRpc(){
        Transform spawnedObject = Instantiate(objectToSpawn, new Vector3(transform.position.x, 10, transform.position.z), Quaternion.identity);
        spawnedObject.GetComponent<NetworkObject>().Spawn(true);
    }
}
