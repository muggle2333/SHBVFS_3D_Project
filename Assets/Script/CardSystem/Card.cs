using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card:MonoBehaviour
{
    public CardComponent card;

    public Text Cardname;
    public Text Description;

    public Sprite Head;
    public Sprite Academies;

    public int Damage;
    public int LoseHp;
    public int Defense;
    public int Health;
    public int VisionRange;
    public PlayerId Cardtarget;

    public CardBuff cardBuff;


    void Start()
    {
        Cardname.text = card.name;
        Description.text = card.Description;
        Head = card.HeadPicture;
        Academies = card.Academies;
        Damage = card.Damage;
        LoseHp = card.LoseHp;
        Defense = card.Defense;
        Health = card.Health;
        VisionRange = card.VisionRange;
        Cardtarget = card.Cardtarget;
        cardBuff = card.CardBuff;
    }
}
