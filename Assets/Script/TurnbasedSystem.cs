using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnbasedSystem : MonoBehaviour
{
    public int RoundNumber = 0;
    public int round;
    public bool IsPlayerDead=false;
    public bool IsControlOvered = true;
    public bool IsEvent1Overed = true;
    public int ControlPhaseTime = 15;
    public int MovingPhaseTime = 15;
    public int AttackPhaseTime = 5;
    public int PlayerHD = 20;
    public GameObject EndMenu;
    public GameObject ControlMenu;
    public GameObject MoveMenu;
    public GameObject EventsMenu;
    public GameObject AttackMenu;


    // Start is called before the first frame update
    void Start()
    {
          for(int i = 0; i < 999; i++)
          {
             RoundNumber++;
             ControlPhase();
             Invoke("Event2", 15);
            if (IsPlayerDead)
            {
                break;
            }
            Invoke("MovePhase", 16);
            Invoke("Event3", 32);
            if (IsPlayerDead)
            {
                break;
            }
            Invoke("AttackPhase", 33);
            Invoke("Event4", 38);
            if (IsPlayerDead)
            {
                break;
            }
          }
            
        

    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerHD<=0)
        {
            IsPlayerDead = true;
        }
         EndJudge();

    }

    void ControlPhase()
    {
        EventsMenu.SetActive(false);
        ControlMenu.SetActive(true);
        Debug.Log("ControlPhase");
    }

    void MovePhase()
    {
        EventsMenu.SetActive(false);
        MoveMenu.SetActive(true);
        Debug.Log("MovePhase");
    }

    void AttackPhase()
    {
        EventsMenu.SetActive(false);
        AttackMenu.SetActive(true);
        Debug.Log("AttackPhase");
    }

    void Event1()
    {

    }

    void Event2()
    {
        ControlMenu.SetActive(false);
        IsControlOvered=true;
        EventsMenu.SetActive(true);
        Debug.Log("Event2");
    }

    void Event3()
    {
       MoveMenu.SetActive(false);
       EventsMenu.SetActive(true);
        Debug.Log("Event3");
    }

    void Event4()
    {
        AttackMenu.SetActive(false);
        EventsMenu.SetActive(true);
        Debug.Log("Event4");
    }
   
    void EndJudge()
    {
        Time.timeScale = 0;
        if(IsPlayerDead)
        {
           EndMenu.SetActive(true);
        }
    }


}
