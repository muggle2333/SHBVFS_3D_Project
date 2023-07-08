using System;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    public event EventHandler OnInteractAction;

    private void Update()
    {
        //if(Input.G)
    }
    private void Interact()
    {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }
}
