using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    #region Data Network
    public struct MyCustomData : INetworkSerializable
    {
        public int _int;
        public bool _bool;
        public FixedString128Bytes message; //string CANT in the networkvariable

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _int);
            serializer.SerializeValue(ref _bool);
            serializer.SerializeValue(ref message);

        }
    }
    //NectworkVariable Must be value type
    private NetworkVariable<int> randomNum = new NetworkVariable<int>(1,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
    private NetworkVariable<MyCustomData> customData = new NetworkVariable<MyCustomData>(
        new MyCustomData
        {
            _int = 10,
            _bool = true,
            message ="null",
        }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    #endregion


    #region ObjectNetwork
    [SerializeField] private Transform spawnObjectPrefab;
    private Transform spawnObjectTransform;
    #endregion

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        randomNum.OnValueChanged += (int previousValue, int newValue) =>
        {
            //Debug.Log(OwnerClientId + " Id:" + randomNum.Value);
        };
        customData.OnValueChanged += (MyCustomData previousValue, MyCustomData newValue) =>
        {
            Debug.Log(OwnerClientId + " Id:" + newValue._int +" + "+newValue._bool+" + " +newValue.message);
        };

    }
    void Update()
    {
        if (!IsOwner) return;

        //Movement
        Vector3 moveDir = new Vector3(Input.GetAxis("Horizontal"),0, Input.GetAxis("Vertical"));
        float moveSpeed = 3f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        //SysNum
        if(Input.GetMouseButtonDown(0))
        {
            //TestServerRpc(new ServerRpcParams());

            //TestClientRpc(new ClientRpcParams { Send = new ClientRpcSendParams {TargetClientIds=new List<ulong>{1}} });

            /*randomNum.Value= Random.Range(0,100);
            customData.Value = new MyCustomData 
            {
                _int = Random.Range(0, 100),
                _bool = false,
                message = "TEST",
            };*/

            spawnObjectTransform = Instantiate(spawnObjectPrefab);
            //spawnObjectTransform.GetComponent<NetworkObject>().Spawn(true);
        }
        
        if(Input.GetMouseButtonDown(1))
        {
            spawnObjectTransform.GetComponent<NetworkObject>().Despawn(true);
            //Destroy(spawnObjectTransform.gameObject);
        }
    } 



    //Only run on server from both server and client
    [ServerRpc]
    private void TestServerRpc(ServerRpcParams serverRpcParams) //Must End with ServerRpc
    {
        Debug.Log("TestServerRpc" + OwnerClientId+" + "+serverRpcParams.Receive.SenderClientId);
    }


    //Send the message from the server and to the client
    [ClientRpc]
    private void TestClientRpc(ClientRpcParams clientRpcPrams) //Can set the clientRpc to send the specific client,eg new ClientRpcParams { Send = new ClientRpcSendParams {TargetClientIds=new List<ulong>{1}} }
    {
        Debug.Log("TestClientRpc" + OwnerClientId);
    }
}
