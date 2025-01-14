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
    public void Start()
    {
        playedCardDict.Clear();
    }
    public void StartStage(Dictionary<Player, List<CardSetting>> playedCardListDict)
    {
        for (int i = 0; i < playedCardListDict[GameplayManager.Instance.playerList[0]].Count; i++)
        {
            if (playedCardListDict[GameplayManager.Instance.playerList[0]][i].effectStage == EffectStage.S4)
            {
                redPlayerNeedsToEffect.Add(playedCardListDict[GameplayManager.Instance.playerList[0]][i].cardId);
                //CardManager.Instance.RemovePlayedCardServerRpc(PlayerId.RedPlayer, playedCardListDict[GameplayManager.Instance.playerList[0]][i].cardId);
            }
        }
        for (int i = 0; i < playedCardListDict[GameplayManager.Instance.playerList[1]].Count; i++)
        {
            if (playedCardListDict[GameplayManager.Instance.playerList[1]][i].effectStage == EffectStage.S4)
            {
                bluePlayerNeedsToEffect.Add(playedCardListDict[GameplayManager.Instance.playerList[1]][i].cardId);
                //CardManager.Instance.RemovePlayedCardServerRpc(PlayerId.BluePlayer, playedCardListDict[GameplayManager.Instance.playerList[1]][i].cardId);
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
        yield return new WaitForSeconds(1);
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
                    
                    CardManager.Instance.CardTakeEffectClientRpc(priorityList[i].Id, playedCardDict[priorityList[i]][0]);
                    playedCardDict[priorityList[i]].RemoveAt(0);
                    //Check dying
                    yield return new WaitForSeconds(3);
                    DyingManager.Instance.CheckIsDying();
                    yield return new WaitForSecondsRealtime(0.2f);
                    yield return new WaitUntil(() => TurnbasedSystem.Instance.isDie.Value == false);
                }
            }
        }
        GameplayManager.Instance.ChangePlayerPriorityClientRpc();
        yield return new WaitForSeconds(0.2f);
        GameplayManager.Instance.AddPlayerHpClientRpc();
        GameplayManager.Instance.DrawEventCardPerRoundClientRpc(0);
        yield return new WaitForSeconds(0.2f);
        GameplayManager.Instance.DrawEventCardPerRoundClientRpc(1);
        yield return new WaitForSeconds(0.2f);
        TurnbasedSystem.Instance.CompleteStage(GameStage.S4);
    }
}
