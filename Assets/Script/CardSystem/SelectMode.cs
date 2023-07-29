using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelectMode : MonoBehaviour
{
    public static SelectMode Instance;
    public GameObject selectModeUI;
    public TMP_Text selectModeText;
    public Dictionary<int,List<Vector2Int>> selectedGridDic = new Dictionary<int, List<Vector2Int>>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
        }
    }
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
    public void saveSelectedGrid()
    {

    }
}
