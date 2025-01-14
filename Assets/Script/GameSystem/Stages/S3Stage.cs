using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class S3Stage : MonoBehaviour
{
    private Dictionary<Player, List<int>> playedCardDict = new Dictionary<Player, List<int>>();
    private List<Player> playerList = new List<Player>();
    public List<int> redPlayerNeedsToEffect = new List<int>();
    public List<int> bluePlayerNeedsToEffect = new List<int>();
    private int i;
    public void Start()
    {
        playedCardDict.Clear();
    }
    public void StartStage(Dictionary<Player, List<CardSetting>> playedCardListDict)
    {
        for (int i = 0; i < playedCardListDict[GameplayManager.Instance.playerList[0]].Count; i++)
        {
            if (playedCardListDict[GameplayManager.Instance.playerList[0]][i].effectStage == EffectStage.S3)
            {
                redPlayerNeedsToEffect.Add(playedCardListDict[GameplayManager.Instance.playerList[0]][i].cardId);
                //CardManager.Instance.RemovePlayedCardServerRpc(PlayerId.RedPlayer, playedCardListDict[GameplayManager.Instance.playerList[0]][i].cardId);
            }
        }
        for (int i = 0; i < playedCardListDict[GameplayManager.Instance.playerList[1]].Count; i++)
        {
            if (playedCardListDict[GameplayManager.Instance.playerList[1]][i].effectStage == EffectStage.S3)
            {
                bluePlayerNeedsToEffect.Add(playedCardListDict[GameplayManager.Instance.playerList[1]][i].cardId);
                //CardManager.Instance.RemovePlayedCardServerRpc(PlayerId.BluePlayer, playedCardListDict[GameplayManager.Instance.playerList[1]][i].cardId);
            }
        }
        if (redPlayerNeedsToEffect.Count > 0)
        {
            Debug.LogError("S3 RED" + redPlayerNeedsToEffect.Count);
            playedCardDict.Add(GameplayManager.Instance.playerList[0], redPlayerNeedsToEffect);
        }
        if (bluePlayerNeedsToEffect.Count > 0)
        {
            Debug.LogError("S3 BLUE" + bluePlayerNeedsToEffect.Count);
            playedCardDict.Add(GameplayManager.Instance.playerList[1], bluePlayerNeedsToEffect);
        }
        playerList = new List<Player>();
        playerList = GameplayManager.Instance.GetPlayer();
        StartCoroutine("S3CardTakeEffect");

    }
    IEnumerator S3CardTakeEffect()
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
                    yield return new WaitForSeconds(3);
                    //Check dying
                    /*List<Player> dyingPlayers = GameplayManager.Instance.GetDyingPlayer();
                    if (dyingPlayers.Count == 0)
                    {
                        TurnbasedSystem.Instance.isDie.Value = false;
                    }
                    else
                    {
                        TurnbasedSystem.Instance.isDie.Value = true;
                        GameplayManager.Instance.PlayerDyingStageClientRpc();
                        yield return new WaitForSeconds(GameplayManager.DYING_TIMER);
                        dyingPlayers = GameplayManager.Instance.GetDyingPlayer();
                        if (dyingPlayers.Count > 0)
                        {
                            GameManager.Instance.SetGameOver();
                        }
                        else
                        {
                            TurnbasedSystem.Instance.isDie.Value = false;
                            GameplayManager.Instance.LeaveDyingStageClientRpc();
                        }
                        DyingManager.Instance.StartDyingStage();
                    }
                    yield return new WaitUntil(() => TurnbasedSystem.Instance.isDie.Value == false);*/
                    DyingManager.Instance.CheckIsDying();
                    yield return new WaitForSecondsRealtime(0.2f);
                    yield return new WaitUntil(() => TurnbasedSystem.Instance.isDie.Value == false);
                }
            }
        }
       
        TurnbasedSystem.Instance.CompleteStage(GameStage.S3);

    }

}
