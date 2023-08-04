using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameplayManager : NetworkBehaviour
{
    public const float DYING_TIMER = 10f;
    public const float DISCARD_TIMER = 10f;

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

    public event EventHandler OnPlayerDying;
    public event EventHandler OnPlayerSelfDying;
    public event EventHandler OnLeaveDyingStage;

    public bool isBluePlayer = false;
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
        if(FindObjectOfType<TutorialManager>()!=null)
        {
            playerStartPoint[1] = new Vector2Int(9,0);
        }else if(SceneManager.GetActiveScene().name== "GameplayScene_3")
        {
            playerStartPoint[0] = new Vector2Int(1, 0);
            playerStartPoint[1] = new Vector2Int(1, 2);
        }
        playerList =FindObjectsOfType<Player>().ToList<Player>();
        playerList = playerList.OrderBy(a=>a.Id).ToList();
        for (int i = 0; i < playerList.Count; i++)
        {
            //playerList[i].transform.position = GridManager.Instance.grid.GetWorldPositionCenter((int)playerStartPoint[i].x, (int)playerStartPoint[i].y);
            //playerList[i].currentGrid = GridManager.Instance.grid.GetGridObject((int)playerStartPoint[i].x, (int)playerStartPoint[i].y);
            //playerList[i].GetComponent<PlayerInteractionComponent>().RefreshLinePath();
            GridObject targetGridObject = GridManager.Instance.grid.GetGridObject((int)playerStartPoint[i].x, (int)playerStartPoint[i].y);
            PlayerManager.Instance.InitializePlayerStartPoint(playerList[i],targetGridObject);

        }
        int currentPlayrId = (int)NetworkManager.Singleton.LocalClientId;
        if(currentPlayrId == 1)
        {
            isBluePlayer= true;
        }
        currentPlayer = playerList[currentPlayrId];
        FindObjectOfType<CameraTest4>().FocusGrid();
        //GridObject currentGridObject = GridManager.Instance.grid.GetGridObject((int)playerStartPoint[currentPlayrId].x, (int)playerStartPoint[currentPlayrId].y);
        //GridVfxManager.Instance.UpdateVfxAcademy(currentGridObject);
        if(FindObjectOfType<TutorialManager>()== null)
        {
            UIManager.Instance.UpdatePlayerDataUI(currentPlayer);
        }else
        {
            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(1f);
            seq.AppendCallback(() =>
            {
                UIManager.Instance.UpdatePlayerDataUI(currentPlayer);
            }); 
        }
        
        
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
        SelectManager.Instance.isClickingCard = false;
        foreach (var player in playerList)
        {
            PlayerManager.Instance.BackupPlayerData(player);
            player.UpdateDataPerTurn();
            player.GetComponentInChildren<PlayerInteractionComponent>().RefreshLinePath();
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
            //PlayerManager.Instance.ResetPlayerDateAfterControlStage(player);
            PlayerManager.Instance.ResetPlayerData(player);
            PlayerManager.Instance.ResetPlayerDateAfterControlStage(player);
        }
        UIManager.Instance.ShowGridObjectUI(false, null);
    }
    public void StartS2Stage()
    {
        s2Stage.StartStage(CardManager.Instance.playedCardDict);
    }

    public void StartMoveStage()
    {
        RefreshDataClientRpc();
        Debug.LogError("MoveStage");
        moveStage.StartStage(controlStage.playerInteractDict);
    }

    [ClientRpc]
    public void RefreshDataClientRpc()
    {
        foreach (var player in playerList)
        {
            //PlayerManager.Instance.ResetPlayerDateAfterControlStage(player);
        }
    }
    [ClientRpc]
    public void RefreshAfterMoveStageClientRpc()
    {
        foreach(var player in playerList)
        {
            player.GetComponentInChildren<PlayerInteractionComponent>().RefreshLinePath();
        }
        //GridVfx is Refreshed in Move Stage Scripts
    }
    public void StartS3Stage()
    {
        RefreshAfterMoveStageClientRpc();
        s3Stage.StartStage(CardManager.Instance.playedCardDict);
    }
    

    public void StartAttackStage()
    {
        attackStage.StartStage();
    }

    public void StartS4Stage()
    {
        s4Stage.StartStage(CardManager.Instance.playedCardDict);
    }

    public int GetWinner()
    {
        List<Player> dyingPlayers = GetDyingPlayer();
        if (dyingPlayers.Count == 1)
        {
            return dyingPlayers[0].Id == PlayerId.RedPlayer ? 1 : 0;
        }
        else if (dyingPlayers.Count == 2)
        {
            dyingPlayers.OrderByDescending(a => a.HP);
            if (dyingPlayers[0].HP != dyingPlayers[1].HP)
            {
                return dyingPlayers[0].Id == PlayerId.RedPlayer ? 1 : 0;
            }
            else{
                int player0LandCount=0;
                int player1LandCount=0;
                int[] player0LandArray = dyingPlayers[0].CountAcademyOwnedPoint();
                int[] player1LandArray = dyingPlayers[1].CountAcademyOwnedPoint();
                for (int i = 0; i < 6; i++)
                {
                    player0LandCount += player0LandArray[i];
                    player1LandCount += player1LandArray[i];
                }
                if (player0LandCount == player1LandCount)
                {
                    return 2;
                }
                else
                {
                    return player0LandCount > player1LandCount ? 0 : 1;
                }
            }
        }
        return 2;
    }
    [ClientRpc]
    public void PlayerDyingStageClientRpc()
    {
        if(GetDyingPlayer().Count != 0)
        {
            OnPlayerDying?.Invoke(this, EventArgs.Empty);
        }
        foreach (var player in playerList)
        {
            if (player.HP <= 0 && player == currentPlayer)
            {
                OnPlayerSelfDying?.Invoke(this, EventArgs.Empty);
                SoundManager.Instance.PlayBgm(Bgm.DyingBGM);
            }
        }
    }
    [ClientRpc]
    public void AddPlayerHpClientRpc()
    {
        playerList[0].HP += playerList[0].HpPerRound;
        if (playerList[0].HP > playerList[0].MaxHP)
        {
            playerList[0].HP = playerList[0].MaxHP;
        }
        playerList[1].HP += playerList[1].HpPerRound;
        if (playerList[1].HP > playerList[1].MaxHP)
        {
            playerList[1].HP = playerList[1].MaxHP;
        }
    }
    [ClientRpc]
    public void DrawEventCardPerRoundClientRpc(int j)
    {
        var drawCardCompoent = FindObjectOfType<DrawCardComponent>();
        
        if (playerList[j] == currentPlayer)
        {
            for (int i = 0; i < playerList[j].eventCardPerRound; i++)
            {
                int randIndex = UnityEngine.Random.Range(0, 40);
                drawCardCompoent.DrawEventCardForTest(randIndex);
            }
            drawCardCompoent.PlayDrawCardAnimationServerRpc(playerList[j].Id, playerList[j].eventCardPerRound);
        }
    }
    public List<Player> GetDyingPlayer()
    {
        List<Player> dyingPlayers = new List<Player>();
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].HP <= 0)
            {
                dyingPlayers.Add(playerList[i]);
            }
        }
        return dyingPlayers;
    }

    public void CheckDyingStage()
    {

    }

    [ClientRpc]
    public void LeaveDyingStageClientRpc()
    {
        OnLeaveDyingStage.Invoke(this, EventArgs.Empty);
        SoundManager.Instance.PlayBgm(Bgm.EarlyBGM);
    }
    public PlayerId GetEnemy(PlayerId playerId)
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerId != playerList[i].Id)
            {
                return playerList[i].Id;
            }
            else continue;
        }
        Debug.LogError("Get Enemy Fail");
        return playerId;
    }
    [ServerRpc]
    public void ChangePlayerPriorityServerRpc()
    {
        ChangePlayerPriorityClientRpc();
    }
    [ClientRpc]
    public void ChangePlayerPriorityClientRpc()
    {
        int priorityContainer;
        priorityContainer = playerList[0].Priority;
        playerList[0].Priority = playerList[1].Priority;
        playerList[1].Priority = priorityContainer;
    }
    public Player PlayerIdToPlayer(PlayerId playerId)
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerId == playerList[i].Id)
            {
                return playerList[i];
            }
            else continue;
        }
        Debug.LogError("PlayerId To Player Fail");
        return null;
    }

    public Player GetCompetitive()
    {
        foreach(var player in playerList)
        {
            if(player.Id!=currentPlayer.Id)
            { return player; }      
        }
        return null;
    }

    [ClientRpc]
    public void SetCameraOverviewClientRpc()
    {
        CameraTest4 camera = FindObjectOfType<CameraTest4>();
        //Vector3 pos = (playerList[0].transform.position + playerList[1].transform.position) / 2;
        //camera.FocusPosition(pos, -35f);
        camera.FocusGirdCenterZoom(playerList[0].currentGrid, playerList[1].currentGrid);
    }
    [ClientRpc]
    public void SetCameraFocusSelfClientRpc()
    {
        CameraTest4 camera = FindObjectOfType<CameraTest4>();
        camera.FocusGrid();
    }
    [ClientRpc]
    public void SetCameraFocusPlayerClientRpc(Vector3 pos)
    {
        CameraTest4 camera = FindObjectOfType<CameraTest4>();
        camera.FocusPosition(pos, 0);
    }

}
