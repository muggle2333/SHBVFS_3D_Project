using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class TutorialUI : MonoBehaviour
{

    [SerializeField] private TMP_Text messageText;
    [SerializeField] private GameObject container;
    [SerializeField] public Button nextBtn;
    [SerializeField] private GameObject Goal;
   // [SerializeField] private GameObject tutorial_1;
    [SerializeField] private GameObject WarningUI;
    [SerializeField] private GameObject Frame;
    [SerializeField] private List<GameObject> Icons = new List<GameObject>();
    [SerializeField] private GameObject Skip;
    [SerializeField] private GameObject AcademyBUffsUI;
    [SerializeField] private GameObject TopLeftFrame;
    [SerializeField] private GameObject LeftFrame;
    private void Start()
    {
        TutorialManager.Instance.OnStartSpecificTutorial += TutorialManager_OnStartSpecificTutorial;
        nextBtn.onClick.AddListener(() =>
        {
            //GoNext();
            TutorialManager.Instance.CompleteSpecificTutorial();
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
    
    public void ShowGoal()
    {
        Goal.SetActive(true);
    }

    public void ShowFrame()
    {
        Frame.SetActive(true);
    }
    public void FrameDisappear()
    {
        Frame.SetActive(false);
    }

    public void ShowWarning()
    {
        WarningUI.SetActive(true);
        Invoke("WarningDisappear", 1.5f);
    }

    public void WarningDisappear()
    {
        WarningUI.SetActive(false);
    }

    public void ShowIcons(int iconIndex)
    {
        Icons[iconIndex].SetActive(true);
    }

    public void IconsDiappear(int iconIndex)
    {
        Icons[iconIndex].SetActive(false);
    }

    public void SkipDiappear()
    {
        Skip.SetActive(false);
    }
    public void ShowSkip()
    {
        Skip.SetActive(true);
    }
    
    public void ShowAcademyBuff()
    {
        AcademyBUffsUI.SetActive(true);
    }

    public void AcademyBuffDisappear()
    {
        AcademyBUffsUI.SetActive(false);
    }
    public void HideTutorial()
    {
        container.SetActive(false);
    }

    public void ShowTopLeft(bool isActive)
    {
        TopLeftFrame.SetActive(isActive);
    }

    public void ShowLeft(bool isActive)
    {
        LeftFrame.SetActive(isActive);
    }
    //public void GoNext()
    //{
    //    //nextBtn.gameObject.SetActive(false);
    //    TutorialManager.Instance.CompleteSpecificTutorial();
    //}

   

    //public void ShowSepecificMessage_Tutorial1()
    //{
    //    container.SetActive(false);
    //    tutorial_1.gameObject.SetActive(true);
    //}

}
