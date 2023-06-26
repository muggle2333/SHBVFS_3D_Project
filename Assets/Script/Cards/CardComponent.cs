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
    public PlayerId Cardtarget;
    
    public CardBuff cardBuff;
}
