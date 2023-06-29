using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caculating : MonoBehaviour
{
    protected int CaculatingPlayerHp;
    protected int CaculatingPlayerActionPoint;
    protected int CaculatingPlayerMaxHp;
    protected int CaculatingPlayerMaxActionPoint;
    protected int CaculatingPlayerAttack;
    protected int CaculatingPlayerDefense;
    protected Card CardData; 

     void Start()
    {
        CaculatingPlayerHp = GetComponent<Player>().HP;
        CaculatingPlayerActionPoint = GetComponent<Player>().ActionPointPerRound;
    }
    void DelataCardData (Card card)
    {
        CardData = card;       
    }    

    void AcademyBuff()
    {

    }

    


}
