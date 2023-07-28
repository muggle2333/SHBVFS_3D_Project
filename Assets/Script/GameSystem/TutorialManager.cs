using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.PackageManager;
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

    private int completeIndex = -1;
    private TutorialAction action = TutorialAction.ClickBuild;
    
    [SerializeField] private Player enemy;
    [SerializeField] private CameraTest4 cameraComponent;
    [SerializeField] private TutorialUI tutorialUI;

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

    IEnumerator Tutorial()
    {
        tutorialUI.ShowMessageText("Welcome to the ZHUZIBAIJIA");
        cameraComponent.LockCamera(true);
        OnStartSpecificTutorial?.Invoke(this, EventArgs.Empty);
        yield return new WaitUntil(() => completeIndex == 0);

        tutorialUI.ShowMessageText("This is You.");
        yield return new WaitForSecondsRealtime(1f);

        tutorialUI.ShowMessageText("This is your RIVAL.");
        cameraComponent.FocusEnemy();
        yield return new WaitForSecondsRealtime(1f);
        
        tutorialUI.ShowMessageText("Your OBJECTIVE is to defeat him.");
        cameraComponent.FocusPosition((GameplayManager.Instance.currentPlayer.transform.position + GameplayManager.Instance.playerList[1].transform.position) / 2,-15f);
        yield return new WaitForSecondsRealtime(1f);

        tutorialUI.ShowMessageText("Press WASD to move the camera.");
        cameraComponent.LockCamera(false);
        OnStartSpecificTutorial?.Invoke(this,EventArgs.Empty);
        yield return new WaitUntil(() => completeIndex==1);

        tutorialUI.ShowMessageText("Press F to focus on yourself / the gird selected.");
        OnStartSpecificTutorial?.Invoke(this, EventArgs.Empty);
        yield return new WaitUntil(() => completeIndex == 2);

        tutorialUI.ShowMessageText("Left-down corn is your information.");
        OnStartSpecificTutorial?.Invoke(this, EventArgs.Empty);
        yield return new WaitUntil(() => completeIndex == 3);

        tutorialUI.ShowMessageText("Your range is only 1 which can't support you to beat the rival.");
        OnStartSpecificTutorial?.Invoke(this, EventArgs.Empty);
        yield return new WaitUntil(() => completeIndex == 4);

        tutorialUI.ShowMessageText("I will teach you how to level up yourself.");
        OnStartSpecificTutorial?.Invoke(this, EventArgs.Empty);
        yield return new WaitUntil(() => completeIndex == 5);

        tutorialUI.ShowMessageText("Let's start the tutorial.");
        OnStartSpecificTutorial?.Invoke(this, EventArgs.Empty);
        yield return new WaitUntil(() => completeIndex == 6);

        TurnbasedSystem.Instance.StartTurnbaseSystem();
        Time.timeScale = 0.01f;
        cameraComponent.LockCamera(true);
        tutorialUI.ShowMessageText("You should decide the action in Control Phase in limited time.");
        OnStartSpecificTutorial?.Invoke(this, EventArgs.Empty);
        yield return new WaitUntil(() => completeIndex == 7);

        Time.timeScale = 1f;
        TurnbasedSystem.Instance.SetPlayerSettingClientRpc();
        FindObjectOfType<CardSelectComponent>().isLocked = true;
        tutorialUI.ShowMessageText("You own the land you steps on.");
        yield return new WaitForSecondsRealtime(1f);

        tutorialUI.ShowMessageText("You will draw 1 card after you occupy the land.");
        OnStartSpecificTutorial?.Invoke(this, EventArgs.Empty);
        yield return new WaitUntil(() => completeIndex == 8);

        
        SelectManager.Instance.SetSpecificSelection(new Vector2Int(1, 0));
        tutorialUI.ShowMessageText("Click on the glowing lands. You can see the information on it");
        yield return new WaitUntil(() => action == TutorialAction.ClickGrid);

        Time.timeScale = 0.01f;
        tutorialUI.ShowMessageText("Click MOVE button.");
        yield return new WaitUntil(() => action == TutorialAction.ClickMove);

        tutorialUI.ShowMessageText("The academy of the land you step on will be known.");
        OnStartSpecificTutorial?.Invoke(this, EventArgs.Empty);
        yield return new WaitUntil(() => completeIndex == 9);

        SelectManager.Instance.selectGridMode = SelectGridMode.Default;
        SelectManager.Instance.UpdateSelectableGridObject();
        tutorialUI.ShowMessageText("Click on the glowing lands & Click OCCUPY button.");
        yield return new WaitUntil(() => action == TutorialAction.ClickOccupy);

        tutorialUI.ShowMessageText("After OCCUPY, you can build & draw card.");
        OnStartSpecificTutorial?.Invoke(this, EventArgs.Empty);
        yield return new WaitUntil(() => completeIndex == 10);

        tutorialUI.ShowMessageText("Click on the glowing lands & Click DRAW button.");
        yield return new WaitUntil(() => action == TutorialAction.ClickDraw);

        tutorialUI.ShowMessageText("After DRAW, you can draw one academy card.");
        OnStartSpecificTutorial?.Invoke(this, EventArgs.Empty);
        yield return new WaitUntil(() => completeIndex == 11);

        tutorialUI.ShowMessageText("Click on the glowing lands & Click BUILD button.");
        yield return new WaitUntil(() => action == TutorialAction.ClickBuild);

        tutorialUI.ShowMessageText("After BUILD, you will draw one more card when drawing.");
        OnStartSpecificTutorial?.Invoke(this, EventArgs.Empty);
        yield return new WaitUntil(() => completeIndex == 12);

        tutorialUI.ShowMessageText("Click on the glowing lands & Click SEARCH button.");
        yield return new WaitUntil(() => action == TutorialAction.ClickSearch);

        tutorialUI.ShowMessageText("After SEARCH, you will know the academy in your range.");
        OnStartSpecificTutorial?.Invoke(this, EventArgs.Empty);
        yield return new WaitUntil(() => completeIndex == 13);

        Time.timeScale = 1f;
        FindObjectOfType<CardSelectComponent>().isLocked = false;
        tutorialUI.ShowMessageText("You can Double Click the card to play it.");
        OnStartSpecificTutorial?.Invoke(this, EventArgs.Empty);
        yield return new WaitUntil(() => completeIndex == 14);

        tutorialUI.ShowMessageText("Now your Action Point is all used.");
        OnStartSpecificTutorial?.Invoke(this, EventArgs.Empty);
        yield return new WaitUntil(() => completeIndex == 15);

        tutorialUI.ShowMessageText("Press SKIP button to skip and turn to the Move Phase.");
        yield return new WaitUntil(() => action == TutorialAction.ClickSkip || TurnbasedSystem.Instance.CurrentGameStage.Value != GameStage.S1);

        tutorialUI.ShowMessageText("Character will action following your decision one by one.");
        OnStartSpecificTutorial?.Invoke(this, EventArgs.Empty);
        yield return new WaitUntil(() => completeIndex == 16);
        //yield return new WaitUntil(() => TurnbasedSystem.Instance.CurrentGameStage.Value == GameStage.AttackStage);

        tutorialUI.ShowMessageText("After the Move Phase, It will enter Attack Phase.");
        OnStartSpecificTutorial?.Invoke(this, EventArgs.Empty);
        yield return new WaitUntil(() => completeIndex == 17);

        tutorialUI.ShowMessageText("Player will Attack the rival if range is enough.");
        OnStartSpecificTutorial?.Invoke(this, EventArgs.Empty);
        yield return new WaitUntil(() => completeIndex == 18);

        Time.timeScale = 0.01f;
        tutorialUI.ShowMessageText("Now it's free to practice.");
        OnStartSpecificTutorial?.Invoke(this, EventArgs.Empty);
        yield return new WaitUntil(() => completeIndex == 19);
        Time.timeScale = 1f;
        tutorialUI.HideTutorial();
        cameraComponent.LockCamera(false);
        yield return null;
    }

    public void CompleteSpecificTutorial()
    {
        completeIndex++;
    }

    public void CompleteTutorialAction(TutorialAction action)
    {
        this.action = action;
    }

}
