using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonClickSound : MonoBehaviour
{
    public void PlayClickSound()
    {
        SoundManager.Instance.PlaySound(Sound.Button);
    }
}
