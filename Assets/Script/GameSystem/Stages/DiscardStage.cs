using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DiscardStage : NetworkBehaviour
{
    public Dictionary<Player, int> discardCount = new Dictionary<Player, int>();
    public void StartStage()
    {
        StartStageClientRpc();
        TurnbasedSystem.Instance.CompleteStage(GameStage.DiscardStage);
    }
    [ClientRpc]
    public void StartStageClientRpc()
    {
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

        //Automatically discard 
        if (GameplayManager.Instance.discardStage.discardCount[GameplayManager.Instance.currentPlayer] > 0)
        {
            for (int i = 0; i < GameplayManager.Instance.discardStage.discardCount[GameplayManager.Instance.currentPlayer]; i++)
            {
                CardManager.Instance.playerHandCardDict[GameplayManager.Instance.currentPlayer][0].gameObject.GetComponent<CardSelectComponent>().CardDiscardAnimation();
                CardManager.Instance.playerHandCardDict[GameplayManager.Instance.currentPlayer].RemoveAt(0);
            }
            foreach (var card in CardManager.Instance.playerHandCardDict[GameplayManager.Instance.currentPlayer])
            {
                card.GetComponent<CardSelectComponent>().EndSelectDiscard();
            }
            PlayerManager.Instance.cardSelectManager.SelectCount[GameplayManager.Instance.currentPlayer] = 0;
            PlayerManager.Instance.cardSelectManager.UpdateCardPos(GameplayManager.Instance.currentPlayer);
        }

    }
}
