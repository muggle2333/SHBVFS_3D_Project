using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    public event EventHandler OnStartSpecificTutorial;

    private int completeIndex;
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

    public void StartTutorial()
    {
        Debug.LogError("Start Tutorial");
        StartCoroutine("Tutorial");
    }

    IEnumerator Tutorial()
    {
        tutorialUI.ShowMessageText("This is You.");
        yield return new WaitForSecondsRealtime(1f);

        tutorialUI.ShowMessageText("This is your RIVAL.");
        cameraComponent.FocusEnemy();
        yield return new WaitForSecondsRealtime(1f);
        
        tutorialUI.ShowMessageText("Your OBJECTIVE is to defeat him.");
        cameraComponent.FocusPosition((GameplayManager.Instance.currentPlayer.transform.position + GameplayManager.Instance.playerList[1].transform.position) / 2,-15f);
        yield return new WaitForSecondsRealtime(1f);

        tutorialUI.ShowMessageText("Press WASD to move the camera");
        OnStartSpecificTutorial?.Invoke(this,EventArgs.Empty);
        yield return new WaitUntil(() => completeIndex==1);

        tutorialUI.ShowMessageText("Press F to focus on yourself / the gird selected");
        OnStartSpecificTutorial?.Invoke(this, EventArgs.Empty);
        yield return new WaitUntil(() => completeIndex == 2);


        tutorialUI.ShowMessageText("Let's start the tutorial");
        OnStartSpecificTutorial?.Invoke(this, EventArgs.Empty);
        yield return new WaitUntil(() => completeIndex == 3);

        TurnbasedSystem.Instance.StartTurnbaseSystem();
        Time.timeScale = 0;
        yield return null;
    }

    public void CompleteSpecificTutorial()
    {
        completeIndex++;
    }
}
