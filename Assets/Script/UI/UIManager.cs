using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private PlayerDataUI playerDataUI;
    private GridObjectUI gridObjectUI;
    public void Awake()
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
        playerDataUI= GetComponentInChildren<PlayerDataUI>();
        gridObjectUI= GetComponentInChildren<GridObjectUI>();
    }
    public void UpdatePlayerDataUI(Player player)
    {
        playerDataUI.UpdatePlayerData(player);
    }
    public void UpdateGridObjectUI(GridObject gridObject, PlayerInteractAuthority authority)
    {
        gridObjectUI.UpdateGridObjectUIData(gridObject, authority);
    }

    public void ShowGridObjectUI(bool isShow)
    {
        gridObjectUI.ShowGridObjectUI(isShow);
    }


}
