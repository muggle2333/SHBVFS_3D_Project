using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageUI : MonoBehaviour
{
    [SerializeField] private TMP_Text messageText;

    public void Start()
    {
        messageText.text = "Press SPACE to Ready Waiting...";
    }
    public void ShowMessageInfo(string info)
    {
        messageText.text = info;
    }
    public void ShowMessage(float timer)
    {
        messageText.gameObject.SetActive(true);
        int time = Mathf.FloorToInt(timer);
        if (time == 0)
        {
            messageText.text = "START";
        }
        else
        {
            messageText.text = Mathf.FloorToInt(timer).ToString();
        }
    }

    public void HideMessage()
    {
        messageText.gameObject.SetActive(false);
    }
}
