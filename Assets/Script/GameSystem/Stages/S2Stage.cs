using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class S2Stage : MonoBehaviour
{
    private Dictionary<Player, List<int>> playedCardDict = new Dictionary<Player, List<int>>();
    private List<Player> playerList = new List<Player>();
    public List<int> redPlayerNeedsToEffect = new List<int>();
    public List<int> bluePlayerNeedsToEffect = new List<int>();
    public void StartStage(Dictionary<Player,List<Card>> playedCardListDict)
    {
        PlayerManager.Instance.cardSelectManager.maxSelected[GameplayManager.Instance.currentPlayer] = 1;
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
        for (int i = 0; i < playedCardListDict[GameplayManager.Instance.playerList[0]].Count; i++)
        {
            if (playedCardListDict[GameplayManager.Instance.playerList[0]][i].effectStage == EffectStage.S2)
            {
                redPlayerNeedsToEffect.Add(playedCardListDict[GameplayManager.Instance.playerList[0]][i].cardId);
                CardManager.Instance.RemovePlayedCardServerRpc(PlayerId.RedPlayer,playedCardListDict[GameplayManager.Instance.playerList[0]][i].cardId);
            }
        }
        for (int i = 0; i < playedCardListDict[GameplayManager.Instance.playerList[1]].Count; i++)
        {
            if (playedCardListDict[GameplayManager.Instance.playerList[1]][i].effectStage == EffectStage.S2)
            {
                bluePlayerNeedsToEffect.Add(playedCardListDict[GameplayManager.Instance.playerList[1]][i].cardId);
                CardManager.Instance.RemovePlayedCardServerRpc(PlayerId.BluePlayer, playedCardListDict[GameplayManager.Instance.playerList[0]][i].cardId);
            }
        }
        if(redPlayerNeedsToEffect.Count > 0)
        {
            playedCardDict.Add(GameplayManager.Instance.playerList[0], redPlayerNeedsToEffect);
        }
        if (bluePlayerNeedsToEffect.Count > 0)
        {
            playedCardDict.Add(GameplayManager.Instance.playerList[1], bluePlayerNeedsToEffect);
        }
        playerList = new List<Player>();
        playerList = GameplayManager.Instance.GetPlayer();
        StartCoroutine("S2CardTakeEffect");

    }
    IEnumerator S2CardTakeEffect()
    {
        List<Player> priorityList = playerList.OrderByDescending(x => x.Priority).ToList();
        while (playedCardDict.Count != 0)
        {
            for (int i = 0; i < priorityList.Count; i++)
            {
                if (playedCardDict[priorityList[i]].Count == 0)
                {
                    playedCardDict.Remove(priorityList[i]);
                    break;
                }
                yield return new WaitForSeconds(1);
                CardManager.Instance.CardTakeEffectClientRpc(priorityList[i].Id, playedCardDict[priorityList[i]][0]);
                playedCardDict[priorityList[i]].RemoveAt(0);
            }
        }

        List<Player> dyingPlayers = new List<Player>();
        List<Player> alivePlayers = new List<Player>();
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].HP <= 0)
            {
                dyingPlayers.Add(playerList[i]);
            }
            else
            {
                alivePlayers.Add(playerList[i]);
            }
        }
        if(dyingPlayers.Count == 0)
        {
            TurnbasedSystem.Instance.isDie.Value = false;
        }
        else
        {
            TurnbasedSystem.Instance.isDie.Value = true;
            PlayerManager.Instance.PlayerDying(dyingPlayers, alivePlayers);
        }
        TurnbasedSystem.Instance.CompleteStage(GameStage.S2);

    }
}
