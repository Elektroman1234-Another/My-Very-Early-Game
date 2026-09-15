using UnityEngine;

public class WeaponEffectListener : MonoBehaviour
{
    public EnemyInBattleManager EM;

    void Start()
{
    if (EM == null)
        return;

    EM.OnDamageDealt += DamageDealt;
}

    void DamageDealt(RuntimeCharacter attacker, EnemyHolder enemy)
    {

        if (attacker == null)
        {
            return;
        }


        if (attacker.baseData.HoldW == null)
        {
            return;
        }


        if (attacker.baseData.HoldW.Effect == null)
        {
            return;
        }


        attacker.baseData.HoldW.Effect.OnDamageDealt(
            attacker,
            enemy
        );
    }

    void OnDestroy()
    {
        if (EM != null)
        {
            EM.OnDamageDealt -= DamageDealt;
        }
    }
}   