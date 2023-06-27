using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GridObjectUI : MonoBehaviour
{
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
        moveBtn.onClick.AddListener(() =>
        {
            PlayerManager.Instance.TryInteract(PlayerInteractType.Move,GameplayManager.Instance.currentPlayer,gridObject);
        });
        occupyBtn.onClick.AddListener(() =>
        {
            PlayerManager.Instance.TryInteract(PlayerInteractType.Occupy,GameplayManager.Instance.currentPlayer,gridObject);

        });
        gachaBtn.onClick.AddListener(() =>
        {
            PlayerManager.Instance.TryInteract(PlayerInteractType.Gacha, GameplayManager.Instance.currentPlayer, gridObject);
        });
        buildBtn.onClick.AddListener(() =>
        {
            PlayerManager.Instance.TryInteract(PlayerInteractType.Build, GameplayManager.Instance.currentPlayer, gridObject);
        });

    }

    public void UpdateGridObjectUIData(GridObject gridObject, PlayerInteractAuthority authority)
    {
        this.gridObject = gridObject;
        academyText.text = gridObject.academy.ToString();
        if(gridObject.owner!=null)
        {
            ownerText.text = gridObject.owner.Id.ToString();
        }
        else
        {
            ownerText.text = "null";
        }
        landBuffText.text = null;
        buildingText.text = gridObject.isHasBuilding.ToString();

        //Set the interactive btn
        occupyBtn.interactable = authority.canOccupy;
        gachaBtn.interactable = authority.canGacha;
        buildBtn.interactable = authority.canBuild;
        moveBtn.interactable = authority.canMove;

    }

    public void ShowGridObjectUI(bool isShow)
    {
        container.SetActive(isShow);
    }
}
