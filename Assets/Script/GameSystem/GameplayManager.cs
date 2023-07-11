using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;

public class GameplayManager : NetworkBehaviour
{
    public static GameplayManager Instance;

    [SerializeField] private Vector2[] playerStartPoint = new Vector2[2];
    //public Player playerRed;
    //public Player playerBlue;
    public List<Player> playerList = new List<Player>();
    public Player currentPlayer;
    public GameplayUI gameplayUI;
    private ControlStage controlStage;
    private MoveStage moveStage;
    private AttackStage attackStage;
    public DiscardStage discardStage;
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
        InitializePlayerClientRpc();
        CardManager.Instance.InitializeCardManager();
        FindObjectOfType<CardSelectManager>().InitializeCardSelectManager();
    }
    private void Update()
    {
        //if (playerRed.HP <= 0 || playerBlue.HP <= 0)
        //{
        //    TurnbasedSystem.Instance.Pause();
        //}
        //if (TurnbasedSystem.Instance.CurrentGameStage != GameStage.S1)
        //{
        //    UIManager.Instance.ShowGridObjectUI(false, null);
        //}
    }
    #region initializePlayer funcation ABANDON
    //can't use
    public void InitializePlayer()
    {
        //connectedClientsList only accessable to the server
        foreach(var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            playerList.Add(client.PlayerObject.GetComponent<Player>());
        }
        for(int i =0;i< playerList.Count;i++)
        {
            playerList[i].transform.position = GridManager.Instance.grid.GetWorldPositionCenter((int)playerStartPoint[i].x, (int)playerStartPoint[i].y);
            playerList[i].currentGrid = GridManager.Instance.grid.GetGridObject((int)playerStartPoint[i].x, (int)playerStartPoint[i].y);
            playerList[i].trueGrid = playerList[i].currentGrid;
            playerList[i].GetComponent<PlayerInteractionComponent>().RefreshLinePath();
        }
        InitializePlayerClientRpc();
    }
    #endregion
    [ClientRpc]
    private void InitializePlayerClientRpc(ClientRpcParams clientRpcParams = default)
    {
        playerList =FindObjectsOfType<Player>().ToList<Player>();
        playerList = playerList.OrderBy(a=>a.Id).ToList();
        for (int i = 0; i < playerList.Count; i++)
        {
            playerList[i].transform.position = GridManager.Instance.grid.GetWorldPositionCenter((int)playerStartPoint[i].x, (int)playerStartPoint[i].y);
            playerList[i].currentGrid = GridManager.Instance.grid.GetGridObject((int)playerStartPoint[i].x, (int)playerStartPoint[i].y);
            playerList[i].GetComponent<PlayerInteractionComponent>().RefreshLinePath();
        }
        currentPlayer = playerList[(int)NetworkManager.Singleton.LocalClientId];
        UIManager.Instance.UpdatePlayerDataUI(currentPlayer);
        
    }
    public void SetCurrentPlayer(Player player)
    {
        currentPlayer = player;

    }

    public List<Player> GetPlayer()
    {
        return playerList;
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
        BackUpControlStageClientRpc();
        controlStage.StartStage();
    }
    [ClientRpc]
    public void BackUpControlStageClientRpc()
    {
        GridManager.Instance.BackupGrid();
        foreach (var player in playerList)
        {
            PlayerManager.Instance.BackupPlayerPosition(player);
        }
    }
    public void StartDiscardStage()
    {
        ResetControlStageClientRpc();
        discardStage.StartStage();
    }
    [ClientRpc]
    public void ResetControlStageClientRpc()
    {
        GridManager.Instance.ResetGrid();
        foreach (var player in playerList)
        {
            PlayerManager.Instance.ResetControlVfx(player);
        }
        UIManager.Instance.ShowGridObjectUI(false, null);
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
