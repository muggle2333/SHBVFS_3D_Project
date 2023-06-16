using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Grid<TGridObject>
{
    private const float HEX_VERTICAL_OFFSET_MULTIPLIER = 0.75F;
    private const float HEX_HORIZONTAL_OFFSET_MULTIPLIER = 0.5F;

    public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
    public class OnGridObjectChangedEventArgs : EventArgs
    {
        public int x;
        public int z;
    }
    public int width;
    public int length;
    private float cellSizeX;
    private float cellSizeY;
    private float offset;

    public TGridObject[,] gridArray;
    public TextMesh[,] debugTextArray;
    private GameObject debugText;


    public Grid(int width, int length, float cellSizeY, float offset, bool isCreateGrid, Func<Grid<TGridObject>, int, int, TGridObject> createGridObject)
    {
        this.width = width;
        this.length = length;
        this.cellSizeX = cellSizeY * 0.5f * Mathf.Sqrt(3);
        this.cellSizeY = cellSizeY;
        this.offset = offset;

        gridArray = new TGridObject[width, length];
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                gridArray[x, z] = createGridObject(this, x, z);
            }
        }
        //Debug.Log($"Create Grid:{width},{length}");

        debugTextArray = new TextMesh[width, length];
        if (isCreateGrid)
        {
            debugText = new GameObject("DebugText");
            for (int x = 0; x < gridArray.GetLength(0); x++)
            {

                for (int z = 0; z < gridArray.GetLength(1); z++)
                {
                    debugTextArray[x, z] = CreateWorldText(gridArray[x, z]?.ToString(), debugText.transform, GetWorldPositionCenter(x, z), new Vector2(x, z));
                    //Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), Color.white, 100f);
                    //Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.white, 100f);
                }
            }
            //Debug.DrawLine(GetWorldPosition(0, length), GetWorldPosition(width, length), Color.white, 100f);
            //Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, length), Color.white, 100f);
        }


        OnGridObjectChanged += (object sender, OnGridObjectChangedEventArgs eventArgs) =>
        {
            //debugTextArray[eventArgs.x, eventArgs.z].text = gridArray[eventArgs.x, eventArgs.z]?.ToString();
        };

    }
    public Vector3 GetWorldPosition(int x, int z)
    {
        return new Vector3(x, 0, 0) * (cellSizeX + offset) + new Vector3(0, 0, z) * (cellSizeY * HEX_VERTICAL_OFFSET_MULTIPLIER + offset) + ((z % 2) == 1 ? new Vector3(1, 0, 0) * cellSizeX * HEX_HORIZONTAL_OFFSET_MULTIPLIER : Vector3.zero);

    }
    public Vector3 GetWorldPositionCenter(int x, int z)
    {
        return GetWorldPosition(x, z) + new Vector3(cellSizeX, 0, cellSizeY) * 0.5f;
    }
    public float GetGridDistance()
    {
        return Vector3.Distance(GetWorldPositionCenter(0, 0), GetWorldPositionCenter(0, 1)) + offset;
    }
    public void GetXZ(Vector3 worldPosition, out int x, out int z)
    {
        int roughX = Mathf.FloorToInt(worldPosition.x / (cellSizeX + offset));
        int roughZ = Mathf.FloorToInt(worldPosition.z / (cellSizeY * HEX_VERTICAL_OFFSET_MULTIPLIER + offset));
        Vector3Int roughXZ = new Vector3Int(roughX, 0, roughZ);
        bool oddRow = roughZ % 2 == 1;
        List<Vector3Int> neighbourList = new List<Vector3Int>
        {
            roughXZ + new Vector3Int(-1,0,0),
            roughXZ + new Vector3Int(+1,0,0),

            roughXZ + new Vector3Int(oddRow ? +1 : -1,0,+1),
            roughXZ + new Vector3Int(+0,0,+1),

            roughXZ + new Vector3Int(oddRow ? +1 : -1,0,-1),
            roughXZ + new Vector3Int(+0,0,-1),
        };

        Vector3Int closestXZ = roughXZ;
        foreach (Vector3Int neighbour in neighbourList)
        {
            if (Vector3.Distance(worldPosition, GetWorldPositionCenter(neighbour.x, neighbour.z)) <
                Vector3.Distance(worldPosition, GetWorldPositionCenter(closestXZ.x, closestXZ.z)))
            {
                closestXZ = neighbour;
            }
        }
        x = closestXZ.x;
        z = closestXZ.z;
    }

    public TextMesh CreateWorldText(string text, Transform parent, Vector3 localPosition, Vector2 indexXY)
    {
        string objectName = "World_Text" + indexXY.x + "_" + indexXY.y;
        GameObject gameObject = new GameObject(objectName);
        Transform transform = gameObject.transform;
        gameObject.transform.Rotate(new Vector3(90, 0, 0));
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        TextMesh textMesh = gameObject.AddComponent<TextMesh>();
        textMesh.text = indexXY.x + "," + indexXY.y ;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.fontSize = 25;

        CreateWorldHex(gameObject.transform);


        return textMesh;
    }
    public void CreateWorldHex(Transform parent)
    {
        GameObject gameObject = new GameObject("hex");
        gameObject.transform.SetParent(parent, false);
        SpriteRenderer sp = gameObject.AddComponent<SpriteRenderer>();
        sp.sprite = Resources.Load("Sprites/HexFrame", typeof(Sprite)) as Sprite;
        //
        //gameObject.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);


    }
    public void SetGridObject(int x, int z, TGridObject value)
    {
        if (x < 0 || z < 0 || x >= width || z >= length) return;
        gridArray[x, z] = value;
        debugTextArray[x, z].text = value.ToString();
        if (OnGridObjectChanged != null)
        {
            OnGridObjectChanged(this, new OnGridObjectChangedEventArgs { x = x, z = z });
        }

    }
    public void TriggerGridObjectChanged(int x, int z)
    {
        if (OnGridObjectChanged != null)
        {
            OnGridObjectChanged(this, new OnGridObjectChangedEventArgs { x = x, z = z });
        }

    }
    public void SetGridObject(Vector3 worldPosition, TGridObject value)
    {
        int x, z;
        GetXZ(worldPosition, out x, out z);
        SetGridObject(x, z, value);
    }
    public TGridObject GetGridObject(int x, int z)
    {
        if (x < 0 || z < 0 || x >= width || z >= length) return default(TGridObject);
        else
        {
            return gridArray[x, z];
        }
    }
    public TGridObject GetGridObject(Vector3 worldPosition)
    {
        int x, z;
        GetXZ(worldPosition, out x, out z);
        return GetGridObject(x, z);
    }

    public void GetGridSetting(out int gridWidth, out int gridLength, out float gridCellSize, out float gridOffset)
    {

        gridWidth = this.width;
        gridLength = this.length;
        gridCellSize = this.cellSizeY;
        gridOffset = this.offset;
    }
    public float GetGridCellSize()
    {
        return cellSizeY;
    }
}
