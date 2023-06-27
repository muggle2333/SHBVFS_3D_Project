using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




public class Card:MonoBehaviour
{
    public CardSetting cardSetting;

    public int cardId;
    public Text cardName;
    public Text description;
    public Sprite headPicture;
    public AcademyType academyType;
    public CardCondition cardCondition;
    public EffectStage effectStage;
    public int effectDuration;
    public CardTarget cardTarget;

    public int effectTime;

    public int[] academyEffectNum = new int[6];

    public int Damage;

    public PlayerDataEffect playerDataEffect;
    public CardType cardType;
    public CardLevel cardLevel;
    public CardBuff CardBuff;
    void Start()
    {
        cardId = cardSetting.cardId;
        cardName.text = cardSetting.cardName;
        description.text = cardSetting.description;
        headPicture = cardSetting.headPicture;
        academyType = cardSetting.academyType;
        cardCondition = cardSetting.cardCondition;
        effectStage = cardSetting.effectStage;
        effectDuration = cardSetting.effectDuration;
        cardTarget = cardSetting.cardTarget;
        effectTime = cardSetting.effectTime;
        academyEffectNum = cardSetting.academyEffectNum;
        Damage = cardSetting.Damage;
        playerDataEffect = cardSetting.playerDataEffect;
        cardType = cardSetting.cardType;
        cardLevel = cardSetting.cardLevel;
        CardBuff = cardSetting.CardBuff;
    }

    public void UpdateCardData()
    {

    }
    public void ActivateCard()
    {

    }

}
