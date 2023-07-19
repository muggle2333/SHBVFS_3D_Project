using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DiscardStage : NetworkBehaviour
{
    public Dictionary<Player, int> discardCount = new Dictionary<Player, int>();
    public NetworkVariable<float> timerValue = new NetworkVariable<float>(0f);
    public event EventHandler OnStartDiscardStage;
    public event EventHandler OnStartSelfDiscardStage;
    public event EventHandler OnCompleteDiscardStage;
    public void StartStage()
    {
        if(CheckShouldDiscard())
        {
            StartCoroutine("StartDiscard");
            return;
        }
        else
        {
            TurnbasedSystem.Instance.CompleteStage(GameStage.DiscardStage);
        }
    }
    private void Update()
    {
        if(timerValue.Value>0)
        {
            timerValue.Value -= Time.deltaTime;
        }else
        {
            timerValue.Value = 0;
        }
    }
    IEnumerator StartDiscard()
    {
        timerValue.Value = GameplayManager.DISCARD_TIMER;
        StartDiscardStageClientRpc();
        //Discard Timer -> Free Discard
        while (timerValue.Value>0)
        {
            if(!CheckShouldDiscard())
            {
                break;
            }
            yield return null;
        }
        if(CheckShouldDiscard())
        {
            AutoDiscardClientRpc();
            yield return new WaitForSeconds(0.5f);
        }
        CompleteDiscardStageClientRpc();
        TurnbasedSystem.Instance.CompleteStage(GameStage.DiscardStage);
    }

    private bool CheckShouldDiscard()
    {
        foreach(var discardNum in CardManager.Instance.CheckDiscardNum())
        {
            if(discardNum>0)
            {
                return true;
            }
        }
        return false;
    }


    [ClientRpc]
    public void StartDiscardStageClientRpc()
    {
        OnStartDiscardStage?.Invoke(this, EventArgs.Empty);
        //Debug.Log("Now enter discard stage");
        //GameplayManager.Instance.gameplayUI.playCard.gameObject.SetActive(false);
        //GameplayManager.Instance.gameplayUI.cancel.gameObject.SetActive(false);
        UIManager.Instance.SetGameplayPlayUI(GameplayUIType.playCard, false);
        UIManager.Instance.SetGameplayPlayUI(GameplayUIType.cancelControl, false);
        discardCount[GameplayManager.Instance.currentPlayer] = CardManager.Instance.playerHandCardDict[GameplayManager.Instance.currentPlayer].Count - GameplayManager.Instance.currentPlayer.HP;
        if (discardCount[GameplayManager.Instance.currentPlayer] > 1)
        {
            PlayerManager.Instance.cardSelectManager.maxSelected[GameplayManager.Instance.currentPlayer] = discardCount[GameplayManager.Instance.currentPlayer];
        }
        if(discardCount[GameplayManager.Instance.currentPlayer] > 0)
        {
            OnStartSelfDiscardStage?.Invoke(this, EventArgs.Empty);
        }


    }
    [ClientRpc]
    public void AutoDiscardClientRpc()
    {
        Player player = GameplayManager.Instance.currentPlayer;
        if (GameplayManager.Instance.discardStage.discardCount[player] > 0)
        {
            for (int i = 0; i < GameplayManager.Instance.discardStage.discardCount[player]; i++)
            {
                CardManager.Instance.playerHandCardDict[player][0].gameObject.GetComponent<CardSelectComponent>().CardDiscardAnimation();
                CardManager.Instance.playerHandCardDict[player].RemoveAt(0);
                CardManager.Instance.RemoveCardFromPlayerHandServerRpc(player.Id, 0);
            }
            foreach (var card in CardManager.Instance.playerHandCardDict[player])
            {
                card.GetComponent<CardSelectComponent>().EndSelectDiscard();
            }
            PlayerManager.Instance.cardSelectManager.SelectCount[player] = 0;
            PlayerManager.Instance.cardSelectManager.UpdateCardPos(player);
        }
        UIManager.Instance.SetGameplayPlayUI(GameplayUIType.discardCards, false);
        UIManager.Instance.SetGameplayPlayUI(GameplayUIType.cancelDiscard, false);
    }

    [ClientRpc]
    private void CompleteDiscardStageClientRpc()
    {
        OnCompleteDiscardStage?.Invoke(this, EventArgs.Empty);
    }
}
