using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public enum SelectGridMode
{
    None,
    Default, // In range
    OneOccupyed,
    TwoOccupyed,
    OneRivalBuilded,
    OneOccupyedNotBuilded,
    Every,
}
public class SelectManager : MonoBehaviour
{
    public static SelectManager Instance { get; private set; }

    public Dictionary<Vector2Int,GameObject> selectedDict = new Dictionary<Vector2Int,GameObject>();
    public int selectCount = 1;
    public SelectGridMode selectGridMode = SelectGridMode.Default;

    private bool isShowGridInfo = false;
    
    private GridObject currentTargetGridObject;
    private GameObject currentTargetVfx;

    private List<GameObject> selectableVfxList = new List<GameObject>();
    private List<Vector2Int> selectableGridObjectList = new List<Vector2Int>();


    private GridObject latestSelectedGrid; //For camera focus
    
    private RaycastHit hitInfo;
    private void Awake()
    {
        Instance = this;
        
    }

    private void Start()
    {
        TurnbasedSystem.Instance.CurrentGameStage.OnValueChanged += TurnbaseSystem_GameStageChanged;
    }

    private void TurnbaseSystem_GameStageChanged(GameStage previousValue, GameStage newValue)
    {
        if(newValue == GameStage.S1)
        {
            selectGridMode = SelectGridMode.Default;
        }
        else
        {
            selectGridMode = SelectGridMode.None;
            ClearSelection();

        }
        UpdateSelectableGridObject();
    }

    public void Update()
    {
        if (GameManager.Instance.wholeGameState.Value != GameManager.WholeGameState.GamePlaying) return;
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                if(EventSystem.current.currentSelectedGameObject != null)
                {
                    //点到ui
                    return;
                }
                Ray currentRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                Debug.DrawLine(currentRay.origin, hitInfo.point, Color.red, 2);
                if (Physics.Raycast(currentRay, out hitInfo))
                {
                    //点到格子上
                    if (hitInfo.collider.GetComponentInChildren<GridObjectComponent>() != null)
                    {
                        //点到格子上了
                        return;
                    }
                    else if(hitInfo.collider.GetComponentInParent<Player>()!=null)
                    {
                        Player player = hitInfo.collider.GetComponentInParent<Player>();
                        if(player.Id != GameplayManager.Instance.currentPlayer.Id)
                        {
                            UIManager.Instance.ShowEnemyUI(true);
                            ClearSelection();
                            return;
                        }
                    }
                }
            }
            ClearSelection();
            UIManager.Instance.ShowEnemyUI(false);
        }
        if(Input.GetMouseButtonDown(1))
        {
            ClearSelection();
            UIManager.Instance.ShowEnemyUI(false);
        }
    }
    public void SetCurrentTargetObject(GridObject gridObject)
    {
        if(!CheckGridSelectable(gridObject)) return;
        if(currentTargetVfx == null)
        {
            currentTargetVfx = Pool.Instance.GetObj("Vfx_SelectedGrid");
            //currentTargetVfx.gameObject.transform.SetParent(transform, false);
        }
        currentTargetGridObject= gridObject;
        PlaceSelectVfx(currentTargetVfx.transform, gridObject);
        
    }
    public void RemoveCurrentSelectObject(GridObject gridObject)
    {
        if(currentTargetGridObject == null) return;
        if(GridManager.Instance.CheckGridObjectIsSame(gridObject,currentTargetGridObject))
        {
            currentTargetGridObject= null;
            currentTargetVfx.gameObject.SetActive(false);
        }
    }
    public void SetSelectObject(GridObject gridObject)
    {
        if (!CheckGridSelectable(gridObject)) return;
        if(FindObjectOfType<TutorialManager>() != null ) { TutorialManager.Instance.CompleteTutorialAction(TutorialAction.ClickGrid); }
        Vector2Int gridObjectXZ = new Vector2Int(gridObject.x, gridObject.z);
        GameObject selectVfx = null;
        //已选的取消
        if (selectedDict.TryGetValue(gridObjectXZ,out selectVfx))
        {
            Pool.Instance.SetObj("Vfx_SelectedGrid", selectVfx);
            selectedDict.Remove(gridObjectXZ);
            latestSelectedGrid = null;
            if(currentTargetVfx!= null)
            currentTargetVfx.gameObject.SetActive(false);
            return;
        }

        //更新
        latestSelectedGrid= gridObject;
        if (selectCount>selectedDict.Count)
        {
            //没超出数量
            selectVfx = Pool.Instance.GetObj("Vfx_SelectedGrid");
            selectedDict.Add(gridObjectXZ,selectVfx);
        }
        else
        {
            //超出了清空
            CleanSelectedGrid();
            //新增选项
            selectVfx = Pool.Instance.GetObj("Vfx_SelectedGrid");
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

    public bool CheckGridSelectable(GridObject gridObject)
    {
        Vector2Int gridObjectXZ = new Vector2Int(gridObject.x, gridObject.z);
        //var selectabelList = GridManager.Instance.GetSelectableGridObject(selectGridMode);
        var selectabelList = selectableGridObjectList;
        foreach(var xz in selectabelList)
        {
            if(xz == gridObjectXZ)
            {
                return true;
            }
        }
        return false;
    }
    
    public void UpdateSelectableGridObject()
    {
        var selectableObjects = GridManager.Instance.GetSelectableGridObject(selectGridMode);
        foreach(var vfx in selectableVfxList)
        {
            Pool.Instance.SetObj("Vfx_SelectableGrid", vfx);
        }
        selectableVfxList.Clear();
        foreach (var xz in selectableObjects)
        {
            var selectableVfx = Pool.Instance.GetObj("Vfx_SelectableGrid");
            var gridObject = GridManager.Instance.grid.GetGridObject(xz.x, xz.y);
            PlaceSelectVfx(selectableVfx.transform, gridObject);
            selectableVfxList.Add(selectableVfx);
        }
        selectableGridObjectList = selectableObjects;
    }

    public void CleanSelectedGrid()
    {
        foreach (KeyValuePair<Vector2Int, GameObject> item in selectedDict)
        {
            Pool.Instance.SetObj("Vfx_SelectedGrid", item.Value);
        }
        selectedDict.Clear();
    }

    public void RemoveCurrentTargetGrid()
    {
        currentTargetGridObject = null;
        if(currentTargetVfx != null)
        {
            Pool.Instance.SetObj("Vfx_SelectedGrid", currentTargetVfx);
        }
    }

    public void ClearSelection()
    {
        CleanSelectedGrid();
        RemoveCurrentTargetGrid();
        latestSelectedGrid = null;
        UIManager.Instance.ShowGridObjectUI(false, null);
    }

    public void SetSpecificSelection(Vector2Int gridXZ)
    {
        foreach (var vfx in selectableVfxList)
        {
            Pool.Instance.SetObj("Vfx_SelectableGrid", vfx);
        }
        selectableVfxList.Clear();
        selectableGridObjectList.Clear();
        selectableGridObjectList.Add(gridXZ);
        var selectableVfx = Pool.Instance.GetObj("Vfx_SelectableGrid");
        var gridObject = GridManager.Instance.grid.GetGridObject(gridXZ.x, gridXZ.y);
        PlaceSelectVfx(selectableVfx.transform, gridObject);
    }

    public void ChangeSelectMode(SelectGridMode mode, int num)
    {
        selectGridMode = mode;
        selectCount = num;
        UpdateSelectableGridObject();
        CleanSelectedGrid();
    }
}
