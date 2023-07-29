using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MainMenuCleanUp : MonoBehaviour
{
    public static MainMenuCleanUp Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        //if(NetworkManager.Singleton!= null)
        //{
        //    Destroy(NetworkManager.Singleton.gameObject);
        //}
        //if (MultiplayerManager.Instance != null)
        //{
        //    Destroy(MultiplayerManager.Instance.gameObject);
        //}
    }
    public void CleanUp()
    {
        if (NetworkManager.Singleton != null)
        {
            Destroy(NetworkManager.Singleton.gameObject);
        }
        if (MultiplayerManager.Instance != null)
        {
            Destroy(MultiplayerManager.Instance.gameObject);
        }
        if(FindObjectOfType<NotificationUI>().gameObject!=null)
        {
            Destroy(FindObjectOfType<NotificationUI>().gameObject);
        }
        if(FindObjectOfType<SoundManager>().gameObject!=null)
        {
            Destroy(FindObjectOfType<SoundManager>().gameObject);
        }
    }
}
