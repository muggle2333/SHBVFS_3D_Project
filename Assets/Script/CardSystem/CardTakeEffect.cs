using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardTakeEffect : MonoBehaviour
{
    public Dictionary<EffectStage,List<Card>> playedCard = new Dictionary<EffectStage,List<Card>>();
    public Caculating calculating;
    void Start()
    {
        for(int i = 0; i < (int)EffectStage.S4+1; i++)
        {
            playedCard.Add((EffectStage)i, new List<Card>());
        }
        calculating = FindObjectOfType<Caculating>();
    }
    public void CheckEffectiveStage(Card card)
    {
        playedCard[card.effectStage].Add(card);
    }
    public void ImmediateCardTakeEffect(Player player)
    {

    }
    public void S1CardTakeEffect(Player player)
    {
        calculating.DelataCardData(playedCard[EffectStage.S1][0]);
        calculating.CalaulatPlayerBaseData(player);
        calculating.CalaulatPlayerData(player);
        /*List<Card> cards = playedCard[EffectStage.S1];
        cards.RemoveAt(0);*/
        playedCard[EffectStage.S1].RemoveAt(0);
    }
    public void S2CardTakeEffect()
    {

    }
    public void S3CardTakeEffect()
    {

    }
    public void S4CardTakeEffect()
    {

    }
}
