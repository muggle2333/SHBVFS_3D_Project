using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField]private Player enemy;

    private void Awake()
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            string playerPath = clientId == 0 ? "PlayerPrefab_Red" : "PlayerPrefab_Blue";
            Transform playerTransform = Instantiate(Resources.Load<GameObject>(playerPath).transform);
            playerTransform.GetComponent<Player>().Id = (PlayerId)clientId;
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }

        enemy.GetComponent<Player>().Id = (PlayerId)1;

    }
}
