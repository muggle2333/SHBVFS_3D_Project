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
    private AttackStage attackStage;
    private DiscardStage discardStage;
    private S2Stage s2Stage;
    private S3Stage s3Stage;
    private S4Stage s4Stage;

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
        

        controlStage = GetComponent<ControlStage>();
        discardStage = GetComponent<DiscardStage>();
        s2Stage = GetComponent<S2Stage>();
        moveStage = GetComponent<MoveStage>();
        s3Stage = GetComponent<S3Stage>();
        attackStage = GetComponent<AttackStage>();
        s4Stage = GetComponent<S4Stage>();
    }
    private void Start()
    {
        InitializePlayer();
    }
    private void Update()
    {
        if(playerRed.HP<=0||playerBlue.HP<=0)
        {
            TurnbasedSystem.Instance.Pause();
        }
        if (TurnbasedSystem.Instance.CurrentGameStage != GameStage.S1)
        {
            UIManager.Instance.ShowGridObjectUI(false,null);
        }
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
    public void ShowGirdObjectData(Transform gridTrans)
    {
        GridObject selectedGridObject = GridManager.Instance.GetSelectedGridObject(gridTrans.position);
        if (selectedGridObject == null) return;
        UIManager.Instance.ShowGridObjectUI(true,gridTrans);
        PlayerManager.Instance.UpdateGridAuthorityData(currentPlayer, selectedGridObject);

    }

    public void StartControlStage()
    {
        GridManager.Instance.BackupGrid();
        PlayerManager.Instance.BackupPlayerPosition(playerRed);
        PlayerManager.Instance.BackupPlayerPosition(playerBlue);
        controlStage.StartStage();
    }

    public void StartDiscardStage()
    {
        GridManager.Instance.ResetGrid();
        PlayerManager.Instance.ResetPlayerPosition(playerRed);
        PlayerManager.Instance.ResetPlayerPosition(playerBlue);
        discardStage.StartStage();
    }
    public void StartS2Stage()
    {
        s2Stage.StartStage(FindObjectOfType<CardManager>().playedCardDict);
    }
    public void StartMoveStage()
    {
        moveStage.StartStage(controlStage.playerInteractDict);
    }

    public void StartS3Stage()
    {
        s3Stage.StartStage(FindObjectOfType<CardManager>().playedCardDict);
    }

    public void StartAttackStage()
    {
        attackStage.StartStage();
    }

    public void StartS4Stage()
    {
        s4Stage.StartStage(FindObjectOfType<CardManager>().playedCardDict);
    }
}
