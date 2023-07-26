using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum PlayerId
{
    RedPlayer,
    BluePlayer,
}


public class Player : Character
{
    public int moveCount;
    public int descoverLandCount;
    public int occuplyCount;
    public GameObject headCard;
    public TMP_Text headCardText;

    public PlayerId Id;
    public NetworkVariable<int> playerIdNetwork;

    public bool canCost1APInEnemy = false;
    public bool canFreeMoveInSelfGrid = false;

    public int CurrentActionPoint; //control阶段也会扣
    public int TrueActionPoint;
    public int ActionPointPerRound;
    public int MaxActionPoint;
    public int MaxCardCount;
    public int freeMoveCount;

    public int gridAR;
    public int gridDF;
    public int cardAR;
    //public NetworkVariable<int> cardARNetwork = new NetworkVariable<int>(0);
    public int cardAD;
    //public NetworkVariable<int> cardADNetwork = new NetworkVariable<int>(0);
    public int cardDF;
    //public NetworkVariable<int> cardDFNetwork = new NetworkVariable<int>(0);

    public int baseMaxHP = 3;
    //public NetworkVariable<int> baseMaxHPNetwork = new NetworkVariable<int>(3);
    public int baseDefense = 0;
    //public NetworkVariable<int> baseDefenseNetwork = new NetworkVariable<int>(0);
    public int baseAttackDamage = 1;
    //public NetworkVariable<int> baseAttackDamageNetwork = new NetworkVariable<int>(1);
    public int baseRange = 1;
    //public NetworkVariable<int> baseRangeNetwork = new NetworkVariable<int>(1);
    public int baseActionPointPerRound = 3;

    public int Priority;
    public NetworkVariable<bool> isDying = new NetworkVariable<bool>(false);

    public List<int> handCards;

    public GridObject targetGrid;
    public GridObject currentGrid;
    public GridObject trueGrid;
    public NetworkList<int> academyOwnedPoint;
    public NetworkList<int> cardAcademyEffectNum;
    public NetworkList<int> totalAcademyOwnedPoint;
    public Dictionary<AcademyType, List<GridObject>> OwnedLandDic = new Dictionary<AcademyType, List<GridObject>>()
    /*    {
            { AcademyType.Null,null },
            { AcademyType.YI,null },
            { AcademyType.DAO,null },
            { AcademyType.MO,null },
            { AcademyType.BING,null },
            { AcademyType.RU,null },
        }*/;

    //public int[] academyOwnedPoint = new int[6];

    [System.Serializable]
    public class OwnedLandTest
    {
        public AcademyType TestTacademy;
        public List<GridObject> Testlist;
    }

    // [SerializeField]
    // private OwnedLandTest ownedLandTest;

    public void Awake()
    {
        academyOwnedPoint = new NetworkList<int>();
        cardAcademyEffectNum = new NetworkList<int>();
        totalAcademyOwnedPoint = new NetworkList<int>();

    }
    void Start()
    {
        MaxHP = 3;
        HP = MaxHP;
        AttackDamage = 1;
        Defence = 0;
        Range = 1;
        ActionPointPerRound = 3;
        CurrentActionPoint = 0;
        TrueActionPoint = CurrentActionPoint;
        Invoke("PlayerAcademyDataServerRpc", 3);

        List<GridObject> yiLand;
        OwnedLandDic.TryGetValue(AcademyType.YI, out yiLand);
        //Debug.Log(yiLand.Count);
    }
    [ServerRpc(RequireOwnership = false)]
    public void PlayerAcademyDataServerRpc()
    {
        for (int i = 0; i < 6; i++)
        {
            totalAcademyOwnedPoint.Add(0);
            academyOwnedPoint.Add(0);
            cardAcademyEffectNum.Add(0);
        }
    }
    void Update()
    {
        if (currentGrid.landType == LandType.Mountain)
        {
            gridAR = 1;
        }
        else if (currentGrid.landType == LandType.Forest)
        {
            gridDF = 1;
        }
        else if (currentGrid.landType == LandType.Lake || currentGrid.landType == LandType.Plain)
        {
            gridAR = 0;
            gridDF = 0;
        }
        MaxActionPoint = ActionPointPerRound * 2;
        MaxCardCount = HP;
        if (Defence < 0)
        {
            Defence = 0;
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (this == GameplayManager.Instance.currentPlayer)
            {
                List<int> a = new List<int>();
                for (int i = 0; i < 6; i++)
                {
                    //a.Add(0);
                    //a[i] = academyOwnedPoint[i];
                    Debug.LogError(academyOwnedPoint[i]);
                }

            }
        }
    }

