using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GridObjectUI : MonoBehaviour
{
    public const float CAMERA_DEFAULT_FOV = 60f;
    public const float GRIDOBJECTUI_OFFSET_Y = 0f;//300
    public const float GRIDOBJECTUI_OFFSET_X = 200f;
    public const float GRIDOBJECTUI_X_MAX = 2000f;
    public const float GRIDOBJECTUI_X_MIN = 0f;
    public const float GRIDOBJECTUI_Y_MAX =2000f;
    public const float GRIDOBJECTUI_Y_MIN = 0f;

    [SerializeField] private GameObject container;

    [SerializeField] private Button moveBtn;
    [SerializeField] private Button occupyBtn;
    [SerializeField] private Button gachaBtn;
    [SerializeField] private Button buildBtn;
    [SerializeField] private Button searchBtn;

    [SerializeField] private TMP_Text academyText;
    [SerializeField] private List<TMP_Text> apCostTextList;
    public EnemyUI enemyUI;
    //[SerializeField] private TMP_Text ownerText;
    //[SerializeField] private TMP_Text landBuffText;
    //[SerializeField] private TMP_Text buildingText;

    private GridObject gridObject;
    private Transform gridTransform;
    private void Awake()
    {
        moveBtn.onClick.AddListener(() =>
        {
            PlayerManager.Instance.TryInteract(PlayerInteractType.Move,GameplayManager.Instance.currentPlayer,gridObject);
            ShowGridObjectUI(false, null);
            //PlayerManager.Instance.TryInteractServerRpc(PlayerInteractType.Move, GameplayManager.Instance.player.Id, new Vector2(gridObject.x, gridObject.z));
        });
        occupyBtn.onClick.AddListener(() =>
        {
            PlayerManager.Instance.TryInteract(PlayerInteractType.Occupy,GameplayManager.Instance.currentPlayer,gridObject);
            ShowGridObjectUI(false, null);
            //PlayerManager.Instance.TryInteractServerRpc(PlayerInteractType.Occupy, GameplayManager.Instance.player.Id, new Vector2(gridObject.x, gridObject.z));
        });
        gachaBtn.onClick.AddListener(() =>
        {
            PlayerManager.Instance.TryInteract(PlayerInteractType.Gacha, GameplayManager.Instance.currentPlayer, gridObject);
            ShowGridObjectUI(false, null);
            //PlayerManager.Instance.TryInteractServerRpc(PlayerInteractType.Move, GameplayManager.Instance.player.Id, new Vector2(gridObject.x, gridObject.z));
        });
        buildBtn.onClick.AddListener(() =>
        {
            PlayerManager.Instance.TryInteract(PlayerInteractType.Build, GameplayManager.Instance.currentPlayer, gridObject);
            ShowGridObjectUI(false, null);
            //PlayerManager.Instance.TryInteractServerRpc(PlayerInteractType.Move, GameplayManager.Instance.player.Id, new Vector2(gridObject.x, gridObject.z));
        });
        searchBtn.onClick.AddListener(() =>
        {
            PlayerManager.Instance.TryInteract(PlayerInteractType.Search, GameplayManager.Instance.currentPlayer, gridObject);
            ShowGridObjectUI(false, null);
        });

    }
    public void Update()
    {
        if (gridTransform == null) return;
        Vector3 pos = new Vector3();
        pos=Camera.main.WorldToScreenPoint(gridTransform.position)+ new Vector3(GRIDOBJECTUI_OFFSET_X, GRIDOBJECTUI_OFFSET_Y, 0)*Mathf.Log(CAMERA_DEFAULT_FOV, Camera.main.fieldOfView);
        //ConstrainUI(pos.x, pos.y);
        //Vector2 worldPosLeftBottom = Camera.main.WorldToScreenPoint(Vector2.zero);
        //Vector2 worldPosTopRight = Camera.main.WorldToScreenPoint(Vector2.one);
        //Debug.Log(worldPosTopRight);
        //Debug.Log(worldPosLeftBottom);
        //pos = new Vector3(Mathf.Clamp(pos.x, worldPosLeftBottom.x, worldPosTopRight.x),
        //                                   Mathf.Clamp(pos.y, worldPosLeftBottom.y, worldPosTopRight.y),
        //                                   pos.z);
        container.transform.position = pos;

        //control stage外锁定一下
    }

    public void UpdateGridObjectUIData(GridObject gridObject, PlayerInteractAuthority authority)
    {
        this.gridObject = gridObject;

        //if(gridObject.landType != LandType.Plain)
        //{
        //    academyText.text = "-----";
        //    ownerText.text="-----";
        //    buildingText.text="-----";
        //}
        //else
        //{
        //    academyText.text = authority.canKnow? gridObject.academy.ToString() : "UNKNOW";
        //    buildingText.text = gridObject.isHasBuilding.ToString();
        //    if (gridObject.owner != null)
        //    {
        //        ownerText.text = gridObject.owner.Id.ToString();
        //    }
        //    else
        //    {
        //        ownerText.text = "N0 OWNER";
        //    }
        //}
        //landBuffText.text = null;
        if(gridObject.landType == LandType.Plain)
        {
            if(!authority.canKnow)
            {
                academyText.text = "未知";
            }else
            {
                switch(gridObject.academy)
                {
                    case AcademyType.Null:
                        academyText.text = "无";break;
                    case AcademyType.YI:
                        academyText.text = "医";break;
                    case AcademyType.DAO:
                        academyText.text = "道";break;
                    case AcademyType.MO:
                        academyText.text = "墨";break;
                    case AcademyType.BING:
                        academyText.text = "兵";break;
                    case AcademyType.RU:
                        academyText.text = "儒";break;
                    case AcademyType.FA:
                        academyText.text = "法";break;
                }
            }
        }
        else
        {
            academyText.text = "无";
        }

        //Set the interactive btn
        occupyBtn.interactable = authority.canOccupy;
        gachaBtn.interactable = authority.canGacha;
        buildBtn.interactable = authority.canBuild;
        moveBtn.interactable = authority.canMove;
        searchBtn.interactable = authority.canSearch;

        if(TurnbasedSystem.Instance.CurrentGameStage.Value!=GameStage.S1||GameManager.Instance.wholeGameState.Value!=GameManager.WholeGameState.GamePlaying)
        {
            occupyBtn.interactable = false;
            gachaBtn.interactable = false;
            buildBtn.interactable = false;
            moveBtn.interactable = false;
            searchBtn.interactable = false;
        }

        //ap
        Player player = GameplayManager.Instance.currentPlayer;
        player.targetGrid = gridObject;
        apCostTextList[0].text = Calculating.Instance.CalculateAPCost(PlayerInteractType.Move, player).ToString();
        apCostTextList[1].text = Calculating.Instance.CalculateAPCost(PlayerInteractType.Occupy, player).ToString();
        apCostTextList[2].text = Calculating.Instance.CalculateAPCost(PlayerInteractType.Build, player).ToString();
        apCostTextList[3].text = Calculating.Instance.CalculateAPCost(PlayerInteractType.Gacha, player).ToString();
        apCostTextList[4].text = Calculating.Instance.CalculateAPCost(PlayerInteractType.Search, player).ToString();
    }

    public void ConstrainUI(float x,float y)
    {
        RectTransform rect = container.GetComponent<RectTransform>();
        float width = rect.sizeDelta.x;
        float height = rect.sizeDelta.y;

        Vector2 pivot = new Vector2();
        pivot.x = x + width <= Screen.width ? 0 : 1;
        pivot.y = y - height >=0 ? 1 : 0;
        rect.pivot = pivot;
        Debug.Log(pivot);
        rect.position = new Vector2(x, y);

    }
    public void ShowGridObjectUI(bool isShow,Transform gridTrans)
    {
        container.SetActive(isShow);
        gridTransform = gridTrans;
        
    }

}
