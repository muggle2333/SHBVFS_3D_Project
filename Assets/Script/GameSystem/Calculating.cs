using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Calculating : MonoBehaviour
{
    public int APCost;

    public int academyMaxHP;
    public int academyHPPerRound;
    public int academyAttackRange;
    public int academyAttackDamage;
    public int academyDefense;
    public int academyAPPerRound;

    public int totalAcademyMaxHP;
    public int totalAcademyHPPerRound;
    public int totalAcademyAttackRange;
    public int totalAcademyAttackDamage;
    public int totalAcademyDefense;
    public int totalAcademyAPPerRound;


    public int cardAttackDamage;
    public int cardDefense;
    public int cardAttackRange;

    public int cardDamage;
    public int cardAP;
    public int cardHP;
    public int cardFreeMoveNum;

    public int totalCardAttackDamage;
    public int totalCardDefense;
    public int totalCardAttackRange;

    public int[] academyEffectNum = new int[6];
    public Dictionary<Player, int[]> totalAcademyEffectNum = new Dictionary<Player, int[]>();
    public AcademyBuffData AcademyBuffData;
    public Card CardData;

    public static Calculating Instance;

    public PlayerAcademyBuffcomponent playerAcademyBuffcomponent;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
        }
    }
    void Start()
    {
        totalAcademyEffectNum.Clear();
        playerAcademyBuffcomponent = FindObjectOfType<PlayerAcademyBuffcomponent>();
        Invoke("AddTotalAcademyEffectNum", 3);
        
    }
    public void AddTotalAcademyEffectNum()
    {
        totalAcademyEffectNum.Add(GameplayManager.Instance.playerList[0], new int[6]);
        totalAcademyEffectNum.Add(GameplayManager.Instance.playerList[1], new int[6]);
    }
    public void DelataCardData (CardSetting card,Player player)
    {
        totalCardAttackRange += card.playerDataEffect.visionRange;
        totalCardDefense += card.playerDataEffect.defence;
        totalCardAttackDamage += card.playerDataEffect.attack;

        player.cardAD = totalCardAttackDamage;
        player.cardAR = totalCardAttackRange;
        player.cardDF = totalCardDefense;

        academyEffectNum = card.academyEffectNum;
        for(int i = 0; i < 6; i++)
        {
            player.academyOwnedPoint[i] += academyEffectNum[i];
            totalAcademyEffectNum[player][i] += academyEffectNum[i];
        }
        
        playerAcademyBuffcomponent.UpdatePlayerAcademyBuff(player);

        
        cardDamage = card.Damage;
        cardAP = card.playerDataEffect.actionPoint;
        cardHP = card.playerDataEffect.hp;

    }
    [ClientRpc]    
    
    public void CardDataInitializeClientRpc(Player player)
    {
        cardAttackDamage = 0;
        cardAttackRange = 0;
        cardDefense = 0;

        cardDamage = 0;
        cardAP = 0;
        cardHP = 0;
        cardFreeMoveNum = 0;
        for (int i = 0; i < 6; i++)
        {
            player.academyOwnedPoint[i] -= totalAcademyEffectNum[player][i];
            
        }
        for (int i = 0; i < 6 ; i++)
        {
            academyEffectNum[i] = 0;
            totalAcademyEffectNum[player][i] = 0;
        }

        playerAcademyBuffcomponent.UpdatePlayerAcademyBuff(player);
    }
    public void AcademyBuff(Dictionary<AcademyType, AcademyBuffData> PlayerAcademyBuffDict,Player player)
    {
        for(int i = 0; i < 6; i++)
        {
            PlayerAcademyBuffDict.TryGetValue((AcademyType)(i + 1), out AcademyBuffData);
            academyMaxHP += AcademyBuffData.maxHp;
            academyHPPerRound += AcademyBuffData.hpPreRound;
            academyAttackRange += AcademyBuffData.attackRange;
            academyAttackDamage += AcademyBuffData.attackDamage;
            academyDefense += AcademyBuffData.defense;
            academyAPPerRound += AcademyBuffData.APPerRound;
        }

        totalAcademyMaxHP = academyMaxHP;
        totalAcademyHPPerRound = academyHPPerRound;
        totalAcademyAttackRange = academyAttackRange;
        totalAcademyAttackDamage = academyAttackDamage;
        totalAcademyDefense = academyDefense;
        totalAcademyAPPerRound = academyAPPerRound;

        academyMaxHP = 0;
        academyHPPerRound = 0;
        academyAttackRange = 0;
        academyAttackDamage = 0;
        academyDefense = 0;
        academyAPPerRound = 0;

        CalculatPlayerBaseData(player);
    }

    public void CalculatPlayerBaseData(Player player)
    {
        for (int i = 0; i < player.academyOwnedPoint.Length; i++)
        {
            player.academyOwnedPoint[i] = player.academyOwnedPoint[i] + academyEffectNum[i];
        }
        //FindObjectOfType<PlayerAcademyBuffcomponent>().UpdatePlayerAcademyBuff(player);

        player.MaxHP = player.baseMaxHP + totalAcademyMaxHP;
        player.AttackDamage = player.baseAttackDamage + totalAcademyAttackDamage + totalCardAttackDamage;
        player.Range = player.baseRange + totalAcademyAttackRange + totalCardAttackRange;
        player.Defence = player.baseDefense + totalAcademyDefense + totalCardDefense;
        player.ActionPointPerRound = player.baseActionPointPerRound + totalAcademyAPPerRound;
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

    public int CalculateAPCost(PlayerInteractType playerInteractType,Player player)
    {
        switch (playerInteractType)
        {
            case PlayerInteractType.Move:
                return CalculateMoveAPCost(player);
            case PlayerInteractType.Occupy:
                return 1;
            case PlayerInteractType.Build:
                return 2;
            case PlayerInteractType.Gacha:
                return 1;
            case PlayerInteractType.Search:
                return 1;
            default:
                return 0;
        }
    }

    public int CalculateMoveAPCost(Player player)
    {
        switch (player.currentGrid.landType)
        {
            case LandType.Mountain:
                switch (player.targetGrid.landType)
                {
                    case LandType.Lake:
                        if (player.freeMoveCount > 0)
                        {
                            player.freeMoveCount--;
                            return 0;
                        }
                        else
                        {
                            return 2;
                        }
                    case LandType.Forest:
                        if (player.freeMoveCount > 0)
                        {
                            player.freeMoveCount--;
                            return 0;
                        }
                        else
                        {
                            return 1;
                        }
                    case LandType.Mountain:
                        if (player.freeMoveCount > 0)
                        {
                            player.freeMoveCount--;
                            return 0;
                        }
                        else
                        {
                            return 1;
                        }
                    case LandType.Plain:
                        if (player.freeMoveCount > 0)
                        {
                            player.freeMoveCount--;
                            return 0;
                        }
                        else
                        {
                            if (player.targetGrid.owner != null)
                            {
                                if (player.targetGrid.owner != player)
                                {
                                    if (player.canCost1APInEnemy)
                                    {
                                        return 1;
                                    }
                                    else
                                    {
                                        return 2;
                                    }
                                }
                                else
                                {
                                    return 1;
                                }
                            }
                            else
                            {
                                return 1;
                            }
                        }
                    default:
                        return 0;
                }
            case LandType.Lake:
                switch (player.targetGrid.landType)
                {
                    case LandType.Lake:
                        return 0;
                    case LandType.Forest:
                        if (player.freeMoveCount > 0)
                        {
                            player.freeMoveCount--;
                            return 0;
                        }
                        else
                        {
                            return 1;
                        }
                    case LandType.Mountain:
                        if (player.freeMoveCount > 0)
                        {
                            player.freeMoveCount--;
                            return 0;
                        }
                        else
                        {
                            return 2;
                        }
                    case LandType.Plain:
                        if (player.freeMoveCount > 0)
                        {
                            player.freeMoveCount--;
                            return 0;
                        }
                        else
                        {
                            if (player.targetGrid.owner != null)
                            {
                                if (player.targetGrid.owner != player)
                                {
                                    if (player.canCost1APInEnemy)
                                    {
                                        return 1;
                                    }
                                    else
                                    {
                                        return 2;
                                    }
                                }
                                else
                                {
                                    return 1;
                                }
                            }
                            else
                            {
                                return 1;
                            }
                        }
                    default:
                        return 0;
                }
            case LandType.Forest:
                switch (player.targetGrid.landType)
                {
                    case LandType.Lake:
                        if (player.freeMoveCount > 0)
                        {
                            player.freeMoveCount--;
                            return 0;
                        }
                        else
                        {
                            return 2;
                        }
                    case LandType.Forest:
                        if (player.freeMoveCount > 0)
                        {
                            player.freeMoveCount--;
                            return 0;
                        }
                        else
                        {
                            return 1;
                        }
                    case LandType.Mountain:
                        if (player.freeMoveCount > 0)
                        {
                            player.freeMoveCount--;
                            return 0;
                        }
                        else
                        {
                            return 2;
                        }
                    case LandType.Plain:
                        if (player.freeMoveCount > 0)
                        {
                            player.freeMoveCount--;
                            return 0;
                        }
                        else
                        {
                            if (player.targetGrid.owner != null)
                            {
                                if (player.targetGrid.owner != player)
                                {
                                    if (player.canCost1APInEnemy)
                                    {
                                        return 1;
                                    }
                                    else
                                    {
                                        return 2;
                                    }
                                }
                                else
                                {
                                    return 1;
                                }
                            }
                            else
                            {
                                return 1;
                            }
                        }
                    default:
                        return 0;
                }
            case LandType.Plain:
                switch (player.targetGrid.landType)
                {
                    case LandType.Lake:
                        if (player.freeMoveCount > 0)
                        {
                            player.freeMoveCount--;
                            return 0;
                        }
                        else
                        {
                            return 2;
                        }
                    case LandType.Forest:
                        if (player.freeMoveCount > 0)
                        {
                            player.freeMoveCount--;
                            return 0;
                        }
                        else
                        {
                            return 1;
                        }
                    case LandType.Mountain:
                        if (player.freeMoveCount > 0)
                        {
                            player.freeMoveCount--;
                            return 0;
                        }
                        else
                        {
                            return 2;
                        }
                    case LandType.Plain:
                        if (player.freeMoveCount > 0)
                        {
                            if (player.canFreeMoveInSelfGrid && player.targetGrid.owner == player && player.currentGrid.owner == player)
                            {
                                return 0;
                            }
                            else
                            {
                                player.freeMoveCount--;
                                return 0;
                            }
                        }
                        else
                        {
                            if (player.targetGrid.owner != null)
                            {
                                if (player.targetGrid.owner != player)
                                {
                                    if (player.canCost1APInEnemy)
                                    {
                                        return 1;
                                    }
                                    else
                                    {
                                        return 2;
                                    }
                                }
                                else
                                {
                                    if (player.currentGrid.owner != player)
                                    {
                                        return 1;
                                    }
                                    else
                                    {
                                        if (player.canFreeMoveInSelfGrid)
                                        {
                                            return 0;
                                        }
                                        else
                                        {
                                            return 1;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                return 1;
                            }
                        }
                    default:
                        return 0;
                }
            default:
                return 0;
        }
    }
}
