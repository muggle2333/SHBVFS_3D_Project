using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum SelectGridMode
{
    None,
    Default, // In range
    OneOccupyed,
    TwoOccupyed,
    OneRivalBuilded,
    OneOccupyedNotBuilded,
    Every,
    Specific,
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


    private GridObject latestSelectedGrid = null; //For camera focus
    
    private RaycastHit hitInfo;

    public bool isClickingCard = false;
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
        Debug.LogError(newValue); ;
        if(newValue == GameStage.S1)
        {
            selectGridMode = SelectGridMode.Default;
            selectCount = 1;
        }
        else
        {
            selectGridMode = SelectGridMode.None;
            selectCount = 1;
        }
        UpdateSelectableGridObject();
        ClearSelection();
        UIManager.Instance.ShowGridObjectUI(false, null);
        UIManager.Instance.ShowEnemyUI(false);
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
                    //点到ui button等             
                    return;
                }
                if(isClickingCard)
                {
                    //Debug.LogError("Click Card");
                    return;
                }
                Ray currentRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                //Debug.DrawLine(currentRay.origin, hitInfo.point, Color.red, 2);
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
                            UIManager.Instance.ShowGridObjectUI(false, null);
                            CardManager.Instance.DeselectCard();
                            return;
                        }
                    }

                }

            }
            ClearSelection();
            UIManager.Instance.ShowGridObjectUI(false, null);
            UIManager.Instance.ShowEnemyUI(false);
            //取消选择卡牌
            CardManager.Instance.DeselectCard();
        }
        if(Input.GetMouseButtonDown(1))
        {
            ClearSelection();
            UIManager.Instance.ShowGridObjectUI(false, null);
            UIManager.Instance.ShowEnemyUI(false);
        }
    }
    public void SetCurrentTargetObject(GridObject gridObject)
    {
        if(!CheckGridSelectable(gridObject)) return;
        if(currentTargetVfx == null)
        {
            currentTargetVfx = Pool.Instance.GetObj("Vfx_TargetGrid");
            //currentTargetVfx.gameObject.transform.SetParent(transform, false);
        }
        currentTargetVfx.gameObject.SetActive(true);
        currentTargetGridObject = gridObject;
        PlaceSelectVfx(currentTargetVfx.transform, gridObject);
        
    }
    public void RemoveCurrentTargetObject(GridObject gridObject)
    {
        if(currentTargetGridObject == null) return;
        if(GridManager.Instance.CheckGridObjectIsSame(gridObject,currentTargetGridObject))
        {
            RemoveCurrentTargetGrid();
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
            RemoveCurrentTargetGrid();
            return;
        }

        //更新
        if (selectCount>selectedDict.Count)
        {
            //没超出数量
            selectVfx = Pool.Instance.GetObj("Vfx_SelectedGrid");
            selectedDict.Add(gridObjectXZ,selectVfx);
        }
        else
        {
            //超出了清空
            ClearSelection();
            //新增选项
            selectVfx = Pool.Instance.GetObj("Vfx_SelectedGrid");
            selectedDict.Add(gridObjectXZ, selectVfx);
        }
        PlaceSelectVfx(selectVfx.transform,gridObject);
        RemoveCurrentTargetGrid();
        latestSelectedGrid = gridObject;

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
            //Pool.Instance.SetObj("Vfx_SelectedGrid", currentTargetVfx);
            currentTargetVfx.gameObject.SetActive(false);
        }
    }

    public void ClearSelection()
    {
        CleanSelectedGrid();
        RemoveCurrentTargetGrid();
        latestSelectedGrid = null;
        //UIManager.Instance.ShowGridObjectUI(false, null);
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
        selectGridMode = SelectGridMode.Specific;
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

    private void OnDestroy()
    {
        TurnbasedSystem.Instance.CurrentGameStage.OnValueChanged += TurnbaseSystem_GameStageChanged;
    }
}
