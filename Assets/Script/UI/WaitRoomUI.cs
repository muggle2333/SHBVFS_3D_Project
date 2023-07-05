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

    public void Awake()
    {
        readyBtn.onClick.AddListener(() =>
        {
            WaitRoomManager.Instance.SetPlayerReady();
        });
        startBtn.onClick.AddListener(() =>
        {
            WaitRoomManager.Instance.SetPlayerReady();
        });
    }
    public void Start()
    {
        CheckPlayerIdentify(NetworkManager.Singleton.IsHost);

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

    public void SetStartBtn(bool isStart)
    {
        startBtn.interactable= isStart;
    }
}
