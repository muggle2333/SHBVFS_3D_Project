using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GridObjectUI : MonoBehaviour
{
    public const float CAMERA_DEFAULT_FOV = 60f;
    public const float GRIDOBJECTUI_OFFSET_Y = 300f;
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
    [SerializeField] private TMP_Text ownerText;
    [SerializeField] private TMP_Text landBuffText;
    [SerializeField] private TMP_Text buildingText;

    private GridObject gridObject;
    private Transform gridTransform;
    private void Awake()
    {
        moveBtn.onClick.AddListener(() =>
        {
            PlayerManager.Instance.TryInteract(PlayerInteractType.Move,GameplayManager.Instance.currentPlayer,gridObject);
            //PlayerManager.Instance.TryInteractServerRpc(PlayerInteractType.Move, GameplayManager.Instance.currentPlayer.Id, new Vector2(gridObject.x, gridObject.z));
        });
        occupyBtn.onClick.AddListener(() =>
        {
            PlayerManager.Instance.TryInteract(PlayerInteractType.Occupy,GameplayManager.Instance.currentPlayer,gridObject);
            //PlayerManager.Instance.TryInteractServerRpc(PlayerInteractType.Occupy, GameplayManager.Instance.currentPlayer.Id, new Vector2(gridObject.x, gridObject.z));
        });
        gachaBtn.onClick.AddListener(() =>
        {
            PlayerManager.Instance.TryInteract(PlayerInteractType.Gacha, GameplayManager.Instance.currentPlayer, gridObject);
            //PlayerManager.Instance.TryInteractServerRpc(PlayerInteractType.Move, GameplayManager.Instance.currentPlayer.Id, new Vector2(gridObject.x, gridObject.z));
        });
        buildBtn.onClick.AddListener(() =>
        {
            PlayerManager.Instance.TryInteract(PlayerInteractType.Build, GameplayManager.Instance.currentPlayer, gridObject);
            //PlayerManager.Instance.TryInteractServerRpc(PlayerInteractType.Move, GameplayManager.Instance.currentPlayer.Id, new Vector2(gridObject.x, gridObject.z));
        });
        searchBtn.onClick.AddListener(() =>
        {
            PlayerManager.Instance.TryInteract(PlayerInteractType.Search, GameplayManager.Instance.currentPlayer, gridObject);
        });

    }
    public void Update()
    {
        if (gridTransform == null) return;
        Vector3 pos = new Vector3();
        pos=Camera.main.WorldToScreenPoint(gridTransform.position)+ new Vector3(0, GRIDOBJECTUI_OFFSET_Y, 0)*Mathf.Log(CAMERA_DEFAULT_FOV, Camera.main.fieldOfView);
        //ConstrainUI(pos.x, pos.y);
        //Vector2 worldPosLeftBottom = Camera.main.WorldToScreenPoint(Vector2.zero);
        //Vector2 worldPosTopRight = Camera.main.WorldToScreenPoint(Vector2.one);
        //Debug.Log(worldPosTopRight);
        //Debug.Log(worldPosLeftBottom);
        //pos = new Vector3(Mathf.Clamp(pos.x, worldPosLeftBottom.x, worldPosTopRight.x),
        //                                   Mathf.Clamp(pos.y, worldPosLeftBottom.y, worldPosTopRight.y),
        //                                   pos.z);
        container.transform.position = pos;
    }

    public void UpdateGridObjectUIData(GridObject gridObject, PlayerInteractAuthority authority)
    {
        this.gridObject = gridObject;

        if(gridObject.landType != LandType.Plain)
        {
            academyText.text = "-----";
            ownerText.text="-----";
            buildingText.text="-----";
        }
        else
        {
            academyText.text = authority.canKnow? gridObject.academy.ToString() : "UNKNOW";
            buildingText.text = gridObject.isHasBuilding.ToString();
            if (gridObject.owner != null)
            {
                ownerText.text = gridObject.owner.Id.ToString();
            }
            else
            {
                ownerText.text = "N0 OWNER";
            }
        }
        //landBuffText.text = null;

        //Set the interactive btn
        occupyBtn.interactable = authority.canOccupy;
        gachaBtn.interactable = authority.canGacha;
        buildBtn.interactable = authority.canBuild;
        moveBtn.interactable = authority.canMove;
        searchBtn.interactable = authority.canSearch;
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
