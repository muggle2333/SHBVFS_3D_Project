using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class S4Stage : MonoBehaviour
{
    private Dictionary<Player, List<int>> playedCardDict = new Dictionary<Player, List<int>>();
    private List<Player> playerList = new List<Player>();
    public List<int> redPlayerNeedsToEffect = new List<int>();
    public List<int> bluePlayerNeedsToEffect = new List<int>();
    private int i;
    //private Dictionary<Player,>;
    public void StartStage(Dictionary<Player, List<CardSetting>> playedCardListDict)
    {
        Debug.LogError("S4");
        for (int i = 0; i < playedCardListDict[GameplayManager.Instance.playerList[0]].Count; i++)
        {
            if (playedCardListDict[GameplayManager.Instance.playerList[0]][i].effectStage == EffectStage.S4)
            {
                redPlayerNeedsToEffect.Add(playedCardListDict[GameplayManager.Instance.playerList[0]][i].cardId);
                CardManager.Instance.RemovePlayedCardServerRpc(PlayerId.RedPlayer, playedCardListDict[GameplayManager.Instance.playerList[0]][i].cardId);
            }
        }
        for (int i = 0; i < playedCardListDict[GameplayManager.Instance.playerList[1]].Count; i++)
        {
            if (playedCardListDict[GameplayManager.Instance.playerList[1]][i].effectStage == EffectStage.S4)
            {
                bluePlayerNeedsToEffect.Add(playedCardListDict[GameplayManager.Instance.playerList[1]][i].cardId);
                CardManager.Instance.RemovePlayedCardServerRpc(PlayerId.BluePlayer, playedCardListDict[GameplayManager.Instance.playerList[1]][i].cardId);
            }
        }
        if (redPlayerNeedsToEffect.Count > 0)
        {
            Debug.LogError("S4 RED" + redPlayerNeedsToEffect.Count);
            playedCardDict.Add(GameplayManager.Instance.playerList[0], redPlayerNeedsToEffect);
        }
        if (bluePlayerNeedsToEffect.Count > 0)
        {
            Debug.LogError("S4 BLUE" + bluePlayerNeedsToEffect.Count);
            playedCardDict.Add(GameplayManager.Instance.playerList[1], bluePlayerNeedsToEffect);
        }
        playerList = new List<Player>();
        playerList = GameplayManager.Instance.GetPlayer();
        StartCoroutine("S4CardTakeEffect");

    }
    IEnumerator S4CardTakeEffect()
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
        if (dyingPlayers.Count == 0)
        {
            TurnbasedSystem.Instance.isDie.Value = false;
        }
        else
        {
            TurnbasedSystem.Instance.isDie.Value = true;
            //PlayerManager.Instance.PlayerDying(dyingPlayers, alivePlayers);
            GameplayManager.Instance.PlayerDyingStageClientRpc();
            yield return new WaitForSeconds(GameplayManager.DYING_TIMER);

            if (GetDyingPlayer().Count > 0)
            {
                GameManager.Instance.SetGameOver();
            }
            else
            {
                GameplayManager.Instance.LeaveDyingStageClientRpc();
            }
        }
        Calculating.Instance.CardDataInitializeClientRpc(playerList[0]);
        Calculating.Instance.CardDataInitializeClientRpc(playerList[1]);
        TurnbasedSystem.Instance.CompleteStage(GameStage.S4);

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
}
