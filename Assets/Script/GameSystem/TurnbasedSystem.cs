using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
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
}
public class TurnbasedSystem : NetworkBehaviour
{
    public static TurnbasedSystem Instance { get; private set; }
    public NetworkVariable<GameStage> CurrentGameStage = new NetworkVariable<GameStage>(GameStage.S1);
    public NetworkVariable<GameStage> CompleteGameStage = new NetworkVariable<GameStage>(GameStage.S4);


    [SerializeField] private float S1PhaseTime = 30;
    [SerializeField] private float DiscardPhaseTime = 10;
    [SerializeField] private float S2PhaseTime = 10;
    [SerializeField] private float MovePhaseTime = 10;
    [SerializeField] private float S3PhaseTime = 10;
    [SerializeField] private float AttackPhaseTime = 10;
    [SerializeField] private float S4PhaseTime = 10;

    public bool IsBackAlive = false;
    public GameObject EndMenu;

    private TurnbaseUI turnbaseUI;
    public NetworkVariable<bool> isStart = new NetworkVariable<bool>(false);
    public NetworkVariable<bool> isDie = new NetworkVariable<bool>(false);
    public NetworkVariable<int> roundIndex = new NetworkVariable<int>(0);

    private Dictionary<ulong, bool> playerSkipDict = new Dictionary<ulong, bool>() { { 0, false }, { 1, false } };
    private bool isPlayerAllSkip = false;
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
        playerSkipDict = new Dictionary<ulong, bool>() { { 0, false }, { 1, false } };
        turnbaseUI = FindObjectOfType<TurnbaseUI>();
        //For test
        if (FindObjectOfType<NetworkManager>() != null) return;
        isStart.Value = true;
        UIManager.Instance.StartTurnbaseUIClientRpc();
        StartCoroutine("TurnStart");
    }
    public void StartTurnbaseSystem()
    {
        if (!NetworkManager.Singleton.IsHost) return;
        isStart.Value = true;
        UIManager.Instance.StartTurnbaseUIClientRpc();
        SetPlayerSettingClientRpc();
        StartCoroutine("TurnStart");
    }

    [ClientRpc]
    public void SetPlayerSettingClientRpc()
    {
        foreach(var player in GameplayManager.Instance.playerList)
        {
            PlayerManager.Instance.SetPlayerSetting(player);
        }
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

    IEnumerator TurnStart()
    {
        roundIndex.Value++;
        //SoundManager.Instance.PlaySoundClientRpc(Sound.GameStart);
        ControlPhase();
        UpdateTimer(S1PhaseTime);
        //yield return new WaitForSecondsRealtime(S1PhaseTime);
        //加设跳过回合，不能用waitforsecondsRealtime
        //var seq = DOTween.Sequence();
        //seq.AppendInterval(S1PhaseTime-5);
        //seq.AppendCallback(() => { SoundManager.Instance.PlayCountDownClientRpc(5); });
        while (timerValue.Value > 0)
        {
            if(timerValue.Value < 5.1f&& timerValue.Value>=5f)
            {
                SoundManager.Instance.PlayCountDownClientRpc(5);
            }
            if (isPlayerAllSkip)
            {
                break;
            }
            else
            {
                yield return null;
            }
        }
        //SoundManager.Instance.PlaySoundClientRpc(Sound.CountDown);

        DiscardPhase();
        yield return new WaitUntil(() => CurrentGameStage.Value == CompleteGameStage.Value);
        //UpdateTimer(DiscardPhaseTime);
        //yield return new WaitForSecondsRealtime(DiscardPhaseTime);


        Event2();
        yield return new WaitUntil(() => CurrentGameStage.Value == CompleteGameStage.Value && !isDie.Value);
        //UpdateTimer(S2PhaseTime);
        //yield return new WaitForSecondsRealtime(S2PhaseTime);


        MovePhase();
        yield return new WaitUntil(() => CurrentGameStage.Value == CompleteGameStage.Value && !isDie.Value);
        //UpdateTimer(MovePhaseTime);
        //yield return new WaitForSecondsRealtime(MovePhaseTime);

        Event3();
        yield return new WaitUntil(() => CurrentGameStage.Value == CompleteGameStage.Value && !isDie.Value);
        //UpdateTimer(S3PhaseTime);
        //yield return new WaitForSecondsRealtime(S3PhaseTime);

        AttackPhase();
        yield return new WaitUntil(() => CurrentGameStage.Value == CompleteGameStage.Value && !isDie.Value);
        //UpdateTimer(AttackPhaseTime);
        //yield return new WaitForSecondsRealtime(AttackPhaseTime);

        Event4();
        yield return new WaitUntil(() => CurrentGameStage.Value == CompleteGameStage.Value && !isDie.Value);
        //UpdateTimer(S4PhaseTime);
        //yield return new WaitForSecondsRealtime(S4PhaseTime);

        StartCoroutine("TurnStart");

    }

    //S1
    void ControlPhase()
    {
        
        RefreshPlayerSkipDict();

        //Debug.Log("ControlPhase");
        CurrentGameStage.Value = GameStage.S1;
        CompleteGameStage.Value = GameStage.S4;
        GameplayManager.Instance.StartControlStage();
        SoundManager.Instance.PlaySoundClientRpc(Sound.GameStart);
        PlayerManager.Instance.cardSelectManager.UpdateCardPos(GameplayManager.Instance.currentPlayer);
        //UpdateTimer(S1PhaseTime);

    }

    void DiscardPhase()
    {
        SoundManager.Instance.StopCountDownClientRpc();
        CurrentGameStage.Value = GameStage.DiscardStage;
        CompleteGameStage.Value = CurrentGameStage.Value - 1;
        GameplayManager.Instance.StartDiscardStage();
    }

    

    void MovePhase()
    {        
        //Debug.Log("MovePhase");
        CurrentGameStage.Value = GameStage.MoveStage;
        CompleteGameStage.Value = CurrentGameStage.Value - 1;
        GameplayManager.Instance.StartMoveStage();
      
    }
    void Event2()
    {
        CurrentGameStage.Value = GameStage.S2;
        CompleteGameStage.Value = CurrentGameStage.Value - 1;
        GameplayManager.Instance.StartS2Stage();
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



    public void UpdateTimer(float timer)
    {
        timerValue.Value = timer;
    }
    public void CompleteStage(GameStage currentGameStage)
    {
        CompleteGameStage.Value = currentGameStage;
    }
    public void Pause()
    {
        Time.timeScale = 0;
    }

    public void Continue()
    {
        Time.timeScale = 1;
    }

    private void RefreshPlayerSkipDict()
    {
        playerSkipDict = new Dictionary<ulong, bool>() { { 0, false }, { 1, false } };
        turnbaseUI.ShowSkipBtnClientRpc(true);
        isPlayerAllSkip = false;
    }
    [ServerRpc(RequireOwnership =false)]
    public void SkipControlStageServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerSkipDict[serverRpcParams.Receive.SenderClientId] = true;
        isPlayerAllSkip = CheckIsAllSkip();
    }

    public bool CheckIsAllSkip()
    {
        bool isAllSkip = true;
        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (playerSkipDict[clientId] == false)
            {
                isAllSkip = false;
                break;
            }
        }
        return isAllSkip;
    }

    
}
