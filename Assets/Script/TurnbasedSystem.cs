using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnbasedSystem : MonoBehaviour
{
    public int RoundNumber = 0;
    public int round;
    public bool IsPlayerDead=false;
    public bool IsControlOvered = true;

    public GameObject EndMenu;


    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
         EndJudge();

    }

    void ControlPhase()
    {

    }

    void MovePhase()
    {

    }

    void AttackPhase()
    {

    }

    void Event1()
    {

    }

    void Event2()
    {

    }

    void Event3()
    {

    }

    void Event4()
    {

    }
   
    void EndJudge()
    {
        if(IsPlayerDead)
        {
           EndMenu.SetActive(true);
        }
    }


}
