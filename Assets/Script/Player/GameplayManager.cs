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
public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance;

    [SerializeField] private Vector2 playerStartPoint;
    private Player player;
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
    private void Start()
    {
        InitializePlayer();
    }
    private void InitializePlayer()
    {
        player = FindObjectOfType<Player>();
        player.transform.position = GridManager.Instance.grid.GetWorldPositionCenter((int)playerStartPoint.x,(int)playerStartPoint.y);
        player.currentGrid = GridManager.Instance.grid.GetGridObject((int)playerStartPoint.x, (int)playerStartPoint.y);
    }

    public void MovePlayer(GridObject gridObject)
    {
        player.GetComponent<PlayerInteractionComponent>().Move(gridObject);
        UpdateGridAuthorityData(gridObject);
    }

    public void Occupy(GridObject gridObject)
    {
        gridObject.SetOwner(player);
        player.OccupyGrid(gridObject);

    }
    public void ShowGirdObjectData(Vector3 pos)
    {
        GridObject selectedGridObject = GridManager.Instance.GetSelectedGridObject(pos);
        if (selectedGridObject == null) return;
        UpdateGridAuthorityData(selectedGridObject);
        GridObjectUI.Instance.ShowGridObjectUI(true);
    }
    public void UpdateGridAuthorityData(GridObject grid)
    {
        GridObjectUI.Instance.UpdateGridObjectUIData(grid, CheckPlayerInteractAuthority(grid));
    }
    public PlayerInteractAuthority CheckPlayerInteractAuthority(GridObject gridObject)
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
        Debug.Log((int)Math.Ceiling(Vector3.Distance(player.gameObject.transform.position, dirPos) / GridManager.Instance.gridDistance));
        return (int)Math.Ceiling(Vector3.Distance(player.gameObject.transform.position, dirPos) / GridManager.Instance.gridDistance);
        
    }
    public bool CheckOccupiable(Player player, GridObject gridObject)
    {
        if (player.currentGrid != gridObject) return false;
        if(!gridObject.canBeOccupied) return false;
        if (gridObject.owner!=null && gridObject.owner == player) return false;
        return true;
    }
    public bool CheckBuildable(Player player, GridObject gridObject)
    {
        if (player.currentGrid != gridObject) return false;
        if (gridObject.isHasBuilding) return false;
        if (gridObject.owner != null && gridObject.owner != player) return false;
        return true;
    }

    public bool CheckGachable(Player player, GridObject gridObject)
    {
        if (player.currentGrid != gridObject) return false;
        if (!gridObject.isHasBuilding) return false;
        if (gridObject.owner != null && gridObject.owner != player) return false;
        return true;
    }


    public int GetActionPointCost(Player player,GridObject currentGrid, GridObject targetGrid)
    {
        return 0;
    }
}
