using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance;

    [SerializeField] private Vector2 playerStartPointRed;
    [SerializeField] private Vector2 playerStartPointBlue;
    [SerializeField] private Player playerRed;
    [SerializeField] private Player playerBlue;

    public Player currentPlayer;


    private ControlStage controlStage;
    private MoveStage moveStage;
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
        controlStage= GetComponent<ControlStage>();
        moveStage= GetComponent<MoveStage>();
    }
    private void InitializePlayer()
    {
        //currentPlayer = FindObjectOfType<Player>();
        playerRed.transform.position = GridManager.Instance.grid.GetWorldPositionCenter((int)playerStartPointRed.x, (int)playerStartPointRed.y);
        playerRed.currentGrid = GridManager.Instance.grid.GetGridObject((int)playerStartPointRed.x, (int)playerStartPointRed.y);
        playerRed.RefreshLinePath();

        playerBlue.transform.position = GridManager.Instance.grid.GetWorldPositionCenter((int)playerStartPointBlue.x, (int)playerStartPointBlue.y);
        playerBlue.currentGrid = GridManager.Instance.grid.GetGridObject((int)playerStartPointBlue.x, (int)playerStartPointBlue.y);
        playerBlue.RefreshLinePath();

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

    public void StartControlStage()
    {
        GridManager.Instance.BackupGrid();
        PlayerManager.Instance.BackupPlayerPosition(playerRed);
        PlayerManager.Instance.BackupPlayerPosition(playerBlue);
        controlStage.StartControlStage();
    }

    public void StartMoveStage()
    {
        GridManager.Instance.ResetGrid();
        PlayerManager.Instance.ResetPlayerPosition(playerRed);
        PlayerManager.Instance.ResetPlayerPosition(playerBlue);
        moveStage.StartMoveStage(controlStage.playerInteractDict);
    }
}
