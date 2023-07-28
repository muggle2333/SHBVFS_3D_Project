using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class PlayerAcademyBuffcomponent : NetworkBehaviour
{
    public Dictionary<AcademyType, AcademyBuffData[]> academyBuffDict = new Dictionary<AcademyType, AcademyBuffData[]>();
    public Dictionary<AcademyType, AcademyBuffData> redPlayerAcademyBuffDict = new Dictionary<AcademyType, AcademyBuffData>();
    public Dictionary<AcademyType, AcademyBuffData> bluePlayerAcademyBuffDict = new Dictionary<AcademyType, AcademyBuffData>();
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
            redPlayerAcademyBuffDict.Add((AcademyType)(i + 1), academyBuffData);
            bluePlayerAcademyBuffDict.Add((AcademyType)(i + 1), academyBuffData);
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
        for (int i = 0; i < 6; i++)
        {
            player.totalAcademyOwnedPoint[i] = player.academyOwnedPoint[i] + player.cardAcademyEffectNum[i];
        }
        UpdatePlayerAcademyBuffClientRpc(playerId);
        FindObjectOfType<Calculating>().AcademyBuffClientRpc(playerId);
    }
    [ClientRpc]
    public void UpdatePlayerAcademyBuffClientRpc(PlayerId playerId)
    {
        Player player = GameplayManager.Instance.PlayerIdToPlayer(playerId);
        for (int i = 0; i < (int)AcademyType.FA; i++)
        {
            academyBuffDict.TryGetValue((AcademyType)(i + 1), out academyBuffDataArr);
            if (player.Id == PlayerId.RedPlayer)
            {
                if (player.totalAcademyOwnedPoint[i] <= 4 && player.totalAcademyOwnedPoint[i] >= 0)
                {
                    redPlayerAcademyBuffDict[(AcademyType)(i + 1)] = academyBuffDataArr[player.totalAcademyOwnedPoint[i]];
                }
                else if (player.totalAcademyOwnedPoint[i] > 4)
                {
                    redPlayerAcademyBuffDict[(AcademyType)(i + 1)] = academyBuffDataArr[4];
                }
                else if (player.totalAcademyOwnedPoint[i] < 0)
                {
                    redPlayerAcademyBuffDict[(AcademyType)(i + 1)] = academyBuffDataArr[0];
                }
            }
            else
            {
                if (player.totalAcademyOwnedPoint[i] <= 4 && player.totalAcademyOwnedPoint[i] >= 0)
                {
                    bluePlayerAcademyBuffDict[(AcademyType)(i + 1)] = academyBuffDataArr[player.totalAcademyOwnedPoint[i]];
                }
                else if (player.totalAcademyOwnedPoint[i] > 4)
                {
                    bluePlayerAcademyBuffDict[(AcademyType)(i + 1)] = academyBuffDataArr[4];
                }
                else if (player.totalAcademyOwnedPoint[i] < 0)
                {
                    bluePlayerAcademyBuffDict[(AcademyType)(i + 1)] = academyBuffDataArr[0];
                }
            }
        }
    }
}
