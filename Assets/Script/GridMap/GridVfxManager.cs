using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;

public class GridVfxManager : MonoBehaviour
{
    [Serializable]
    public struct VfxTransform
    {
        public Transform academyVfx;
        public Transform ownerVfx;
        public Transform buildingVfx;
        public Transform gachaVfx;
    }
    public static GridVfxManager Instance { get; private set; }

    [SerializeField] private VfxTransform[,] vfxTransformArray;
    [SerializeField] private VfxTransform vfxPrefabs;
    private void Awake()
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
        Debug.Log("CreateVFX");
        CreateVfx();
    }
    public void CreateVfx()
    {
        GameObject vfxContainers = new GameObject("VfxConainers");
        Grid<GridObject> grid = GridManager.Instance.grid;
        vfxTransformArray = new VfxTransform[grid.width,grid.width];
        for (int x = 0; x < grid.width; x++)
        {
            for (int z = 0; z < grid.length; z++)
            {
                GridObject gridObject = grid.gridArray[x, z];
                if (gridObject.landType != LandType.Plain) continue;

                List<Transform> listTrans = new List<Transform>();
                GameObject container = new GameObject("vfxConainer" + x + "_" + z);
                container.transform.SetParent(vfxContainers.transform);
                container.transform.position = grid.GetWorldPositionCenter(x, z);

                GameObject atmpVfx = Instantiate(vfxPrefabs.academyVfx.gameObject,container.transform);
                GameObject otmpVfx = Instantiate(vfxPrefabs.ownerVfx.gameObject, container.transform);
                GameObject btmpVfx = Instantiate(vfxPrefabs.buildingVfx.gameObject,  container.transform);
                btmpVfx.SetActive(false);
                //GameObject gtmpVfx = Instantiate(vfxPrefabs.gachaVfx.gameObject, container.transform);

                vfxTransformArray[x, z] = new VfxTransform {
                    academyVfx = atmpVfx.transform,
                    ownerVfx = otmpVfx.transform,
                    buildingVfx = btmpVfx.transform,
                    //gachaVfx=gtmpVfx.transform,
                    gachaVfx = null,
                };

                //atmpVfx.GetComponentInChildren<TextMesh>().text = gridObject.academy.ToString();
            }
        }
    }

    public void UpdateVfx(GridObject gridObject)
    {
        UpdateVfxOwner(gridObject, false);
        UpdateVfxBuilding(gridObject, false);
        UpdateVfxAcademy(gridObject);
    }
    public void UpdateVfxOwner(GridObject gridObject,bool isControlStage)
    {
        if(gridObject.landType!=LandType.Plain) return;
        Transform ownerVfx = vfxTransformArray[gridObject.x,gridObject.z].ownerVfx;
        Player owner = gridObject.owner;
        if (owner == null)
        {
            Color vfxColor = new Color(0, 0, 0, 0f);
            ownerVfx.gameObject.GetComponentInChildren<SpriteRenderer>().color = vfxColor;
        }
        else if (owner.Id == PlayerId.RedPlayer)
        {
            Color vfxColor = isControlStage ? new Color(1, 0, 0, 0.5f) : new Color(1, 0, 0, 1);
            ownerVfx.gameObject.GetComponentInChildren<SpriteRenderer>().color = vfxColor;
        }
        else if (owner.Id == PlayerId.BluePlayer)
        {
            Color vfxColor = isControlStage ? new Color(0, 0, 1, 0.5f) : new Color(0, 0, 1, 1);
            ownerVfx.gameObject.GetComponentInChildren<SpriteRenderer>().color = vfxColor;
        }
    }

    public void UpdateVfxBuilding(GridObject gridObject, bool isControlStage)
    {
        if (gridObject.landType != LandType.Plain) return;
        Transform buildingVfx = vfxTransformArray[gridObject.x,gridObject.z].buildingVfx;
        bool isHasBuilding = gridObject.isHasBuilding;
        buildingVfx.gameObject.SetActive(isHasBuilding);
        
    }

    public void UpdateVfxAcademy(GridObject gridObject)
    {
        if (gridObject.landType != LandType.Plain) return;
        Transform academyVfx = vfxTransformArray[gridObject.x,gridObject.z].academyVfx;
        if (gridObject.isDiscovered || gridObject.CheckKnowAuthority(GameplayManager.Instance.currentPlayer)) 
        {
            academyVfx.gameObject.GetComponentInChildren<TextMesh>().text = gridObject.academy.ToString();
        }
        else
        {
            academyVfx.gameObject.GetComponentInChildren<TextMesh>().text = null;
        }
    }
}
