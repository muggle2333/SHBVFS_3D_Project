using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AcademyBuffData
{
    public int hp;
    public int hpPreRound;
    public int attackRange;
    public int attackDamage;
    public int defense;
    public int APPerRound;
}
public class PlayerAcademyBuffcomponent : MonoBehaviour
{
    //public Dictionary<AcademyType, Dictionary<int,AcademyBuffData>> AcademyBuff = new Dictionary<AcademyType, Dictionary<int, AcademyBuffData>>();
    // Start is called before the first frame update
    public Player player;

    public List<int>[][] academyBuff;
    void Start()
    {
        //AcademyBuff.Add(AcademyType.YI, ( 0,  ));
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdatePlayerAcademyBuff()
    {
        
    }
}
