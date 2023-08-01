using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DyingManager : MonoBehaviour
{
    public static DyingManager Instance { get; private set; }
    private float timerValue = 0;

    public void Awake()
    {
        Instance = this; 
    }
    public void Update()
    {
        if(timerValue> 0)
        {
            timerValue -= Time.deltaTime;
        }
        else
        {
            timerValue = 0;
        }
    }
    public void CheckIsDying()
    {
        List<Player> dyingPlayers = GameplayManager.Instance.GetDyingPlayer();
        if (dyingPlayers.Count == 0)
        {
            TurnbasedSystem.Instance.isDie.Value = false;
        }
        else
        {
            TurnbasedSystem.Instance.isDie.Value = true;
            StartDyingStage();
        }
    }
    public void StartDyingStage()
    {
        StartCoroutine("StartDying");
    }

    IEnumerator StartDying()
    {   
        GameplayManager.Instance.PlayerDyingStageClientRpc();
        timerValue = GameplayManager.DYING_TIMER;
        while (timerValue>0)
        {
            if(!CheckIsAnyoneDying())
            {
                break;
            }
            yield return null;
        }
        CheckLeaveDying();
        yield return null;
    }
    private bool CheckIsAnyoneDying()
    {
        List<Player> dyingPlayers = GameplayManager.Instance.GetDyingPlayer();
        return dyingPlayers.Count > 0;
    }
    private void CheckLeaveDying()
    {
        if (CheckIsAnyoneDying())
        {
            GameManager.Instance.SetGameOver();
        }
        else
        {
            TurnbasedSystem.Instance.isDie.Value = false;
            GameplayManager.Instance.LeaveDyingStageClientRpc();
        }
    }
}
