using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectStage
{
    Every,
    S1,
    S2,
    S3,
    S4,
}
public enum CardTarget
{
    self,
    opponent,
    both,
    land,
}
public enum CardType
{
    Basic,
    Data,
    Buff,
    Function,
    Nulll,
}

public enum CardLevel
{
    Primary,
    Top,
}

[Serializable]
public struct PlayerDataEffect
{
    public int hp;
    public int actionPoint;
    public int defence;
    public int attack;
    public int visionRange;
    public int addBaseCardNum;
    public int freeMoveNum;
}
[CreateAssetMenu(fileName ="Card", menuName ="Card")]
public class CardSetting : ScriptableObject
{
    public int cardId;
    public string cardName;
    public string description;
    public Sprite headPicture;
    public AcademyType academyType;
    public CardCondition cardCondition;
    public EffectStage effectStage;
    public int effectDuration;
    public CardTarget cardTarget;
    
    //生效次数1、x
    public int effectTime;

    public int[] academyEffectNum = new int[6];

    public int Damage;

    public PlayerDataEffect playerDataEffect;
    public CardType cardType;
    public CardLevel cardLevel;
    public CardBuff CardBuff;

}
