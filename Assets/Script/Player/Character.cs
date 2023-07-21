using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Character : NetworkBehaviour
{
    public int HP;
    //public NetworkVariable<int> HPNetwork = new NetworkVariable<int>(0);
    public int MaxHP;
    //public NetworkVariable<int> MaxHPNetwork = new NetworkVariable<int>(0);
    public int AttackDamage;
    //public NetworkVariable<int> AttackDamageNetwork = new NetworkVariable<int>(0);
    public int Defence;
    //public NetworkVariable<int> DefenceNetwork = new NetworkVariable<int>(0);
    public int Range;
    //public NetworkVariable<int> RangeNetwork = new NetworkVariable<int>(0);

    public bool hasAttcaked;



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
        hasAttcaked = true;
        AttackTarget = null;
    }
}
