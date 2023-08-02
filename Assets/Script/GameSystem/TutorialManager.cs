using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.Netcode;
using UnityEngine;

public enum TutorialAction
{
    ClickGrid,
    ClickMove,
    ClickOccupy,
    ClickDraw,
    ClickBuild,
    ClickSearch,
    ClickPlayCard,
    ClickSkip,
}
public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    public event EventHandler OnStartSpecificTutorial;

    private int completeIndex = 6;   //-4
    private TutorialAction action = TutorialAction.ClickSkip;
    private bool isActionCompleted=false;

    [SerializeField] private Player enemy;
    [SerializeField] private CameraTest4 cameraComponent;
    [SerializeField] private TutorialUI tutorialUI;

    private float buildTimes=0;
    private float searchTimes=0;
    private float drawTimes = 0;
    private bool firstPartTutorialOver = false;
    private bool isPlayCardCorotine=false;
    private bool isCardPlayed = false;
    public bool waterTrigger;
    private int enterWaterTimes=0;
    private bool IsGreat;
    private int test=0;
    private void Awake()
    {
        Instance= this;

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            string playerPath = clientId == 0 ? "PlayerPrefab_Red" : "PlayerPrefab_Blue";
            Transform playerTransform = Instantiate(Resources.Load<GameObject>(playerPath).transform);
            playerTransform.GetComponent<Player>().Id = (PlayerId)clientId;
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }
        enemy.GetComponent<Player>().Id = (PlayerId)1;

      
    }

    public void Start()
    {
        
        tutorialUI.AcademyBuffDisappear();
        Player player = GameplayManager.Instance.currentPlayer;
        player.CurrentActionPoint = player.MaxActionPoint;
        player.TrueActionPoint = player.MaxActionPoint;

        SelectManager.Instance.selectGridMode = SelectGridMode.None;
        SelectManager.Instance.UpdateSelectableGridObject();
    }
    public void StartTutorial()
    {
   
        Debug.LogError("Start Tutorial");
        FindObjectOfType<PlayerDeck>().InitializePlayerDeck();
        Player player = GameplayManager.Instance.currentPlayer;
        player.CurrentActionPoint = player.MaxActionPoint;
        player.TrueActionPoint = player.MaxActionPoint;
        StartCoroutine("Tutorial");
        
    }

    void Update()
    {

        if (completeIndex < 14)
        {
            TurnbasedSystem.Instance.timerValue.Value = 60f;
        }

        switch (completeIndex)
        {
            case -2: 
            CameraMoveJudge();
            break;

            case -1:
            CameraFocusJudge();
            break;

            case 10:
            CardPlayedJudge();
            break;
            
        }

        if(buildTimes < 1)
        {
            BuildJudge();
            
            //ActionPointCheck();
        }

        if(searchTimes < 1)
        {
            SearchJudge();
        }
       
        if(drawTimes<1)
        {
            DrawCardJudge();
        }
        
        if(waterTrigger&&(enterWaterTimes<1))
        {
            StartCoroutine("WaterLand");
            enterWaterTimes++;
        }
       

    }
    IEnumerator Tutorial()
    {
        //isActionCompleted = true;
        ////SelectManager.Instance.selectGridMode = SelectGridMode.None;
        //tutorialUI.ShowMessageText("Welcome! General");
        //cameraComponent.LockCamera(true);
        //OnStartSpecificTutorial?.Invoke(this, EventArgs.Empty);
        //yield return new WaitUntil(() => completeIndex == -3);


        //tutorialUI.ShowMessageText("This is your RIVAL");
        //cameraComponent.FocusEnemy();
        //yield return new WaitForSecondsRealtime(1f);


        //isActionCompleted = true;                                 // No judge button, have to reset
        //tutorialUI.ShowMessageText("Your OBJECTIVE is to defeat him");
        //cameraComponent.FocusPosition((GameplayManager.Instance.currentPlayer.transform.position + GameplayManager.Instance.playerList[1].transform.position) / 2,-15f);
        //OnStartSpecificTutorial?.Invoke(this, EventArgs.Empty);
        //yield return new WaitUntil(() => completeIndex == -2);


        ////tutorialUI.ShowGoal();
        //tutorialUI.ShowMessageText("Press WASD to move the camera  \n" +
        //    "Scroll MOUSE WEHEL to zoom in and out  \n" +
        //    "Press MOUSE WHEEL to rotate");
        //cameraComponent.LockCamera(false);
        //OnStartSpecificTutorial?.Invoke(this,EventArgs.Empty);
        ////Debug.Log(isActionCompleted);
        ////Debug.Log(completeIndex);
        //yield return new WaitUntil(() => completeIndex==-1);


        //tutorialUI.ShowMessageText("Press F to focus on yourself / the grid selected");
        //OnStartSpecificTutorial?.Invoke(this, EventArgs.Empty);
        //yield return new WaitUntil(() => completeIndex == 0);


        //isActionCompleted = true;                                 // No judge button, have to reset
        //tutorialUI.ShowMessageText("LeftFrame-down corn is your information");
        //tutorialUI.ShowPlayerData(true);
        //tutorialUI.ShowFrame();
        //OnStartSpecificTutorial?.Invoke(this, EventArgs.Empty);
        //yield return new WaitUntil(() => completeIndex == 1);


        //isActionCompleted = true;                                 // No judge button, have to reset
        //tutorialUI.ShowIcons(0);
        //tutorialUI.ShowMessageText("      is your action point, which you spend in every turn");
        //OnStartSpecificTutorial?.Invoke(this, EventArgs.Empty);
        //yield return new WaitUntil(() => completeIndex == 2);



        //isActionCompleted = true;                                 // No judge button, have to reset
        //tutorialUI.IconsDiappear(0);
        //tutorialUI.ShowIcons(1);
        //tutorialUI.ShowMessageText("       is your health, you will die if it comes to 0");
        //OnStartSpecificTutorial?.Invoke(this, EventArgs.Empty);
        //yield return new WaitUntil(() => completeIndex == 3);



        //isActionCompleted = true;                                 // No judge button, have to reset
        //tutorialUI.IconsDiappear(1);
        //tutorialUI.ShowIcons(2);
        //tutorialUI.ShowMessageText( "        is your attack damage");
        //OnStartSpecificTutorial?.Invoke(this, EventArgs.Empty);
        //yield return new WaitUntil(() => completeIndex == 4);



        //isActionCompleted = true;                                 // No judge button, have to reset
        //tutorialUI.IconsDiappear(2);
        //tutorialUI.ShowIcons(3);
        //tutorialUI.ShowMessageText( "       is your defense, which can resist some damage for you");
        //OnStartSpecificTutorial?.Invoke(this, EventArgs.Empty);
        //yield return new WaitUntil(() => completeIndex == 5);



        //isActionCompleted = true;                                 // No judge button, have to reset
        //tutorialUI.IconsDiappear(3);
        //tutorialUI.ShowIcons(4);
        //tutorialUI.ShowMessageText("        is your range");
        //OnStartSpecificTutorial?.Invoke(this, EventArgs.Empty);
        //yield return new WaitUntil(() => completeIndex == 6);


        tutorialUI.IconsDiappear(4);
        TurnbasedSystem.Instance.StartTurnbaseSystem();
        Debug.Log(test);
        Debug.Log(completeIndex);
        //Time.timeScale = 0.01f;
        isActionCompleted = true;                                 // No judge button, have to reset
        cameraComponent.LockCamera(true);
        SelectManager.Instance.selectGridMode = SelectGridMode.None;
        SelectManager.Instance.UpdateSelectableGridObject();
        tutorialUI.ShowMessageText("Now,the turn begins");
        //start SFX
        OnStartSpecificTutorial?.Invoke(this, EventArgs.Empty);
        yield return new WaitUntil(() => completeIndex == 7);


        TurnbasedSystem.Instance.StartTurnbaseSystem();
        SelectManager.Instance.selectGridMode = SelectGridMode.None;
        SelectManager.Instance.UpdateSelectableGridObject();
        tutorialUI.SkipDiappear();
        //Time.timeScale = 0.01f;
        isActionCompleted = true;                                 // No judge button, have to reset
        cameraComponent.LockCamera(true);
        tutorialUI.ShowMessageText("In Control Phase, you should give orders in limited time");
        OnStartSpecificTutorial?.Invoke(this, EventArgs.Empty);
        yield return new WaitUntil(() => completeIndex == 8);


        isActionCompleted = true;                                 // No judge button, have to reset
        Time.timeScale = 1f;
        TurnbasedSystem.Instance.SetPlayerSettingClientRpc();
        FindObjectOfType<CardSelectComponent>().isLocked = true;
        tutorialUI.ShowMessageText("You'll automatically occupy the land you steps on without actionpoint, when the game begins");
        OnStartSpecificTutorial?.Invoke(this, EventArgs.Empty);
        yield return new WaitUntil(() => completeIndex == 9);



        isActionCompleted = true;                                 // No judge button, have to reset
        tutorialUI.ShowMessageText("You will get 1 card after you occupy the land");
        OnStartSpecificTutorial?.Invoke(this, EventArgs.Empty);
        yield return new WaitUntil(() => completeIndex == 10);


        
        tutorialUI.ShowMessageText("Now, you can play cards");
        yield return new WaitForSecondsRealtime(1f);


        tutorialUI.ShowMessageText("Left Click the card to choose, Right Click to cancel, Double Click the card to play");
        FindObjectOfType<CardSelectComponent>().isLocked = false;
        OnStartSpecificTutorial?.Invoke(this, EventArgs.Empty);
        yield return new WaitUntil(() => completeIndex == 11);


        isActionCompleted = true;
        tutorialUI.ShowMessageText("Used card will go into top left corner to be record, waiting for taking effect ");
        tutorialUI.ShowTopLeft(true);
        OnStartSpecificTutorial?.Invoke(this, EventArgs.Empty);
        yield return new WaitUntil(() => completeIndex == 12);


        isActionCompleted = true;
        tutorialUI.ShowTopLeft(false);
        tutorialUI.ShowMessageText("The bar on the left will record the history of used card");
        tutorialUI.ShowLeft(true);
        OnStartSpecificTutorial?.Invoke(this, EventArgs.Empty);
        yield return new WaitUntil(() => completeIndex == 13);
      


        isActionCompleted = true;                                 // No judge button, have to reset
        SelectManager.Instance.SetSpecificSelection(new Vector2Int(1, 0));
        //Time.timeScale = 0.01f;
        tutorialUI.ShowMessageText("Now choose the glowing land and click MOVE button,which spend 1 action point");
        //completeIndex++;                        //14
        yield return new WaitUntil(() => action == TutorialAction.ClickMove);
       


        
        isActionCompleted = true;                                 // No judge button, have to reset
        SelectManager.Instance.SetSpecificSelection(new Vector2Int(1, 0));
        tutorialUI.ShowMessageText("Click OCCUPY button");
        //completeIndex++;                        //15
        yield return new WaitUntil(() => action == TutorialAction.ClickOccupy);
       


        isActionCompleted = true;                                 // No judge button, have to reset
        tutorialUI.ShowMessageText("Now, you can give further orders to this grid, try BUILD");
        //completeIndex++;                        //16
        firstPartTutorialOver = true;
        yield return new WaitUntil(() => action == TutorialAction.ClickBuild);
        
    }

 

    IEnumerator WaterLand()
    {
        tutorialUI.ShowMessageText("It will cause 2 action points if you step into water. But you can move without actionpoint in water. And it will only cause 1 action point to step out water.");
        isActionCompleted = true;
        OnStartSpecificTutorial?.Invoke(this, EventArgs.Empty);
        yield return null;
    }
    public void CompleteSpecificTutorial()
    {
        if(isActionCompleted)                                       
        {
            completeIndex++;
            FindObjectOfType<TutorialUI>().nextBtn.gameObject.SetActive(false);
            isActionCompleted = false;
            tutorialUI.HideTutorial();
            
            if (((searchTimes == 1) || (buildTimes == 1) || (drawTimes == 1))&&!IsGreat)
            {
                //tutorialUI.ShowMessageText("Now, you can give further orders to this grid");
                //completeIndex--;
                tutorialUI.ShowMessageText("Great,now you can choose to coninue ordering or skip");
                isActionCompleted=true;
                SelectManager.Instance.selectGridMode = SelectGridMode.Default;
                SelectManager.Instance.UpdateSelectableGridObject();
                OnStartSpecificTutorial?.Invoke(this, EventArgs.Empty);
                tutorialUI.ShowSkip();
                IsGreat = true;
                completeIndex--;
            }
            //else
            //{
            //    IsGreat = true;
            //}
          

        }
        else
        {
            tutorialUI.ShowWarning();
        }

        //if(completeIndex==16)
        //{
        //    tutorialUI.HideTutorial();
        //    firstPartTutorialOver = true;
        //}

        
    }

    public void CameraMoveJudge()                                            
    {
    
        if(Input.GetKeyDown(KeyCode.W)|| Input.GetKeyDown(KeyCode.A)|| Input.GetKeyDown(KeyCode.S)|| Input.GetKeyDown(KeyCode.D))
        {
          
            isActionCompleted = true;
        }
    }
    public void CameraFocusJudge()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            isActionCompleted = true;
        }
    }

    public void DrawCardJudge()
    {
        if((action == TutorialAction.ClickDraw)&&(drawTimes<1))
        {
            tutorialUI.ShowMessageText("A wise choice! Drawing a card can make you have more startegy choices");
            drawTimes++;
            isActionCompleted = true;
            OnStartSpecificTutorial?.Invoke(this, EventArgs.Empty);
        }
    }

    public void BuildJudge()
    {
        
        if ((action == TutorialAction.ClickBuild)&&(buildTimes<1))
        {
            tutorialUI.ShowMessageText("A key decision! Building a city can enable you draw 2 cards on this land.");
            buildTimes++;
            isActionCompleted = true;
            OnStartSpecificTutorial?.Invoke(this, EventArgs.Empty);
        }
    }


    public void SearchJudge()
    {
        if ((action == TutorialAction.ClickSearch)&&(searchTimes<1))
        {
            tutorialUI.ShowMessageText("Careful and meticulous thinking! Searching can detect the academies in the range, which make you have specific future plan");
            searchTimes++;
            isActionCompleted = true;
            OnStartSpecificTutorial?.Invoke(this, EventArgs.Empty);
        }
    }

    public void CardPlayedJudge()
    {
        SelectManager.Instance.selectGridMode = SelectGridMode.None;
        SelectManager.Instance.UpdateSelectableGridObject();

        if (PlayerManager.Instance.redPlayerHandCardsList.Count < 1)
        {
            completeIndex++;
            isActionCompleted = true;
            //Debug.Log(CardManager.Instance.playedCardDict[GameplayManager.Instance.currentPlayer].Count);
           // Debug.Log(isActionCompleted);

        }
    }

    public void ActionPointCheck()
    {
        if ((GameplayManager.Instance.currentPlayer.CurrentActionPoint == 0)&&(isCardPlayed))
        {
            tutorialUI.ShowSkip();
            tutorialUI.ShowMessageText("Now, your action point is ran out. You can press SKIP to skip order phase");
            //tutorialUI.ShowMessageText("You can press SKIP button to skip and turn to the Move Phase");
        }

    }
    public void CompleteTutorialAction(TutorialAction action)
    {
        this.action = action;
    }




}
