using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NUnit.Framework;
using TMPro;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor.U2D.Aseprite;
using UnityEditor.UIElements;
using UnityEngine;

public class EnemyInBattleManager : MonoBehaviour
{
    public SuperLibrary SuperLibrary;
    public List <EnemyHolder> ActiveEnemies=new List<EnemyHolder>();
    public List <BattleAction> CEBA=new List<BattleAction>();
    public List <TextMeshProUGUI> Holder;
    public List <TextMeshProUGUI> HolderActs;
    public TextMeshProUGUI Numbertext;
    public List <GameObject> NearSprites1;
    public List <GameObject> NearSprites2;
    public event Action<RuntimeCharacter, EnemyHolder> OnDamageDealt;
    Color prevcolorEnemy;
    Color prevcolorAct;
    Color prevcolorItem;
    public int currentEnemyChoosing=0;
    public int currentActChoosing=0;
    public int currentItemChoosing=0;
    int logicHolder=0;
    public GameObject SpriteHolder;
    public EnemyGang justtest;
    int currentpage=0;
    int currentpageTwinkle=0;
    int currentitempage=0;
    public bool enemychoose=false;
    public bool actchoose=false;
    public bool itemchoose=false;
    public bool twinklehelp=false;
    public bool ActiveFight=false;
    public BattleAction Check;
    public BattleAction Fight;
    public BattleAction Defend;
    public BattleAction Mercy;
    public BattleAction Reroute;
    bool justOpened = false;
    public GameObject AlaskaPlayer;

