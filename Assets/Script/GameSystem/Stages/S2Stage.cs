using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class S2Stage : MonoBehaviour
{
    private Dictionary<Player, List<Card>> playedCardDict = new Dictionary<Player, List<Card>>();
    private List<Player> playerList = new List<Player>();
    private int i;
    public void StartStage(Dictionary<Player,List<Card>> playerCardListDict)
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
                card.GetComponent<CardSelectComponent>().EndSelect();
            }
            PlayerManager.Instance.cardSelectManager.SelectCount[GameplayManager.Instance.currentPlayer] = 0;
            PlayerManager.Instance.cardSelectManager.UpdateCardPos(GameplayManager.Instance.currentPlayer);
        }
        playedCardDict = playerCardListDict;
        playerList = new List<Player>();
        StartCoroutine("S2CardTakeEffect");

    }
    IEnumerator S2CardTakeEffect()
    {
        for (int i = 0; i < playedCardDict.Count; i++)
        {
            playerList.Add(playedCardDict.ElementAt(i).Key);
        }
        List<Player> priorityList = playerList.OrderByDescending(x => x.Priority).ToList();
        while (playedCardDict[priorityList[i]].Count != 0)
        {
            for (int i = 0; i < priorityList.Count; i++)
            {
                List<Card> playedCard = null;
                if (playedCardDict.TryGetValue(priorityList[i], out playedCard))
                {
                    if (playedCard.Count == 0)
                    {
                        playedCardDict.Remove(priorityList[i]);
                        break;
                    }
                    //Interact 
                    //Debug.Log(priorityList[i].name + " " + playerInteract[0].PlayerInteractType);

                    yield return new WaitForSeconds(1);
                    CardManager.Instance.CardTakeEffect(priorityList[i], EffectStage.S2);


                    playedCard.RemoveAt(0);
                    playedCardDict[priorityList[i]] = playedCard;
                }
            }
        }
        TurnbasedSystem.Instance.TurnToNextStage();

    }
}