    public void UpdateDataPerTurn()
    {
        CurrentActionPoint = CurrentActionPoint + ActionPointPerRound > MaxActionPoint ? MaxActionPoint : CurrentActionPoint + ActionPointPerRound;
        TrueActionPoint = CurrentActionPoint;

    }
    public bool OccupyGrid(GridObject gridObject)
    {
        occuplyCount++;
        List<GridObject> gridList;
        if (OwnedLandDic.TryGetValue(gridObject.academy, out gridList))
        {
            gridList.Add(gridObject);
            OwnedLandDic[gridObject.academy] = gridList;
        }
        else
        {
            List<GridObject> gridListNew = new List<GridObject>();
            gridListNew.Add(gridObject);
            OwnedLandDic.Add(gridObject.academy, gridListNew);
        }

        if (NetworkManager.Singleton.IsServer)
        {
            academyOwnedPoint[(int)gridObject.academy - 1]++;
        }
        return true;
    }


    public int[] CountAcademyOwnedPoint()
    {
        int[] academyOwnedPoint = new int[6];
        //for (int i=0;i<6;i++)
        //{
        //    List<GridObject> gridList;
        //    OwnedLandDic.TryGetValue((AcademyType)(i+1), out gridList); //academy 第一个是 null
        //    academyOwnedPoint[i] = gridList.Count;
        //}
        var grid = GridManager.Instance.grid;
        for (int x = 0; x < grid.width; x++)
        {
            for (int z = 0; z < grid.length; z++)
            {
                if (grid.gridArray[x, z].owner == this && grid.gridArray[x, z].academy != AcademyType.Null)
                {
                    int academyIndex = (int)grid.gridArray[x, z].academy;
                    academyOwnedPoint[academyIndex - 1]++;
                }
            }
        }
        return academyOwnedPoint;
    }
    public void RemoveOwnedLand(GridObject gridObject)
    {
        List<GridObject> gridList;
        if (OwnedLandDic.TryGetValue(gridObject.academy, out gridList))
        {
            foreach (var tmpGridObject in gridList)
            {
                if (GridManager.Instance.CheckGridObjectIsSame(gridObject, tmpGridObject))
                {
                    gridList.Remove(tmpGridObject);
                    break;
                }
            }
            OwnedLandDic[gridObject.academy] = gridList;
            if (NetworkManager.Singleton.IsServer)
            {
                academyOwnedPoint[(int)gridObject.academy - 1]--;
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void RefreshAcademyOwnedPointServerRpc()
    {
        List<int> playeracademyOwnedPoint = CountAcademyOwnedPoint().ToList();
        for (int i = 0; i < playeracademyOwnedPoint.Count; i++)
        {
            academyOwnedPoint[i] = playeracademyOwnedPoint[i];
        }
    }

    public void ChangeAcademyOwnPoint(int[] academyPointEffect)
    {
        for (int i = 0; i < academyOwnedPoint.Count; i++)
        {
            academyOwnedPoint[i] += academyPointEffect[i];
        }
    }

    public void UpdatePlayerDate(int[] academyPointEffect, PlayerDataEffect playerDataEffect)
    {
        ChangeAcademyOwnPoint(academyPointEffect);
    }

    public bool UseActionPoint(int apCost)
    {
        if (CurrentActionPoint < apCost)
        {
            return false;
        }
        else
        {
            CurrentActionPoint -= apCost;
            return true;
        }
    }

    public bool IsApEnough(int apCost)
    {
        return CurrentActionPoint >= apCost;
    }


    //public void RefreshLinePath()
    //{
    //    LineRenderer lineRenderer= GetComponentInChildren<LineRenderer>();
    //    lineRenderer.transform.rotation = Quaternion.LookRotation(new Vector3(0, -0.5f, 0), lineRenderer.transform.up);
    //    lineRenderer.positionCount= 1;
    //    lineRenderer.SetPosition(0,transform.position+new Vector3(0,0.1f,0));
    //}
    //public void UpdateLinePath(LandType landType)
    //{
    //    LineRenderer lineRenderer = GetComponentInChildren<LineRenderer>();
    //    lineRenderer.positionCount += 1;
    //    Vector3 offset = new Vector3(0,0.1f,0);
    //    if(landType==LandType.Mountain)
    //    {
    //        offset = new Vector3(0, 1.7f, 0);
    //    }
    //    lineRenderer.SetPosition(lineRenderer.positionCount -1, transform.position+offset);
    //}
    [ServerRpc(RequireOwnership =false)]
    public void RecoverHpServerRpc(int hp)
    {
        RecoverHpClientRpc(hp);
    }
    [ClientRpc]
    public void RecoverHpClientRpc(int hp)
    {
        Debug.LogError("addHp");
        HP += hp;
        if(HP > MaxHP)
        {
            HP = MaxHP;
        }
    }
}
