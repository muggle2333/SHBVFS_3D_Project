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
    public Player currentPlayer;


    private ControlStage controlStage;
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
        //currentPlayer = FindObjectOfType<Player>();
        playerRed.transform.position = GridManager.Instance.grid.GetWorldPositionCenter((int)playerStartPointRed.x, (int)playerStartPointRed.y);
        playerRed.GetComponent<Player>().startGrid = GridManager.Instance.grid.GetGridObject((int)playerStartPointRed.x, (int)playerStartPointRed.y);
        playerRed.GetComponent<Player>().currentGrid = playerRed.GetComponent<Player>().startGrid;
        playerRed.GetComponent<Player>().RefreshLinePath();

        playerBlue.transform.position = GridManager.Instance.grid.GetWorldPositionCenter((int)playerStartPointBlue.x, (int)playerStartPointBlue.y);
        playerBlue.GetComponent<Player>().startGrid = GridManager.Instance.grid.GetGridObject((int)playerStartPointBlue.x, (int)playerStartPointBlue.y);
        playerBlue.GetComponent<Player>().currentGrid = playerBlue.GetComponent<Player>().startGrid;
        playerBlue.GetComponent<Player>().RefreshLinePath();

        currentPlayer = playerRed.GetComponent<Player>();
        UIManager.Instance.UpdatePlayerDataUI(currentPlayer);
    }

    public List<Player> GetPlayer()
    {
        List<Player> list = new List<Player>();
        list.Add(playerRed.GetComponent<Player>());
        list.Add(playerBlue.GetComponent<Player>());
        return list;
    }
    public void UpdateSelectPlayer(Player player)
    {
        this.currentPlayer = player;
        UIManager.Instance.UpdatePlayerDataUI(this.currentPlayer);
    }
    public void ShowGirdObjectData(Vector3 pos)
    {
        GridObject selectedGridObject = GridManager.Instance.GetSelectedGridObject(pos);
        if (selectedGridObject == null) return;
        UIManager.Instance.ShowGridObjectUI(true);
        PlayerManager.Instance.UpdateGridAuthorityData(currentPlayer, selectedGridObject);

    }
}
