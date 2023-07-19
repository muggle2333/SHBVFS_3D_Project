using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Calculating : NetworkBehaviour
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

    public int cardDamage;
    public int cardAP;
    public int cardHP;
    public int cardFreeMoveNum;

    public int totalCardAttackDamage;
    public int totalCardDefense;
    public int totalCardAttackRange;

    public Dictionary<Player, int> totalCardAttackDamageDic = new Dictionary<Player, int>();
    public Dictionary<Player, int> totalCardDefenseDic = new Dictionary<Player, int>();
    public Dictionary<Player, int> totalCardAttackRangeDic = new Dictionary<Player, int>();

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
        totalCardAttackDamageDic.Clear();
        totalCardDefenseDic.Clear();
        totalCardAttackRangeDic.Clear();
        playerAcademyBuffcomponent = FindObjectOfType<PlayerAcademyBuffcomponent>();
        Invoke("AddTotalAcademyEffectNum", 3);
        
    }
    public void AddTotalAcademyEffectNum()
    {
        totalCardAttackDamageDic.Add(GameplayManager.Instance.playerList[0], 0);
        totalCardAttackDamageDic.Add(GameplayManager.Instance.playerList[1], 0);
        totalCardDefenseDic.Add(GameplayManager.Instance.playerList[0], 0);
        totalCardDefenseDic.Add(GameplayManager.Instance.playerList[1], 0);
        totalCardAttackRangeDic.Add(GameplayManager.Instance.playerList[0], 0);
        totalCardAttackRangeDic.Add(GameplayManager.Instance.playerList[1], 0);
        totalAcademyEffectNum.Add(GameplayManager.Instance.playerList[0], new int[6]);
        totalAcademyEffectNum.Add(GameplayManager.Instance.playerList[1], new int[6]);
    }
    public void DelataCardData (CardSetting card,Player player)
    {
        totalCardAttackRangeDic[player] += card.playerDataEffect.visionRange;
        totalCardDefenseDic[player] += card.playerDataEffect.defence;
        totalCardAttackDamageDic[player] += card.playerDataEffect.attack;

        player.cardAD = totalCardAttackDamageDic[player];
        player.cardAR = totalCardAttackRangeDic[player];
        player.cardDF = totalCardDefenseDic[player];

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
    public void CardDataInitializeClientRpc(PlayerId playerId)
    {
        cardDamage = 0;
        cardAP = 0;
        cardHP = 0;
        cardFreeMoveNum = 0;
        for (int i = 0; i < 6; i++)
        {
            GameplayManager.Instance.playerList[(int)playerId].academyOwnedPoint[i] -= totalAcademyEffectNum[GameplayManager.Instance.playerList[(int)playerId]][i];
            
        }
        GameplayManager.Instance.playerList[(int)playerId].cardAD = 0;
        GameplayManager.Instance.playerList[(int)playerId].cardAR = 0;
        GameplayManager.Instance.playerList[(int)playerId].cardDF = 0;
        for (int i = 0; i < 6 ; i++)
        {
            academyEffectNum[i] = 0;
            totalAcademyEffectNum[GameplayManager.Instance.playerList[(int)playerId]][i] = 0;
            totalCardAttackRangeDic[GameplayManager.Instance.playerList[(int)playerId]] = 0;
            totalCardDefenseDic[GameplayManager.Instance.playerList[(int)playerId]] = 0;
            totalCardAttackDamageDic[GameplayManager.Instance.playerList[(int)playerId]] = 0;
        }

        playerAcademyBuffcomponent.UpdatePlayerAcademyBuff(GameplayManager.Instance.playerList[(int)playerId]);
    }
    public void AcademyBuff(Dictionary<AcademyType, AcademyBuffData> PlayerAcademyBuffDict,Player player)
    {
        for (int i = 0; i < player.academyOwnedPoint.Length; i++)
        {
            player.academyOwnedPoint[i] = player.academyOwnedPoint[i] + academyEffectNum[i];
        }
        for (int i = 0; i < 6; i++)
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
        player.MaxHP = player.baseMaxHP + totalAcademyMaxHP;
        player.AttackDamage = player.baseAttackDamage + totalAcademyAttackDamage + totalCardAttackDamageDic[player];
        player.Range = player.baseRange + totalAcademyAttackRange + totalCardAttackRangeDic[player] + player.gridAR;
        player.Defence = player.baseDefense + totalAcademyDefense + totalCardDefenseDic[player] + player.gridDF;
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
