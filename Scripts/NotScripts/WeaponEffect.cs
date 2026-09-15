using UnityEngine;

public abstract class WeaponEffect : ScriptableObject
{
    public abstract void OnDamageDealt(
        RuntimeCharacter attacker,
        EnemyHolder enemy
    );
    public abstract float CritIncrease(
        RuntimeCharacter attacker,
        float precision
    );
}