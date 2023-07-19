using System;
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
        //if(NetworkManager.Singleton.IsServer)
        //{
        //    NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnect;
        //}
        GetComponent<PlayerInteractionComponent>().SetPlayerName(IsOwner);
    }

    private void NetworkManager_OnClientDisconnect(ulong clientId)
    {
        //if(clientId == GetComponent<NetworkObject>().OwnerClientId)
        //{
        //    Destroy(gameObject);
        //}
    }



}
