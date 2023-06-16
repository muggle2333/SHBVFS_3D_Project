using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public int HP;
    public int MaxHP;
    public int AttackDamage;
    public int Defence;
    public int Range;

    
   


    public Character AttackTarget;

    public bool hasMoved = false; // 判断此回合是否移动过
    
    void Start()
    {
        


    }
    public void Attack()
    {
        int Damage;
        Damage = AttackDamage - AttackTarget.Defence;
        if (Damage < 0)
        {
            Damage = 0;
        }
        AttackTarget.HP -= (Damage);
        AttackTarget = null;
    }
}
