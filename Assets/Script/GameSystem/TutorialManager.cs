using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }


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
        tutorialUI.ShowMessageText("This is your RIVAL, your OBJECTIVE is to beat him.");
        cameraComponent.FocusEnemy();
        yield return null;
    }
}
