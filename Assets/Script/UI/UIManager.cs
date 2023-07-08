using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Netcode;
using UnityEngine;

public class UIManager : NetworkBehaviour
{
    public static UIManager Instance { get; private set; }

    private PlayerDataUI playerDataUI;
    private GridObjectUI gridObjectUI;
    private MessageUI MessageUI;
    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
        }

    }
    public void Start()
    {
        playerDataUI= GetComponentInChildren<PlayerDataUI>();
        gridObjectUI= GetComponentInChildren<GridObjectUI>();
        MessageUI= GetComponentInChildren<MessageUI>();
    }
    public void UpdatePlayerDataUI(Player player)
    {
        playerDataUI.UpdatePlayerData(player);
    }
    public void UpdateGridObjectUI(GridObject gridObject, PlayerInteractAuthority authority)
    {
        gridObjectUI.UpdateGridObjectUIData(gridObject, authority);
    }

    public void ShowGridObjectUI(bool isShow,Transform gridPos)
    {
        gridObjectUI.ShowGridObjectUI(isShow,gridPos);
    }
    [ClientRpc]
    public void ShowMessageInfoClientRpc(string info,ClientRpcParams clientRpcParams=default)
    {
        MessageUI.ShowMessageInfo(info);
    }
    [ClientRpc]
    public void ShowMessageTimerClientRpc(float timer,ClientRpcParams clientRpcParams=default)
    {
        MessageUI.ShowMessage(timer);
    }
    [ClientRpc]
    public void HideMessageTimerClientRpc(ClientRpcParams clientRpcParams = default)
    {
        MessageUI.HideMessage();
    }

}
