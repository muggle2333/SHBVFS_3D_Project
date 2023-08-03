using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class AttackStage : MonoBehaviour
{
    private List<Player> players;
    private int distance;

    public void StartStage()
    {
        StartCoroutine("StartAttack");
        #region Johnny
        //players = GameplayManager.Instance.GetPlayer();
        //Debug.Log(players.Count);
        //distance = PlayerManager.Instance.CheckDistance(players[0], players[1].currentGrid);
        //if (players[0].Range >= distance && players[1].Range >= distance)
        //{
        //    if (players[0].Priority > players[1].Priority)
        //    {
        //        players[0].AttackTarget = players[1];
        //        players[0].Attack();
        //        players[1].AttackTarget = players[0];
        //        players[1].Attack();
        //    }
        //    else
        //    {
        //        players[1].AttackTarget = players[0];
        //        players[1].Attack();
        //        players[0].AttackTarget = players[1];
        //        players[0].Attack();
        //    }
        //}
        //else if(players[0].Range >= distance && players[1].Range < distance)
        //{
        //    players[0].AttackTarget = players[1];
        //    players[0].Attack();
        //}
        //else if (players[0].Range < distance && players[1].Range >= distance)
        //{
        //    players[1].AttackTarget = players[0];
        //    players[1].Attack();
        //}
        #endregion
    }

    IEnumerator StartAttack()
    {
        yield return new WaitForSeconds(1);
        List<Player> playerList = GameplayManager.Instance.GetPlayer();
        playerList = playerList.OrderByDescending(x => x.Priority).ToList();
        //Attack per Player
        for (int i = 0; i < playerList.Count; i++)
        {
            int minDistance = 10;
            int targetIndex = i;
            if (playerList[i].canAttack == false) continue;
            //Find target in min distance
            playerList[i].GetComponentInChildren<PlayerInteractionComponent>().PlayRangeVfx(playerList[i].transform.position);
            for (int j = 0; j < playerList.Count; j++)
            {
                if (j == i) continue;
                //distance = PlayerManager.Instance.CheckDistance(playerList[i], playerList[j].currentGrid);
                distance = GridManager.Instance.GetGridDistance(playerList[i].currentGrid, playerList[j].currentGrid);
                //Debug.LogError(distance);
                //Debug.LogError(playerList[i] + " distance " + distance);
                
                //if (distance < minDistance && playerList[i].Range >= distance)
                if (playerList[i].Range >= distance)
                {
                    minDistance = distance;
                    targetIndex = j;
                    if (targetIndex != i)
                    {
                        if (FindObjectOfType<NetworkManager>() == null)
                        {
                            playerList[i].AttackTarget = playerList[targetIndex];
                            playerList[i].Attack();
                            playerList[targetIndex].GetComponentInChildren<PlayerInteractionComponent>().PlayHitVfxRed();
                            SoundManager.Instance.PlaySound(Sound.Attack);
                            //Debug.LogError(playerList[i] + " attack " + playerList[targetIndex]);
                        }
                        else
                        {
                            PlayerManager.Instance.SetAttackClientRpc(playerList[i].Id, playerList[targetIndex].Id);
                        }
                    }
                }

            }
            yield return new WaitForSecondsRealtime(2f);
            //Check dying
            //TurnbasedSystem.Instance.isDie.Value = true;
            DyingManager.Instance.CheckIsDying();
            yield return new WaitForSecondsRealtime(0.2f);
            yield return new WaitUntil(() => TurnbasedSystem.Instance.isDie.Value == false);
        }
        TurnbasedSystem.Instance.CompleteStage(GameStage.AttackStage);
    }

}
