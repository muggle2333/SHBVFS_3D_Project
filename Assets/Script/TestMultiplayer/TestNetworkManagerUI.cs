using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode.Transports.UTP;

public class TestNetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button serverBtn;
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;
    [SerializeField] private Button testBtn;
    [SerializeField] private GameObject testImg;
    [SerializeField] private TMP_Text ipText;
    [SerializeField] private UnityTransport selfTransport;
    [SerializeField] private TMP_InputField ipInput;

    private void Awake()
    {
        serverBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
        });
        hostBtn.onClick.AddListener(() =>
        {
            StartHost(GetLocalIP());
        });
        clientBtn.onClick.AddListener(() =>
        {
            ConnectHost(ipInput.text);
        });
        testBtn.onClick.AddListener(() =>
        {
            SetTestImg();
        });
    }

    private void Start()
    {
        ipText.text = GetLocalIP();
    }
    
    private void StartHost(string ip)
    {
        selfTransport.SetConnectionData(ip, 7777);
        NetworkManager.Singleton.StartHost();
    }

    private void ConnectHost(string ip)
    {
        selfTransport.SetConnectionData(ip, 7777);
        NetworkManager.Singleton.StartClient();
    }

    private void SetTestImg()
    {
        testImg.SetActive(!testImg.activeSelf);
    }

    private string GetLocalIP()
    {
        try
        {
            IPHostEntry ipHostEntry = Dns.GetHostEntry(Dns.GetHostName());
            foreach(IPAddress item in ipHostEntry.AddressList)
            {
                if(item.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
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
