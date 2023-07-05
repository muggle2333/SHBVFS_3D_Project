using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button createBtn;
    [SerializeField] private Button joinBtn;
    [SerializeField] private Button quitBtn;
    [SerializeField] private Button tryEnterBtn;

    [SerializeField] private GameObject clientPanel;
    [SerializeField] private GameObject connectingPanel;

    [SerializeField] private TMP_InputField ipInputText;



    private GameObject currentPanel;

    public void Awake()
    {
        createBtn.onClick.AddListener(() =>
        {
            CreateGame();
        });
        joinBtn.onClick.AddListener(() =>
        {
            JoinGame();
        });
        tryEnterBtn.onClick.AddListener(() =>
        {
            TryEnterGame();
        });
        quitBtn.onClick.AddListener(() =>
        {
            QuitGame();
        });
    }
    public void Start()
    {
        MultiplayerManager.Instance.OnTryingToJoinGame += MultiplayerManager_OnTryingToJoinGame;
        MultiplayerManager.Instance.OnFailToJoinGame += MultiplayerManager_OnFailedToJoinGame;
    }
    public void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            ExitPanel();
        }
    }
    private void OnDestroy()
    {
        MultiplayerManager.Instance.OnTryingToJoinGame -= MultiplayerManager_OnTryingToJoinGame;
        MultiplayerManager.Instance.OnFailToJoinGame -= MultiplayerManager_OnFailedToJoinGame;
    }
    private void CreateGame()
    {
        MultiplayerManager.Instance.StartHost();
    }

    private void JoinGame()
    {
        currentPanel = clientPanel;
        currentPanel.SetActive(true);

    }

    private void TryEnterGame()
    {
        if(ipInputText.text!=null)
        {
            MultiplayerManager.Instance.StartClient(ipInputText.text);
        }
        else
        {
            Debug.Log("Print string");
        }
    }

    private void QuitGame()
    {
        Application.Quit();
    }

    private void ExitPanel()
    {
        currentPanel.SetActive(false);
    }

    private string GetLocalIP()
    {
        try
        {
            IPHostEntry ipHostEntry = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress item in ipHostEntry.AddressList)
            {
                if (item.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return item.ToString();
                }
            }
            return "";
        }
        catch
        {
            return "";
        }
    }

    private void MultiplayerManager_OnTryingToJoinGame(object sender, EventArgs e)
    {
        connectingPanel.SetActive(true);
    }
    private void MultiplayerManager_OnFailedToJoinGame(object sender, EventArgs e)
    {
        connectingPanel.SetActive(false);
    }
}
