using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerAcademyBuffcomponent : MonoBehaviour
{
    public Dictionary<AcademyType, AcademyBuffData[]> academyBuffDict = new Dictionary<AcademyType, AcademyBuffData[]>();
    public Dictionary<AcademyType, AcademyBuffData> PlayerAcademyBuffDict = new Dictionary<AcademyType, AcademyBuffData>();
    // Start is called before the first frame update
    //public int[][] academyBuff;
    public AcademyBuffData[] academyBuffDataArr;
    public AcademyBuffData academyBuffData;
    public Player player;
    public int a;
    void Start()
    {
        AcademyBuff[] buffs = Resources.LoadAll<AcademyBuff>("AcademyBuffs");
        foreach(var buff in buffs)
        {

            academyBuffDict.Add(buff.academyType, buff.academyBuffDatas);
        }

        for (int i = 0; i < (int)AcademyType.FA; i++)
        {
            PlayerAcademyBuffDict.Add((AcademyType)(i + 1), academyBuffData);
        }
    }
    

    // Update is called once per frame
    /*void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            academyBuffDict.TryGetValue((AcademyType)a, out academyBuffDataArr);
            academyBuffData = academyBuffDataArr[player.academyOwnedPoint[a]];
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            academyBuffDict.TryGetValue((AcademyType)a, out academyBuffDataArr);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            UpdatePlayerAcademyBuff(player);
        }
    }*/

    public void UpdatePlayerAcademyBuff(Player player)
    {
        for (int i = 0; i < (int)AcademyType.FA; i++)
        {
            academyBuffDict.TryGetValue((AcademyType)(i+1), out academyBuffDataArr);
            if (player.academyOwnedPoint[i] <= 4 && player.academyOwnedPoint[i] >=0)
            {
                PlayerAcademyBuffDict[(AcademyType)(i + 1)] = academyBuffDataArr[player.academyOwnedPoint[i]];
            }
            else if(player.academyOwnedPoint[i] >4)
            {
                PlayerAcademyBuffDict[(AcademyType)(i + 1)] = academyBuffDataArr[4];
            }
            else if (player.academyOwnedPoint[i] <0)
            {
                PlayerAcademyBuffDict[(AcademyType)(i + 1)] = academyBuffDataArr[0];
            }
        }
        FindObjectOfType<Calculating>().AcademyBuff(PlayerAcademyBuffDict,player);
    }
}
