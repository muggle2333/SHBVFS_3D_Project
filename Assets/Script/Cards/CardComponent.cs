using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Card", menuName ="Card")]
public class CardComponent : ScriptableObject
{
    public new string CardName;
    public string Description;

    public Sprite HeadPicture;
    public Sprite Academies;

    public int Damage;
    public int LoseHp;
    public int Defense;
    public int Health;
    public int VisionRange;
    public int ActionPoint;
    public int Attack;
    public int GetCardsNumber;
    public int FreeMoves;

    public PlayerId Cardtarget;

    public AcademyType AcademyType;
    
    public CardBuff CardBuff;

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
}
