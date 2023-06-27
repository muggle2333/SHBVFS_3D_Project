using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance;

    [SerializeField] private Vector2 playerStartPointRed;
    [SerializeField] private Vector2 playerStartPointBlue;
    [SerializeField] private Transform playerBlue;
    [SerializeField] private Transform playerRed;
    public Player player;

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
    private void Start()
    {
        InitializePlayer();
    }
    private void InitializePlayer()
    {
        //player = FindObjectOfType<Player>();
        playerRed.transform.position = GridManager.Instance.grid.GetWorldPositionCenter((int)playerStartPointRed.x, (int)playerStartPointRed.y);
        playerRed.GetComponent<Player>().currentGrid = GridManager.Instance.grid.GetGridObject((int)playerStartPointRed.x, (int)playerStartPointRed.y);
        
        playerBlue.transform.position = GridManager.Instance.grid.GetWorldPositionCenter((int)playerStartPointBlue.x, (int)playerStartPointBlue.y);
        playerBlue.GetComponent<Player>().currentGrid = GridManager.Instance.grid.GetGridObject((int)playerStartPointBlue.x, (int)playerStartPointBlue.y);
        
        player = playerRed.GetComponent<Player>();
        UIManager.Instance.UpdatePlayerDataUI(player);
    }

    public void UpdateSelectPlayer(Player player)
    {
        this.player = player;
        UIManager.Instance.UpdatePlayerDataUI(this.player);
    }
    public void ShowGirdObjectData(Vector3 pos)
    {
        GridObject selectedGridObject = GridManager.Instance.GetSelectedGridObject(pos);
        if (selectedGridObject == null) return;
        UIManager.Instance.ShowGridObjectUI(true);
        PlayerManager.Instance.UpdateGridAuthorityData(player, selectedGridObject);

    }
}
