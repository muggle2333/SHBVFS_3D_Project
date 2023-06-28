using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerInteractAuthority
{
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
}

[Serializable]
public struct PlayerInteract
{
    public PlayerInteractType PlayerInteractType;
    public GridObject GridObject;
}

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

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
        PlayerInteract playerInteract = new PlayerInteract() { PlayerInteractType = playerInteractType, GridObject = gridObject };
        controlStage.AddPlayerInteract(player, playerInteract);
        switch(playerInteractType)
        {
            case PlayerInteractType.Move:
                MovePlayer(player, gridObject);break;
            case PlayerInteractType.Occupy:
                Occupy(player, gridObject,true);break;
            case PlayerInteractType.Build:
                Build(player, gridObject,true);break;
            case PlayerInteractType.Gacha:
                Gacha(player, gridObject);break;

        }
    }

    public void Interact(Player player,PlayerInteract playerInteract)
    {
        GridObject gridObject = playerInteract.GridObject;
        switch (playerInteract.PlayerInteractType)
        {
            case PlayerInteractType.Move:
                MovePlayer(player, gridObject); break;
            case PlayerInteractType.Occupy:
                Occupy(player, gridObject,false); break;
            case PlayerInteractType.Build:
                Build(player, gridObject,false); break;
            case PlayerInteractType.Gacha:
                Gacha(player, gridObject); break;

        }
    }
    public void MovePlayer(Player player,GridObject gridObject)
    {
        player.GetComponent<PlayerInteractionComponent>().Move(gridObject);
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
    
    public void Gacha(Player player, GridObject gridObject)
    {
        drawCardComponent.DrawCard(player);
    }

    
    public void UpdateGridAuthorityData(Player player, GridObject gridObject)
    {
        UIManager.Instance.UpdateGridObjectUI(gridObject, CheckPlayerInteractAuthority(player, gridObject));
    }
    public PlayerInteractAuthority CheckPlayerInteractAuthority(Player player, GridObject gridObject)
    {
        PlayerInteractAuthority authority = new PlayerInteractAuthority();
        authority.canMove = CheckMoveable(player, gridObject);
        authority.canOccupy = CheckOccupiable(player, gridObject);
        authority.canBuild= CheckBuildable(player, gridObject);
        authority.canGacha = CheckGachable(player, gridObject);
        return authority;
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
}
