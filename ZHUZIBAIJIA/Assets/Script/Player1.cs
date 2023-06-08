using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1 : Character
{
    void Start()
    {
        MaxHP = 3;
        HP = MaxHP;
        AttackDamage = 1;
        Defence = 0;
        Range = 1;
        ActionPointPerRound = 3;
    }
    void Update()
    {
        MaxActionPoint = ActionPointPerRound * 2;
        MaxCardCount = HP;
        if (AttackTarget != null)
        {
            Attack();
        }
        if (HP > MaxHP)
        {
            HP = MaxHP;
        }
    }
}
