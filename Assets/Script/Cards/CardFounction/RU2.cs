using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RU2 : CardFounction
{
    public bool hasAdded = false;
    public bool CanAdd = false;
    // Start is called before the first frame update
    void Start()
    {
        Founction(GameplayManager.Instance.currentPlayer);
    }

    // Update is called once per frame
    void Update()
    {
        if(TurnbasedSystem.Instance.CurrentGameStage == GameStage.S1 && hasAdded == false && CanAdd == true)
        {
            GameplayManager.Instance.currentPlayer.Defence += 2;
            hasAdded = true;
        }
        if(TurnbasedSystem.Instance.CurrentGameStage == GameStage.S4 && hasAdded == true)
        {
            GameplayManager.Instance.currentPlayer.Defence -= 2;
            Destroy(gameObject);
        }
    }
    void Founction(Player player)
    {
        if (player.hasAttcaked == false)
        {
            CanAdd = true;
        }
    }
}
