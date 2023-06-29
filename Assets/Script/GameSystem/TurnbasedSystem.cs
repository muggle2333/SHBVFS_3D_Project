using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public enum GameStage
{
    S1, //Opreation Stage
    DiscardStage,
    S2,
    MoveStage,
    S3,
    AttackStage,
    S4,

}
public class TurnbasedSystem : MonoBehaviour
{
    public static TurnbasedSystem Instance { get; private set; }
    public GameStage  CurrentGameStage;
    public int RoundNumber = 0;
    public float ControlPhaseTime = 180;
    public float MovingPhaseTime = 15;
    public float AttackPhaseTime = 5;
    public float PlayerHp = 20;
    public bool IsPlayerDead = false;
    public bool IsControlOvered = true;
    public bool IsEvent1Overed = true;
    public bool IsMoveOvered = false;
    public bool IsBackAlive = false;
    public GameObject EndMenu;
    public GameObject ControlMenu;
    public GameObject MoveMenu;
    public GameObject EventsMenu;
    public GameObject AttackMenu;
    private TurnbaseUI turnbaseUI;

    private void Awake()
    {
        if(Instance!=null&&Instance!=this)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
        }
    }

    #region Timer
    public float timerValue ;

    public TextMeshProUGUI Timer;

    #endregion

    public TextMeshProUGUI Rounds;

    // Start is called before the first frame update
    void Start()
    {
        turnbaseUI = FindObjectOfType<TurnbaseUI>();

        timerValue = ControlPhaseTime;
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
       
        if(timerValue>0)
        {
            timerValue -= Time.deltaTime;
        }
        else
        {
            timerValue = 0;
        }
        DisplayTimer(timerValue);

        Rounds.text = RoundNumber.ToString();

        turnbaseUI.UpdateStageInfo(CurrentGameStage, timerValue, RoundNumber);
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
        Debug.Log("ControlPhase");
        CurrentGameStage = GameStage.S1;
        GameplayManager.Instance.StartControlStage();
        timerValue = ControlPhaseTime;
        EventsMenu.SetActive(false);
        ControlMenu.SetActive(true);

       
    }


    void MovePhase()
    {        
        Debug.Log("MovePhase");
        CurrentGameStage = GameStage.MoveStage;
        GameplayManager.Instance.StartMoveStage();

        EventsMenu.SetActive(false);
        MoveMenu.SetActive(true);

       
    }

    void AttackPhase()
    {
        Debug.Log("AttackPhase");
        CurrentGameStage = GameStage.AttackStage;
        GameplayManager.Instance.StartAttackStage();

        EventsMenu.SetActive(false);
        AttackMenu.SetActive(true);

       
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
        yield return new WaitUntil(()=>CurrentGameStage==GameStage.S2+1);
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

        Timer.text = Mathf.FloorToInt(timerValue) .ToString();
    }

    #endregion

    public void TurnOver()
    {
        StopCoroutine("TurnStart");
        StartCoroutine("Event2");
    }

    public void TurnToNextStage()
    {
        CurrentGameStage += 1;
    }
}
