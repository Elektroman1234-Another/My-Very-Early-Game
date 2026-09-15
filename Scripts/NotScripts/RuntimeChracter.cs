using System.Data.Common;

public class RuntimeCharacter
{
    public Character baseData;
    public int currentHP;

    public int currentdefence;
    public bool active;
    

    public RuntimeCharacter(Character data)
    {
        baseData = data;
        currentHP = data.maxHp; // starts full, copied from the template
        currentdefence = data.StandartDefence;
        active=false;
    }
    public RuntimeCharacter Clone()
{
    RuntimeCharacter copy = new RuntimeCharacter(baseData);
    copy.currentHP = currentHP;
    copy.currentdefence = currentdefence;
    copy.active = active;
    return copy;
}

    public void TakeDamage(int amount)
    {
        currentHP -= amount;
    }
    public void Heal(int amount)
    {
        currentHP +=amount;   
    }
    public void IncreaseDefence()
    {
        currentdefence=currentdefence+baseData.AddDefencife;
    }
    public void BackToStandart()
    {
        currentdefence = baseData.StandartDefence;
    }
}