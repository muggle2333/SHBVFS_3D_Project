using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caculating : MonoBehaviour
{
    protected int academyMaxHP;
    protected int academyHPPerRound;
    protected int academyAttackRange;
    protected int academyAttackDamage;
    protected int academyDefense;
    protected int academyAPPerRound;

    
    protected int cardAttackDamage;
    protected int cardDefense;
    protected int cardAttackRange;

    protected int cardDamage;
    protected int cardAP;
    protected int cardHP;
    protected int cardFreeMoveNum;

    protected int totalCardAttackDamage;
    protected int totalCardDefense;
    protected int totalCardAttackRange;

    public int[] academyEffectNum = new int[6];

    protected AcademyBuffData AcademyBuffData;
    protected Card CardData; 

     void Start()
    {
        
    }
    public void DelataCardData (Card card)
    {
        totalCardAttackRange += card.playerDataEffect.visionRange;
        totalCardDefense += card.playerDataEffect.defence;
        totalCardAttackDamage += card.playerDataEffect.defence;

        academyEffectNum = card.academyEffectNum;

        cardDamage = card.Damage;
        cardAP = card.playerDataEffect.actionPoint;
        cardHP = card.playerDataEffect.hp;
    
    }    

    public void CardDataInitialize()
    {
      
        cardAttackDamage = 0;
        cardAttackRange = 0;
        cardDefense = 0;

        cardDamage = 0;
        cardAP = 0;
        cardHP = 0;
        cardFreeMoveNum = 0;

        for(int i = 0; i < 6 ; i++)
        {
            academyEffectNum[i] = 0;
        }
    }
    public void AcademyBuff(AcademyBuffData academyBuffData)
    {
        AcademyBuffData = academyBuffData;
        TotalAcademyBuff();
    }

    public void TotalAcademyBuff()
    {
        academyMaxHP = AcademyBuffData.maxHp;
        academyHPPerRound = AcademyBuffData.hpPreRound;
        academyAttackRange = AcademyBuffData.attackRange;
        academyAttackDamage = AcademyBuffData.attackDamage;
        academyDefense = AcademyBuffData.defense;
        academyAPPerRound = AcademyBuffData.APPerRound;
    }



    public void CalaulatPlayerBaseData(Player player)
    {
        for (int i = 0; i < player.academyOwnedPoint.Length; i++)
        {
            player.academyOwnedPoint[i] = player.academyOwnedPoint[i] + academyEffectNum[i];
        }
        FindObjectOfType<PlayerAcademyBuffcomponent>().UpdatePlayerAcademyBuff(player);

        player.MaxHP = 3 + academyMaxHP;
        player.AttackDamage = 1 + academyAttackDamage + totalCardAttackDamage;
        player.Range = 1 + academyAttackRange + totalCardAttackRange;
        player.Defence = academyDefense + totalCardDefense;
        player.ActionPointPerRound = 3 + academyAPPerRound;

        
    }

    public void CalaulatPlayerData(Player player)
    {
        player.HP += cardHP;

        if (player.HP > player.MaxHP)
        {
            player.HP = player.MaxHP;
        }

        if (cardDamage > player.Defence)
        {
            player.HP -= (cardDamage - player.Defence);
        }
        player.CurrentActionPoint += cardAP;
    }

}
