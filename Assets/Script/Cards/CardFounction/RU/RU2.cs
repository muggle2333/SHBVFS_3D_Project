using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RU2 : CardFunction
{
    public bool hasAdded = false;
    public bool CanAdd = false;
    // Start is called before the first frame update
    void Start()
    {
        Function(GameplayManager.Instance.currentPlayer);
    }

    // Update is called once per frame
    void Update()
    {
        if(TurnbasedSystem.Instance.CurrentGameStage.Value == GameStage.S1 && hasAdded == false && CanAdd == true)
        {
            GameplayManager.Instance.currentPlayer.baseDefense += 2;
            Calculating.Instance.CalculatPlayerBaseData(GameplayManager.Instance.currentPlayer);
            hasAdded = true;
        }
        if(TurnbasedSystem.Instance.CurrentGameStage.Value == GameStage.S4 && hasAdded == true)
        {
            GameplayManager.Instance.currentPlayer.baseDefense -= 2;
            Calculating.Instance.CalculatPlayerBaseData(GameplayManager.Instance.currentPlayer);
            Destroy(gameObject);
        }
    }
    void Function(Player player)
    {
        if (player.hasAttcaked == false)
        {
            CanAdd = true;
        }
    }
}
