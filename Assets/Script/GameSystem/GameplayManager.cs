using System.Collections;
using System.Collections.Generic;
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
        //InitializePlayerServerRpc();
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
    public void InitializePlayer()
    {
        foreach(var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            playerList.Add(client.PlayerObject.GetComponent<Player>());
        }
        for(int i =0;i< playerList.Count;i++)
        {
            Debug.LogError(i);
            playerList[i].transform.position = GridManager.Instance.grid.GetWorldPositionCenter((int)playerStartPoint[i].x, (int)playerStartPoint[i].y);
            playerList[i].currentGrid = GridManager.Instance.grid.GetGridObject((int)playerStartPoint[i].x, (int)playerStartPoint[i].y);
            playerList[i].RefreshLinePath();
        }

        Debug.LogError(IsClient + " " + IsServer + " " + IsHost);
        Debug.LogError("server");
        InitializePlayerClientRpc();
        TestClientRpc();
    }
    [ClientRpc]
    public void TestClientRpc(ClientRpcParams clientRpcParams = default)
    {
        Debug.LogError(clientRpcParams.Receive.ToString());
    }
    [ClientRpc]
    private void InitializePlayerClientRpc(ClientRpcParams clientRpcParams = default)
    {
        Debug.LogError(clientRpcParams.Receive.ToString());
        Debug.LogError("rpc");
        currentPlayer = playerList[(int)NetworkManager.Singleton.LocalClientId];
        //playerRed = NetworkManager.Singleton.ConnectedClients[0].PlayerObject.GetComponent<Player>();
        //playerRed.transform.position = GridManager.Instance.grid.GetWorldPositionCenter((int)playerStartPointRed.x, (int)playerStartPointRed.y);
        //playerRed.currentGrid = GridManager.Instance.grid.GetGridObject((int)playerStartPointRed.x, (int)playerStartPointRed.y);
        //playerRed.RefreshLinePath();

        //playerBlue = NetworkManager.Singleton.ConnectedClients[1].PlayerObject.GetComponent<Player>();
        //playerBlue.transform.position = GridManager.Instance.grid.GetWorldPositionCenter((int)playerStartPointBlue.x, (int)playerStartPointBlue.y);
        //playerBlue.currentGrid = GridManager.Instance.grid.GetGridObject((int)playerStartPointBlue.x, (int)playerStartPointBlue.y);
        //playerBlue.RefreshLinePath();
    
    
        UIManager.Instance.UpdatePlayerDataUI(currentPlayer);
    }
    public void SetCurrentPlayer(Player player)
    {
        currentPlayer = player;

    }

    public List<Player> GetPlayer()
    {
        //List<Player> list = new List<Player>();
        //list.Add(playerRed.GetComponent<Player>());
        //list.Add(playerBlue.GetComponent<Player>());
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
        GridManager.Instance.BackupGrid();
        foreach(var player in playerList)
        {
            PlayerManager.Instance.BackupPlayerPosition(player);
        }
        controlStage.StartStage();
    }

    public void StartDiscardStage()
    {
        GridManager.Instance.ResetGrid();
        foreach (var player in playerList)
        {
            PlayerManager.Instance.BackupPlayerPosition(player);
        }
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
