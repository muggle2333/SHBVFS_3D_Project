using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public enum GameStage
{
    S1, //Control Stage
    RemoveStage,
    S2,
    MoveStage,
    S3,
    AttackStage,
    S4,

}
public class TurnbasedSystem : MonoBehaviour
{
    public GameStage  CurrentGameStage;
    public int RoundNumber = 0;
    public float ControlPhaseTime = 180;
    public float MovingPhaseTime = 15;
    public float AttackPhaseTime = 5;
    public float PlayerHp = 20;
    public bool IsPlayerDead = false;
    public bool IsControlOvered = true;
    public bool IsEvent1Overed = true;
    public bool IsBackAlive = false;
    public GameObject EndMenu;
    public GameObject ControlMenu;
    public GameObject MoveMenu;
    public GameObject EventsMenu;
    public GameObject AttackMenu;


    #region Timer
    public float TimerValue ;

    public TextMeshProUGUI Timer;

    #endregion

    public TextMeshProUGUI Rounds;

    // Start is called before the first frame update
    void Start()
    {
        TimerValue = ControlPhaseTime;
        StartCoroutine("TurnStart");

    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerHp<=0)
        {
            IsPlayerDead = true;
        }
        EndJudge();
        BackAlive();
       
        if(TimerValue>0)
        {
            TimerValue -= Time.deltaTime;
        }
        else
        {
            TimerValue = 0;
        }
        DisplayTimer(TimerValue);

        Rounds.text = RoundNumber.ToString();
    }
    #region One turn
    IEnumerator TurnStart()
    {
        RoundNumber++;
        ControlPhase();
        yield return new WaitForSecondsRealtime(ControlPhaseTime);
        StartCoroutine("Event2");
    }
    #endregion

    #region Single phases
    //S1
    void ControlPhase()
    {
        CurrentGameStage = GameStage.S1;
        TimerValue = ControlPhaseTime;
        EventsMenu.SetActive(false);
        ControlMenu.SetActive(true);
        Debug.Log("ControlPhase");
       
    }


    void MovePhase()
    {
        CurrentGameStage = GameStage.MoveStage;
        EventsMenu.SetActive(false);
        MoveMenu.SetActive(true);
        Debug.Log("MovePhase");
       
    }

    void AttackPhase()
    {
        CurrentGameStage = GameStage.AttackStage;
        EventsMenu.SetActive(false);
        AttackMenu.SetActive(true);
        Debug.Log("AttackPhase");
       
    }

    void Event1()
    {

    }

    IEnumerator Event2()
    {
        CurrentGameStage = GameStage.S2;
        ControlMenu.SetActive(false);
        IsControlOvered=true;
        EventsMenu.SetActive(true);
        Debug.Log("Event2");
        MovePhase();
        yield return new WaitForSecondsRealtime(MovingPhaseTime);
        Event3();
        AttackPhase();
        yield return new WaitForSecondsRealtime(AttackPhaseTime);
        Event4();
    }

    void Event3()
    {
        CurrentGameStage = GameStage.S3;
        MoveMenu.SetActive(false);
        EventsMenu.SetActive(true);
        Debug.Log("Event3");
       
    }

    void Event4()
    {
        CurrentGameStage = GameStage.S4;
        // PlayerHp--;
        AttackMenu.SetActive(false);
        EventsMenu.SetActive(true);
        //RoundNumber++;
        Debug.Log("Event4");
        StartCoroutine("TurnStart");
        
       
    }
    #endregion

    void EndJudge()
    {
        //Time.timeScale = 0;
        if(IsPlayerDead)
        {
            Time.timeScale = 0;
            EndMenu.SetActive(true);
        }
    }

    void BackAlive()
    {
        if(IsBackAlive)
        {
            EndMenu.SetActive(false);
            Time.timeScale = 1;
            IsBackAlive = false;
            IsPlayerDead = false;
        }
    }

    #region Display TurnBasedTimer
   void DisplayTimer(float timeToDisplay)
    {
        if(timeToDisplay < 0)
        {
            timeToDisplay = 0;
        }

        Timer.text = Mathf.FloorToInt(TimerValue) .ToString();
    }

    #endregion

    public void TurnOver()
    {
        StopCoroutine("TurnStart");
        StartCoroutine("Event2");
    }
}
