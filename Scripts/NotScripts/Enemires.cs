using UnityEngine;

[CreateAssetMenu(fileName = "Enemires", menuName = "Scriptable Objects/Enemires")]
public class Enemies : ScriptableObject
{
        public string Name;
        public int HP;
        public int damage;
        public int Defence;
        public EnemyAttack[] AttackSet;
        public BattleAction[] Acts;
        public BattleAction[] RerouteActs;
        public DialogueEntry CheckText;
        

        public GameObject enemyPrefab;
}
