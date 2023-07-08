using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardStage : MonoBehaviour
{
    public Dictionary<Player, int> discardCount = new Dictionary<Player, int>();
    public void StartStage()
    {
        GameplayManager.Instance.gameplayUI.playCard.gameObject.SetActive(false);
        GameplayManager.Instance.gameplayUI.cancel.gameObject.SetActive(false);
        discardCount[GameplayManager.Instance.currentPlayer] = CardManager.Instance.playerHandCardDict[GameplayManager.Instance.currentPlayer].Count - GameplayManager.Instance.currentPlayer.HP;
        if (discardCount[GameplayManager.Instance.currentPlayer] > 1)
        {
            PlayerManager.Instance.cardSelectManager.maxSelected[GameplayManager.Instance.currentPlayer] = discardCount[GameplayManager.Instance.currentPlayer];
        }
    }
}
