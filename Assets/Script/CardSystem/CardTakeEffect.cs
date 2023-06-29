using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardTakeEffect : MonoBehaviour
{
    public Dictionary<EffectStage,List<Card>> playedCard = new Dictionary<EffectStage,List<Card>>();
    void Start()
    {
        for(int i = 0; i < (int)EffectStage.S4; i++)
        {
            playedCard.Add((EffectStage)i, new List<Card>());
        }
        
    }
    public void CheckEffectiveStage(Card card)
    {
        playedCard[card.effectStage].Add(card);
    }
    public void ImmediateCardTakeEffect()
    {

    }
    public void S1CardTakeEffect()
    {
        int a = playedCard[EffectStage.S1].Count;
        for(int i = 0; i < a; i++)
        {
            
        }
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
