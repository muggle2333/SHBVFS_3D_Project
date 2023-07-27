using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private GameObject container;
    [SerializeField] private Button nextBtn;
    private void Start()
    {
        TutorialManager.Instance.OnStartSpecificTutorial += TutorialManager_OnStartSpecificTutorial;
        nextBtn.onClick.AddListener(() =>
        {
            GoNext();
        });
    }

    private void TutorialManager_OnStartSpecificTutorial(object sender, System.EventArgs e)
    {
        nextBtn.gameObject.SetActive(true);
    }

    public void ShowMessageText(string text)
    {
        container.SetActive(true);
        messageText.text = text;
    }
    public void GoNext()
    {
        nextBtn.gameObject.SetActive(false);
        TutorialManager.Instance.CompleteSpecificTutorial();
    }
}
