using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerAcademyBuffcomponent : MonoBehaviour
{
    public Dictionary<AcademyType, AcademyBuffData[]> academyBuffDict = new Dictionary<AcademyType, AcademyBuffData[]>();
    // Start is called before the first frame update
    //public int[][] academyBuff;
    public AcademyBuffData[] academyBuffDataArr;
    public AcademyBuffData academyBuffData;
    void Start()
    {
        AcademyBuff[] buffs = Resources.LoadAll<AcademyBuff>("AcademyBuffs");
        foreach(var buff in buffs)
        {
            academyBuffDict.Add(buff.academyType, buff.academyBuffDatas);
        }
    }
    

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdatePlayerAcademyBuff(Player player)
    {
        for (int i = 0; i < (int)AcademyType.FA; i++)
        {
            academyBuffDict.TryGetValue((AcademyType)i, out academyBuffDataArr);
            academyBuffData = academyBuffDataArr[player.academyOwnedPoint[i]];
            FindObjectOfType<Caculating>().AcademyBuff(academyBuffData);
        }
        
    }
}
