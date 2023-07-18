using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public struct PlayerInteractAuthority
{
    public bool canKnow;
    public bool canMove;
    public bool canBuild;
    public bool canOccupy;
    public bool canGacha;
    public bool canSearch;
}

public enum PlayerInteractType
{
    Move,
    Occupy,
    Build,
    Gacha,
    Search,
}

[Serializable]
public struct PlayerInteract : INetworkSerializable
{
    public PlayerInteractType PlayerInteractType;
    public Vector2 GridObjectXZ;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T: IReaderWriter
    {
        serializer.SerializeValue(ref PlayerInteractType);
        serializer.SerializeValue(ref GridObjectXZ);
    }
}

public class PlayerManager : NetworkBehaviour
{
    public GameObject dyingPlayerUI;
    public GameObject alivePlayerUI;
    public GameObject winUI;
    public GameObject loseUI;
    public GameObject drawUI;

    public NetworkList<int> redPlayerHandCardsList;
    public NetworkList<int> bluePlayerHandCardsList;

    public int player0LandCount;
    public int player1LandCount;

    public float dyingTimer = 10;

    public static PlayerManager Instance;
    public CardSelectManager cardSelectManager;
    private DrawCardComponent drawCardComponent;
    private ControlStage controlStage;


    public void Awake()
    {
        redPlayerHandCardsList = new NetworkList<int>();
        bluePlayerHandCardsList = new NetworkList<int>();
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
        }
        
    }
    public void Update()
    {
        if (TurnbasedSystem.Instance.isDie.Value == true)
        {
            dyingTimer -= Time.deltaTime;
        }
        //if (dyingTimer <= 0)
        //{
        //    alivePlayerUI.SetActive(false);
        //    dyingPlayerUI.SetActive(false);
        //    GameOver();
        //}
    }
    public void Start()
    {
        cardSelectManager = FindObjectOfType<CardSelectManager>();
        drawCardComponent = FindObjectOfType<DrawCardComponent>();
        controlStage = FindObjectOfType<ControlStage>();
    }
    public void ResetPlayerDateAfterControlStage(Player player)
    {
        //player.GetComponent<PlayerInteractionComponent>().HideVfxPlayer();
        //player.GetComponent<PlayerInteractionComponent>().ResetGachaVfx();
        //player.GetComponent<PlayerInteractionComponent>().RefreshLinePath();
        player.CurrentActionPoint = player.TrueActionPoint;
    }

    public void BackupPlayerData(Player player)
    {
        player.trueGrid= player.currentGrid;
        player.TrueActionPoint = player.CurrentActionPoint;
    }

    public void TryInteract(PlayerInteractType playerInteractType,Player player, GridObject gridObject)
    {
        switch (playerInteractType)
        {
            case PlayerInteractType.Move:
                TryMove(player, gridObject); break;
            case PlayerInteractType.Occupy:
                Occupy(player, gridObject,true); break;
            case PlayerInteractType.Build:
                Build(player, gridObject, true); break;
            case PlayerInteractType.Gacha:
                TryGacha(player, gridObject); break;
            case PlayerInteractType.Search:
                TrySearch(player);break;
        }
        if(FindObjectOfType<NetworkManager>())
        {
            SaveInteractServerRpc(playerInteractType, player.Id, new Vector2(gridObject.x, gridObject.z));
        }else
        {
            SaveInteract(playerInteractType, player,new Vector2(gridObject.x, gridObject.z));
        }
        
    }

    [ServerRpc(RequireOwnership =false)]
    public void SaveInteractServerRpc(PlayerInteractType playerInteractType, PlayerId playerId, Vector2 gridObjectXZ)
    {
        PlayerInteract playerInteract = new PlayerInteract() { PlayerInteractType = playerInteractType, GridObjectXZ = gridObjectXZ };
        Player player= GameplayManager.Instance.playerList[(int)playerId];
        controlStage.AddPlayerInteract(player, playerInteract);
    }
    public void SaveInteract(PlayerInteractType playerInteractType,Player player, Vector2 gridObjectXZ)
    {
        PlayerInteract playerInteract = new PlayerInteract() { PlayerInteractType = playerInteractType, GridObjectXZ = gridObjectXZ };
        controlStage.AddPlayerInteract(player, playerInteract);
    }
    public void Interact(Player player,PlayerInteract playerInteract)
    {
        GridObject gridObject = GridManager.Instance.grid.gridArray[(int)playerInteract.GridObjectXZ.x, (int)playerInteract.GridObjectXZ.y]; 
        switch (playerInteract.PlayerInteractType)
        {
            case PlayerInteractType.Move:
                MovePlayer(player, gridObject); break;
            case PlayerInteractType.Occupy:
                Occupy(player, gridObject,false); break;
            case PlayerInteractType.Build:
                Build(player, gridObject,false); break;
            case PlayerInteractType.Gacha:
                DrawCard(player, gridObject); break;
            case PlayerInteractType.Search:
                Search(player); break;
        }
        //GridVfxManager.Instance.UpdateVfx(gridObject);
        foreach (var tmpPlayer in GameplayManager.Instance.playerList)
        {
            tmpPlayer.GetComponentInChildren<PlayerInteractionComponent>().SetPlayerPointed(tmpPlayer == player);
        }
    }
    [ClientRpc]
    public void InteractClientRpc(PlayerId playerId,PlayerInteract playerInteract,ClientRpcParams clientRpcParams = default)
    {
        Player player = GameplayManager.Instance.playerList[(int)playerId];
        Interact(player, playerInteract);
    }
    public void InitializePlayerStartPoint(Player player,GridObject gridObject)
    {
        player.GetComponent<PlayerInteractionComponent>().Move(gridObject);
        GridManager.Instance.DiscoverGridObject(player, gridObject);
        player.GetComponent<PlayerInteractionComponent>().RefreshLinePath();
    }
    public bool MovePlayer(Player player,GridObject gridObject)
    {
        //if (player.Id != GameplayManager.Instance.currentPlayer.Id) return false;
        player.targetGrid = gridObject;
        int apCost = Calculating.Instance.CalculateAPCost(PlayerInteractType.Move, player);
        if (!player.UseActionPoint(apCost)) return false;
        player.GetComponent<PlayerInteractionComponent>().Move(gridObject);
        GridManager.Instance.DiscoverGridObject(player,gridObject);
        UpdateGridAuthorityData(player, gridObject);
        //player.GetComponent<PlayerInteractionComponent>().UpdateLinePath(gridObject.landType);
        player.GetComponent<PlayerInteractionComponent>().DeduceFirstPath();
        GridVfxManager.Instance.UpdateVfxAcademy(gridObject);
        return true;
    }
    public void TryMove(Player player,GridObject gridObject)
    {
        player.targetGrid = gridObject;
        int apCost = Calculating.Instance.CalculateAPCost(PlayerInteractType.Move, player);
        if (!player.UseActionPoint(apCost)) return;
        player.GetComponent<PlayerInteractionComponent>().MoveVfxPlayer(gridObject);
        UpdateGridAuthorityData(player, gridObject);
        player.GetComponent<PlayerInteractionComponent>().UpdateLinePath(gridObject.landType);
    }

    public bool Occupy(Player player,GridObject gridObject,bool isControlStage)
    {
        int apCost = Calculating.Instance.CalculateAPCost(PlayerInteractType.Occupy, player);
        if (!player.UseActionPoint(apCost)) return false;
        gridObject = GridManager.Instance.ManageOwner(gridObject, player,isControlStage);
        player.OccupyGrid(gridObject);
        GridVfxManager.Instance.UpdateVfxOwner(gridObject,isControlStage);
        GridVfxManager.Instance.UpdateVfxAcademy(gridObject);
        if (isControlStage)
        {
            UpdateGridAuthorityData(player, gridObject);
        }
        return true;
    }

    public bool Build(Player player,GridObject gridObject, bool isControlStage)
    {
        int apCost = Calculating.Instance.CalculateAPCost(PlayerInteractType.Build, player);
        if (!player.UseActionPoint(apCost)) return false;

        gridObject = GridManager.Instance.ManageBuilding(gridObject,isControlStage);
        GridVfxManager.Instance.UpdateVfxBuilding(gridObject,isControlStage);
        if (isControlStage)
        {
            UpdateGridAuthorityData(player, gridObject);
        }
        return true;
        
    }
    public void TrySearch(Player player)
    {
        int apCost = Calculating.Instance.CalculateAPCost(PlayerInteractType.Search, player);
        if (!player.UseActionPoint(apCost)) return;
    }
    public bool Search(Player player)
    {
        int apCost = Calculating.Instance.CalculateAPCost(PlayerInteractType.Search, player);
        if (!player.UseActionPoint(apCost)) return false;

        var neighbourList = GridManager.Instance.grid.GetNeighbour(player.currentGrid);
        neighbourList.Add(player.currentGrid);
        foreach(GridObject neighbour in neighbourList)
        {
            GridManager.Instance.ManageKnowable(player, neighbour);
            GridVfxManager.Instance.UpdateVfxAcademy(neighbour);
        }
        return true;
    }
    public void TryGacha(Player player, GridObject gridObject)
    {
        int apCost = Calculating.Instance.CalculateAPCost(PlayerInteractType.Gacha, player);
        if (!player.UseActionPoint(apCost)) return;
        GridVfxManager.Instance.UpdateVfxGacha(gridObject,true);
        //player.GetComponent<PlayerInteractionComponent>().TryGacha(player.currentGrid);

        //drawCardComponent.TryDrawCard();
        //drawCardComponent.DrawCard(GameplayManager.Instance.currentPlayer);
    }

    public bool DrawCard(Player player, GridObject gridObject)
    {
        int apCost = Calculating.Instance.CalculateAPCost(PlayerInteractType.Gacha, player);
        if (!player.UseActionPoint(apCost)) return false;

        if (player.Id != GameplayManager.Instance.currentPlayer.Id) return false;
        GridVfxManager.Instance.UpdateVfxGacha(gridObject, false);
        //drawCardComponent.DrawCardServerRpc(player.Id);
        drawCardComponent.DrawCard(player);
        return true;
    }
    public void UpdateGridAuthorityData(Player player, GridObject gridObject)
    {
        UIManager.Instance.UpdateGridObjectUI(gridObject, CheckPlayerInteractAuthority(player, gridObject));
    }
    public PlayerInteractAuthority CheckPlayerInteractAuthority(Player player, GridObject gridObject)
    {
        PlayerInteractAuthority authority = new PlayerInteractAuthority();
        authority.canKnow = CheckKnowable(player, gridObject);
        authority.canMove = CheckMoveable(player, gridObject);
        authority.canOccupy = CheckOccupiable(player, gridObject);
        authority.canBuild= CheckBuildable(player, gridObject);
        authority.canGacha = CheckGachable(player, gridObject);
        authority.canSearch = CheckSearchable(player, gridObject);
        return authority;
    }
    public bool CheckSearchable(Player player, GridObject gridObject)
    {
        int apCost = Calculating.Instance.CalculateAPCost(PlayerInteractType.Search, player);
        return player.IsApEnough(apCost) && player.currentGrid == gridObject;
    }
    public bool CheckKnowable(Player player, GridObject gridObject)
    {
        if(player.trueGrid == gridObject) return true;
        return gridObject.CheckKnowAuthority(player);
    }
    public bool CheckMoveable(Player player, GridObject gridObject)
    {
        player.targetGrid =gridObject;
        int apCost = Calculating.Instance.CalculateAPCost(PlayerInteractType.Move, player);
        if (CheckGridObjectIsSame(gridObject,player.currentGrid)) return false;
        return player.IsApEnough(apCost) && CheckDistance(player, gridObject) <= 1;
    }

    public int CheckDistance(Player player, GridObject gridObject)
    {
        Vector3 dirPos = GridManager.Instance.grid.GetWorldPositionCenter(gridObject.x, gridObject.z);
        //Vector3 startPos = new Vector3(player.gameObject.transform.position.x, 0, player.gameObject.transform.position.z);
        Vector3 startPos = GridManager.Instance.grid.GetWorldPositionCenter(player.currentGrid.x, player.currentGrid.z);
        return (int)Math.Ceiling(Vector3.Distance(startPos, dirPos) / GridManager.Instance.gridDistance);
        
    }
 
    public bool CheckOccupiable(Player player, GridObject gridObject)
    {
        int apCost = Calculating.Instance.CalculateAPCost(PlayerInteractType.Occupy, player);
        if (!player.IsApEnough(apCost)) return false;

        if (!CheckGridObjectIsSame(gridObject, player.currentGrid)) return false;
        if(!gridObject.canBeOccupied) return false;
        if (gridObject.owner!=null && gridObject.owner== player) return false;
        return true;
    }
    public bool CheckBuildable(Player player, GridObject gridObject)
    {
        int apCost = Calculating.Instance.CalculateAPCost(PlayerInteractType.Build, player);
        if (!player.IsApEnough(apCost)) return false;

        if (!CheckGridObjectIsSame(gridObject, player.currentGrid)) return false;
        if (gridObject.isHasBuilding) return false;
        if (gridObject.owner == null || gridObject.owner != player) return false;
        return true;
    }

    public bool CheckGachable(Player player, GridObject gridObject)
    {
        int apCost = Calculating.Instance.CalculateAPCost(PlayerInteractType.Gacha, player);
        if (!player.IsApEnough(apCost)) return false;

        if (!CheckGridObjectIsSame(gridObject, player.currentGrid)) return false;
        //if (!gridObject.isHasBuilding) return false;
        if (gridObject.owner == null || gridObject.owner != null && gridObject.owner != player) return false;
        return true;
    }

    private bool CheckGridObjectIsSame(GridObject gridObject1, GridObject gridObject2)
    {
        return gridObject1.x == gridObject2.x && gridObject1.z == gridObject2.z;
    }
    public int GetActionPointCost(Player player,GridObject currentGrid, GridObject targetGrid)
    {
        return 0;
    }

    public void EffectPlayer(Player player, int[] academyPointEffect, PlayerDataEffect playerDataEffect)
    {
        
    }

    public void PlayCard()
    {
        cardSelectManager.PlayCards(GameplayManager.Instance.currentPlayer);
    }

    public void PlayerDying(List<Player> dyingPlayerList, List<Player> alivePlayerList)
    {

        if (alivePlayerList != null)
        {
            if (alivePlayerList[0] == GameplayManager.Instance.currentPlayer)
            {
                alivePlayerUI.SetActive(true);
            }
            else
            {
                dyingPlayerUI.SetActive(true);
            }
        }
        else
        {
            for(int i = 0; i < dyingPlayerList.Count; i++)
            {
                dyingPlayerList[i].isDying.Value = true;
                if (dyingPlayerList[i] == GameplayManager.Instance.currentPlayer)
                {
                    dyingPlayerUI.SetActive(true);
                }
            }
        }
    }
    [ClientRpc]
    public void SetAttackClientRpc(PlayerId attackPlayerId, PlayerId attackTargetId, ClientRpcParams clientRpcParams = default)
    {
        Player attackPlayer = GameplayManager.Instance.playerList[(int)attackPlayerId];
        Player attackTarget = GameplayManager.Instance.playerList[(int)attackTargetId];
        attackPlayer.AttackTarget = attackTarget;
        attackPlayer.Attack();

        VfxManager.Instance.PlayAttackVfx(attackPlayer.transform, attackTarget.transform);
    }
    public void GameOver()
    {
        List<Player> dyingPlayers = new List<Player>();
        for (int i = 0;i < GameplayManager.Instance.playerList.Count; i++)
        {
            if(GameplayManager.Instance.playerList[i].isDying.Value)
            {
                dyingPlayers.Add(GameplayManager.Instance.playerList[i]);
            }
        }
        if(dyingPlayers.Count == 1)
        {
            if(dyingPlayers[0] == GameplayManager.Instance.currentPlayer)
            {
                loseUI.SetActive(true);
            }
            else
            {
                winUI.SetActive(true);
            }
        }
        else if(dyingPlayers.Count == 2)
        {
            if (dyingPlayers[0].HP > dyingPlayers[1].HP)
            {
                if (dyingPlayers[0] == GameplayManager.Instance.currentPlayer)
                {
                    winUI.SetActive(true);
                }
                else
                {
                    loseUI.SetActive(true);
                }
            }
            else if(dyingPlayers[0].HP < dyingPlayers[1].HP)
            {
                if (dyingPlayers[0] == GameplayManager.Instance.currentPlayer)
                {
                    loseUI.SetActive(true);
                }
                else
                {
                    winUI.SetActive(true);
                }
            }
            else if (dyingPlayers[0].HP == dyingPlayers[1].HP)
            {
                for(AcademyType i = AcademyType.YI; i < AcademyType.FA; i++)
                {
                    player0LandCount += dyingPlayers[0].OwnedLandDic[i].Count;
                    player1LandCount += dyingPlayers[1].OwnedLandDic[i].Count;
                }
                if(player0LandCount > player1LandCount)
                {
                    if(dyingPlayers[0] == GameplayManager.Instance.currentPlayer)
                    {
                        winUI.SetActive(true);
                    }
                    else
                    {
                        loseUI.SetActive(true);
                    }
                }
                else if(player0LandCount < player1LandCount)
                {
                    if (dyingPlayers[0] == GameplayManager.Instance.currentPlayer)
                    {
                        loseUI.SetActive(true);
                    }
                    else
                    {
                        winUI.SetActive(true);
                    }
                }
                else if(player0LandCount == player1LandCount)
                {
                    drawUI.SetActive(true);
                }
            }
        }
    }
}
