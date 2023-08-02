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

    /*public int totalAcademyMaxHP;
    public int totalAcademyHPPerRound;
    public int totalAcademyAttackRange;
    public int totalAcademyAttackDamage;
    public int totalAcademyDefense;
    public int totalAcademyAPPerRound;
    public int totalAcademyBasicCardPerRound;*/

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

    public Dictionary<Player, int> totalAcademyMaxHPDic = new Dictionary<Player, int>();
    public Dictionary<Player, int> totalAcademyHPPerRoundDic = new Dictionary<Player, int>();
    public Dictionary<Player, int> totalAcademyAttackRangeDic = new Dictionary<Player, int>();
    public Dictionary<Player, int> totalAcademyAttackDamageDic = new Dictionary<Player, int>();
    public Dictionary<Player, int> totalAcademyDefenseDic = new Dictionary<Player, int>();
    public Dictionary<Player, int> totalAcademyAPPerRoundDic = new Dictionary<Player, int>();
    public Dictionary<Player, int> totalAcademyBasicCardPerRoundDic = new Dictionary<Player, int>();
    public Dictionary<Player, AcademyBuffData> AcademyBuffDataDict = new Dictionary<Player, AcademyBuffData>();
    public int[] AcademyEffectNum = new int[6];
    public Dictionary<Player, int[]> totalAcademyEffectNum = new Dictionary<Player, int[]>();
    //public AcademyBuffData AcademyBuffData;
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
        totalAcademyMaxHPDic.Clear();
        totalAcademyHPPerRoundDic.Clear();
        totalAcademyAttackRangeDic.Clear();
        totalAcademyAttackDamageDic.Clear();
        totalAcademyDefenseDic.Clear();
        totalAcademyAPPerRoundDic.Clear();
        totalAcademyBasicCardPerRoundDic.Clear();
        totalAcademyEffectNum.Clear();
        totalCardAttackDamageDic.Clear();
        totalCardDefenseDic.Clear();
        totalCardAttackRangeDic.Clear();
        cardFreeMoveNumDic.Clear();
        AcademyBuffDataDict.Clear();
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

        totalAcademyMaxHPDic.Add(GameplayManager.Instance.playerList[0], 0);
        totalAcademyHPPerRoundDic.Add(GameplayManager.Instance.playerList[0], 0);
        totalAcademyAttackRangeDic.Add(GameplayManager.Instance.playerList[0], 0);
        totalAcademyAttackDamageDic.Add(GameplayManager.Instance.playerList[0], 0);
        totalAcademyDefenseDic.Add(GameplayManager.Instance.playerList[0], 0);
        totalAcademyAPPerRoundDic.Add(GameplayManager.Instance.playerList[0], 0);
        totalAcademyBasicCardPerRoundDic.Add(GameplayManager.Instance.playerList[0], 0);

        totalAcademyMaxHPDic.Add(GameplayManager.Instance.playerList[1], 0);
        totalAcademyHPPerRoundDic.Add(GameplayManager.Instance.playerList[1], 0);
        totalAcademyAttackRangeDic.Add(GameplayManager.Instance.playerList[1], 0);
        totalAcademyAttackDamageDic.Add(GameplayManager.Instance.playerList[1], 0);
        totalAcademyDefenseDic.Add(GameplayManager.Instance.playerList[1], 0);
        totalAcademyAPPerRoundDic.Add(GameplayManager.Instance.playerList[1], 0);
        totalAcademyBasicCardPerRoundDic.Add(GameplayManager.Instance.playerList[1], 0);
    }
    public void DelataCardData (CardSetting card,Player player)
    {
        totalCardAttackRangeDic[player] += card.playerDataEffect.visionRange;
        totalCardDefenseDic[player] += card.playerDataEffect.defence;
        totalCardAttackDamageDic[player] += card.playerDataEffect.attack;
        cardFreeMoveNumDic[player] += card.playerDataEffect.freeMoveNum;
        player.cardAD = totalCardAttackDamageDic[player];
        player.cardAR = totalCardAttackRangeDic[player];
        player.cardDF = totalCardDefenseDic[player];
        cardDamage = card.Damage;
        cardAP = card.playerDataEffect.actionPoint;
        cardHP = card.playerDataEffect.hp;
        for(int i=0;i< card.academyEffectNum.Length; i++)
        {
            AcademyEffectNum[i] = card.academyEffectNum[i];
        }
        
        if (NetworkManager.Singleton.IsServer)
        {
            if (card.cardTarget == CardTarget.opponent)
            {
                var enemy = GameplayManager.Instance.PlayerIdToPlayer(GameplayManager.Instance.GetEnemy(player.Id));
                for (int i = 0; i < 6; i++)
                {
                    enemy.cardAcademyEffectNum[i] += AcademyEffectNum[i];
                }
                playerAcademyBuffcomponent.UpdatePlayerAcademyBuffServerRpc(enemy.Id);
            }
            else
            {
                for (int i = 0; i < 6; i++)
                {
                    player.cardAcademyEffectNum[i] += AcademyEffectNum[i];
                }
                playerAcademyBuffcomponent.UpdatePlayerAcademyBuffServerRpc(player.Id);
            }
        }
        
    }

    [ClientRpc]
    public void CardDataInitializeClientRpc(PlayerId playerId)
    {
        Player player = GameplayManager.Instance.PlayerIdToPlayer(playerId);
        player.freeMoveCount += cardFreeMoveNumDic[player];
        player.trueFreeMoveCount += cardFreeMoveNumDic[player];
        cardFreeMoveNumDic[player] = 0;
        player.damageThisRound = 0;
        player.occuplyCount = 0;
        player.descoverLandCount = 0;
        player.moveCount = 0;
        player.cardAD = 0;
        player.cardAR = 0;
        player.cardDF = 0;
        player.canCost1APInEnemy = false;
        player.playedCardCount = 0;
        for (int i = 0; i < 6 ; i++)
        {
            AcademyEffectNum[i] -= 0;
        }
        totalCardAttackRangeDic[player] = 0;
        totalCardDefenseDic[player] = 0;
        totalCardAttackDamageDic[player] = 0;
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
            if (player.Id == PlayerId.RedPlayer)
            {
                //playerAcademyBuffcomponent.redPlayerAcademyBuffDict.TryGetValue((AcademyType)(i + 1), out AcademyBuffDataDict[player]);
                if (AcademyBuffDataDict.ContainsKey(player))
                {
                    AcademyBuffDataDict[player] = playerAcademyBuffcomponent.redPlayerAcademyBuffDict[(AcademyType)(i + 1)];
                }
                else
                {
                    AcademyBuffDataDict.Add(player, playerAcademyBuffcomponent.redPlayerAcademyBuffDict[(AcademyType)(i + 1)]);
                }
            }
            else
            {
                //playerAcademyBuffcomponent.bluePlayerAcademyBuffDict.TryGetValue((AcademyType)(i + 1), out AcademyBuffData);
                if (AcademyBuffDataDict.ContainsKey(player))
                {
                    AcademyBuffDataDict[player] = playerAcademyBuffcomponent.bluePlayerAcademyBuffDict[(AcademyType)(i + 1)];
                }
                else
                {
                    AcademyBuffDataDict.Add(player, playerAcademyBuffcomponent.bluePlayerAcademyBuffDict[(AcademyType)(i + 1)]);
                }
            }
            
            academyMaxHP += AcademyBuffDataDict[player].maxHp;
            academyHPPerRound += AcademyBuffDataDict[player].hpPreRound;
            academyAttackRange += AcademyBuffDataDict[player].attackRange;
            academyAttackDamage += AcademyBuffDataDict[player].attackDamage;
            academyDefense += AcademyBuffDataDict[player].defense;
            academyAPPerRound += AcademyBuffDataDict[player].APPerRound;
            academyBasicCardPerRound += AcademyBuffDataDict[player].basicCardPerRound;
        }

        totalAcademyMaxHPDic[player] = academyMaxHP;
        totalAcademyHPPerRoundDic[player] = academyHPPerRound;
        totalAcademyAttackRangeDic[player] = academyAttackRange;
        totalAcademyAttackDamageDic[player] = academyAttackDamage;
        totalAcademyDefenseDic[player] = academyDefense;
        totalAcademyAPPerRoundDic[player] = academyAPPerRound;
        totalAcademyBasicCardPerRoundDic[player] = academyBasicCardPerRound;

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
        player.MaxHP = player.baseMaxHP + totalAcademyMaxHPDic[player];
        player.HpPerRound = totalAcademyHPPerRoundDic[player];
        player.AttackDamage = player.baseAttackDamage + totalAcademyAttackDamageDic[player] + totalCardAttackDamageDic[player];
        player.Range = player.baseRange + totalAcademyAttackRangeDic[player] + totalCardAttackRangeDic[player] + player.gridAR;
        player.Defence = player.baseDefense + totalAcademyDefenseDic[player] + totalCardDefenseDic[player] + player.gridDF;
        player.ActionPointPerRound = player.baseActionPointPerRound + totalAcademyAPPerRoundDic[player];
        player.eventCardPerRound = totalAcademyBasicCardPerRoundDic[player];
        
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
            enemy.HP += cardHP;
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
        if (cardAP < 0)
        {
            enemy.CurrentActionPoint += cardAP;
            enemy.TrueActionPoint += cardAP;
            if (enemy.CurrentActionPoint < 0)
            {
                enemy.CurrentActionPoint = 0;
                enemy.TrueActionPoint = 0;
            }
        }
        else
        {
            player.CurrentActionPoint += cardAP;
            player.TrueActionPoint += cardAP;
        }

        cardAP = 0;
        cardHP = 0;
        cardDamage = 0;
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
                            //player.freeMoveCount--;
                            return 0;
                        }
                        else
                        {
                            return 2;
                        }
                    case LandType.Forest:
                        if (player.freeMoveCount > 0)
                        {
                            //player.freeMoveCount--;
                            return 0;
                        }
                        else
                        {
                            return 1;
                        }
                    case LandType.Mountain:
                        if (player.freeMoveCount > 0)
                        {
                            //player.freeMoveCount--;
                            return 0;
                        }
                        else
                        {
                            return 1;
                        }
                    case LandType.Plain:
                        if (player.freeMoveCount > 0)
                        {
                            //player.freeMoveCount--;
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
                            //player.freeMoveCount--;
                            return 0;
                        }
                        else
                        {
                            return 1;
                        }
                    case LandType.Mountain:
                        if (player.freeMoveCount > 0)
                        {
                            //player.freeMoveCount--;
                            return 0;
                        }
                        else
                        {
                            return 2;
                        }
                    case LandType.Plain:
                        if (player.freeMoveCount > 0)
                        {
                            //player.freeMoveCount--;
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
                            //player.freeMoveCount--;
                            return 0;
                        }
                        else
                        {
                            return 2;
                        }
                    case LandType.Forest:
                        if (player.freeMoveCount > 0)
                        {
                            //player.freeMoveCount--;
                            return 0;
                        }
                        else
                        {
                            return 1;
                        }
                    case LandType.Mountain:
                        if (player.freeMoveCount > 0)
                        {
                            //player.freeMoveCount--;
                            return 0;
                        }
                        else
                        {
                            return 2;
                        }
                    case LandType.Plain:
                        if (player.freeMoveCount > 0)
                        {
                            //player.freeMoveCount--;
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
                            //player.freeMoveCount--;
                            return 0;
                        }
                        else
                        {
                            return 2;
                        }
                    case LandType.Forest:
                        if (player.freeMoveCount > 0)
                        {
                            //player.freeMoveCount--;
                            return 0;
                        }
                        else
                        {
                            return 1;
                        }
                    case LandType.Mountain:
                        if (player.freeMoveCount > 0)
                        {
                            //player.freeMoveCount--;
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
                                //player.freeMoveCount--;
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
