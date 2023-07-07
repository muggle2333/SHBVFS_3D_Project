using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BING6 : CardFunction
{
    private List<Player> players;
    private int distance;
    private Player thisPlayer;
    private Player otherPlayer;
    // Start is called before the first frame update
    void Start()
    {
        players = GameplayManager.Instance.GetPlayer();
        Function(GameplayManager.Instance.currentPlayer);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Function(Player player)
    {
        thisPlayer = player;
        if (players[0] == player)
        {
            otherPlayer = players[1];
        }
        else
        {
            otherPlayer = players[0];
        }
        distance = PlayerManager.Instance.CheckDistance(thisPlayer, otherPlayer.currentGrid);
        if(thisPlayer.Range >= distance)
        {
            for (int i = 0; i < Mathf.FloorToInt(CardManager.Instance.playedCardDict[player].Count / 3); i++)
            {
                thisPlayer.AttackTarget = otherPlayer;
                player.Attack();
            }
        }
        Destroy(gameObject);
    }
}
