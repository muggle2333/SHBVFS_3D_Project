using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkComponent : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        //if(IsOwner)
        //{
        //    GameplayManager.Instance.SetCurrentPlayer(GetComponent<Player>());
        //}
        GetComponent<Player>().Id = (PlayerId)GetComponent<NetworkObject>().OwnerClientId;
    }

    public void Update()
    {

    }

}
