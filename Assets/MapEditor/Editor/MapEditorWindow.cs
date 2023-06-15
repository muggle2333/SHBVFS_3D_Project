using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using LitJson;
using System.IO;
using System;


public class MapEditorWindow : EditorWindow
{
    public const string GRID_DATA_PATH = "/StreamingAssets/GridDatas/";
    public const string GRID_SETTING_PATH = "/StreamingAssets/GridSettings/";
    int levelWidth = 10;
    int levelLength = 10;
    float levelCellSize = 8f;
    float levelOffset = 0.05f;
    int levelIndex = 1;



    Grid<GridObject> grid;
    Vector3 gridPosition;

    //Brush Setting
    MeshType meshType;
    LandType landType;
    GameObject[, ] landMesh = new GameObject[4,2];

    [MenuItem("Tools/MapEditorWindow")]
    static void OpenWindow()
    {
        MapEditorWindow window = (MapEditorWindow)GetWindow(typeof(MapEditorWindow));
        window.minSize = new Vector2(500, 300);
        window.Show();
    }
    //similar to start,awake
    private void OnEnable()
    {

    }
    //initialize Texture2D values
    private void InitTextures()
    {

    }
    //similar to update, called 1 or more times per interaction
    private void OnFocus()
    {
        //SceneView.onSceneGUIDelegate -= OnSceneGUI;
        SceneView.duringSceneGui -= OnSceneGUI;
        SceneView.duringSceneGui += OnSceneGUI;
        //SceneView.onSceneGUIDelegate += OnSceneGUI;
        Repaint();

    }
    private void OnDestroy()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }
    private void OnSceneGUI(SceneView scene)
    {
        //Lock the selection in scene
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        //ClickToChange the grid
        gridPosition = GetMousePosToScene();
        if (Event.current.type == EventType.MouseDown)
        {
            Paint(gridPosition);
        }
    }
    public static Vector3 GetMousePosToScene()
    {
        SceneView sceneView = SceneView.currentDrawingSceneView;
        //当前屏幕坐标,左上角(0,0)右下角(camera.pixelWidth,camera.pixelHeight)
        Vector2 mousePos = Event.current.mousePosition;
        //retina 屏幕需要拉伸值
        float mult = 1;
#if UNITY_5_4_OR_NEWER
        mult = EditorGUIUtility.pixelsPerPoint;
#endif
        //转换成摄像机可接受的屏幕坐标,左下角是(0,0,0);右上角是(camera.pixelWidth,camera.pixelHeight,0)
        mousePos.y = sceneView.camera.pixelHeight - mousePos.y * mult;
        mousePos.x *= mult;
        //近平面往里一些,才能看到摄像机里的位置
        Vector3 fakePoint = mousePos;
        fakePoint.z = 20;
        Vector3 point = sceneView.camera.ScreenToWorldPoint(fakePoint);
        return new Vector3(point.x, 0, point.z);
    }
    private void OnGUI()
    {
        DrawMapSetting();

    }

    void DrawMapSetting()
    {
        //Level
        GUILayout.BeginVertical();
        GUILayout.Label("Level", EditorStyles.boldLabel);
        levelIndex = EditorGUILayout.IntField("LevelIndex", levelIndex);
        GUILayout.EndVertical();
        GUILayout.Space(10);

        //Grid
        GUILayout.BeginVertical();
        GUILayout.Label("MAP", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        levelWidth = EditorGUILayout.IntField("Width", levelWidth);
        levelLength = EditorGUILayout.IntField("Length", levelLength);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        levelCellSize = EditorGUILayout.FloatField("CellSize", levelCellSize);
        levelOffset = EditorGUILayout.FloatField("Offset", levelOffset);
        GUILayout.EndHorizontal();
        //hexGrid = (GameObject)EditorGUILayout.ObjectField(hexGrid, typeof(GameObject),true);
        if (GUILayout.Button("Create Grid", GUILayout.MaxWidth(200)))
        {
            //Create Grid
            CreateGrid();
        }
        GUILayout.Space(10);

        //GridMesh
        GUILayout.Label("LAND MESHES", EditorStyles.boldLabel);
        for (int i =0;i< landMesh.GetLength(0); i++)
        {
            GUILayout.BeginHorizontal();
            for (int j = 0; j < landMesh.GetLength(1);j++)
            {
                landMesh[i,j]= (GameObject)EditorGUILayout.ObjectField($"{Enum.GetName(typeof(LandType),i)} & {Enum.GetName(typeof(MeshType), j)}", landMesh[i, j], typeof(GameObject), true);
                Debug.Log(landMesh[i, j]);
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.Space(10);

        //Select GridType
        GUILayout.Label("BRUSH", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        landType = (LandType)EditorGUILayout.EnumPopup(landType);
        meshType = (MeshType)EditorGUILayout.EnumPopup(meshType);
        GUILayout.EndHorizontal();
        GUILayout.Space(10);


        //Grid Edit
        GUILayout.Label("GRID OPTION", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Clear the Grid", GUILayout.MaxWidth(200)))
        {
            ClearGrid();
        }
        if (GUILayout.Button("Place the Mesh", GUILayout.MaxWidth(200)))
        {
            PlaceMesh();
        }
        if (GUILayout.Button("Clear the Mesh", GUILayout.MaxWidth(200)))
        {
            ClearMesh();
        }

        GUILayout.EndHorizontal();
        GUILayout.Space(10);

        //TOOL
        GUILayout.Label("TOOL", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Save", GUILayout.MaxWidth(200)))
        {
            SaveGridData();

        }
        if (GUILayout.Button("Load", GUILayout.MaxWidth(200)))
        {
            //LoadGridData();
            LoadGrid();
        }
        GUILayout.EndHorizontal();



        GUILayout.EndVertical();
        EditorGUILayout.HelpBox("White : Plain \nBlack : Mountain \nBlue : Lake \nGreen : Forest \n1) CreateGrid -> 2) Click text to change the type -> 3) CreatMaps", MessageType.Info);
    }


    public void CreateGrid()
    {

        grid = new Grid<GridObject>(levelWidth, levelLength, levelCellSize, levelOffset, true, (Grid<GridObject> g, int x, int z) => new GridObject(g, x, z));

    }
    public void ClearGrid()
    {
        GameObject textGrid = GameObject.Find("DebugText");
        while (textGrid)
        {
            DestroyImmediate(textGrid);
            textGrid = GameObject.Find("DebugText");
        }

    }
    public void ClearMesh()
    {
        GameObject meshGrid = GameObject.Find("Maps");
        while (meshGrid)
        {
            DestroyImmediate(meshGrid);
            meshGrid = GameObject.Find("Maps");
        }
    }
    public void Paint(Vector3 position)
    {
        if (grid.GetGridObject(position) != null)
        {
            Debug.Log(grid.GetGridObject(position));
            grid.GetGridObject(position).SetGridType(landType,meshType);
        }
    }

    public void PlaceMesh()
    {
        SaveGridData();
        GridPlaceEditor.CreateMaps(levelIndex,landMesh);

    }
    public void SaveGridData()
    {
        //Save the Grid Array
        foreach (var gridObject in grid.gridArray)
        {
            gridObject.grid = null;
        }
        string gridDataJson = JsonMapper.ToJson(grid.gridArray);
        Debug.Log(gridDataJson);
        string gridDataPath = "GridData_" + levelIndex + ".json";
        File.WriteAllText(Application.dataPath + GRID_DATA_PATH + gridDataPath, gridDataJson);

        //Save the Scriptable Grid Setting
        GridSetting gridSetting = ScriptableObject.CreateInstance<GridSetting>();
        gridSetting.width = levelWidth;
        gridSetting.length = levelLength;
        gridSetting.offset = levelOffset;
        gridSetting.cellSize = levelCellSize;
        string gridSettingPath = "GridSetting_" + levelIndex + ".json";
        string setJson = JsonMapper.ToJson(gridSetting);
        File.WriteAllText(Application.dataPath + GRID_SETTING_PATH + gridSettingPath, setJson);
        AssetDatabase.Refresh();
    }
    //Test load asset
    public void LoadGridData()
    {
        string gridDataPath = "GridData_" + levelIndex + ".json";
        string gridDataJson = File.ReadAllText(Application.dataPath + GRID_DATA_PATH + gridDataPath);
        GridObject[] gridObject = JsonMapper.ToObject<GridObject[]>(gridDataJson);
        Debug.Log(gridObject.Length);

        string gridSettingPath = "GridSetting_" + levelIndex + ".json";
        string gridSettingJson = File.ReadAllText(Application.dataPath + GRID_SETTING_PATH + gridSettingPath);
        //GridSetting gridSetting = Resources.Load<GridSetting>(gridSettingPath);
        GridSetting gridSetting = ScriptableObject.CreateInstance<GridSetting>();
        JsonUtility.FromJsonOverwrite(gridSettingJson, gridSetting);

    }
    public void LoadGrid()
    {
        GridPlaceEditor.CreateMaps(levelIndex, landMesh);
    }
}
