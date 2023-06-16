using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Card", menuName ="Card")]
public class CardComponent : ScriptableObject
{
    public new string cardname;
    public string description;

    public Sprite artwork;

    public int damage;
    public int defense;
    public PlayerId Cardtarget;
}
