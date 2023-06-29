using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlStage : MonoBehaviour
{
    public Dictionary<Player, List<PlayerInteract>> playerInteractDict = new Dictionary<Player, List<PlayerInteract>>();

    //用来inspector上观察的
    public List<PlayerInteract> RedPlayerInteractList;
    public List<PlayerInteract> BluePlayerInteractList;

    public void StartStage()
    {
        playerInteractDict = new Dictionary<Player, List<PlayerInteract>>();
    }
    public void AddPlayerInteract(Player player,PlayerInteract playerInteract)
    {
        List<PlayerInteract> list = null;
        if(playerInteractDict.TryGetValue(player, out list))
        {
            list.Add(playerInteract);
        }
        else
        {
            list = new List<PlayerInteract>();
            list.Add(playerInteract);
            playerInteractDict.Add(player, list);
        }
       
        
        if(player.Id==PlayerId.RedPlayer)
        {
            playerInteractDict.TryGetValue(player, out RedPlayerInteractList);
        }
        else
        {
            playerInteractDict.TryGetValue(player, out BluePlayerInteractList);
        }
        
    }

}
