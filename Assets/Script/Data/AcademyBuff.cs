using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct AcademyBuffData
{
    public int maxHp;
    public int hpPreRound;
    public int attackRange;
    public int attackDamage;
    public int defense;
    public int APPerRound;
    public int basicCardPerRound;
}

[CreateAssetMenu(fileName ="AcademyBuff",menuName ="AcademyBuff")]
public class AcademyBuff : ScriptableObject
{
    public AcademyType academyType;

    public AcademyBuffData[] academyBuffDatas = new AcademyBuffData[5];
}
