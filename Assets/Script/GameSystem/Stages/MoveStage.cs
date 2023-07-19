using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

//Move 和 Attack 的优先级是轮次的
public class MoveStage : NetworkBehaviour
{
    private Dictionary<Player, List<PlayerInteract>> playerInteractDict = new Dictionary<Player, List<PlayerInteract>>();

    private List<Player> playerList = new List<Player>();

    public void StartStage(Dictionary<Player, List<PlayerInteract>> playerInteractDict)
    {
        if (FindObjectOfType<NetworkManager>()!=null && !NetworkManager.Singleton.IsHost) return;
        this.playerInteractDict = playerInteractDict;
        playerList = new List<Player>();
        StartCoroutine("StartMove");
    }

    IEnumerator StartMove()
    {
        for(int i = 0;i< playerInteractDict.Count;i++)
        {
            playerList.Add(playerInteractDict.ElementAt(i).Key);
        }
        List<Player> priorityList = null;
        if (TurnbasedSystem.Instance.roundIndex.Value %2 != 0)
        {
            priorityList = playerList.OrderByDescending(x => x.Priority).ToList();
        }
        else
        {
            priorityList = playerList.OrderBy(x => x.Priority).ToList();
        }

        while(playerInteractDict.Count!=0)
        {
            for(int i = 0;i< priorityList.Count;i++) 
            {
                List<PlayerInteract> playerInteract = null;
                if (playerInteractDict.TryGetValue(priorityList[i], out playerInteract))
                {
                    if(playerInteract.Count==0)
                    {
                        playerInteractDict.Remove(priorityList[i]);
                        break;
                    }
                    //Interact 
                    //Debug.Log(priorityList[i].name + " " + playerInteract[0].PlayerInteractType);

                    yield return new WaitForSeconds(1);


                    if(FindObjectOfType<NetworkManager>())
                    {
                        //针对特定的client执行操作
                        /*PlayerManager.Instance.InteractClientRpc(priorityList[i].Id, playerInteract[0], new ClientRpcParams
                        {
                            Send = new ClientRpcSendParams
                            {
                                TargetClientIds = new ulong[] { (ulong)priorityList[i].Id }
                            }
                        });*/

                        PlayerManager.Instance.InteractClientRpc(priorityList[i].Id, playerInteract[0]);
                    }
                    else
                    {
                        PlayerManager.Instance.Interact(priorityList[i], playerInteract[0]);
                    }

                    playerInteract.RemoveAt(0);
                    playerInteractDict[priorityList[i]] = playerInteract;
                }
            }
        }
        EndStage();
    }

    public void EndStage()
    {
        GridManager.Instance.RefreshGridVfxClientRpc();
        TurnbasedSystem.Instance.CompleteStage(GameStage.MoveStage);
    }
}
