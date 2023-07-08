using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;
    public Dictionary<Player, List<Card>> playerHandCardDict = new Dictionary<Player, List<Card>>();
    public Dictionary<Player, List<Card>> playedCardDict = new Dictionary<Player, List<Card>>();
    public Calculating calculating;
    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
        }
    }
    void Start()
    {
        List<Player> playerList = new List<Player>();
        playerList = GameplayManager.Instance.playerList;
        for(int i = 0; i < playerList.Count; i++)
        {
            playedCardDict.Add(playerList[i], new List<Card>());  
            playerHandCardDict.Add(playerList[i], new List<Card>());
        }
        calculating = FindObjectOfType<Calculating>();
    }
    public void ImmediateCardTakeEffect(Player player)
    {

    }
    public void S1CardTakeEffect(Player player)
    {
        for(int i = 0; i < playedCardDict[player].Count; i++)
        {
            if (playedCardDict[player][i].effectStage == EffectStage.S1)
            {
                if (playedCardDict[player][i].cardFounction != null)
                {
                    Instantiate(playedCardDict[player][i].cardFounction);
                }
                calculating.DelataCardData(playedCardDict[player][i], player);
                calculating.CalculatPlayerBaseData(player);
                calculating.CalaulatPlayerData(player);
                playedCardDict[player].RemoveAt(i);
            }
        }
    }
    public void CardTakeEffect(Player player,EffectStage stage)
    {
        for (int i = 0; i < playedCardDict[player].Count; i++)
        {
            if (playedCardDict[player][i].effectStage == stage)
            {
                if (playedCardDict[player][i].cardFounction != null)
                {
                    Instantiate(playedCardDict[player][i].cardFounction);
                }
                playedCardDict[player][i].gameObject.GetComponent<CardSelectComponent>().CardTakeEffectAnimation();
                Debug.Log(player);
                Debug.Log(playedCardDict[player][i]);
                
                calculating.DelataCardData(playedCardDict[player][i], player);
                calculating.CalculatPlayerBaseData(player);
                calculating.CalaulatPlayerData(player);
                playedCardDict[player].RemoveAt(i);
                break;
            }
        }
    }
}
