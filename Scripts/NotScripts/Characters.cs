using UnityEngine;

[CreateAssetMenu(fileName = "Characters", menuName = "Scriptable Objects/Characters")]
public class Character : ScriptableObject
{
        public string characterName;
        public int maxHp;
        public int Number;
        public int StandartAttack;
        public int StandartDefence;
        public int AddDefencife;
        public int TPGain;
        public Weapon HoldW;
        public BattleAction[] HisOwnAction;
        public Sprite Face_Sprite;
        public Sprite SomeTestSprite;
        public Sprite Mini_Face;

}
