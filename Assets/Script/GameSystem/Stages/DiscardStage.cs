using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DiscardStage : NetworkBehaviour
{
    public Dictionary<Player, int> discardCount = new Dictionary<Player, int>();
    [ClientRpc]
    public void StartStageClientRpc()
    {
        discardCount.Clear();
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
        //TurnbasedSystem.Instance.CompleteStage(GameStage.DiscardStage);
    }
}
