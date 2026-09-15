using System.Collections.Generic;
using UnityEngine;

public class IntentorySystem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public List<BattleAction> MainInventory = new List<BattleAction>();
    public List<BattleAction> HolderInventory = new List<BattleAction>();
    public BattleAction TestItem1;
    public SuperLibrary SuperLibrary;

    public void AddItemM(BattleAction Ba)
    {
        if (MainInventory.Count > 16)
        {
            Debug.Log("No more space :(");
            return;
        }
        if (!Ba.MainDefiner.StartsWith("ITEM"))
        {
            Debug.Log("Thats Not an item");
            return;
        }
        MainInventory.Add(Ba);
    }
    public void AddItemH(BattleAction Ba)
    {
        if (HolderInventory.Count > 16)
        {
            Debug.Log("No more space :(");
            return;
        }
        if (!Ba.MainDefiner.StartsWith("ITEM"))
        {
            Debug.Log("Thats Not an item");
            return;
        }
        HolderInventory.Add(Ba);
    }
    public void ConsumeItem(RuntimeCharacter Ch,int i,EnemyHolder EH,RuntimeCharacter Al)
    {
        if (i>15 || i>MainInventory.Count-1 || i < 0)
        {
            Debug.Log("Item Out of range");
            return;
        }else
        {
            SuperLibrary.AM.ChooseAction(Ch,MainInventory[i],EH,Al,"ItemSprite");
            MainInventory.RemoveAt(i);
        }
    }
    public void ReturnItem(BattleAction Ba,int i)
    {
        Debug.Log("retuna");
        if (!Ba.MainDefiner.StartsWith("ITEM") || (i>15 || i < 0))
        {
            Debug.Log("Cannot return an item");
        }
        else
        {
            AddItemM(Ba);
        }
    }
    public void DebugShowHowManyItem()
    {
        Debug.Log(MainInventory.Count);
        Debug.Log(HolderInventory.Count);
    }
    public void Add4Ones()
    {
        AddItemM(TestItem1);
        AddItemM(TestItem1);
        AddItemM(TestItem1);
        AddItemM(TestItem1);
        Debug.Log("Added 4 items :3");
    }
}
