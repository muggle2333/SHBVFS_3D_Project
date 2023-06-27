using Autodesk.Fbx;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public enum AcademyType
{
    Null,
    YI,
    DAO,
    MO,
    BING,
    RU,
    FA,
}

public enum LandType
{
    Plain,
    Mountain,
    Lake,
    Forest,
}

public enum MeshType
{
    Grass,
    Sand,
}
[Serializable]
public class GridObject
{
    public LandType landType = LandType.Plain;
    public AcademyType academy = AcademyType.YI;
    public MeshType meshType = MeshType.Grass;

    public Player owner;
    public bool isHasBuilding = false;
    public bool canBuild = true;
    public bool canBeOccupied = true;

    public Grid<GridObject> grid;
    public Transform landTransform; //place the building
    public Transform vfxTransform;
    
    public int x;
    public int z;

    private bool isDiscovered = false;

    public GridObject()
    {
        this.landType = LandType.Plain;
        owner = null;
        landTransform = null;
        vfxTransform = null;
        isHasBuilding = false;
        canBuild = true;
        canBeOccupied = true;
    }
    public GridObject(int x, int z)
    {
        this.x = x;
        this.z = z;
        owner = null;
        landTransform = null;
        vfxTransform = null;
        this.landType = LandType.Plain;
        isHasBuilding = false;
        canBuild = true;
        canBeOccupied = true;
    }
    public GridObject(Grid<GridObject> grid, int x, int z)
    {
        this.grid = grid;
        this.x = x;
        this.z = z;
        owner = null;
        landTransform = null;
        vfxTransform = null;
        this.landType = LandType.Plain;
        isHasBuilding = false;
        canBuild = true;
        canBeOccupied = true;
    }
    public GridObject(GridObject tmpObject)
    {
        this.grid = tmpObject.grid;
        this.x = tmpObject.x;
        this.z = tmpObject.z;
        owner = tmpObject.owner;
        academy= tmpObject.academy;
        landTransform = tmpObject.landTransform;
        vfxTransform = tmpObject.vfxTransform;
        this.landType = tmpObject.landType;
        isHasBuilding = tmpObject.isHasBuilding;
        canBuild = tmpObject.canBuild;
        canBeOccupied = tmpObject.canBeOccupied;
    }
    //public void SetGridType()
    //{
    //    this.landType = this.landType + 1 > LandType.Forest ? LandType.Plain : this.landType + 1;
    //    Debug.Log(this.landType);
    //    UpdateGrid();
    //    grid.TriggerGridObjectChanged(x, z);
    //}

    public void SetGridType(LandType landType,MeshType meshType)
    {
        this.landType = landType;
        this.meshType = meshType;
        UpdateGrid();
        grid.TriggerGridObjectChanged(x, z);
    }

    public void UpdateGrid()
    {

        switch((int)this.landType)
        {
            case (int)LandType.Plain:
                grid.debugTextArray[x, z].color = Color.white;
                break;
            case (int)LandType.Mountain:
                grid.debugTextArray[x, z].color = Color.black;
                break;
            case (int)LandType.Lake:
                grid.debugTextArray[x, z].color = Color.blue;
                break;
            case (int)LandType.Forest:
                grid.debugTextArray[x, z].color = Color.green;
                break;     
        }
        switch((int)this.meshType)
        {
            case (int)MeshType.Grass:
                grid.debugTextArray[x, z].GetComponentInChildren<SpriteRenderer>().color = Color.white;
                break;
            case (int)MeshType.Sand:
                grid.debugTextArray[x, z].GetComponentInChildren<SpriteRenderer>().color = Color.yellow;
                break;
        }
        UpdateGridData();
    }

    public void UpdateGridData()
    {
        if(this.landType == LandType.Plain)
        {
            canBeOccupied = true;
            canBuild = true;
        }
        else
        {
            canBeOccupied = false;
            canBuild = false;
        }

    }

    public void SetOwner(Player player,Transform vfx,bool isControlStage)
    {
        if(vfx != null)
        {
            vfxTransform = vfx;
        }
        if (owner == player) return;
        owner = player;
        UpdateVfxColor(isControlStage);
        grid.TriggerGridObjectChanged(x, z);

    }
  
    public void UpdateVfxColor(bool isControlStage)
    {
        if(owner==null)
        {
            if (vfxTransform == null) return;
            Color vfxColor = new Color(0, 0, 0, 0f);
            vfxTransform.gameObject.GetComponentInChildren<SpriteRenderer>().color = vfxColor;
        }
        else if(owner.Id == PlayerId.RedPlayer)
        {
            Color vfxColor = isControlStage?new Color(1, 0, 0, 0.5f): new Color(1, 0, 0, 1);
            vfxTransform.gameObject.GetComponentInChildren<SpriteRenderer>().color = vfxColor;
        }
        else if(owner.Id == PlayerId.BluePlayer)
        {
            Color vfxColor = isControlStage ? new Color(0, 0,1, 0.5f) : new Color(0, 0, 1, 1);
            vfxTransform.gameObject.GetComponentInChildren<SpriteRenderer>().color = vfxColor;
        }
        
    }
    public void SetBuilding(GameObject building)
    {
        isHasBuilding = true;
        landTransform = building.transform;
        int randomRotation = UnityEngine.Random.Range(0, 6);
        landTransform.transform.Rotate(new Vector3(0, 60f * randomRotation, 0));
        landTransform.position = grid.GetWorldPositionCenter(x, z);
        grid.TriggerGridObjectChanged(x, z);
    }
    public void DiscoverLand()
    {
        isDiscovered = true;
        grid.TriggerGridObjectChanged(x, z);
    }
}
