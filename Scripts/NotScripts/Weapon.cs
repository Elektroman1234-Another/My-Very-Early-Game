using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Scriptable Objects/Weapon")]
public class Weapon : ScriptableObject
{
    public string WeaponName;
    public string WeaponDescription;
    public int Damage;
    public Character WhocanUseit;

    public WeaponEffect Effect;
}
