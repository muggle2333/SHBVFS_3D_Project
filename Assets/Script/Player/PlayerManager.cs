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
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

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
    

    public void MovePlayer(Player player,GridObject gridObject)
    {
        player.GetComponent<PlayerInteractionComponent>().Move(gridObject);
        UpdateGridAuthorityData(player, gridObject);
    }

    public void Occupy(Player player,GridObject gridObject)
    {
        gridObject = GridManager.Instance.ManageOwner(gridObject, player);
        player.OccupyGrid(gridObject);
        UpdateGridAuthorityData(player, gridObject);
    }

    public void Build(Player player,GridObject gridObject)
    {
        gridObject = GridManager.Instance.ManageBuilding(gridObject);
        UpdateGridAuthorityData(player, gridObject);
    }
    
    public void Gacha(Player player, GridObject gridObject)
    {

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
        return CheckDistance(player, gridObject) <= player.Range;
    }

    public int CheckDistance(Player player, GridObject gridObject)
    {
        Vector3 dirPos = GridManager.Instance.grid.GetWorldPositionCenter(gridObject.x, gridObject.z);
        return (int)Math.Ceiling(Vector3.Distance(player.gameObject.transform.position, dirPos) / GridManager.Instance.gridDistance);
        
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
        if (!gridObject.isHasBuilding) return false;
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
