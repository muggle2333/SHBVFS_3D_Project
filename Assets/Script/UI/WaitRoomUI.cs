using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class WaitRoomUI : MonoBehaviour
{
    [SerializeField] private Button readyBtn;
    [SerializeField] private GameObject hostPanel;
    [SerializeField] private TMP_Text ipText;

    public void Awake()
    {
        readyBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.SetPlayerReadyServerRpc();
        });
    }
    public void Start()
    {
        CheckPlayerIdentify();
    }
    private void CheckPlayerIdentify()
    {
        if(NetworkManager.Singleton.IsHost)
        {
            ShowHostPanel(true);
        }
        else
        {

            ShowHostPanel(false);
        }

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
}
