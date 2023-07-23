using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelectMode : MonoBehaviour
{
    public GameObject selectModeUI;
    public TMP_Text selectModeText;
    // Start is called before the first frame update 
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void EnterSelectMode(int SelectCount)
    {
        selectModeUI.SetActive(true);
        selectModeText.text = "Select" + SelectCount + "Grid";
        SelectManager.Instance.selectCount = SelectCount;
    }
    public void ExitSelectMode()
    {
        selectModeUI.SetActive(false);
        SelectManager.Instance.selectCount = 1;
    }
}
