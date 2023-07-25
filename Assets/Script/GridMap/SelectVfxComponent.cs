using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectVfxComponent : MonoBehaviour
{
    [SerializeField] private GameObject selectableVfx;
    [SerializeField] private GameObject selectedVfx;

    public void SetSelectable(bool isSelectable)
    {
        selectableVfx.SetActive(isSelectable);
    }

    public void SetSelected(bool isSelected)
    {
        selectedVfx.SetActive(isSelected);
    }
}
