using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.UI;
using static UnityEditor.PlayerSettings;
using static UnityEditor.Progress;

public class SelectManager : MonoBehaviour
{
    public static SelectManager Instance { get; private set; }

    public Dictionary<Vector2Int,GameObject> selectedDict = new Dictionary<Vector2Int,GameObject>();
    public int selectCount = 1;

    private bool isShowGridInfo = false;
    
    private GridObject currentSelected;
    private SpriteRenderer currentSelectVfx;
    private RaycastHit hitInfo;

    private GridObject latestSelectedGrid;
    private void Awake()
    {
        Instance = this;
    }

    public void Update()
    { 
        //if(Input.GetMouseButtonDown(1))
        //{
        //    Debug.Log(EventSystem.current.IsPointerOverGameObject());
        //    Debug.Log(EventSystem.current.currentSelectedGameObject);
        //}
        if(Input.GetMouseButtonDown(0))
        {
            //Click UI
            //if(EventSystem.current.IsPointerOverGameObject())
            //{
            //    //Debug.Log(EventSystem.current.currentSelectedGameObject);
            //}
            //else
            //{
                Ray currentRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                Debug.DrawLine(currentRay.origin, hitInfo.point, Color.red, 2);
                if(Physics.Raycast(currentRay,out hitInfo,100))
                {
                    if (hitInfo.collider.GetComponentInChildren<GridObjectComponent>() != null)
                    {
                        //点到格子上了
                        //GameplayManager.Instance.ShowGirdObjectData(gameObject.transform);
                    }
                }
                else
                {
                    //什么都没点到
                    foreach (KeyValuePair<Vector2Int, GameObject> item in selectedDict)
                    {
                        Pool.Instance.SetObj("Vfx_SelectGrid", item.Value);
                    }
                    selectedDict.Clear();
                    UIManager.Instance.ShowGridObjectUI(false,null);
                }
            //}
        }
    }
    public void SetCurrentSelectObject(GridObject gridObject)
    {
        Debug.Log(gridObject);
        if(currentSelectVfx == null)
        {
            currentSelectVfx = Pool.Instance.GetObj("Vfx_SelectGrid").GetComponentInChildren<SpriteRenderer>();
            currentSelectVfx.gameObject.transform.SetParent(transform, false);
        }
        currentSelected= gridObject;
        PlaceSelectVfx(currentSelectVfx.transform, gridObject);
        
    }
    public void RemoveCurrentSelectObject(GridObject gridObject)
    {
        if(GridManager.Instance.CheckGridObjectIsSame(gridObject,currentSelected))
        {
            currentSelected= null;
            currentSelectVfx.gameObject.SetActive(false);
        }
    }
    public void SetSelectObject(GridObject gridObject)
    {
        Vector2Int gridObjectXZ = new Vector2Int(gridObject.x, gridObject.z);
        GameObject selectVfx = null;
        if (selectedDict.TryGetValue(gridObjectXZ,out selectVfx))
        {
            Pool.Instance.SetObj("Vfx_SelectGrid", selectVfx);
            selectedDict.Remove(gridObjectXZ);
            latestSelectedGrid = null;
            if(currentSelectVfx!= null)
            currentSelectVfx.gameObject.SetActive(false);
            return;
        }

        latestSelectedGrid= gridObject;
        if (selectCount>selectedDict.Count)
        {
            selectVfx = Pool.Instance.GetObj("Vfx_SelectGrid");
            selectedDict.Add(gridObjectXZ,selectVfx);
        }
        else
        {
            foreach(KeyValuePair<Vector2Int,GameObject> item in selectedDict)
            {
                Pool.Instance.SetObj("Vfx_SelectGrid", item.Value);
            }
            selectedDict.Clear();
            selectVfx = Pool.Instance.GetObj("Vfx_SelectGrid");
            selectedDict.Add(gridObjectXZ, selectVfx);
        }
        PlaceSelectVfx(selectVfx.transform,gridObject);
    }


    private void PlaceSelectVfx(Transform trans,GridObject gridObject)
    {
        trans.gameObject.SetActive(true);
        Vector3 offset = new Vector3(0, 0.1f, 0);
        if (gridObject.landType == LandType.Lake)
        {
            offset = new Vector3(0, -0.9f, 0);
        }
        trans.position = GridManager.Instance.grid.GetWorldPositionCenter(gridObject.x, gridObject.z) + offset;
    }

    public GridObject GetLatestGridObject()
    {
        return latestSelectedGrid;
    }
    
}
