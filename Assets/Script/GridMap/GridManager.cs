using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity.Netcode;
using TMPro;
using System;

public class GridManager : NetworkBehaviour
{
    public int levelIndex =1;
    public Grid<GridObject> grid;
    public GridObject[,] backupGrid = new GridObject[10,10];
    public GameObject gridUI;
    public float gridDistance;
    public static GridManager Instance { get; private set; }

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

        grid = LoadGridData(levelIndex);
        gridDistance = grid.GetGridDistance();
        //GridVfxManager.Instance.CreateVfx();
    }
    public void Start()
    {
        //GridVfxManager.Instance.CreateVfx();
    }
    public void Update()
    {
       
    }
    public float GetGridSize()
    {
        return grid.GetGridCellSize();
    }
 
    public  Grid<GridObject> LoadGridData(int levelIndex)
    {
        //Load Grid Data
#if UNITY_EDITOR
        string gridDataPath = Application.dataPath + "/StreamingAssets/GridDatas/" + "GridData_" + levelIndex + ".json";
#else
        string gridDataPath = Application.streamingAssetsPath+"GridDatas/GridData_" + levelIndex + ".json";
#endif 
        string json = File.ReadAllText(gridDataPath);
        List<GridObject> gridObjectList = JsonMapper.ToObject<List<GridObject>>(json);

        //Load Grid Setting
#if UNITY_EDITOR
        string gridSettingPath = Application.dataPath + "/StreamingAssets/GridSettings/" + "GridSetting_" + levelIndex + ".json";
#else
        string gridSettingPath = Application.streamingAssetsPath+"GridSettings/GridSetting_" + levelIndex + ".json";
#endif
        string gridSettingJson = File.ReadAllText(gridSettingPath);
        GridSetting gridSetting = ScriptableObject.CreateInstance<GridSetting>();
        JsonUtility.FromJsonOverwrite(gridSettingJson, gridSetting);

        //Create Grid
        Grid<GridObject> grid = new Grid<GridObject>(gridSetting.width, gridSetting.length, gridSetting.cellSize, gridSetting.offset, false, (Grid<GridObject> g, int x, int z) => new GridObject(g, x, z));

        //Set gridArray
        GridObject[,] gridArray = new GridObject[gridSetting.width, gridSetting.length];
        for (int x = 0; x < gridSetting.width; x++)
        {
            for (int z = 0; z < gridSetting.length; z++)
            {
                //Debug.Log(gridObjectList[x * gridSetting.length + z]);
                gridArray[x, z] = gridObjectList[x * gridSetting.length + z];
                gridArray[x, z].grid = grid;

                //Initialize the Land AcademyType
                if (gridArray[x, z].landType == LandType.Plain)
                {
                    gridArray[x, z].academy = (AcademyType)UnityEngine.Random.Range(1, 6);
                    
                }
                else
                {
                    gridArray[x, z].academy = AcademyType.Null;
                }
            }
        }

        grid.gridArray = gridArray;
        return grid;
    }
    

    public GridObject GetSelectedGridObject(Vector3 pointPos)
    {
       return grid.GetGridObject(pointPos);
    }
    public void SetAimGrid(int x, int z)
    {
        gridUI.transform.position = grid.GetWorldPositionCenter(x, z);
        gridUI.SetActive(true);
    }
    public void HideAimGrid()
    {
        gridUI.SetActive(false);
    }

    public GridObject ManageOwner(GridObject gridObject ,Player player,bool isControlStage)
    {
        gridObject = grid.GetGridObject(gridObject.x, gridObject.z);
        gridObject.SetOwner(player, isControlStage);
        return gridObject;
    }


    [ServerRpc(RequireOwnership = false)]
    public void ManageOwnerServerRpc(Vector2 gridObjectXZ, PlayerId playerId)
    {
        ManageOwnerClientRpc(gridObjectXZ, playerId);
    }

    [ClientRpc]
    public void ManageOwnerClientRpc(Vector2 gridObjectXZ, PlayerId playerId)
    {
        GridObject gridObject = grid.GetGridObject((int)gridObjectXZ.x, (int)gridObjectXZ.y);
        Player player = GameplayManager.Instance.playerList[(int)playerId];
        gridObject.SetOwner(player, false);
    }

    public GridObject ManageBuilding(GridObject gridObject,bool isControlStage)
    {
        gridObject = grid.GetGridObject(gridObject.x, gridObject.z);
        gridObject.SetBuilding(true,isControlStage);
        return gridObject;
    }

    public GridObject ManageKnowable(Player player,GridObject gridObject)
    {
        gridObject.SetKnowAuthority(player);
        return gridObject;
    }
    public void BackupGrid()
    {
        backupGrid = new GridObject[10, 10];
        for (int x = 0; x < grid.width; x++)
        {
            for (int z = 0; z < grid.length; z++)
            {
                backupGrid[x, z] = new GridObject(grid.gridArray[x, z]);
            }
        }
    }

    public void ResetGrid()
    {
        //除了探知 都恢复原样
        for (int x = 0; x < grid.width; x++)
        {
            for (int z = 0; z < grid.length; z++)
            {
                //Transform tmp = grid.gridArray[x, z].ownerVfxTransform;
                grid.gridArray[x, z] = backupGrid[x,z];
                //grid.gridArray[x, z].ownerVfxTransform = tmp;
                GridVfxManager.Instance.UpdateVfx(grid.gridArray[x, z]);
            }
        }
        //Clear all the S1 buildingOwner & vfx
        //for (int i = 0; i < buildingContainerS1.childCount; i++)
        //{
        //    Destroy(buildingContainerS1.GetChild(i).gameObject);
        //}
        //for (int x = 0; x < grid.width; x++)
        //{
        //    for (int z = 0; z < grid.length; z++)
        //    {
        //        GridObject gridObject = grid.gridArray[x, z];
        //        GridVfxManager.Instance.UpdateVfx(gridObject);
        //    }
        //}
    }
}

