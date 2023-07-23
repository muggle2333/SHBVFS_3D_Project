using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;




public class Card:MonoBehaviour
{
    public CardSetting cardSetting;

    public int cardId;
    public Text cardName;
    public Text description;
    public Image headPicture;
    public Texture2D cardTexture;
    public AcademyType academyType;
    public CardCondition cardCondition;
    public EffectStage effectStage;
    public int effectDuration;
    public CardTarget cardTarget;

    public int effectTime;

    public int[] academyEffectNum;

    public int Damage;

    public int needSelectCount;

    public PlayerDataEffect playerDataEffect;
    public CardType cardType;
    public CardLevel cardLevel;
    public CardBuff CardBuff;
    public GameObject cardFounction;
    void Start()
    {
        UpdateCardData(cardSetting);
    }

    public void UpdateCardData(CardSetting cardSetting)
    {
        cardTexture = cardSetting.cardTexture;
        academyEffectNum = new int[6];
        this.cardSetting = cardSetting;
        cardId = cardSetting.cardId;
        cardName.text = cardSetting.cardName;
        headPicture.sprite= cardSetting.headPicture;
        description.text = cardSetting.description;
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
        cardFounction = cardSetting.cardFounction;
        needSelectCount = cardSetting.needSelectCount;
    }
    public void ActivateCard()
    {

    }

}
