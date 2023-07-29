using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FA5 : CardFunction
{
    // Start is called before the first frame update
    void Start()
    {
        Function();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Function()
    {
        if (GameplayManager.Instance.currentPlayer == player)
        {
            DrawCardComponent.Instance.DrawBasicCard(player);
        }
        if (NetworkManager.Singleton.IsServer)
        {
            DrawCardComponent.Instance.PlayDrawCardAnimationServerRpc(player.Id, 1);
        }
        Destroy(gameObject);
    }
}
