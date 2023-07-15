using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageUI : MonoBehaviour
{
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject messageContent;
    

    public void Start()
    {
        messageText.text = "Press SPACE to Ready";
        GameManager.Instance.OnLocalPlayerReadyChanged += GameManager_OnLocalPlayerReadyChanged;

    }

    public void Update()
    {
    }

    private void GameManager_OnLocalPlayerReadyChanged(object sender, EventArgs e)
    {
        messageText.text = "Waiting for others to Ready";
    }

    public void ShowMessageInfo(string info)
    { 
        messageText.text = info;
        messageContent.SetActive(true);
        timerText.gameObject.SetActive(false);
    }
    public void ShowMessageTimer(float timer)
    {
        messageContent.SetActive(false);
        timerText.gameObject.SetActive(true);
        int time = Mathf.FloorToInt(timer);
        if (time == 0)
        {
            //timerText.text = "START";
            timerText.text = "0";
        }
        else
        {
            timerText.text = Mathf.FloorToInt(timer).ToString();
        }
    }

    public void HideMessage()
    { 
        content.gameObject.SetActive(false);
    }
}
