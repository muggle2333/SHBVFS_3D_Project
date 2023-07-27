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
}
public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    public event EventHandler OnStartSpecificTutorial;

    private int completeIndex;
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
    }
    public void StartTutorial()
    {
        Debug.LogError("Start Tutorial");
        StartCoroutine("Tutorial");
    }

    IEnumerator Tutorial()
    {
        tutorialUI.ShowMessageText("This is You.");
        cameraComponent.LockCamera(true);
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


        tutorialUI.ShowMessageText("Let's start the tutorial.");
        OnStartSpecificTutorial?.Invoke(this, EventArgs.Empty);
        yield return new WaitUntil(() => completeIndex == 3);

        TurnbasedSystem.Instance.StartTurnbaseSystem();
        Time.timeScale = 0.01f;
        cameraComponent.LockCamera(true);
        tutorialUI.ShowMessageText("You should decide the action in Control Phase in limited time.");
        OnStartSpecificTutorial?.Invoke(this, EventArgs.Empty);
        yield return new WaitUntil(() => completeIndex == 4);

        Time.timeScale = 1f;
        TurnbasedSystem.Instance.SetPlayerSettingClientRpc();
        FindObjectOfType<CardSelectComponent>().isLocked = true;
        tutorialUI.ShowMessageText("You own the land you steps on.");
        yield return new WaitForSecondsRealtime(1f);

        tutorialUI.ShowMessageText("You will draw 1 card after you occupy the land.");
        yield return new WaitForSecondsRealtime(1f);

        Time.timeScale = 0.01f;
        tutorialUI.ShowMessageText("Click on the glowing lands.");
        yield return new WaitUntil(() => action == TutorialAction.ClickGrid);

        tutorialUI.ShowMessageText("Click MOVE button.");
        yield return new WaitUntil(() => action == TutorialAction.ClickMove);

        tutorialUI.ShowMessageText("The academy of the land you step on will be known.");
        yield return new WaitForSecondsRealtime(1f);

        tutorialUI.ShowMessageText("Click OCCUPY button.");
        yield return new WaitUntil(() => action == TutorialAction.ClickOccupy);

        tutorialUI.ShowMessageText("After OCCUPY, you can build & draw card.");
        yield return new WaitForSecondsRealtime(1f);

        tutorialUI.ShowMessageText("Click DRAW button.");
        yield return new WaitUntil(() => action == TutorialAction.ClickDraw);

        tutorialUI.ShowMessageText("After DRAW, you can draw one academy card.");
        yield return new WaitForSecondsRealtime(1f);

        tutorialUI.ShowMessageText("Click BUILD button.");
        yield return new WaitUntil(() => action == TutorialAction.ClickBuild);

        tutorialUI.ShowMessageText("After BUILD, you will draw one more card when drawing.");
        yield return new WaitForSecondsRealtime(1f);

        tutorialUI.ShowMessageText("Click SEARCH button.");
        yield return new WaitUntil(() => action == TutorialAction.ClickSearch);

        tutorialUI.ShowMessageText("After SEARCH, you will know the academy in your range.");
        yield return new WaitForSecondsRealtime(1f);



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
