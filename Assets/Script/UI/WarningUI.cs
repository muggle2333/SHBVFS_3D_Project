using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WarningUI : MonoBehaviour
{
    [SerializeField] private GameObject container;
    [SerializeField] public TMP_Text warningText;
    public void Show(string message)
    {
        container.SetActive(warningText);
        warningText.text = message;
    }

    // Update is called once per frame
    public void Hide()
    {
        container.SetActive(false);
    }
}
