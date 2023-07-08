using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static GameManager;

public enum GameStage
{
    S1, //Control Stage
    DiscardStage,
    S2,
    MoveStage,
    S3,
    AttackStage,
    S4,
    DyingStage,
}
public class TurnbasedSystem : NetworkBehaviour
{ 
    public static TurnbasedSystem Instance { get; private set; }
    public NetworkVariable<GameStage>  CurrentGameStage = new NetworkVariable<GameStage>(GameStage.S1);
    public NetworkVariable<GameStage> CompleteGameStage = new NetworkVariable<GameStage>(GameStage.S1);


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
    public NetworkVariable<bool> isStart = new NetworkVariable<bool>(false);
    public NetworkVariable<int> roundIndex = new NetworkVariable<int>(0);
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
    public NetworkVariable<float> timerValue = new NetworkVariable<float>(0f);

    public TextMeshProUGUI Timer;

    #endregion

    public TextMeshProUGUI Rounds;

    // Start is called before the first frame update
    void Start()
    {
        turnbaseUI = FindObjectOfType<TurnbaseUI>();
        //For test
        if (FindObjectOfType<NetworkManager>() != null) return;
        isStart.Value = true;
        StartCoroutine("TurnStart");
    }
    public void StartTurnbaseSystem()
    {
        if (!NetworkManager.Singleton.IsHost) return;
        isStart.Value = true;
        StartCoroutine("TurnStart");
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!isStart.Value) return;

        turnbaseUI.UpdateStageInfo(CurrentGameStage.Value, timerValue.Value, roundIndex.Value);

        if (FindObjectOfType<NetworkManager>()!=null && !NetworkManager.Singleton.IsHost) return;
       
        if(timerValue.Value>0)
        {
            timerValue.Value -= Time.deltaTime;
        }
        else
        {
            timerValue.Value = 0;
        }
    }
    #region One turn
    IEnumerator TurnStart()
    {
        roundIndex.Value++;
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
        //Debug.Log("ControlPhase");
        CurrentGameStage.Value = GameStage.S1;
        CompleteGameStage.Value = GameStage.S4;
        GameplayManager.Instance.StartControlStage();

        UpdateTimer(S1PhaseTime);
    
    }

    void DiscardPhase()
    {
        CurrentGameStage.Value = GameStage.DiscardStage;
        CompleteGameStage.Value = CurrentGameStage.Value - 1;
        GameplayManager.Instance.StartDiscardStage();
    }

    IEnumerator Event2()
    {
        CurrentGameStage.Value = GameStage.S2;
        CompleteGameStage.Value = CurrentGameStage.Value - 1;
        GameplayManager.Instance.StartS2Stage();
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
        //Debug.Log("MovePhase");
        CurrentGameStage.Value = GameStage.MoveStage;
        CompleteGameStage.Value = CurrentGameStage.Value - 1;
        GameplayManager.Instance.StartMoveStage();
      
    }

    void AttackPhase()
    {
        //Debug.Log("AttackPhase");
        CurrentGameStage.Value = GameStage.AttackStage;
        CompleteGameStage.Value = CurrentGameStage.Value - 1;
        GameplayManager.Instance.StartAttackStage();

    }
   

    void Event3()
    {
        CurrentGameStage.Value = GameStage.S3;
        CompleteGameStage.Value = CurrentGameStage.Value - 1;
        GameplayManager.Instance.StartS3Stage();
        //Debug.Log("Event3");
       
    }

    void Event4()
    {
        CurrentGameStage.Value = GameStage.S4;
        CompleteGameStage.Value = CurrentGameStage.Value - 1;
        GameplayManager.Instance.StartS4Stage();
        //Debug.Log("Event4");
        
        
    }
    #endregion

    public void TurnOver()
    {
        StopCoroutine("TurnStart");
        StartCoroutine("Event2");
    }

    public void UpdateTimer(float timer)
    {
        timerValue.Value = timer;
    }
    public void TurnToNextStage()
    {
        CompleteGameStage.Value += 1;
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
