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

    public int player0LandCount;
    public int player1LandCount;

    public float dyingTimer = 10;

    public static PlayerManager Instance;
    public CardSelectManager cardSelectManager;
    private DrawCardComponent drawCardComponent;
    private ControlStage controlStage;


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
    public void Update()
    {
        if (TurnbasedSystem.Instance.isDie.Value == true)
        {
            dyingTimer -= Time.deltaTime;
        }
        if (dyingTimer <= 0)
        {
            alivePlayerUI.SetActive(false);
            dyingPlayerUI.SetActive(false);
            GameOver();
        }
    }
    public void Start()
    {
        cardSelectManager = FindObjectOfType<CardSelectManager>();
        drawCardComponent = FindObjectOfType<DrawCardComponent>();
        controlStage = FindObjectOfType<ControlStage>();
    }
    public void ResetPlayerPosition(Player player)
    {
        //player.GetComponent<PlayerInteractionComponent>().Move(player.trueGrid)
        player.GetComponent<PlayerInteractionComponent>().HideVfxPlayer();
        player.GetComponent<PlayerInteractionComponent>().ResetGachaVfx();
        player.RefreshLinePath();
    }

    public void BackupPlayerPosition(Player player)
    {
        player.trueGrid= player.currentGrid;
    }

    public void Research(Player player,GridObject gridObject)
    {

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
    }
    [ClientRpc]
    public void InteractClientRpc(PlayerId playerId,PlayerInteract playerInteract,ClientRpcParams clientRpcParams = default)
    {
        Player player = GameplayManager.Instance.playerList[(int)playerId];
        Interact(player, playerInteract);
    }
    public void MovePlayer(Player player,GridObject gridObject)
    {
        player.targetGrid = gridObject;
        int APCost = Calculating.Instance.CalculateAPCost(PlayerInteractType.Move, player);
        player.GetComponent<PlayerInteractionComponent>().Move(gridObject);
        UpdateGridAuthorityData(player, gridObject);
        player.UpdateLinePath(gridObject.landType);
    }
    public void TryMove(Player player,GridObject gridObject)
    {
        player.targetGrid = gridObject;
        int APCost = Calculating.Instance.CalculateAPCost(PlayerInteractType.Move, player);
        player.GetComponent<PlayerInteractionComponent>().MoveVfxPlayer(gridObject);
        UpdateGridAuthorityData(player, gridObject);
        player.UpdateLinePath(gridObject.landType);
    }

    public void Occupy(Player player,GridObject gridObject,bool isControlStage)
    {
        int APCost = Calculating.Instance.CalculateAPCost(PlayerInteractType.Occupy, player);
        gridObject = GridManager.Instance.ManageOwner(gridObject, player,isControlStage);
        player.OccupyGrid(gridObject);
        GridVfxManager.Instance.UpdateVfxOwner(gridObject,isControlStage);
        if (isControlStage)
        {
            UpdateGridAuthorityData(player, gridObject);
        }
        
    }

    public void Build(Player player,GridObject gridObject, bool isControlStage)
    {
        int APCost = Calculating.Instance.CalculateAPCost(PlayerInteractType.Build, player);
        gridObject = GridManager.Instance.ManageBuilding(gridObject,isControlStage);
        GridVfxManager.Instance.UpdateVfxBuilding(gridObject,isControlStage);
        if (isControlStage)
        {
            UpdateGridAuthorityData(player, gridObject);
        }
        
    }
    public void TrySearch(Player player)
    {
        int APCost = Calculating.Instance.CalculateAPCost(PlayerInteractType.Search, player);

    }
    public void Search(Player player)
    {
        int APCost = Calculating.Instance.CalculateAPCost(PlayerInteractType.Search, player);
        var neighbourList = GridManager.Instance.grid.GetNeighbour(player.currentGrid);
        neighbourList.Add(player.currentGrid);
        foreach(GridObject neighbour in neighbourList)
        {
            GridManager.Instance.ManageKnowable(player, neighbour);
            GridVfxManager.Instance.UpdateVfxAcademy(neighbour);
        }
       
    }
    public void TryGacha(Player player, GridObject gridObject)
    {
        int APCost = Calculating.Instance.CalculateAPCost(PlayerInteractType.Gacha, player);

        player.GetComponent<PlayerInteractionComponent>().TryGacha(player.currentGrid);
        //drawCardComponent.TryDrawCard();
        //drawCardComponent.DrawCard(GameplayManager.Instance.currentPlayer);
    }

    public void DrawCard(Player player, GridObject gridObject)
    {
        int APCost = Calculating.Instance.CalculateAPCost(PlayerInteractType.Gacha, player);

        drawCardComponent.DrawCard(GameplayManager.Instance.currentPlayer);
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
        return player.currentGrid == gridObject;
    }
    public bool CheckKnowable(Player player, GridObject gridObject)
    {
        if(player.trueGrid == gridObject) return true;
        return gridObject.CheckKnowAuthority(player);
    }
    public bool CheckMoveable(Player player, GridObject gridObject)
    {
        if (player.currentGrid == gridObject) return false;
        return CheckDistance(player, gridObject) <= 1;
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
        if (player.currentGrid != gridObject) return false;
        if(!gridObject.canBeOccupied) return false;
        if (gridObject.owner!=null && gridObject.owner== player) return false;
        return true;
    }
    public bool CheckBuildable(Player player, GridObject gridObject)
    {
        if (player.currentGrid != gridObject) return false;
        if (gridObject.isHasBuilding) return false;
        if (gridObject.owner == null || gridObject.owner != player) return false;
        return true;
    }

    public bool CheckGachable(Player player, GridObject gridObject)
    {
        if (player.currentGrid != gridObject) return false;
        //if (!gridObject.isHasBuilding) return false;
        if (gridObject.owner == null || gridObject.owner != null && gridObject.owner != player) return false;
        return true;
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
