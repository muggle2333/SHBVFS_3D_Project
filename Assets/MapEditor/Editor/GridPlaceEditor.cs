using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.AI;
using Unity.VisualScripting;

public class GridPlaceEditor : MonoBehaviour
{
    public static Grid<GridObject> grid;

    public static void CreateMaps(int levelIndex, GameObject[,] landMesh)
    {
        grid = LoadGridData(levelIndex);
        //PlaceGround(grid, paths, plains);
        PlaceGround(grid, landMesh);
    }
    public static Grid<GridObject> LoadGridData(int levelIndex)
    {
        //Load Grid Data
        string gridDataPath = Application.dataPath + MapEditorWindow.GRID_DATA_PATH + "GridData_" + levelIndex + ".json";
        string json = File.ReadAllText(gridDataPath);
        List<GridObject> gridObjectList = JsonMapper.ToObject<List<GridObject>>(json);

        //Load Grid Setting
        string gridSettingPath = Application.dataPath + MapEditorWindow.GRID_SETTING_PATH + "GridSetting_" + levelIndex + ".json";
        string gridSettingJson = File.ReadAllText(gridSettingPath);
        GridSetting gridSetting = ScriptableObject.CreateInstance<GridSetting>();
        JsonUtility.FromJsonOverwrite(gridSettingJson, gridSetting);

        //Set gridArray
        GridObject[,] gridArray = new GridObject[gridSetting.width, gridSetting.length];
        for (int x=0;x<gridSetting.width;x++)
        {
            for(int z=0;z<gridSetting.length;z++)
            {
                //Debug.Log(gridObjectList[x * gridSetting.length + z]);
                gridArray[x,z] = gridObjectList[x*gridSetting.length+z];
            }
        }
        //Create Grid
        grid = new Grid<GridObject>(gridSetting.width, gridSetting.length, gridSetting.cellSize, gridSetting.offset, false, (Grid<GridObject> g, int x, int z) => new GridObject(g, x, z));
        grid.gridArray = gridArray;
        return grid;
    }
    public static void PlaceGround(Grid<GridObject> grid, GameObject[,] landMesh)
    {
        GameObject maps = new GameObject("Maps"); //最外面的父物体

        for (int x=0;x<grid.width;x++)
        {
            for(int z=0;z< grid.length;z++)
            {
                float offsetY = 0;
                GameObject mesh = landMesh[(int)grid.gridArray[x, z].landType, (int)grid.gridArray[x, z].meshType];

                if (grid.gridArray[x, z].landType == LandType.Lake)
                {
                    offsetY = -1f;
                }
                PlaceSingleGround(x, z, mesh,maps,offsetY) ;
            }
        }
        //AutoBoxCollider(maps);
    }
    private static void PlaceSingleGround(int x, int z, GameObject mesh, GameObject parent, float offsetY)
    {
        GameObject ground = Instantiate(mesh, grid.GetWorldPositionCenter(x, z)+new Vector3 (0,offsetY,0), Quaternion.identity);
        ground.name = mesh.name+"_" + x + "_" + z;
        ground.transform.SetParent(parent.transform, false);
        int randomRotation = Random.Range(0, 6);
        ground.transform.Rotate(new Vector3(0, 60f * randomRotation, 0));
        //grid.gridArray[x, z].landTransform = ground.transform;
        grid.TriggerGridObjectChanged(x, z);
    }
    private static void AutoBoxCollider(GameObject grounds)
    {
        //如果未选中任何物体 返回
        if (grounds == null) return;
        //计算中心点
        Vector3 center = Vector3.zero;
        var renders = grounds.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renders.Length; i++)
        {
            center += renders[i].bounds.center;
        }
        center /= renders.Length;
        //创建边界盒
        Bounds bounds = new Bounds(center, Vector3.zero);
        foreach (var render in renders)
        {
            bounds.Encapsulate(render.bounds);
        }
        //先判断当前是否有碰撞器 进行销毁
        var currentCollider = grounds.GetComponent<Collider>();
        if (currentCollider != null) Object.DestroyImmediate(currentCollider);
        //添加BoxCollider 设置中心点及大小
        var boxCollider = grounds.AddComponent<BoxCollider>();
        boxCollider.isTrigger= true;
        boxCollider.center = bounds.center - grounds.transform.position;
        boxCollider.size = bounds.size;
        //Add rigidbody
        var rg = grounds.AddComponent<Rigidbody>();
        rg.isKinematic = true;
        rg.useGravity= false;
    }
}
