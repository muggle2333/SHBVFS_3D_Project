using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DAO5 : CardFunction
{
    private bool isUnlock = false;
    void Start()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            if (player.Priority == 1)
            {
                Destroy(gameObject);
            }
            Function();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        if(TurnbasedSystem.Instance.CurrentGameStage.Value == GameStage.S4 && isUnlock == false)
        {
            isUnlock = true;
            Function();
            Destroy(gameObject);
        }
    }
    void Function()
    {
        GameplayManager.Instance.ChangePlayerPriorityServerRpc();
    }
}
