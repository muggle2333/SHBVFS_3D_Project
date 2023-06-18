using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerId
{
    RedPlayer,
    BluePlayer,
    None,
    Both,
}

public class Player : Character
{
    public PlayerId Id;

    public int CurrentActionPoint;
    public int ActionPointPerRound;
    public int MaxActionPoint;
    public int MaxCardCount;

    public GridObject currentGrid;

    Dictionary<AcademyType, List<GridObject>> OwendLandDic = new Dictionary<AcademyType, List<GridObject>>();

    [System.Serializable]
    public class OwnedLandTest
    {
        public AcademyType TestTacademy;
        public List<GridObject> Testlist;
    }

   // [SerializeField]
   // private OwnedLandTest ownedLandTest;
    void Start()
    {
        MaxHP = 3;
        HP = MaxHP;
        AttackDamage = 1;
        Defence = 0;
        Range = 1;
        ActionPointPerRound = 3;

        List<GridObject> yiLand;
        OwendLandDic.TryGetValue(AcademyType.YI, out yiLand);
        //Debug.Log(yiLand.Count);
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