    public EnemyHolder enemholder=null;
    public RuntimeCharacter CharacterHolder=null;
    public BattleAction BAHolder=null;
    public Stack<string> ActionHistory=new Stack<string>();
    public bool CanGoback;
    public bool IsItFighting=false;
    public bool IsItMercy=false;
    public int TPMeter=0;
    //---------------
    Vector3 StartPosition = new Vector3(0f,-1.761f,0f);
    Vector3 EndPosition = new Vector3(0f,0f,0f);
    public GameObject WhiteBarShower;
    public GameObject YellowBarShower;
    Coroutine yellowBarMovement;
    Coroutine CoolTPCounterCoro;
    public TextMeshProUGUI DescriptionText;
    [Header("Sound")]
    public AudioData ButtonChange;
    public AudioData Enterpress;
    //---------------
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ShowNP();
    }

    // Update is called once per frame
    void Update()
    {
        if (justOpened)
    {
        justOpened = false; // consume the guard, don't process input this frame
        return;
    }
        if (Input.GetKeyDown(KeyCode.W) && enemychoose==true)
        {
            ChangeECB(-1);
            SuperLibrary.AudioM.PlaySFX(ButtonChange);
            return;
        }
        if (Input.GetKeyDown(KeyCode.S) && enemychoose==true)
        {
            ChangeECB(1);
            SuperLibrary.AudioM.PlaySFX(ButtonChange);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Return) && enemychoose == true)
        {
            SuperLibrary.AudioM.PlaySFX(Enterpress);
            ActionHistory.Push("EnemyChoose");
            UpdateCanGoBack();
            EnemOrAlly();
            return;
        }
        //----------------
        if (Input.GetKeyDown(KeyCode.D) && actchoose==true)
        {
            Debug.Log("Bruh");
            SuperLibrary.AudioM.PlaySFX(ButtonChange);
            ChangeACB(1);
            return;
        }
        if (Input.GetKeyDown(KeyCode.A) && actchoose==true)
        {
            ChangeACB(-1);
            SuperLibrary.AudioM.PlaySFX(ButtonChange);
            return;
        }
        if (Input.GetKeyDown(KeyCode.W) && actchoose==true)
        {
            ChangeACB(-2);
            SuperLibrary.AudioM.PlaySFX(ButtonChange);
            return;
        }
        if (Input.GetKeyDown(KeyCode.S) && actchoose==true)
        {
            ChangeACB(2);
            SuperLibrary.AudioM.PlaySFX(ButtonChange);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Return) && actchoose==true)
        {
            if (CheckTP(CEBA[currentActChoosing])==true){
                Debug.Log(CEBA[currentActChoosing].TPCost);
                Debug.Log("You dont have enought tp");
                return;
            }
            if (CheckParty(CEBA[currentActChoosing]) == false)
            {
                Debug.Log("Something Happened to your teammate");
                return;
            }
            if (twinklehelp == true)
            {
                ActionHistory.Push("Twinkle");
            }
            else
            {
                ActionHistory.Push("ActionChoose");
            }
            UpdateCanGoBack();
            Debug.Log("What");
            SuperLibrary.AudioM.PlaySFX(Enterpress);
            ChooseEnemy(CEBA[currentActChoosing],enemholder,CharacterHolder);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Escape) && CanGoback)
        {
            GoBackLogic();
            SuperLibrary.AudioM.PlaySFX(Enterpress);
        }
        if (Input.GetKeyDown(KeyCode.D) && itemchoose==true)
        {
            ChangeICB(1);
            SuperLibrary.AudioM.PlaySFX(ButtonChange);
            return;
        }
        if (Input.GetKeyDown(KeyCode.A) && itemchoose==true)
        {
            ChangeICB(-1);
            SuperLibrary.AudioM.PlaySFX(ButtonChange);
            return;
        }
        if (Input.GetKeyDown(KeyCode.W) && itemchoose==true)
        {
            ChangeICB(-2);
            SuperLibrary.AudioM.PlaySFX(ButtonChange);
            return;
        }
        if (Input.GetKeyDown(KeyCode.S) && itemchoose==true)
        {
            ChangeICB(2);
            SuperLibrary.AudioM.PlaySFX(ButtonChange);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Return) && itemchoose==true)
        {
            ActionHistory.Push("ItemChoose");
            UpdateCanGoBack();
            ChooseItem();
            SuperLibrary.AudioM.PlaySFX(Enterpress);
            return;
        }
    }

    public void StartFight(EnemyGang EG)
    {
        if (ActiveFight==true)
        {
            return;
        }
        ActiveFight=true;
        int i=0;
        foreach (Enemies enem in EG.gang)
        {

            ActiveEnemies.Add(new EnemyHolder{Enemy=enem,mercybar=0,currenthealth=enem.HP,place=i});
            Instantiate(enem.enemyPrefab);
            i++;            
        }
        SuperLibrary.MH.StartTurn();
        HideAllButtons();
        HideActsButtons();
        TPMeter=0;
        SuperLibrary.MH.UIactive=true;
        SuperLibrary.MH.ChoosingBetweenButtons=true;
        currentEnemyChoosing=0;
        currentActChoosing=0;
        currentItemChoosing=0;
        logicHolder=0;
        SuperLibrary.MH.currentcharacterchoosingbuttons=0;
        SuperLibrary.MH.currentbuttonchoosing=0;
        ChangeTPNumber();
    }
    public void EndFight()
    {
        if (ActiveFight==false)
        {
            return;
        }
        AlaskaScript alaskaScript = AlaskaPlayer != null
            ? AlaskaPlayer.GetComponent<AlaskaScript>()
            : FindFirstObjectByType<AlaskaScript>();
        if (alaskaScript != null)
        {
            alaskaScript.CanMove = true;
        }
        ActiveFight=false;
        SuperLibrary.MH.Recolor(false);
        Recolor(false);
        RecolorAct(false);
        RecolorItem(false);
        SuperLibrary.MH.SlowlyChangeTextCorStart(false);
        ActiveEnemies.Clear();
        TPMeter=0;
        enemychoose=false;
        actchoose=false;
        twinklehelp=false;
        itemchoose=false;
        currentEnemyChoosing=0;
        currentActChoosing=0;
        currentItemChoosing=0;
        logicHolder=0;
        SuperLibrary.MH.currentcharacterchoosingbuttons=0;
        SuperLibrary.MH.currentbuttonchoosing=0;
        ActionHistory.Clear();
        CanGoback=false;
        IsItFighting=false;
        IsItMercy=false;
        SuperLibrary.MH.UIactive=false;
        SuperLibrary.MH.ChoosingBetweenButtons=false;
        SuperLibrary.MH.allThingHasToShutDown();
        ChangeTPNumber();
        //HereChangeTheMovement<-
    }
    void ShowNP()
    {
        foreach (EnemyHolder EH in ActiveEnemies)
        {
            Debug.Log("The enemy name: "+EH.Enemy.name+". His place is: "+EH.place);
        }
    }

public void ShowAvailableTargets(int page)
{
    HideActsButtons();
    SuperLibrary.MH.TurnOffCBB();
    itemchoose=false;
    actchoose=false;
    twinklehelp=false;
    enemychoose = true;
    justOpened = true;
    currentpage = page;
    int startIndex = (page - 1) * 3;
    for (int i = 0; i < 3; i++)
        {
            Holder[i].text="";
            Holder[i].enabled = false;
        }
    if (BAHolder && BAHolder.MainDefiner.Contains("ALLY"))
        {
            logicHolder=SuperLibrary.CM.party.Count;
            Debug.Log("WORKING DUDE");
            int ally=0;
            for (int i=startIndex; i<startIndex +3 && i < SuperLibrary.CM.party.Count; i++)
            {
                Holder[ally].text=SuperLibrary.CM.party[i].baseData.name;
                Holder[ally].enabled=true;
                Holder[ally].color=Color.white;
                ally++;
            }
            Recolor(true);
        }
        else
        {
            logicHolder=ActiveEnemies.Count;
            int enem = 0;
            for (int i = startIndex; i < startIndex + 3 && i < ActiveEnemies.Count; i++)
            {
                Holder[enem].text = ActiveEnemies[i].Enemy.name;
                Holder[enem].enabled = true;
                Holder[enem].color = Color.white;
                enem++;
            }
    Recolor(true);
        }
}

    public void HideAllButtons()
    {
        foreach(TextMeshProUGUI TMP in Holder)
        {
            TMP.enabled=false;
        }
    }
