
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyGang", menuName = "Scriptable Objects/EnemyGang")]
public class EnemyGang : ScriptableObject
{
    public string key;

    public Enemies[] gang;
    public List<DialogueEntry> BL;
    public List<DialogueEntry> GF;
    public bool boss;
}
