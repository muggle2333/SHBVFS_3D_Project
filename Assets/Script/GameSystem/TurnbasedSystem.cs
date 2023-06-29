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
    public GameStage  CompleteGameStage;
    public int roundIndex = 0;

    [SerializeField]private float S1PhaseTime = 30;
    [SerializeField]private float DiscardPhaseTime = 10;
    [SerializeField]private float S2PhaseTime = 10;
    [SerializeField]private float MovePhaseTime = 10;
    [SerializeField]private float S3PhaseTime = 10;
    [SerializeField]private float AttackPhaseTime = 10;
    [SerializeField]private float S4PhaseTime = 10;

    public bool IsBackAlive = false;
    public GameObject EndMenu;

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
        StartCoroutine("TurnStart");

    }

    // Update is called once per frame
    void Update()
    {
        if(timerValue>0)
        {
            timerValue -= Time.deltaTime;
        }
        else
        {
            timerValue = 0;
        }
        turnbaseUI.UpdateStageInfo(CurrentGameStage, timerValue, roundIndex);

    }
    #region One turn
    IEnumerator TurnStart()
    {
        roundIndex++;
        ControlPhase();
        UpdateTimer(S1PhaseTime);
        yield return new WaitForSecondsRealtime(S1PhaseTime);

        DiscardPhase();
        UpdateTimer(DiscardPhaseTime);
        yield return new WaitForSecondsRealtime(DiscardPhaseTime);


        StartCoroutine("Event2");

    }
    #endregion

    #region Single phases
    //S1
    void ControlPhase()
    {
        Debug.Log("ControlPhase");
        CurrentGameStage = GameStage.S1;
        CompleteGameStage = GameStage.S4;
        GameplayManager.Instance.StartControlStage();

        UpdateTimer(S1PhaseTime);
    
    }

    void DiscardPhase()
    {
        CurrentGameStage = GameStage.DiscardStage;
        CompleteGameStage = CurrentGameStage - 1;
        GameplayManager.Instance.StartDiscardStage();
    }

    IEnumerator Event2()
    {
        CurrentGameStage = GameStage.S2;
        CompleteGameStage = CurrentGameStage - 1;
        UpdateTimer(MovePhaseTime);
        yield return new WaitForSecondsRealtime(S2PhaseTime);

        MovePhase();
        yield return new WaitUntil(()=>CurrentGameStage==CompleteGameStage);
        UpdateTimer(MovePhaseTime);
        yield return new WaitForSecondsRealtime(MovePhaseTime);

        Event3();
        UpdateTimer(S3PhaseTime);
        yield return new WaitForSecondsRealtime(S3PhaseTime);

        AttackPhase();
        UpdateTimer(AttackPhaseTime);
        yield return new WaitForSecondsRealtime(AttackPhaseTime);

        Event4();
        UpdateTimer(S4PhaseTime);
        yield return new WaitForSecondsRealtime(S4PhaseTime);

        StartCoroutine("TurnStart");
    }

    void MovePhase()
    {        
        Debug.Log("MovePhase");
        CurrentGameStage = GameStage.MoveStage;
        CompleteGameStage = CurrentGameStage - 1;
        GameplayManager.Instance.StartMoveStage();
      
    }

    void AttackPhase()
    {
        Debug.Log("AttackPhase");
        CurrentGameStage = GameStage.AttackStage;
        CompleteGameStage = CurrentGameStage - 1;
        GameplayManager.Instance.StartAttackStage();

    }


   

    void Event3()
    {
        CurrentGameStage = GameStage.S3;
        CompleteGameStage = CurrentGameStage - 1;
        GameplayManager.Instance.StartS3Stage();
        Debug.Log("Event3");
       
    }

    void Event4()
    {
        CurrentGameStage = GameStage.S4;
        CompleteGameStage = CurrentGameStage - 1;
        GameplayManager.Instance.StartS4Stage();
        Debug.Log("Event4");
        
        
    }
    #endregion

    public void TurnOver()
    {
        StopCoroutine("TurnStart");
        StartCoroutine("Event2");
    }

    public void UpdateTimer(float timer)
    {
        timerValue = timer;
    }
    public void TurnToNextStage()
    {
        CompleteGameStage += 1;
    }

    public void Pause()
    {
        Time.timeScale = 0;
    }

    public void Continue()
    {
        Time.timeScale = 1;
    }
}
