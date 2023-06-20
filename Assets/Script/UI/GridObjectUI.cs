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
            GameplayManager.Instance.MovePlayer(gridObject);
        });
        occupyBtn.onClick.AddListener(() =>
        {
            GameplayManager.Instance.Occupy(gridObject);

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
        if(gridObject.owner!=null)
        {
            ownerText.text = gridObject.owner.ToString();
        }
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
