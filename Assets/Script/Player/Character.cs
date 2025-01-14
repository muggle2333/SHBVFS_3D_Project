﻿using System;
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
    public int HpPerRound;
    public int eventCardPerRound;
    public bool hasAttcaked;
    public int damageThisRound;


    public Character AttackTarget;

    public bool hasMoved = false; // 判断此回合是否移动过
    
    void Start()
    {
        


    }
    public void Attack()
    {
        int Damage;
        Damage = AttackDamage - AttackTarget.Defence;
        if (Damage <= 0)
        {
            Damage = 0;
        }
        damageThisRound = Damage;
        AttackTarget.HP -= (Damage);
        AttackTarget.ChangeHP(-Damage);
        hasAttcaked = true;
        AttackTarget = null;
    }

    public void ChangeHP(int hpTmp)
    {
        if(hpTmp>0)
        {
            GetComponentInChildren<PlayerInteractionComponent>().PlayRecoverVfx();
            VfxManager.Instance.PlayVfx(transform.position, VfxType.HpAdd);
        }
        else if(hpTmp<0)
        {
            GetComponentInChildren<PlayerInteractionComponent>().PlayHitVfxRed();
            VfxManager.Instance.PlayVfx(transform.position, VfxType.HpDeduce);
        }
        else
        {
            GetComponentInChildren<PlayerInteractionComponent>().PlayDefendVfx();
        }

        //HP += hpTmp;
    }
    public void ChangeVfx(VfxType vfxType,int changeCount)
    {
        switch (vfxType)
        {
            case VfxType.Damage:
                if (changeCount > 0)
                {
                    VfxManager.Instance.PlayVfx(transform.position, VfxType.Damage);
                }
                return;
            case VfxType.Defence:
                if (changeCount > 0)
                {
                    VfxManager.Instance.PlayVfx(transform.position, VfxType.Defence);
                }
                return;
            case VfxType.Range:
                if (changeCount > 0)
                {
                    VfxManager.Instance.PlayVfx(transform.position, VfxType.Range);
                }
                return;
            case VfxType.AP:
                if (changeCount > 0)
                {
                    VfxManager.Instance.PlayVfx(transform.position, VfxType.AP);
                }
                return;
            case VfxType.AcademyAdd:
                if (changeCount > 0)
                {
                    VfxManager.Instance.PlayVfx(transform.position, VfxType.AcademyAdd);
                }
                return;
            case VfxType.AcademyDeduce:
                if (changeCount < 0)
                {
                    VfxManager.Instance.PlayVfx(transform.position, VfxType.AcademyDeduce);
                }
                return;
        }
    }
}
