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
    public Transform orientation;
    private Rigidbody rb;
    public Camera currnet;


    private void Start() {
        rb = GetComponent<Rigidbody>();
        Instance = this;
        if(!IsOwner) return;
        //Camera.SetupCurrent(currnet);
        currnet.enabled = true;
        currnet.GetComponent<AudioListener>().enabled = true;
    }
    

    void Update()
    {
        if(!IsOwner) return;
        /*Vector3 dir = new Vector3( , 0 , );
        tranform.position += dir.normalized * speed * Time.deltaTime;*/
        if(Input.GetKeyDown(KeyCode.Q)) testServerRpc("test");
        if(Input.GetKeyDown(KeyCode.M) && UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != SceneLoader.Scene.Game.ToString() && TestLobby.Instance.IsHost()) SceneLoader.LoadNetwork(SceneLoader.Scene.Game);

    }
    private void FixedUpdate() {
        if(!IsOwner) return;
        Vector3 dir = orientation.forward * Input.GetAxisRaw("Vertical") + orientation.right * Input.GetAxisRaw("Horizontal");
        rb.AddForce(dir.normalized * speed * 10f, ForceMode.Force);
        transform.rotation = orientation.rotation;
        
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
