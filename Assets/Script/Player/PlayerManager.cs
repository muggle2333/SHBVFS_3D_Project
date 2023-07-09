using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public struct PlayerInteractAuthority
{
    public bool canKnow;
    public bool canMove;
    public bool canBuild;
    public bool canOccupy;
    public bool canGacha;
}

public enum PlayerInteractType
{
    Move,
    Occupy,
    Build,
    Gacha,
    DrawBasic,
    DrawEvent,
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
    public void Start()
    {
        cardSelectManager = FindObjectOfType<CardSelectManager>();
        drawCardComponent = FindObjectOfType<DrawCardComponent>();
        controlStage = FindObjectOfType<ControlStage>();
    }
    public void ResetPlayerPosition(Player player)
    {
        player.GetComponent<PlayerInteractionComponent>().Move(player.backupGridPos);
        player.RefreshLinePath();
    }

    public void BackupPlayerPosition(Player player)
    {
        player.backupGridPos= player.currentGrid;
    }

    public void TryInteract(PlayerInteractType playerInteractType,Player player, GridObject gridObject)
    {
        switch (playerInteractType)
        {
            case PlayerInteractType.Move:
                MovePlayer(player, gridObject); break;
            case PlayerInteractType.Occupy:
                Occupy(player, gridObject, true); break;
            case PlayerInteractType.Build:
                Build(player, gridObject, true); break;
            case PlayerInteractType.Gacha:
                TryGacha(player, gridObject); break;
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
        player.GetComponent<PlayerInteractionComponent>().Move(gridObject);
        UpdateGridAuthorityData(player, gridObject);
        player.UpdateLinePath(gridObject.landType);
    }

    [ClientRpc]
    public void TryMovePlayerClientRpc(PlayerId playerId,Vector2 gridObjectXZ,ClientRpcParams clientRpcParams = default)
    {
        Player player = GameplayManager.Instance.playerList[(int)playerId];
        GridObject gridObject = GridManager.Instance.grid.gridArray[(int)gridObjectXZ.x, (int)gridObjectXZ.y];
        UpdateGridAuthorityData(player, gridObject);
        player.UpdateLinePath(gridObject.landType);
    }

    public void Occupy(Player player,GridObject gridObject,bool isControlStage)
    {
        
        gridObject = GridManager.Instance.ManageOwner(gridObject, player,isControlStage);
        player.OccupyGrid(gridObject);
        UpdateGridAuthorityData(player, gridObject);
    }

    public void Build(Player player,GridObject gridObject, bool isControlStage)
    {
        gridObject = GridManager.Instance.ManageBuilding(gridObject,isControlStage);
        UpdateGridAuthorityData(player, gridObject);
    }
    
    public void TryGacha(Player player, GridObject gridObject)
    {
        //drawCardComponent.TryDrawCard();
        //drawCardComponent.DrawCard(GameplayManager.Instance.currentPlayer);
    }

    public void DrawCard(Player player, GridObject gridObject)
    {
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
        return authority;
    }

    public bool CheckKnowable(Player player, GridObject gridObject)
    {
        return CheckDistance(player, gridObject) <= player.Range;
    }
    public bool CheckMoveable(Player player, GridObject gridObject)
    {
        if (player.currentGrid == gridObject) return false;
        return CheckDistance(player, gridObject) <= 1;
    }

    public int CheckDistance(Player player, GridObject gridObject)
    {
        Vector3 dirPos = GridManager.Instance.grid.GetWorldPositionCenter(gridObject.x, gridObject.z);
        Vector3 startPos = new Vector3(player.gameObject.transform.position.x, 0, player.gameObject.transform.position.z);
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
                if(dyingPlayerList[i] == GameplayManager.Instance.currentPlayer)
                {
                    dyingPlayerUI.SetActive(true);
                }
            }
        }
    }
}
