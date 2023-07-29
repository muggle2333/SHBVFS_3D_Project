using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class WaitRoomUI : MonoBehaviour
{
    [SerializeField] private Button readyBtn;
    [SerializeField] private Button startBtn;
    [SerializeField] private GameObject hostPanel;
    [SerializeField] private TMP_Text ipText;
    [SerializeField] private Toggle tutorialToggle;
    [SerializeField] private TMP_Dropdown levelDropdown;
    public void Awake()
    {
        readyBtn.onClick.AddListener(() =>
        {
            WaitRoomManager.Instance.SetPlayerReady();
        });
        startBtn.onClick.AddListener(() =>
        {
            WaitRoomManager.Instance.SetPlayerReady();
            WaitRoomManager.Instance.StartGameplay();
        });
        tutorialToggle.onValueChanged.AddListener(delegate
        {
            if(tutorialToggle.isOn)
            {
                WaitRoomManager.Instance.SetToggleTutorial(true);
            }
            else
            {
                WaitRoomManager.Instance.SetToggleTutorial(false);
            }
        });
        levelDropdown.onValueChanged.AddListener(delegate
        {
            WaitRoomManager.Instance.SetLevelIndexClientRpc(levelDropdown.value);
        });

    }
    public void Start()
    {
        startBtn.interactable = false;
        levelDropdown.value = WaitRoomManager.Instance.levelIndex.Value;
        CheckPlayerIdentify(NetworkManager.Singleton.IsHost);
        if(!NetworkManager.Singleton.IsHost)
        {
            tutorialToggle.interactable = false;
            levelDropdown.interactable = false;
        }
        

    }
    private void CheckPlayerIdentify(bool isHost)
    {
        startBtn.gameObject.SetActive(isHost);
        readyBtn.gameObject.SetActive(!isHost);
        ShowHostPanel(isHost);


    }
    public void ShowHostPanel(bool isShow)
    {
        ipText.text = GetLocalIP();
        hostPanel.SetActive(isShow);
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

    public void SetToggle(bool isTrue)
    {
        tutorialToggle.isOn = isTrue;
    }
    public void SetStartBtn(bool isStart)
    {
        startBtn.interactable= isStart;
    }

    public void SetLevelDropdown(int levelIndex)
    {
        levelDropdown.value = levelIndex;
    }

}
