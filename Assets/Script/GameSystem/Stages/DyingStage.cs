using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DyingStage : MonoBehaviour
{
    public void StartStage(List<Player> dyingPlayerList,List<Player> alivePlayerList)
    {
        PlayerManager.Instance.PlayerDying(dyingPlayerList, alivePlayerList);

    }
    public void Update()
    {
        /*if(TurnbasedSystem.Instance.CurrentGameStage.Value == GameStage.DyingStage)
        {

        }*/
    }
}
