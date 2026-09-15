using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public List<Character> characterTemplates = new List<Character>(); //Just here :)
    public List<RuntimeCharacter> party = new List<RuntimeCharacter>();
    public SuperLibrary SuperLibrary;


    public void AddToParty(Character template){
        RuntimeCharacter newMember = new RuntimeCharacter(template);
        party.Add(newMember);
    }
    public void RemoveFromParty(RuntimeCharacter member){
        party.Remove(member);
    }
    public void PrintParty(){
    foreach (RuntimeCharacter member in party)
    {
        Debug.Log(member.baseData.characterName + " - HP: " + member.currentHP);
    }
    }
    public int GetIndexOf(Character character)
{
    for (int i = 0; i < party.Count; i++)
    {
        if (party[i].baseData == character) // assuming ActiveCharacter has a field "CH" referencing the Character
        {
            return i;
        }
    }

    return -1; // indicates "not found" — caller should check for this
}
    public void DamageCharacter(string characterName, int amount){
    RuntimeCharacter target = party.Find(m => m.baseData.characterName == characterName);
    int Index = party.FindIndex(m => m.baseData.characterName == characterName);

    if (target != null)
    {
        target.TakeDamage(amount);
        CheckHP(Index);
    }
    else
    {
    }
    
}
    public void HealCharacter(string characterName, int amount)
    {
        RuntimeCharacter target = party.Find(m => m.baseData.characterName == characterName);
        int Index = party.FindIndex(m => m.baseData.characterName == characterName);

    if (target != null)
    {
        target.Heal(amount);
        NoOverHeal();
        CheckHP(Index);
    }
    else
    {
    }
        
    }
    public void NoOverHeal()
    {
        foreach(RuntimeCharacter CH in party)
        {
            if (CH.currentHP > CH.baseData.maxHp)
            {
                Debug.Log("No you cannot OverHeal");
                CH.currentHP=CH.baseData.maxHp;
            }
        }
    }
    public void IncreaseDefend(string characterName)
    {
        RuntimeCharacter target = party.Find(m => m.baseData.characterName == characterName);
        if (target != null)
        {
            target.IncreaseDefence();
        }
    }
    public void BasicDefence(string characterName)
    {
        RuntimeCharacter target = party.Find(m => m.baseData.characterName == characterName);
        if (target != null)
        {
            target.BackToStandart();
        }
    }
    public void ReturnDefenceAll()
    {
        foreach(RuntimeCharacter CH in party)
        {
            CH.BackToStandart();
        }
    }
    public List<RuntimeCharacter> GetParty()
    {
        return party;
    }

    public void Startofthebattle()
    {
        int i=0;
        foreach (RuntimeCharacter C in party)
        {
            CheckHP(i);
        }
    }
    public void StartOftheTurn()
    {
        for (int i = 0; i < party.Count; i++)
        {
            Debug.Log("STARTOFTURN!!!!");
            Debug.Log(party[i].currentHP);
        }
    }
    void CharacterDown(int i)
    {
        party[i].active=true;
        Debug.Log(party[i].active);
    }
    public void CheckHP(int i)
    {
        Debug.Log("THIS GUY HAASSS");
        Debug.Log(party[i].currentHP);
        if (party[i].currentHP < -999)
        {
            party[i].currentHP=-999;
            CharacterDown(i);
        }
        if (party[i].currentHP <= 0)
        {
            Debug.Log("He died");
            CharacterDown(i);
        }
        if (party[i].currentHP > 0)
        {
            Debug.Log("ALIVE AND WELL");
            party[i].active=false;
        }
        SuperLibrary.MH.ChangeHPAmount(i,party[i],30);
        SuperLibrary.MH.MoveHPbar(party[i]);
    }
    public void FUCKINGDIE(int i)
    {
        party[i].currentHP=0;
        CheckHP(i);
    }
    public void CheckEverythingHealth()
    {
        int i=0;
        foreach(RuntimeCharacter RC in party)
        {
            CheckHP(i);
            i++;
        }
    }
}
