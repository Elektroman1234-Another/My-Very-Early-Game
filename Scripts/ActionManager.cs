using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using NUnit.Framework;

public class ActionManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    List <ChosenAction> turnChoices=new List<ChosenAction>();
    Stack<TurnSnapshot> history = new Stack<TurnSnapshot>();
    Stack<BattleAction> itemhistory = new Stack<BattleAction>();
    public SuperLibrary SuperLibrary;
    public BattleAction Nothing;
    public int TPN;
    void SaveSnapshot()
{
    List<RuntimeCharacter> partyClone = new List<RuntimeCharacter>();
    foreach (RuntimeCharacter rc in SuperLibrary.CM.party)
    {
        partyClone.Add(rc.Clone());
    }

    history.Push(new TurnSnapshot(partyClone, SuperLibrary.MH.currentcharacterchoosingbuttons, SuperLibrary.MH.currentbuttonchoosing, new List<ChosenAction>(turnChoices)));
}
    public void Undo()
{
    if (history.Count > 0 && itemhistory.Count > 0)
    {
        // Return item to inventory if one was used in this action
        BattleAction lastAction = itemhistory.Peek();
        if (lastAction != null)
        {
            SuperLibrary.IS.ReturnItem(lastAction, 2);
        }
        itemhistory.Pop(); // Remove from item history
        
        SuperLibrary.EM.EraseText();
        SuperLibrary.EM.FromTheStart();

        TurnSnapshot last = history.Pop();
        RuntimeCharacter characterToRestore = null;
        if (turnChoices.Count > 0)
        {
            ChosenAction undoneChoice = turnChoices[turnChoices.Count - 1];
            characterToRestore = undoneChoice.character;
            SuperLibrary.EM.TPMeter -= undoneChoice.TPC;
            SuperLibrary.EM.ChangeTPNumber();
        }

        SuperLibrary.CM.party = last.party;
        SuperLibrary.MH.TBGD(SuperLibrary.MH.currentcharacterchoosingbuttons);
        SuperLibrary.MH.ChangeTheStatus(SuperLibrary.MH.currentcharacterchoosingbuttons,false);
        SuperLibrary.MH.currentcharacterchoosingbuttons = last.currentCharacterIndex;
        SuperLibrary.MH.ChangeTheStatus(SuperLibrary.MH.currentcharacterchoosingbuttons,true);
        SuperLibrary.MH.TBGU(SuperLibrary.MH.currentcharacterchoosingbuttons);
        SuperLibrary.MH.currentbuttonchoosing = last.currentCharacterButton;
        turnChoices = last.choices;
        if (characterToRestore != null)
        {
            SuperLibrary.MH.ChangeSprite(characterToRestore,"StandartFace");
        }
        
    }
}


    public void ChooseAction(RuntimeCharacter C,BattleAction A, EnemyHolder E, RuntimeCharacter Al, string Rule)
    {
        TPN=0;
        int TPDefence=0;
        if (A && A.MainDefiner.Contains("ITEM"))
        {
            itemhistory.Push(A);
        }
        else
        {
             itemhistory.Push(null);
        }
        if (A == null)
        {
            A=Nothing;
        }
        if (A.actionName=="Defence Up"){
            Debug.Log("WE SHOULD DEFENCE UP!!!!!!!");
            TPDefence=+C.baseData.TPGain;
        }
        ChosenAction choice = new ChosenAction { character = C, BA = A, enem=E,ally=Al,TPC=A.TPCost+TPDefence};
        SaveSnapshot();
        SuperLibrary.EM.HideAllButtons();
        SuperLibrary.EM.HideActsButtons();
        SuperLibrary.EM.TPMeter=SuperLibrary.EM.TPMeter+A.TPCost+TPDefence;
        if (SuperLibrary.EM.TPMeter > 100)
        {
            choice.TPC=100-(SuperLibrary.EM.TPMeter-(A.TPCost+TPDefence));
        }
        SuperLibrary.EM.ChangeTPNumber();
        turnChoices.Add(choice);
        SuperLibrary.CM.party[SuperLibrary.CM.GetIndexOf(C.baseData)].active = true;

        if (A.whouses.Count > 1)
    {
        // duo — mark the partner as acted too, without adding a SEPARATE turnChoices entry
        foreach (Character partner in A.whouses)
        {
            if (partner != C.baseData)
                SuperLibrary.CM.party[SuperLibrary.CM.GetIndexOf(partner)].active = true;
        }
    }
        SuperLibrary.BTH.ShowButton();
        SuperLibrary.MH.ChangeSprite(C,Rule);
        SuperLibrary.EM.enemychoose=false;
        SuperLibrary.EM.actchoose=false;
        SuperLibrary.EM.BAHolder=null;
        SuperLibrary.EM.IsItFighting=false;
        SuperLibrary.EM.IsItMercy=false;
        SuperLibrary.EM.enemholder=null;
        SuperLibrary.MH.NextCharacter();
        SuperLibrary.MH.TurnOnCBB();
    }
    public IEnumerator ExecuteTurn()
{
    Debug.Log("Execute");
    if (turnChoices == null || turnChoices.Count == 0)
    {
        yield break;
    }

    List<ChosenAction> actionsToExecute = new List<ChosenAction>(turnChoices);

    foreach (ChosenAction choice in actionsToExecute)
    {
        if (choice == null || choice.BA == null)
        {
            continue;
        }

        Debug.Log(choice.enem+"|"+choice.ally);
        yield return StartCoroutine(choice.BA.Perform(SuperLibrary,choice.character,choice.enem,choice.ally));
        while (SuperLibrary.TW.IsTyping)
        {
            yield return null;
        }
    }
    SuperLibrary.AMG.FMIHPrint();
    while (SuperLibrary.AMG.Activitism)
        {
            yield return null;
        }
    StartCoroutine(SuperLibrary.MH.WaitJustALittleAndBattleStart());
    turnChoices.Clear(); // reset for next round
    history.Clear();     // clear undo history, since this turn is locked in
    itemhistory.Clear(); // clear item history too
}
}

