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
    [SerializeField] private GameObject hostPanel;
    [SerializeField] private GameObject clientPanel;
    [SerializeField] private TMP_Text ipText;
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

    public void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            ExitPanel();
        }
    }
    private void CreateGame()
    {
        currentPanel = hostPanel;
        currentPanel.SetActive(true);
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
}
