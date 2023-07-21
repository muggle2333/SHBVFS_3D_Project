using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class PlayerAcademyBuffcomponent : NetworkBehaviour
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
            UpdatePlayerAcademyBuffServerRpc(player);
        }
    }*/
    [ServerRpc]
    public void UpdatePlayerAcademyBuffServerRpc(PlayerId playerId)
    {
        Player player = GameplayManager.Instance.PlayerIdToPlayer(playerId);
        for (int i=0; i < 6; i++)
        {
            player.totalAcademyOwnedPoint[i] = player.academyOwnedPoint[i] + player.cardAcademyEffectNum[i];
        }
        
        for (int i = 0; i < (int)AcademyType.FA; i++)
        {
            academyBuffDict.TryGetValue((AcademyType)(i+1), out academyBuffDataArr);
            if (player.totalAcademyOwnedPoint[i] <= 4 && player.totalAcademyOwnedPoint[i] >=0)
            {
                PlayerAcademyBuffDict[(AcademyType)(i + 1)] = academyBuffDataArr[player.totalAcademyOwnedPoint[i]];
            }
            else if(player.totalAcademyOwnedPoint[i] >4)
            {
                PlayerAcademyBuffDict[(AcademyType)(i + 1)] = academyBuffDataArr[4];
            }
            else if (player.totalAcademyOwnedPoint[i] <0)
            {
                PlayerAcademyBuffDict[(AcademyType)(i + 1)] = academyBuffDataArr[0];
            }
        }
        FindObjectOfType<Calculating>().AcademyBuffClientRpc(playerId);
    }
}
