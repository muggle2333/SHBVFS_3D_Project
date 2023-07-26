using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private GameObject container;
    public void ShowMessageText(string text)
    {
        container.SetActive(true);
        messageText.text = text;
    }
}
