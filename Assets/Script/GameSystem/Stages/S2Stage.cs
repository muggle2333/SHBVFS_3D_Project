using System;
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
    private int i;
    public void StartStage(Dictionary<Player,List<CardSetting>> playedCardListDict)
    {

        playedCardDict.Clear();
        Debug.LogError("S2");
        PlayerManager.Instance.cardSelectManager.maxSelected[GameplayManager.Instance.currentPlayer] = 1;
/*        if (GameplayManager.Instance.discardStage.discardCount[GameplayManager.Instance.player] > 0)
        {
            for (int i = 0; i < GameplayManager.Instance.discardStage.discardCount[GameplayManager.Instance.player]; i++)
            {
                CardManager.Instance.playerHandCardDict[GameplayManager.Instance.player][0].gameObject.GetComponent<CardSelectComponent>().CardDiscardAnimation();
                CardManager.Instance.playerHandCardDict[GameplayManager.Instance.player].RemoveAt(0);
            }
            foreach (var card in CardManager.Instance.playerHandCardDict[GameplayManager.Instance.player])
            {
                card.GetComponent<CardSelectComponent>().EndSelectDiscard();
            }
            PlayerManager.Instance.cardSelectManager.SelectCount[GameplayManager.Instance.player] = 0;
            PlayerManager.Instance.cardSelectManager.UpdateCardPos(GameplayManager.Instance.player);
        }*/
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
                CardManager.Instance.RemovePlayedCardServerRpc(PlayerId.BluePlayer, playedCardListDict[GameplayManager.Instance.playerList[1]][i].cardId);
            }
        }
        if(redPlayerNeedsToEffect.Count > 0)
        {
            Debug.LogError("S2 RED" + redPlayerNeedsToEffect.Count);
            playedCardDict.Add(GameplayManager.Instance.playerList[0], redPlayerNeedsToEffect);
        }
        if (bluePlayerNeedsToEffect.Count > 0)
        {
            Debug.LogError("S2 BLUE" + bluePlayerNeedsToEffect.Count);
            playedCardDict.Add(GameplayManager.Instance.playerList[1], bluePlayerNeedsToEffect);
        }
        playerList = new List<Player>();
        playerList = GameplayManager.Instance.GetPlayer();
        Calculating.Instance.CardDataInitializeClientRpc(playerList[0].Id);
        Calculating.Instance.CardDataInitializeClientRpc(playerList[1].Id);
        StartCoroutine("S2CardTakeEffect");

    }
    IEnumerator S2CardTakeEffect()
    {
        List<Player> priorityList = playerList.OrderByDescending(x => x.Priority).ToList();
        while (playedCardDict.Count != 0)
        {
            for (i = 0; i < priorityList.Count; i++)
            {
                if (playedCardDict.ContainsKey(priorityList[i]))
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
        }
        i = 0;
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
            //PlayerManager.Instance.PlayerDying(dyingPlayers, alivePlayers);

            GameplayManager.Instance.PlayerDyingStageClientRpc();
            yield return new WaitForSeconds(GameplayManager.DYING_TIMER);

            if(GetDyingPlayer().Count>0)
            {
                GameManager.Instance.SetGameOver();
            }
            else
            {
                GameplayManager.Instance.LeaveDyingStageClientRpc();
            }
        }
        TurnbasedSystem.Instance.CompleteStage(GameStage.S2);

    }

    private List<Player> GetDyingPlayer()
    {
        List<Player> dyingPlayers = new List<Player>();
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].HP <= 0)
            {
                dyingPlayers.Add(playerList[i]);
            }
        }
        return dyingPlayers;
    }
    private List<Player> GetAlivePlayer()
    {
        List<Player> alivePlayers = new List<Player>();
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].HP > 0)
            {
                alivePlayers.Add(playerList[i]);
            }
        }
        return alivePlayers;
    }
}
