using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event EventHandler OnLoalPlayerChanged;
    private enum GameState
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }

    private GameState gameState = GameState.WaitingToStart;
    private bool isLocalPlayerReady = false;
    private void GameInput_OnInteractAction()
    {
        if(gameState == GameState.WaitingToStart)
        {
            isLocalPlayerReady= true;
            SetPlyaerReadyServerRpc();
            OnLoalPlayerChanged?.Invoke(this,EventArgs.Empty);
        }
    }

    [ServerRpc(RequireOwnership =false)]
    private void SetPlyaerReadyServerRpc(ServerRpcParams serverRpcParams=default)
    {
    
    }


    public bool IsWaitingToStart()
    {
        return gameState == GameState.WaitingToStart;
    }
}
