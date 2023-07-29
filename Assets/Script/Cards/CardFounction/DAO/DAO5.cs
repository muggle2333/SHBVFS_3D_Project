using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DAO5 : MonoBehaviour
{
    private bool isUnlock = false;
    void Start()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            FunctionServerRpc();
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
            FunctionServerRpc();
            Destroy(gameObject);
        }
    }
    [ServerRpc]
    void FunctionServerRpc()
    {
        GameplayManager.Instance.ChangePlayerPriorityClientRpc();
    }
}
