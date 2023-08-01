using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RU3 : CardFunction
{
    // Start is called before the first frame update
    void Start()
    {
        if (player.baseDefense > 0)
        {
            Function();
        }
        else
        {
            Destroy(gameObject);
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        if (TurnbasedSystem.Instance.CurrentGameStage.Value == GameStage.S4)
        {
            player.baseAttackDamage -= 1;
            player.cardAD -= 1;
            Calculating.Instance.CalculatPlayerBaseData(player);
            Destroy(gameObject);
        }
    }
    void Function()
    {
        player.baseAttackDamage += 1;
        player.baseDefense -= 1;
        player.cardAD += 1;
        Calculating.Instance.CalculatPlayerBaseData(player);
    }
}
