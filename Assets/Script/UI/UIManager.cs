using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : NetworkBehaviour
{
    public static UIManager Instance { get; private set; }
    private PlayerDataUI playerDataUI;
    private GridObjectUI gridObjectUI;
    private MessageUI messageUI;
    private GameplayUI gameplayUI;
    private PauseMenuUI pauseMenuUI;
    private TurnbaseUI turnbaseUI;
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
        messageUI= GetComponentInChildren<MessageUI>();
        gameplayUI= GetComponentInChildren<GameplayUI>();
        pauseMenuUI= GetComponentInChildren<PauseMenuUI>();
        turnbaseUI = GetComponentInChildren<TurnbaseUI>();
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
    public void StartTurnbaseUIClientRpc()
    {
        turnbaseUI.StartTurnbaseUI();
    }
    [ClientRpc]
    public void ShowMessageInfoClientRpc(string info,ClientRpcParams clientRpcParams=default)
    {
        messageUI.ShowMessageInfo(info);
    }
    [ClientRpc]
    public void ShowMessageTimerClientRpc(float timer,ClientRpcParams clientRpcParams=default)
    {
        messageUI.ShowMessageTimer(timer);
    }
    [ClientRpc]
    public void HideMessageClientRpc(ClientRpcParams clientRpcParams = default)
    {
        messageUI.HideMessage();
    }

    public void SetGameplayPlayUI(GameplayUIType gameplayUItype, bool isActive)
    {
        switch(gameplayUItype)
        {
            case GameplayUIType.playCard:
                gameplayUI.playCard.gameObject.SetActive(isActive); break;
            case GameplayUIType.cancelControl:
                gameplayUI.cancelControl.gameObject.SetActive(isActive); break;
            case GameplayUIType.discardCards:
                gameplayUI.discardCards.gameObject.SetActive(isActive); break;
            case GameplayUIType.cancelDiscard:
                gameplayUI.cancelDiscard.gameObject.SetActive(isActive); break;
            case GameplayUIType.playHP:
                gameplayUI.playHP.gameObject.SetActive(isActive); break;
            case GameplayUIType.cancelDying:
                gameplayUI.cancelDying.gameObject.SetActive(isActive); break;
        }
    }
    public void SetGameplayPlayUIInteractable(GameplayUIType gameplayUItype, bool isActive)
    {
        switch (gameplayUItype)
        {
            case GameplayUIType.playCard:
                gameplayUI.playCard.gameObject.GetComponent<Button>().interactable = isActive; break;
            case GameplayUIType.discardCards:
                gameplayUI.discardCards.gameObject.GetComponent<Button>().interactable = isActive; break;
            case GameplayUIType.playHP:
                gameplayUI.playHP.gameObject.GetComponent<Button>().interactable = isActive; break;

        }
    }

}
