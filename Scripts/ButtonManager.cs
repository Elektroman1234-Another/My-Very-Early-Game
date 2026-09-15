using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public SuperLibrary SuperLibrary;
    public string Whatdoyoudo;
    public string WhatYouDid;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void YouChooseButton(RuntimeCharacter ch,int button)
    {
        if (ch.baseData.name == "Alaska")
        {
            if (SuperLibrary.MH.currentcharacterchoosingbuttons!=0)
            {
                Debug.Log("Ok that SHOULD NOT happen");
            }
            switch (button){
                case(0):
                    FightAll();
                    break;
                case(1):
                    AlaskaTwinkle();
                    break;
                case(2):
                    AlaskaAct();
                    break;
                case(3):
                    ItemAll();
                    break;
                case(4):
                    MercyAll();
                    break;
                case(5):
                    Defend();
                    break;
                default:
                    Debug.Log("Bruh");
                    break;
                }
        }
        else
        {
            switch (button)
            {
                case(0):
                    FightAll();
                    break;
                case(1):
                    OtherAct();
                    break;
                case(2):
                    ItemAll();
                    break;
                case(3):
                    MercyAll();
                    break;
                case(4):
                    Defend();
                    break;
                default:
                    break;

            }
        }
    }

    public void FromTheStart()
{
    SuperLibrary.EM.enemychoose=false;
    SuperLibrary.EM.actchoose=false;
    SuperLibrary.EM.BAHolder=null;
    SuperLibrary.EM.enemholder=null;
    SuperLibrary.EM.CharacterHolder=null;
    SuperLibrary.EM.CEBA.Clear();
    SuperLibrary.EM.EraseText();
}
    void FightAll()
    {
        SuperLibrary.EM.currentEnemyChoosing=0;
        SuperLibrary.EM.currentActChoosing=0;
        SuperLibrary.EM.ActionHistory.Push("Main Menu");
        SuperLibrary.EM.UpdateCanGoBack();
        SuperLibrary.EM.IsItFighting=true;
        SuperLibrary.EM.ShowAvailableTargets(1);
        return;
    }
    void AlaskaAct()
    {
        Debug.Log("YES");
        SuperLibrary.EM.currentEnemyChoosing=0;
        SuperLibrary.EM.currentActChoosing=0;
        SuperLibrary.EM.ActionHistory.Push("Main Menu");
        SuperLibrary.EM.UpdateCanGoBack();
        SuperLibrary.EM.ShowAvailableTargets(1);
    }
    void AlaskaTwinkle()
    {
        SuperLibrary.EM.currentEnemyChoosing=0;
        SuperLibrary.EM.currentActChoosing=0;
        SuperLibrary.EM.ActionHistory.Push("Main Menu");
        SuperLibrary.EM.UpdateCanGoBack();
        SuperLibrary.EM.TwinkleHelp();
    }
    void OtherAct()
    {
        Debug.Log("YES");
        SuperLibrary.EM.currentEnemyChoosing=0;
        SuperLibrary.EM.currentActChoosing=0;
        SuperLibrary.EM.ActionHistory.Push("Main Menu");
        SuperLibrary.EM.UpdateCanGoBack();
        SuperLibrary.EM.ChooseAct(null,null);
    }
    void Defend()
    {
        SuperLibrary.EM.currentEnemyChoosing=0;
        SuperLibrary.EM.currentActChoosing=0;
        SuperLibrary.EM.DefendAdd();
    }
    void ItemAll()
    {
        SuperLibrary.EM.currentEnemyChoosing=0;
        SuperLibrary.EM.currentActChoosing=0;
        SuperLibrary.EM.ActionHistory.Push("Main Menu");
        SuperLibrary.EM.UpdateCanGoBack();
        SuperLibrary.EM.ItemsLogic();
    }
    void MercyAll()
    {
        SuperLibrary.EM.currentEnemyChoosing=0;
        SuperLibrary.EM.currentActChoosing=0;
        SuperLibrary.EM.ActionHistory.Push("Main Menu");
        SuperLibrary.EM.UpdateCanGoBack();
        SuperLibrary.EM.IsItMercy=true;
        SuperLibrary.EM.ShowAvailableTargets(1);
        return;
    }
}
