using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RU3 : CardFunction
{
    // Start is called before the first frame update
    void Start()
    {
        GameplayManager.Instance.currentPlayer.baseAttackDamage += 1;
        GameplayManager.Instance.currentPlayer.baseDefense -= 1;
        Calculating.Instance.CalculatPlayerBaseData(GameplayManager.Instance.currentPlayer);
    }

    // Update is called once per frame
    void Update()
    {
        if (TurnbasedSystem.Instance.CurrentGameStage.Value == GameStage.S4 || GameplayManager.Instance.currentPlayer.hasAttcaked == true)
        {
            GameplayManager.Instance.currentPlayer.baseAttackDamage -= 1;
            Calculating.Instance.CalculatPlayerBaseData(GameplayManager.Instance.currentPlayer);
            Destroy(gameObject);
        }
    }
}
