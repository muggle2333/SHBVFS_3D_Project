using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GridObjectUI : MonoBehaviour
{
    public static GridObjectUI Instance;
    [SerializeField] private GameObject container;

    [SerializeField] private Button moveBtn;
    [SerializeField] private Button occupyBtn;
    [SerializeField] private Button gachaBtn;
    [SerializeField] private Button buildBtn;

    [SerializeField] private TMP_Text academyText;
    [SerializeField] private TMP_Text ownerText;
    [SerializeField] private TMP_Text landBuffText;
    [SerializeField] private TMP_Text buildingText;

    private GridObject gridObject;
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

        moveBtn.onClick.AddListener(() =>
        {
            PlayerManager.Instance.MovePlayer(new Vector2(gridObject.x,gridObject.z));
        });
        occupyBtn.onClick.AddListener(() =>
        {

        });
        gachaBtn.onClick.AddListener(() =>
        {

        });
        buildBtn.onClick.AddListener(() =>
        {

        });

    }

    public void UpdateGridObjectUIData(GridObject gridObject, PlayerInteractAuthority authority)
    {
        this.gridObject = gridObject;
        academyText.text = gridObject.academy.ToString();
        ownerText.text = null;
        landBuffText.text = null;
        buildingText.text = gridObject.isHasBuilding.ToString();

        //Set the interactive btn
        occupyBtn.interactable = authority.canBuild;
        gachaBtn.interactable = authority.canGacha;
        buildBtn.interactable = authority.canBuild;
        moveBtn.interactable = authority.canMove;

    }

    public void ShowGridObjectUI(bool isShow)
    {
        container.SetActive(isShow);
    }
}
