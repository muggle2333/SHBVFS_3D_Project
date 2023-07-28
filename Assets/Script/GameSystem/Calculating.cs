using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public int academyBasicCardPerRound;

    public int totalAcademyMaxHP;
    public int totalAcademyHPPerRound;
    public int totalAcademyAttackRange;
    public int totalAcademyAttackDamage;
    public int totalAcademyDefense;
    public int totalAcademyAPPerRound;
    public int totalAcademyBasicCardPerRound;

    public int cardDamage;
    public int cardAP;
    public int cardHP;
    public int totalCardAttackDamage;
    public int totalCardDefense;
    public int totalCardAttackRange;

    public Dictionary<Player, int> cardFreeMoveNumDic = new Dictionary<Player, int>();
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
        cardFreeMoveNumDic.Clear();
        playerAcademyBuffcomponent = FindObjectOfType<PlayerAcademyBuffcomponent>();
        Invoke("AddTotalAcademyEffectNum", 3);
        
    }
    public void AddTotalAcademyEffectNum()
    {
        cardFreeMoveNumDic.Add(GameplayManager.Instance.playerList[0], 0);
        cardFreeMoveNumDic.Add(GameplayManager.Instance.playerList[1], 0);
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
        cardFreeMoveNumDic[player] = card.playerDataEffect.freeMoveNum;
        player.cardAD = totalCardAttackDamageDic[player];
        player.cardAR = totalCardAttackRangeDic[player];
        player.cardDF = totalCardDefenseDic[player];

        academyEffectNum = card.academyEffectNum;
        if (NetworkManager.Singleton.IsServer)
        {
            if (card.cardTarget == CardTarget.opponent)
            {
                var enemy = GameplayManager.Instance.PlayerIdToPlayer(GameplayManager.Instance.GetEnemy(player.Id));
                for (int i = 0; i < 6; i++)
                {
                    enemy.cardAcademyEffectNum[i] += academyEffectNum[i];
                }
                playerAcademyBuffcomponent.UpdatePlayerAcademyBuffServerRpc(enemy.Id);
            }
            else
            {
                for (int i = 0; i < 6; i++)
                {
                    player.cardAcademyEffectNum[i] += academyEffectNum[i];
                }
                playerAcademyBuffcomponent.UpdatePlayerAcademyBuffServerRpc(player.Id);
            }
        }
        cardDamage = card.Damage;
        cardAP = card.playerDataEffect.actionPoint;
        cardHP = card.playerDataEffect.hp;
    }

    [ClientRpc]
    public void CardDataInitializeClientRpc(PlayerId playerId)
    {
        Player player = GameplayManager.Instance.PlayerIdToPlayer(playerId);
        cardDamage = 0;
        cardAP = 0;
        cardHP = 0;
        player.freeMoveCount = cardFreeMoveNumDic[player];
        cardFreeMoveNumDic[player] = 0;
        player.damageThisRound = 0;
        player.occuplyCount = 0;
        player.descoverLandCount = 0;
        player.moveCount = 0;
        player.cardAD = 0;
        player.cardAR = 0;
        player.cardDF = 0;
        for (int i = 0; i < 6 ; i++)
        {
            academyEffectNum[i] = 0;
            totalCardAttackRangeDic[player] = 0;
            totalCardDefenseDic[player] = 0;
            totalCardAttackDamageDic[player] = 0;
        }
        if (NetworkManager.Singleton.IsServer)
        {
            CardAcademyEffectNumInitializeServerRpc(playerId);
        }

    }
    [ServerRpc]
    public void CardAcademyEffectNumInitializeServerRpc(PlayerId playerId)
    {
        Player player = GameplayManager.Instance.PlayerIdToPlayer(playerId);
        for (int i = 0; i < 6; i++)
        {
            player.cardAcademyEffectNum[i] = 0;
        }
        playerAcademyBuffcomponent.UpdatePlayerAcademyBuffServerRpc(player.Id);
    }
    [ClientRpc]
    public void AcademyBuffClientRpc(PlayerId playerId)
    {
        Player player = GameplayManager.Instance.PlayerIdToPlayer(playerId);
        AcademyBuff(player);
    }

    public void AcademyBuff(Player player)
    {
        for (int i = 0; i < 6; i++)
        {
            playerAcademyBuffcomponent.PlayerAcademyBuffDict.TryGetValue((AcademyType)(i + 1), out AcademyBuffData);
            academyMaxHP += AcademyBuffData.maxHp;
            academyHPPerRound += AcademyBuffData.hpPreRound;
            academyAttackRange += AcademyBuffData.attackRange;
            academyAttackDamage += AcademyBuffData.attackDamage;
            academyDefense += AcademyBuffData.defense;
            academyAPPerRound += AcademyBuffData.APPerRound;
            academyBasicCardPerRound += AcademyBuffData.basicCardPerRound;
        }

        totalAcademyMaxHP = academyMaxHP;
        totalAcademyHPPerRound = academyHPPerRound;
        totalAcademyAttackRange = academyAttackRange;
        totalAcademyAttackDamage = academyAttackDamage;
        totalAcademyDefense = academyDefense;
        totalAcademyAPPerRound = academyAPPerRound;
        totalAcademyBasicCardPerRound = academyBasicCardPerRound;

        academyMaxHP = 0;
        academyHPPerRound = 0;
        academyAttackRange = 0;
        academyAttackDamage = 0;
        academyDefense = 0;
        academyAPPerRound = 0;
        academyBasicCardPerRound = 0;
        CalculatPlayerBaseData(player);
    }

    public void CalculatPlayerBaseData(Player player)
    {
        player.MaxHP = player.baseMaxHP + totalAcademyMaxHP;
        player.HpPerRound = totalAcademyHPPerRound;
        player.AttackDamage = player.baseAttackDamage + totalAcademyAttackDamage + totalCardAttackDamageDic[player];
        player.Range = player.baseRange + totalAcademyAttackRange + totalCardAttackRangeDic[player] + player.gridAR;
        player.Defence = player.baseDefense + totalAcademyDefense + totalCardDefenseDic[player] + player.gridDF;
        player.ActionPointPerRound = player.baseActionPointPerRound + totalAcademyAPPerRound;
        player.basicCardPerRound = totalAcademyBasicCardPerRound;
        
    }

    public void CalaulatPlayerData(Player player)
    {
        var enemy = GameplayManager.Instance.PlayerIdToPlayer(GameplayManager.Instance.GetEnemy(player.Id));
        if (cardHP >= 0)
        {
            player.HP += cardHP;
        }
        else
        {
            enemy.HP -= cardHP;
        }

        if (player.HP > player.MaxHP)
        {
            player.HP = player.MaxHP;
        }

        if(enemy.HP > enemy.MaxHP)
        {
            enemy.HP = enemy.MaxHP;
        }

        if (cardDamage > enemy.Defence)
        {
            enemy.HP -= (cardDamage - enemy.Defence);
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
                return 1;
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
        player.currentGrid = GridManager.Instance.GetCurrentGridObject(player.currentGrid);
        if (player.targetGrid.owner == player && player.targetGrid.isHasBuilding)
        {
            return 0;
        }
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
