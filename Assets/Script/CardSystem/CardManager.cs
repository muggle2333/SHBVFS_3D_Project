using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;


    public Dictionary<Player,List<Card>> playedCard = new Dictionary<Player, List<Card>>();
    public Caculating calculating;
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
        playerList = FindObjectOfType<GameplayManager>().GetPlayer();
        for(int i = 0; i < playerList.Count; i++)
        {
            playedCard.Add(playerList[i], new List<Card>());  
        }
        calculating = FindObjectOfType<Caculating>();
    }
    public void AddPlayedCard(Card card,Player player)
    {
        playedCard[player].Add(card);
    }
    public void ImmediateCardTakeEffect(Player player)
    {

    }
    public void S1CardTakeEffect(Player player)
    {
        for(int i = 0; i < playedCard[player].Count; i++)
        {
            if (playedCard[player][i].effectStage == EffectStage.S1)
            {
                calculating.DelataCardData(playedCard[player][i], player);
                calculating.CalculatPlayerBaseData(player);
                calculating.CalaulatPlayerData(player);
                playedCard[player].RemoveAt(i);
            }
        }
    }
    public void CardTakeEffect(Player player,EffectStage stage)
    {
        for (int i = 0; i < playedCard[player].Count; i++)
        {
            if (playedCard[player][i].effectStage == stage)
            {
                calculating.DelataCardData(playedCard[player][i], player);
                calculating.CalculatPlayerBaseData(player);
                calculating.CalaulatPlayerData(player);
                playedCard[player].RemoveAt(i);
            }
        }
    }
}