void ChangeECB(int i)
{
    Recolor(false);

    int absoluteIndex = (currentpage - 1) * 3 + currentEnemyChoosing + i;

    if (absoluteIndex >= logicHolder)
    {
        absoluteIndex = 0; // wrap forward to the very first enemy
    }
    else if (absoluteIndex < 0)
    {
        absoluteIndex = logicHolder - 1; // wrap backward to the very last enemy
    }

    int newPage = (absoluteIndex / 3) + 1;
    int newSlot = absoluteIndex % 3;

    if (newPage != currentpage)
    {
        currentEnemyChoosing = newSlot;
        ShowAvailableTargets(newPage);
        return;
    }

    currentEnemyChoosing = newSlot;
    Recolor(true);
}
void ChangeACB(int i) //<---
    {
        RecolorAct(false);
        if (i == -1 || i==1)
        {
            if (currentActChoosing==CEBA.Count-1 && currentActChoosing % 2 == 0)
            {
                RecolorAct(true);
                Debug.Log(currentActChoosing);
                DescriptionText.text=CEBA[currentActChoosing].Description;
                return;
            }
            switch (currentActChoosing%2)
            {
                case(0):
                    currentActChoosing=currentActChoosing+1;
                    break;
                case(1):
                    currentActChoosing=currentActChoosing-1;
                    break;
            }
            RecolorAct(true);
            Debug.Log(currentActChoosing);
            DescriptionText.text=CEBA[currentActChoosing].Description;
            return;
        }
        if (currentActChoosing + i > CEBA.Count-1)
        {
            switch (currentActChoosing%2)
            {
                case(0):
                    currentActChoosing=0;
                    break;
                case(1):
                    currentActChoosing=1;
                    break;
            }
            RecolorAct(true);
            Debug.Log(currentActChoosing);
            DescriptionText.text=CEBA[currentActChoosing].Description;
            return;
        }
        if (currentActChoosing + i < 0)
        {
           switch (currentActChoosing%2)
            {
                case(0):
                    currentActChoosing=CEBA.Count - 2;
                    break;
                case(1):
                    currentActChoosing=CEBA.Count - 1;
                    break;
            }
            if (currentActChoosing < 0)
            {
                currentActChoosing=0;
            }
            RecolorAct(true);
            Debug.Log(currentActChoosing);
            DescriptionText.text=CEBA[currentActChoosing].Description;
            return; 
        }
        currentActChoosing=currentActChoosing+i;
        Debug.Log(currentActChoosing);
        DescriptionText.text=CEBA[currentActChoosing].Description;
        RecolorAct(true); 
    }

    public void Recolor(bool cl)
    {
        Debug.Log("RECOLOR");
        TextMeshProUGUI Chosenbutton=Holder[currentEnemyChoosing];
        switch (cl)
        {
            case true:
                prevcolorEnemy=Chosenbutton.color;
                Chosenbutton.color=Color.yellow;
                break;
            case false:
                Chosenbutton.color=prevcolorEnemy;
                break;
        }
    }
    public void RecolorAct(bool cl)
    {
        TextMeshProUGUI Chosenbutton=HolderActs[currentActChoosing];
        switch (cl)
        {
            case true:
                prevcolorAct=Chosenbutton.color;
                Chosenbutton.color=Color.yellow;
                break;
            case false:
                Chosenbutton.color=prevcolorAct;
                break;
        }
    }
    public void RecolorItem(bool cl)
    {
        TextMeshProUGUI Chosenbutton=HolderActs[currentItemChoosing];
        switch (cl)
        {
            case true:
                prevcolorItem=Chosenbutton.color;
                Chosenbutton.color=Color.yellow;
                break;
            case false:
                Chosenbutton.color=prevcolorItem;
                break;
        }
    }
    void LoadUPAllActions(Enemies? EH)
    {
        Debug.Log("FUCK0");
        CEBA.Clear();
        if (EH == null)
        {
            CEBA.Add(Reroute);
            foreach(BattleAction BI in SuperLibrary.CM.party[SuperLibrary.MH.currentcharacterchoosingbuttons].baseData.HisOwnAction)
        {
            if (!BI.MainDefiner.StartsWith("ALASKA+"))
            {
                Debug.Log(BI.MainDefiner);
                CEBA.Add(BI);
            }
        }
            return;
        }
        if (SuperLibrary.MH.currentcharacterchoosingbuttons == 0)
        {
            CEBA.Add(Check);
        }else
        {
        }
        foreach(BattleAction BI in SuperLibrary.CM.party[SuperLibrary.MH.currentcharacterchoosingbuttons].baseData.HisOwnAction)
        {
            if (!BI.MainDefiner.StartsWith("ALASKA+"))
            {
                Debug.Log(BI.MainDefiner);
                CEBA.Add(BI);
            }
        }
        foreach(BattleAction Ba in EH.Acts)
        {
            if (Ba.whouses[0]== SuperLibrary.CM.party[SuperLibrary.MH.currentcharacterchoosingbuttons].baseData)
            {
                CEBA.Add(Ba);
            }
            else
            {
                Debug.Log(Ba.whouses[0]);
                Debug.Log(SuperLibrary.CM.party[SuperLibrary.MH.currentcharacterchoosingbuttons].baseData);
                Debug.Log("FUCK1");
            }
        }     
    }
    public void ShowAllActs()
    {
        justOpened = true;
    for (int i = 0; i < 8; i++)
        {
            HolderActs[i].text="";
            DescriptionText.enabled=true;
            HolderActs[i].enabled = false;
            HolderActs[i].color = Color.white;
            NearSprites1[i].active=false;
            SpriteRenderer SRH=NearSprites1[i].GetComponent<SpriteRenderer>();
            NearSprites1[i].transform.localScale=SpriteHolder.transform.localScale;
            SpriteRenderer SRH1=SpriteHolder.GetComponent<SpriteRenderer>();
            SRH.sprite=SRH1.sprite;
            NearSprites2[i].active=false;
            SRH=NearSprites2[i].GetComponent<SpriteRenderer>();
            NearSprites2[i].transform.localScale=SpriteHolder.transform.localScale;
            SRH.sprite=SRH1.sprite;
            DescriptionText.text=CEBA[currentActChoosing].Description;
        }
    int idk=0;
    foreach(BattleAction Ba in CEBA)
        {
            if (Ba == null)
            {
                HolderActs[idk].text="The fuck?";
                HolderActs[idk].enabled=true;
                idk++;
            }
            else
            {
                HolderActs[idk].text=Ba.actionName;
                HolderActs[idk].enabled=true;
                if (Ba.whouses.Count>1){
                    if (Ba.whouses[1].Mini_Face != null)
                    {
                        NearSprites1[idk].active=true;
                        SpriteRenderer SRH=NearSprites1[idk].GetComponent<SpriteRenderer>();
                        Vector2 oldSize = SRH.sprite.bounds.size;
                        SRH.sprite=Ba.whouses[1].Mini_Face;
                        Vector2 newSize = SRH.sprite.bounds.size;
                        if (newSize.x > 0 && newSize.y > 0)
                        {
                            NearSprites1[idk].transform.localScale = new Vector3(
                                NearSprites1[idk].transform.localScale.x * oldSize.x / newSize.x,
                                NearSprites1[idk].transform.localScale.y * oldSize.y / newSize.y,
                                NearSprites1[idk].transform.localScale.z);
                        }   
                    }
                }
                if (Ba.whouses.Count>2){
                    if (Ba.whouses[2].Mini_Face != null)
                    {
                        NearSprites2[idk].active=true;
                        SpriteRenderer SRH=NearSprites2[idk].GetComponent<SpriteRenderer>();
                        Vector2 oldSize = SRH.sprite.bounds.size;
                        SRH.sprite=Ba.whouses[2].Mini_Face;
                        Vector2 newSize = SRH.sprite.bounds.size;
                        if (newSize.x > 0 && newSize.y > 0)
                        {
                            NearSprites2[idk].transform.localScale = new Vector3(
                                NearSprites2[idk].transform.localScale.x * oldSize.x / newSize.x,
                                NearSprites2[idk].transform.localScale.y * oldSize.y / newSize.y,
                                NearSprites2[idk].transform.localScale.z);
                        }
                    }
                }
                idk++;
            }
        }
    }
    public void EraseText()
    {
        for (int i = 0; i < 8; i++)
        {
            HolderActs[i].text="";
            NearSprites1[i].active=false;
            NearSprites2[i].active=false;
        }
    }
    public void HideActsButtons()
    {
        for (int i = 0; i < 8; i++)
        {
            HolderActs[i].enabled = false;
            NearSprites1[i].active=false;
            NearSprites2[i].active=false;
        }
        DescriptionText.enabled=false;
    }
    public void ChooseAct(EnemyHolder? E, RuntimeCharacter? C)
    {
        if (E == null && C==null)
        {
        Recolor(false);
        currentActChoosing=0;
        SuperLibrary.MH.TurnOffCBB();
        LoadUPAllActions(null);
        HideAllButtons();
        ShowAllActs();
        actchoose=true;
        enemychoose=false;
        itemchoose=false;
        prevcolorAct=Color.white;
        RecolorAct(true);
        return;
        }else if(BAHolder!=null && C==null){
            Recolor(false);
            SuperLibrary.AM.ChooseAction(SuperLibrary.CM.party[SuperLibrary.MH.currentcharacterchoosingbuttons],BAHolder,E,CharacterHolder,"ActSprite");
            return;
        }else if(C!=null && BAHolder != null)
        {
            Recolor(false);
            SuperLibrary.AM.ChooseAction(SuperLibrary.CM.party[SuperLibrary.MH.currentcharacterchoosingbuttons],BAHolder,enemholder,C,"ActSprite");
            return;
        }
        if (HolderActs[currentActChoosing].text=="The fuck?")
        {
            Debug.Log("MYSTARD");
            foreach (BattleAction Bu in E.Enemy.Acts)
            {
                if (Bu.whouses[0] == SuperLibrary.CM.party[SuperLibrary.MH.currentcharacterchoosingbuttons].baseData && Bu.MainDefiner=="UNIQUE")
                {
                    RecolorAct(false);
                    SuperLibrary.AM.ChooseAction(SuperLibrary.CM.party[SuperLibrary.MH.currentcharacterchoosingbuttons],Bu,E,CharacterHolder,"ActSprite");
                    return;
                }
                else
                {
                    RecolorAct(false);
                    SuperLibrary.AM.ChooseAction(SuperLibrary.CM.party[SuperLibrary.MH.currentcharacterchoosingbuttons],null,E,CharacterHolder,"ActSprite");
                    return;
                }
            }
        }
        if (IsItFighting)
        {
            E=ActiveEnemies[(currentpage - 1) * 3 + currentEnemyChoosing];
            SuperLibrary.AM.ChooseAction(SuperLibrary.CM.party[SuperLibrary.MH.currentcharacterchoosingbuttons],Fight,E,null,"AttackSprite");
            return;
        }
        if (IsItMercy)
        {
            E=ActiveEnemies[(currentpage - 1) * 3 + currentEnemyChoosing];
            SuperLibrary.AM.ChooseAction(SuperLibrary.CM.party[SuperLibrary.MH.currentcharacterchoosingbuttons],Mercy,E,null,"MercySprite");
            return;
        }
        Recolor(false);
        currentActChoosing=0;
        SuperLibrary.MH.TurnOffCBB();
        LoadUPAllActions(E.Enemy);
        HideAllButtons();
        ShowAllActs();
        actchoose=true; 
        enemychoose=false;
        itemchoose=false;
        prevcolorAct=Color.white;
        enemholder=E;
        RecolorAct(true);
    }
    void ChooseEnemy(BattleAction? Ba, EnemyHolder? Enem, RuntimeCharacter? Ally)
    {
        if (Ba != null && Ba.MainDefiner.Contains("INSTANT"))
    {
        Recolor(false);
        RecolorAct(false);
        SuperLibrary.AM.ChooseAction(SuperLibrary.CM.party[SuperLibrary.MH.currentcharacterchoosingbuttons], Ba, null, null,"ActSprite");
        return;
    }
        if (Enem == null)
        {
            Debug.Log("It goes here");
            BAHolder=Ba;
            ShowAvailableTargets(1);
            return;
        }
        Recolor(false);
        RecolorAct(false);
        if (Ba.MainDefiner.Contains("ITEM"))
        {
            Debug.Log("BomboClat!!!!!!!!!");
            return;
        }
        SuperLibrary.AM.ChooseAction(SuperLibrary.CM.party[SuperLibrary.MH.currentcharacterchoosingbuttons],Ba,Enem,Ally,"ActSprite");
    }
    public void FromTheStart()
    {
        enemychoose=false;
        actchoose=false;
        twinklehelp=false;
        itemchoose=false;
        BAHolder=null;
        IsItFighting=false;
        IsItMercy=false;
    }
    public void UpdateCanGoBack()
    {
        CanGoback=ActionHistory.Count > 0;
    }
    public void GoBackLogic()
    {
        switch (ActionHistory.Peek())
        {
            case("Main Menu"):
                enemychoose=false;
                actchoose=false;
                twinklehelp=false;
                itemchoose=false;
                BAHolder=null;
                enemholder=null;
                CharacterHolder=null;
                IsItFighting=false;
                IsItMercy=false;
                HideAllButtons();
                HideActsButtons();
                SuperLibrary.MH.TurnOnCBB();
                SuperLibrary.BTH.ShowButton();
                ActionHistory.Pop();
                UpdateCanGoBack();
                break;
            case("EnemyChoose"):
                currentEnemyChoosing=0;
                currentActChoosing=0;
                ShowAvailableTargets(1);
                BAHolder = null;
                IsItFighting=false;
                IsItMercy=false;
                ActionHistory.Pop();
                UpdateCanGoBack();
                break;
            case("ActionChoose"):
                currentActChoosing = 0;
                SuperLibrary.MH.TurnOffCBB();
                BAHolder = null;
                IsItFighting=false;
                IsItMercy=false;
                if (enemholder==null)
                {
                    LoadUPAllActions(null);
                }
                else
                {
                    LoadUPAllActions(enemholder.Enemy);// rebuild the same Acts list as before
                }
                BAHolder = null;
                HideAllButtons();
                ShowAllActs();
                actchoose = true;
                enemychoose = false;
                itemchoose=false;
                RecolorAct(true);
                ActionHistory.Pop();
                UpdateCanGoBack();
                break;
            case("ItemChoose"):
                Recolor(false);
                RecolorAct(false);
                BAHolder = null;
                actchoose = false;
                enemychoose = false;
                itemchoose=true;
                currentItemChoosing=0;
                SuperLibrary.MH.TurnOffCBB();
                LoadUpItems(1);
                HideAllButtons();
                ShowAllActs();
                prevcolorItem=Color.white;
                RecolorItem(true);
                ActionHistory.Pop();
                UpdateCanGoBack();
                break;
            case("Twinkle"):
            currentActChoosing = 0;
                SuperLibrary.MH.TurnOffCBB();
                BAHolder = null;
                IsItFighting=false;
                IsItMercy=false;
                if (enemholder==null)
                LoadTwinkleHelp(1);
                BAHolder = null;
                HideAllButtons();
                ShowAllActs();
                twinklehelp=true;
                actchoose = true;
                enemychoose = false;
                itemchoose=false;
                RecolorAct(true);
                ActionHistory.Pop();
                UpdateCanGoBack();
                break;
        }
    }
    public void EnemOrAlly()
    {
        if (!BAHolder ||BAHolder.MainDefiner.Contains("ENEMY"))
        {
            ChooseAct(ActiveEnemies[(currentpage - 1) * 3 + currentEnemyChoosing],null);
        }else if (BAHolder.MainDefiner.Contains("ITEM") && BAHolder.MainDefiner.Contains("ALLY"))
        {
            RuntimeCharacter selectedAlly = SuperLibrary.CM.party[(currentpage - 1) * 3 + currentEnemyChoosing];
            Debug.Log("Ally affected: " + selectedAlly.baseData.name);
            SuperLibrary.IS.ConsumeItem(SuperLibrary.CM.party[SuperLibrary.MH.currentcharacterchoosingbuttons], currentItemChoosing+(currentitempage-1)*8, null, selectedAlly);
            return;
        }else if (BAHolder.MainDefiner.Contains("ALLY"))
        {
            ChooseAct(null,SuperLibrary.CM.party[(currentpage - 1) * 3 + currentEnemyChoosing]);
        }else if (BAHolder.MainDefiner.Contains("REROUTE"))
        {
            foreach (BattleAction Ba in ActiveEnemies[(currentpage - 1) * 3 + currentEnemyChoosing].Enemy.RerouteActs)
            {
                if (Ba.whouses[0].characterName == SuperLibrary.CM.party[SuperLibrary.MH.currentcharacterchoosingbuttons].baseData.characterName)
                {
                    SuperLibrary.AM.ChooseAction(SuperLibrary.CM.party[SuperLibrary.MH.currentcharacterchoosingbuttons],Ba,ActiveEnemies[(currentpage - 1) * 3 + currentEnemyChoosing],null,"ActSprite");
                    return;
                }
            }
            SuperLibrary.AM.ChooseAction(SuperLibrary.CM.party[SuperLibrary.MH.currentcharacterchoosingbuttons],null,ActiveEnemies[(currentpage - 1) * 3 + currentEnemyChoosing],null,"ActSprite");
        }
    }
    //------------THERE SHOULD BE A FIGHT LOGIC!!!!!!!!!!!!!!
    //------------Just So you know and dont lose it
    public void DealDamage(int place, RuntimeCharacter attacker,int Damage,float precision)
    {

        int index = ActiveEnemies.FindIndex(x => x.place == place);

        if (index < 0)
        {
            Debug.Log("Enemy not found!");
            return;
        }
        float pr=precision;

        if (attacker != null &&
            attacker.baseData.HoldW != null &&
            attacker.baseData.HoldW.Effect != null)
        {
            pr = attacker.baseData.HoldW.Effect.CritIncrease(attacker, pr);
        }

        ActiveEnemies[index].currenthealth =ActiveEnemies[index].currenthealth-Mathf.RoundToInt(Damage * pr);
        Debug.Log("Damage Dealth:"+Damage * pr);
        Debug.Log(ActiveEnemies[index].currenthealth+":Hp left");
        OnDamageDealt?.Invoke(attacker, ActiveEnemies[index]);
        CheckAliveOrMercy(ActiveEnemies[index]);

    }


    public void CheckAliveOrMercy(EnemyHolder EM)
    {
        int Index=-9999;
        Index=ActiveEnemies.FindIndex(e => e.place ==EM.place);
        if (Index == null || Index<0)
        {
            Debug.Log("Something Happened to enemy");
            return;
        }
        if (ActiveEnemies[Index].currenthealth <= 0)
        {
           BattleEnemyRemove(Index); 
        }
        if (ActiveEnemies[Index].mercybar >= 100)
        {
            MercyEnemyRemove(Index);
        }
        
    }
    public void BattleEnemyRemove(int dude)
    {
        ActiveEnemies.RemoveAt(dude);
        Debug.Log("Enemy Escaped/Died");
    }
    public void MercyEnemyRemove(int dude)
    {
       ActiveEnemies.RemoveAt(dude);
       Debug.Log("Enemy is mercied");
    }
    public bool CheckTP(BattleAction? Ba){
        if (Ba == null)
        {
            return true;
        }
        Debug.Log("Checking TP");
        return TPMeter+Ba.TPCost<0;
    }
    public void ChangeTPNumber()
    {
        if (TPMeter > 100)
        {
            TPMeter=100;
        }else if (TPMeter < 0)
        {
            TPMeter=0;
        }
        CoolTPCounter(TPMeter);
        Vector3 NewPosition=StartPosition/100*(100-TPMeter);
        WhiteBarShower.transform.localPosition=NewPosition;
        MoveYelloWMask(NewPosition);

    }
    public void CoolTPCounter(int TPChange)
    {
       if (CoolTPCounterCoro != null)
        {
            StopCoroutine(CoolTPCounterCoro);
        }
        CoolTPCounterCoro=StartCoroutine(CoolTPCounterStart(TPChange));
    }
    System.Collections.IEnumerator CoolTPCounterStart(int TPchange)
    {
        if (!int.TryParse(Numbertext.text, out int previousTP))
        {
            Numbertext.text="0";
        }
        int PreviousTP=Int32.Parse(Numbertext.text);
        int TPDifference=TPchange-PreviousTP;
        Debug.Log("THE TP DIFFERENCE IS:"+TPDifference);
        const float duration =0.5f;
        float elapsed = 0f;
        float speedmodifier=1;
        while (elapsed < duration)
        {
            if (elapsed < 0.125f*3)
            {
                speedmodifier=0.5f;
            }
            else
            {
                speedmodifier=1.5f;
            }
            elapsed += Time.deltaTime/speedmodifier;
            Numbertext.text=(PreviousTP+Math.Round(TPDifference*(elapsed/duration))).ToString();
            yield return null;
        }
        Numbertext.text=TPchange.ToString();
    }
    public void MoveYelloWMask(Vector3 NP)
    {
       if (yellowBarMovement != null)
       {
           StopCoroutine(yellowBarMovement);
       }

       yellowBarMovement = StartCoroutine(MoveYellowBar(NP));
    }

    System.Collections.IEnumerator MoveYellowBar(Vector3 targetPosition)
    {
        Vector3 startPosition = YellowBarShower.transform.localPosition;
        const float duration = 0.5f;
        float elapsed = 0f;
        float speedmodifier=1;
        while (elapsed < duration)
        {
            if (elapsed < 0.125f*3)
            {
                speedmodifier=0.5f;
            }
            else
            {
                speedmodifier=1.5f;
            }
            elapsed += Time.deltaTime/speedmodifier;
            YellowBarShower.transform.localPosition = Vector3.Lerp(
                startPosition,
                targetPosition,
                elapsed / duration);
            yield return null;
        }

        YellowBarShower.transform.localPosition = targetPosition;
        yellowBarMovement = null;
    }
    public bool CheckParty(BattleAction Ba){
        Debug.Log("Checking Party");
        foreach (Character ch in Ba.whouses){
            RuntimeCharacter target = SuperLibrary.CM.party.Find(m => m.baseData.characterName == ch.characterName);
            if (target.active==true){
                Debug.Log("Someone Is dead:(");
                return false;
            }
        }
        return true;
    }
     //------------THERE SHOULD BE A TWINKLE LOGIC!!!!!!!!!!!!!!
    //------------Just So you know and dont lose it
    void LoadTwinkleHelp(int page)
    {
        currentpageTwinkle=page;
        CEBA.Clear();
        for (int ip=(page-1)*8; ip<(page*8); ip++)
        {
            if (ip <= SuperLibrary.CM.party[SuperLibrary.MH.currentcharacterchoosingbuttons].baseData.HisOwnAction.Length-1)
            {
                CEBA.Add(SuperLibrary.CM.party[SuperLibrary.MH.currentcharacterchoosingbuttons].baseData.HisOwnAction[ip]);
            }
        }
    }

    public void TwinkleHelp()
    {
        Recolor(false);
        currentActChoosing=0;
        SuperLibrary.MH.TurnOffCBB();
        LoadTwinkleHelp(1);
        HideAllButtons();
        ShowAllActs();
        actchoose=true;
        twinklehelp=true;
        enemychoose=false;
        itemchoose=false;
        prevcolorAct=Color.white;
        RecolorAct(true);
    }
    //------------THERE SHOULD BE A DEFEND LOGIC!!!!!!!!!!!!!!
    //------------Just So you know and dont lose it
    public void DefendAdd()
    {
        Debug.Log("ITS DEFENDING!!!");
        SuperLibrary.AM.ChooseAction(SuperLibrary.CM.party[SuperLibrary.MH.currentcharacterchoosingbuttons], Defend, null, null,"DefendSprite");
    }
    public void EmptyTPOnStart()
    {
        TPMeter=0;
    }
    //------------THERE SHOULD BE A ITEM LOGIC!!!!!!!!!!!!!!
    //------------Just So you know and dont lose it
    public void LoadUpItems(int page)
    {
        Debug.Log(page);
        currentitempage=page;
        Debug.Log(currentitempage);
        CEBA.Clear();
        for (int ip=(currentitempage-1)*8; ip<(page*8); ip++)
        {
            if (ip <= SuperLibrary.IS.MainInventory.Count-1)
            {
                Debug.Log(ip);
                CEBA.Add(SuperLibrary.IS.MainInventory[ip]);
            }
        }
    }
    public void ItemsLogic()
    {
        Recolor(false);
        RecolorAct(false);
        currentItemChoosing=0;
        SuperLibrary.MH.TurnOffCBB();
        LoadUpItems(1);
        HideAllButtons();
        ShowAllActs();
        if (CEBA.Count == 0)
        {
            itemchoose=false;
            Debug.Log("FUCK!!!!");
        }
        else
        {
         itemchoose=true;   
        }
        actchoose=false;
        enemychoose=false;
        twinklehelp=false;
        prevcolorAct=Color.white;
        RecolorItem(true);
    }
    public void ChangeICB(int i)
    {
        RecolorItem(false);
        if (i == -1 || i==1)
        {
            if (currentItemChoosing==CEBA.Count-1 && currentItemChoosing % 2 == 0)
            {
                RecolorItem(true);
                Debug.Log(currentItemChoosing+(currentitempage-1)*8);
                DescriptionText.text=CEBA[currentItemChoosing].Description;
                return;
            }
            switch (currentItemChoosing%2)
            {
                case(0):
                    currentItemChoosing=currentItemChoosing+1;
                    break;
                case(1):
                    currentItemChoosing=currentItemChoosing-1;
                    break;
            }
            RecolorItem(true);
            Debug.Log(currentItemChoosing+(currentitempage-1)*8);
            DescriptionText.text=CEBA[currentItemChoosing].Description;
            return;
        }
        if (currentItemChoosing + i < 0)
        {
            if (currentitempage == 1)
            {
                if (SuperLibrary.IS.MainInventory.Count > 8)
                {
                    LoadUpItems(2);
                    ShowAllActs();
                    currentItemChoosing=CEBA.Count-1;
                    RecolorItem(true);
                    Debug.Log(currentItemChoosing+(currentitempage-1)*8);
                    DescriptionText.text=CEBA[currentItemChoosing].Description;
                    return;
                }
                else
                {
                    currentItemChoosing=CEBA.Count-1;
                    RecolorItem(true);
                    Debug.Log(currentItemChoosing+(currentitempage-1)*8);
                    DescriptionText.text=CEBA[currentItemChoosing].Description;
                    return;
                }
            }
            else
            {
                LoadUpItems(1);
                ShowAllActs();
                currentItemChoosing=CEBA.Count-1-((currentItemChoosing+1)%2);
                RecolorItem(true);
                Debug.Log(currentItemChoosing+(currentitempage-1)*8);
                DescriptionText.text=CEBA[currentItemChoosing].Description;
                return;
            }
        }
        if (currentItemChoosing + i > CEBA.Count-1)
        {
            if (currentitempage == 1)
            {
                if (SuperLibrary.IS.MainInventory.Count > 8)
                {
                    LoadUpItems(2);
                    ShowAllActs();
                    currentItemChoosing=0;
                    RecolorItem(true);
                    Debug.Log(currentItemChoosing+(currentitempage-1)*8);
                    DescriptionText.text=CEBA[currentItemChoosing].Description;
                    return;
                }
                else
                {
                    currentItemChoosing=0;
                    RecolorItem(true);
                    Debug.Log(currentItemChoosing+(currentitempage-1)*8);
                    DescriptionText.text=CEBA[currentItemChoosing].Description;
                    return;
                }
            }
            else
            {
                LoadUpItems(1);
                ShowAllActs();
                currentItemChoosing=0;
                RecolorItem(true);
                Debug.Log(currentItemChoosing+(currentitempage-1)*8);
                DescriptionText.text=CEBA[currentItemChoosing].Description;
                return;
            }
        }
        currentItemChoosing+=i;
        RecolorItem(true);
        Debug.Log(currentItemChoosing+(currentitempage-1)*8);
        DescriptionText.text=CEBA[currentItemChoosing].Description;
    }
    public void ChooseItem()
    {   
        BAHolder=CEBA[currentItemChoosing];
        ChooseEnemy(BAHolder,enemholder,CharacterHolder);
    }
    public void StartFightInDebug()
    {
        StartFight(justtest);
    }
}
