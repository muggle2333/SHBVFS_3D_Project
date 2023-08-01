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
    void Function()
    {
        if (GameplayManager.Instance.currentPlayer.Id == player.Id)
        {
            DrawCardComponent.Instance.DrawBasicCard(player);
        }
        player.HP--;
        if (NetworkManager.Singleton.IsServer)
        {
            DrawCardComponent.Instance.PlayDrawCardAnimationServerRpc(player.Id, 1);
        }
        Destroy(gameObject);
    }
}
