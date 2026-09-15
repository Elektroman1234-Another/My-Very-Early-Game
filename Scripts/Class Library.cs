using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
[System.Serializable]
public class DialogueEntry
{
    public string mainText;
    public string[] addTexts;
}

[System.Serializable]

public abstract class EnemyAttack: MonoBehaviour
{
    public string attackname;
    public abstract IEnumerator Engage();
}

public class EnemyHolder
{
    public Enemies Enemy;

    public int mercybar;
    public int currenthealth;
    public int place;
}

public class ActiveCharacter
{
    public Character CH;
    public bool act;
}

public abstract class BattleAction : MonoBehaviour
{
    public string actionName;
    public List<Character> whouses;
    public string MainDefiner;
    public int TPCost;
    public string Description; 
    public abstract IEnumerator Perform(SuperLibrary SuperLibrary,RuntimeCharacter CharacterItself, EnemyHolder PassedEnemy, RuntimeCharacter PassedAlly); // renamed from Actthing — this is generic now
}

public class ChosenAction
{
    public RuntimeCharacter character;
    public BattleAction BA;
    public EnemyHolder enem;
    public RuntimeCharacter ally;
    public int TPC;
}

public class TurnSnapshot
{
    public List<RuntimeCharacter> party;
    public int currentCharacterIndex;
    public int currentCharacterButton;
    public List<ChosenAction> choices;


    public TurnSnapshot(List<RuntimeCharacter> activeList, int charIndex,int butIndex, List<ChosenAction> choicesList)
    {
        party = new List<RuntimeCharacter>(activeList);
        currentCharacterIndex = charIndex;
        currentCharacterButton=butIndex;
        choices = new List<ChosenAction>(choicesList);
        
    }
}

public class FightMinigameInfoHolder
{
    public RuntimeCharacter Chara;
    public EnemyHolder EnemyH;
}

