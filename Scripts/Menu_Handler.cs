using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEditor.U2D.Aseprite;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using UnityEditor.Experimental.GraphView;
using System;
using UnityEngine.U2D.IK;
using Unity.Burst.CompilerServices;

public class Menu_Handler : MonoBehaviour
{
    Coroutine FadeEverything;
    public Transform canvasTransform;
    public List<GameObject> characterTemplates = new List<GameObject>();
    public SuperLibrary SuperLibrary;
    public BattleAction Test1;
    public BattleAction Test2;
    public List<Transform> SPH=new List<Transform>();
    public GameObject StandartSizeSprite;

    Dictionary<GameObject, List<GameObject>> cardButtons = new Dictionary<GameObject, List<GameObject>>();
    List <GameObject> UpperHalf=new List<GameObject>();
    List<GameObject> HPMeter = new List<GameObject>();
    List<Vector3> StandardSpriteScales = new List<Vector3>();

    public GameObject CharacterUI;

    Color prevcolor;
    List<GameObject> WhichUiExists = new List<GameObject>();
    List<GameObject> MashForUI = new List<GameObject>();
    public GameObject SpawnHolder=null;
    List <(GameObject,GameObject)> Spawner=new List<(GameObject,GameObject)>();
    public List <GameObject> SpawnedObject=new List<GameObject>();
    List<Vector3> Vec3 = new List<Vector3>();
    List<DialogueEntry> test = new List<DialogueEntry>
{
    new DialogueEntry { mainText = "Holy", addTexts = new string[] { " It Works"} },
    new DialogueEntry { mainText = "It Works, Properly", addTexts = new string[0] }
};
    float size;
    float startpoint;
    public int currentcharacterchoosingbuttons=0;
    public int currentbuttonchoosing=0;
    public bool active=false;
    public bool UIactive=false;
    float TextSize;
    public TextMeshProUGUI Buttontext;
    public bool ChoosingBetweenButtons=true;
    public GameObject MainMenuBlackBoxHolder;
    public GameObject AllBoxesHolder;
    public GameObject MovableTPBar;
    Coroutine MoveMainBoxCoro;
    Coroutine MoveTPBarCoro;
    Coroutine UPMovement;
    Coroutine DownMovement;
    Coroutine SlowTextAppear;
    float HolderForSpritex;
    float ChangeSpriteSizex=1;
    float HolderForSpritey;
    float ChangeSpriteSizey=1;
    float MoveBigBox=290f;
    public Sprite AttackSprite;
    public Sprite ActSprite;
    public Sprite ItemSprite;
    public Sprite DefendSprite;
    public Sprite MercySprite;
    public Sprite SleepSprite;
    [Header("Sound")]
    public AudioData ButtonChange;
    public AudioData Enterpress;
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) && UIactive == true && ChoosingBetweenButtons==true)
        {
            ChangeButton(-1);
            SuperLibrary.AudioM.PlaySFX(ButtonChange);
        }
        if (Input.GetKeyDown(KeyCode.D) && UIactive == true && ChoosingBetweenButtons==true)
        {
            ChangeButton(1);
            SuperLibrary.AudioM.PlaySFX(ButtonChange);
        }
        if (Input.GetKeyDown(KeyCode.Return) && UIactive == true && ChoosingBetweenButtons==true)
        {
            SuperLibrary.BTH.HideButton();
            List<RuntimeCharacter> party=SuperLibrary.CM.GetParty();
            SuperLibrary.AudioM.PlaySFX(Enterpress);
            SuperLibrary.BM.YouChooseButton(party[currentcharacterchoosingbuttons],currentbuttonchoosing);
        }
        if (Input.GetKeyDown(KeyCode.Escape) && UIactive == true && !SuperLibrary.EM.CanGoback) 
        {
            SuperLibrary.AudioM.PlaySFX(Enterpress);
            Recolor(false);
            SuperLibrary.AM.Undo();
            ChangeText();
            Recolor(true);
        }
    }

    public void SpawnUI()
    {
        ClearUI();
        List<RuntimeCharacter> party = SuperLibrary.CM.GetParty();

        if (party.Count == 0)
        {
            return;
        }

        switch (party.Count)
        {
            case 1:
                size=0;
                startpoint=0;
                break;
            case 2:
                size=5.6f;
                startpoint=-2.8f;
                break;
            case 3:
                size=5.3f;
                startpoint=-5.3f;
                break;
        }
        int current=0;
        foreach (RuntimeCharacter member in party)
        {
            int index = member.baseData.Number;

            if (index >= 0 && index < characterTemplates.Count)
            {
                GameObject prefabToSpawn = characterTemplates[index];
                GameObject card = Instantiate(prefabToSpawn, canvasTransform);
                card.transform.SetParent(AllBoxesHolder.transform);
                WhichUiExists.Add(card);
                card.transform.position = new Vector3(startpoint+(size*current), -2.18f, 0);

                List<GameObject> buttons = new List<GameObject>();
                foreach (Transform child in card.GetComponentsInChildren<Transform>(true))
                {
                    if (child.CompareTag("MenuButton"))
                    {
                        buttons.Add(child.gameObject);
                    }
                    if (child.CompareTag("UpperPart"))
                    {
                       Debug.Log("Uppy");
                       UpperHalf.Add(child.gameObject);
                       AddSomethingFromChild(card,child.gameObject,member);
                    }
                    if (child.CompareTag("LittleSpawner"))
                    {
                        if (!SpawnHolder)
                        {
                            SpawnHolder=child.gameObject;
                        }
                        else
                        {
                            Spawner.Add((SpawnHolder,child.gameObject));
                            Spawner SP=SpawnHolder.GetComponent<Spawner>();
                            SP.Activate=false;
                            SP.MH=this;
                            SP.Direction="Right";
                            SP=child.gameObject.GetComponent<Spawner>();
                            SP.MH=this;
                            SP.Activate=false;
                            SP.Direction="Left";
                            Debug.Log("Pair Added");
                            SpawnHolder=null;
                        }
                    }

                }
                cardButtons[card] = buttons;
            }
            else
            {
            }
        current++;
        }
        DoeverythingAfterSpawningUI();
    }
    void DoeverythingAfterSpawningUI()
    {
        GoToBottom();
        GoToTheSide();
        SuperLibrary.EM.HideActsButtons();
        SuperLibrary.EM.HideAllButtons();
    }
    void ClearUI()
    {
        WhichUiExists.Clear();
        Spawner.Clear();
        SpawnHolder=null;
        MashForUI.Clear();
        Vec3.Clear();
        cardButtons.Clear();
        SPH.Clear();
        StandardSpriteScales.Clear();
        HPMeter.Clear();
        UpperHalf.Clear();
        for (int i = canvasTransform.childCount - 1; i >= 0; i--)
    {
        Transform child = canvasTransform.GetChild(i);

        if (child.CompareTag("MainMenu"))
        {
            Destroy(child.gameObject);
        }
    }
    }

    public void StartTurn()
    {
        StartCoroutine(EnemyAttackMoveMenu(true));
        AllGoBackToFaceSprite();
        SuperLibrary.BM.FromTheStart();
        SlowlyChangeTextCorStart(true);
        SuperLibrary.CM.ReturnDefenceAll();
        SuperLibrary.BTH.ShowButton();
        ChangeTheStatus(currentcharacterchoosingbuttons,false);
        if (UIactive)
        {
            return;
        }
        UIactive=true;
        if (WhichUiExists.Count==0)
        {
            return;
        }
        if (!active)
        {
            ChangeUIActive();
        }
        //BTH.ChangeInformation(test,null,null,null,null);
        SuperLibrary.BTH.ShowDialogue("Standart_1");
        currentcharacterchoosingbuttons=0;
        int i=0;
        SuperLibrary.CM.CheckEverythingHealth();
        AllBarsGoDown();
        foreach(RuntimeCharacter RC in SuperLibrary.CM.party)
        {
            SuperLibrary.CM.CheckHP(i);
            if (RC.active == true)
            {
                currentcharacterchoosingbuttons=currentcharacterchoosingbuttons+1;
                Debug.Log(RC.currentHP);
                Debug.Log(RC.active);
            }
            else
            {
                break;
            }
            i++;
        }
        TBGU(currentcharacterchoosingbuttons);
        currentbuttonchoosing=0;
        GameObject TargetButton = cardButtons[WhichUiExists[currentcharacterchoosingbuttons]][currentbuttonchoosing];
        SpriteRenderer buttonImage = TargetButton.GetComponent<SpriteRenderer>();
        prevcolor=buttonImage.color;
        buttonImage.color = Color.yellow;
        ChangeTheStatus(currentcharacterchoosingbuttons,true);
        ChangeText();
    }
    public void StartBattle(EnemyGang? Eg)
    {
        if (SuperLibrary.EM.ActiveFight == true)
        {
            return;
        }
        SuperLibrary.CM.Startofthebattle();
        if (Eg == null)
        {
            SuperLibrary.EM.StartFightInDebug();
        }
        else
        {
            SuperLibrary.EM.StartFight(Eg);
        }
        GoToBottom();
        GoToTheSide();
        MoveMainBox(true);
        MoveTPBar(true);
        SlowlyChangeTextCorStart(true);
    }
    public void ChangeButton(int step)
    {
        Recolor(false);
        if (currentbuttonchoosing + step < 0)
        {
            currentbuttonchoosing=cardButtons[WhichUiExists[currentcharacterchoosingbuttons]].Count-1;
        }else if (currentbuttonchoosing + step>=cardButtons[WhichUiExists[currentcharacterchoosingbuttons]].Count)
        {
            currentbuttonchoosing=0;
        }
        else
        {
            currentbuttonchoosing=currentbuttonchoosing+step;
        }
        Recolor(true);
        ChangeText();
    }
    public void NextCharacter()
{
    SuperLibrary.EM.ActionHistory.Clear();
    SuperLibrary.EM.UpdateCanGoBack();
    SuperLibrary.EM.EraseText();
    SuperLibrary.EM.FromTheStart();
    ChangeTheStatus(currentcharacterchoosingbuttons,false);
    Recolor(false);
    int nextIndex = currentcharacterchoosingbuttons + 1;
    Debug.Log("That Bar should Go down");
    TBGD(currentcharacterchoosingbuttons);
    while (nextIndex < WhichUiExists.Count && SuperLibrary.CM.party[nextIndex].active == true)
    {
        Debug.Log(SuperLibrary.CM.party[nextIndex].currentHP);
        Debug.Log(SuperLibrary.CM.party[nextIndex].baseData.characterName);
        nextIndex++;
    }

    if (nextIndex >= WhichUiExists.Count)
    {
        AllBarsGoDown();
        Debug.Log("It Got here");
        EndOfYourTurn();
        StartCoroutine(ExecuteTurnAndEnd());
        return;
    }
    else
    {
        currentcharacterchoosingbuttons = nextIndex;
        Debug.Log("That Bar should go UP!!!!");
        TBGU(currentcharacterchoosingbuttons);
        currentbuttonchoosing = 0;
        Recolor(true);
    }
    ChangeTheStatus(currentcharacterchoosingbuttons,true);
    ChangeText();
    SuperLibrary.EM.actchoose=false;
}

    IEnumerator ExecuteTurnAndEnd()
    {
        yield return StartCoroutine(SuperLibrary.AM.ExecuteTurn());
    }

    public void EndOfYourTurn()
    {
        UIactive=false;
        foreach (GameObject GO in SpawnedObject)
        {
        Destroy(GO);          
        }
        SpawnedObject.Clear();
        SlowlyChangeTextCorStart(false);

    }
    public void ChangeButtonText(bool switcher)
    {
        Buttontext.enabled=switcher;
    }
    public void SlowlyChangeTextCorStart(bool switcher)
    {
        if (SlowTextAppear != null)
        {
            StopCoroutine(SlowTextAppear);
        }
        ChangeButtonText(true);
        SlowTextAppear=StartCoroutine(SlowTextCor(switcher));
    }

    IEnumerator SlowTextCor(bool switcher)
{
    const float duration = 0.4f;
    float elapsed = 0f;
    float startnumber;
    float endnumber;

    if (switcher)
    {
        startnumber = 0f;
        endnumber = 1f;
    }
    else
    {
        startnumber = 1f;
        endnumber = 0f;
    }

    while (elapsed < duration)
    {
        elapsed+=Time.deltaTime;
        float t = elapsed / duration;
        Color color = Buttontext.color;
        color.a = Mathf.Lerp(startnumber, endnumber, t);
        Buttontext.color = color;

        elapsed += Time.deltaTime;
        yield return null;
    }

    Color finalColor = Buttontext.color;
    finalColor.a = endnumber;
    Buttontext.color = finalColor;
    ChangeButtonText(switcher);
}

    public void Recolor(bool cl)
    {
        GameObject TargetButton = cardButtons[WhichUiExists[currentcharacterchoosingbuttons]][currentbuttonchoosing];
        SpriteRenderer buttonImage = TargetButton.GetComponent<SpriteRenderer>();
        switch (cl)
        {
            case true:
                prevcolor=buttonImage.color;
                buttonImage.color = Color.yellow;
                break;
            case false:
                buttonImage.color=prevcolor;
                break;
        }
    }

    public void ChangeUIActive()
    {
        active=!active;
        CharacterUI.SetActive(active);
    }

    void ChangeText()
    {
        List<RuntimeCharacter> party = SuperLibrary.CM.GetParty();
        name=party[currentcharacterchoosingbuttons].baseData.characterName;
        Buttontext.text=FindCorrectText(name);
        Buttontext.fontSize=TextSize;
        float getx=cardButtons[WhichUiExists[currentcharacterchoosingbuttons]][currentbuttonchoosing].transform.position.x;
        float gety=cardButtons[WhichUiExists[currentcharacterchoosingbuttons]][currentbuttonchoosing].transform.position.y;
        float getz=cardButtons[WhichUiExists[currentcharacterchoosingbuttons]][currentbuttonchoosing].transform.position.z;
        Buttontext.transform.position=new Vector3(getx+0.01f, gety-0.6f, getz);
    }
    string FindCorrectText(string name)
    {
        string returnname="fart";
        switch (name)
        {
            case "Alaska":
                switch (currentbuttonchoosing)
                {
                    case 0:
                        returnname="FIGHT";TextSize=23;break;
                    case 1:
                        returnname="TWINKLE\nHELP";TextSize=16;break;
                    case 2:
                        returnname="ACT";TextSize=32;break;
                    case 3:
                        returnname="ITEM";TextSize=28;break;
                    case 4:
                        returnname="SPARE";TextSize=21;break;
                    case 5:
                        returnname="DEFEND";TextSize=19;break;
                }
                break;
            default:
            {
                switch (currentbuttonchoosing)
                    {
                        case 0:
                            returnname="FIGHT";TextSize=23;break;
                        case 1:
                            returnname="ACT";TextSize=32;break;
                        case 2:
                            returnname="ITEM";TextSize=28;break;
                        case 3:
                            returnname="SPARE";TextSize=21;break;
                        case 4:
                            returnname="DEFEND";TextSize=19;break;
                    }
                    break;
                }
        }
        return returnname;
    }
    public void TurnOffCBB()
    {
        ChoosingBetweenButtons=false;
    }
    public void TurnOnCBB()
    {
        ChoosingBetweenButtons=true;
    }
    ///Shit i add while im doing something
    public void AddSomethingFromChild(GameObject card,GameObject Child,RuntimeCharacter RC)
    {
        foreach(Transform cc in Child.GetComponentsInChildren<Transform>(true))
        {
            if (cc.CompareTag("HPMeter"))
            {
                HPMeter.Add(cc.gameObject);
            }
            if (cc.CompareTag("FaceSprite"))
            {
                Debug.Log("Found Face :3");
                SpriteRenderer sp = cc.GetComponent<SpriteRenderer>();
                Vector2 oldSize = sp.sprite.bounds.size;
                sp.sprite =RC.baseData.Face_Sprite;
                Vector2 newSize = sp.sprite.bounds.size;
                if (newSize.x > 0 && newSize.y > 0)
                {
                    cc.localScale = new Vector3(
                        cc.localScale.x * oldSize.x / newSize.x,
                        cc.localScale.y * oldSize.y / newSize.y,
                        cc.localScale.z);
                }
                SPH.Add(cc);
                StandardSpriteScales.Add(cc.localScale);
            }
            if (cc.CompareTag("HealthBar"))
            {
                Debug.Log("Mask Added");
                MashForUI.Add(cc.gameObject);
                Vec3.Add(cc.localPosition);
            }
        }
    }
    public void ChangeHPAmount(int place,RuntimeCharacter CH,int DamageTaker)
    {
        TextMeshProUGUI hptext=HPMeter[place].GetComponent<TextMeshProUGUI>();
        string HpText=CH.currentHP.ToString();
        HPMeter[place].transform.localPosition=new Vector3(0.95f,-0.03f, 90);
        switch (HpText.Length)
        {
            case(1):
                HpText="  "+HpText;
                break;
            case(2):
                HpText=" "+HpText;
                break;
            case(4):
                HPMeter[place].transform.localPosition=new Vector3(
                    HPMeter[place].transform.localPosition.x-0.1f,
                    HPMeter[place].transform.localPosition.y,
                    HPMeter[place].transform.localPosition.z);
                break;
            default:
                break;
        }
        hptext.text=HpText+"/"+CH.baseData.maxHp;
        if (CH.currentHP <= 0)
        {
            hptext.color=Color.red;
        }
    }
    public void MoveHPbar(RuntimeCharacter RC){
        int ndex = SuperLibrary.CM.party.FindIndex(m => m.baseData.characterName == RC.baseData.characterName);
        float MVBX=1f-(float)RC.currentHP/RC.baseData.maxHp;
        if (MVBX>1){
            MVBX=1;
        }
        MashForUI[ndex].transform.localPosition=new Vector3(
                    Vec3[ndex].x-(0.78f*MVBX),
                    Vec3[ndex].y,
                    Vec3[ndex].z);

    }
    public void ChangeSprite(RuntimeCharacter RC,string Rule)
    {
        int ndex = SuperLibrary.CM.party.FindIndex(m => m.baseData.characterName == RC.baseData.characterName);
        if (ndex < 0 || ndex >= SPH.Count || ndex >= StandardSpriteScales.Count)
        {
            return;
        }

        SpriteRenderer sp = SPH[ndex].GetComponent<SpriteRenderer>();
        Vector2 standardSize = RC.baseData.Face_Sprite.bounds.size;
        HolderForSpritex = 1;
        HolderForSpritey = 1;
        if (Rule == "StandartFace")
        {
            sp.sprite =RC.baseData.Face_Sprite;
            sp.color=Color.white;
        }
        if (Rule == "AttackSprite")
        {
            HolderForSpritex=1.2f;
            HolderForSpritey=0.84f;
            sp.sprite=AttackSprite;
            }
        if (Rule == "ActSprite")
        {
            HolderForSpritex=1.2f;
            HolderForSpritey=0.84f;
            sp.sprite=ActSprite;
            }
        if (Rule == "ItemSprite")
        {
            HolderForSpritex=1.2f;
            HolderForSpritey=0.84f;
            sp.sprite=ItemSprite;
            }
        if (Rule == "DefendSprite")
        {
            HolderForSpritex=1.2f;
            HolderForSpritey=0.84f;
            sp.sprite=DefendSprite;
            }
        if (Rule == "MercySprite")
        {
            HolderForSpritex=1.2f;
            HolderForSpritey=0.84f;
            sp.sprite=MercySprite;
            }
        if (Rule == "SleepSprite")
        {
            HolderForSpritex=1.2f;
            HolderForSpritey=0.84f;
            sp.sprite=SleepSprite;
            }

        if (sp.sprite == null)
        {
            return;
        }
        if (Rule != "StandartFace")
        {
            foreach (Transform child in WhichUiExists[ndex].GetComponentsInChildren<Transform>(true)){
                if (child.CompareTag("UpperPart"))
                    {
                         foreach (Transform child2 in child.GetComponentsInChildren<Transform>(true)){
                            if(child2.CompareTag("Color")){
                                SpriteRenderer Holdit=child2.GetComponent<SpriteRenderer>();
                                sp.color=Holdit.color;
                            }
                         }
                    }
            }
        }

        Vector2 newSize = sp.sprite.bounds.size;
        Vector3 standardScale = StandardSpriteScales[ndex];
        SPH[ndex].localScale = new Vector3(
            standardScale.x * HolderForSpritex,
            standardScale.y * HolderForSpritey,
            standardScale.z);

        if (Rule != "StandartFace" && newSize.x > 0 && newSize.y > 0)
        {
            SPH[ndex].localScale = new Vector3(
                SPH[ndex].localScale.x * standardSize.x / newSize.x / ChangeSpriteSizex,
                SPH[ndex].localScale.y * standardSize.y / newSize.y / ChangeSpriteSizey,
                SPH[ndex].localScale.z);
        }
    }
    public void AllGoBackToFaceSprite(){
        foreach(RuntimeCharacter RC in SuperLibrary.CM.party){
            ChangeSprite(RC,"StandartFace");
        }
    }
    public void GoToBottom()
    {
        MainMenuBlackBoxHolder.transform.localPosition=new Vector3 (
            MainMenuBlackBoxHolder.transform.localPosition.x,
            MainMenuBlackBoxHolder.transform.localPosition.y-300,
            MainMenuBlackBoxHolder.transform.localPosition.z
        );
    }
    public void MoveMainBox(bool point)
    {
        if (MoveMainBoxCoro != null)
        {
            StopCoroutine(MoveMainBoxCoro);
        }
        MoveMainBoxCoro=StartCoroutine(MoveMainBoxStart(point));
    }

    System.Collections.IEnumerator MoveMainBoxStart(bool point)
    {
        float yendpoint=MoveBigBox;
        if (!point)
        {
            yendpoint=0;
        }
        Vector3 StartingPosition=MainMenuBlackBoxHolder.transform.localPosition;
        Vector3 EndPosition=new Vector3 (
            40.26f,
            -538.2233f+yendpoint,
            -538.2233f
        );
        const float duration = 0.3f;
        float elapsed = 0f;
        float speedmodifier=1;
        while (elapsed < duration)
        {
            if (elapsed < duration / 4 * 3)
            {
                speedmodifier=0.4f;
            }
            else
            {
                speedmodifier=1.25f;
            }
            elapsed += Time.deltaTime/speedmodifier;
            MainMenuBlackBoxHolder.transform.localPosition=Vector3.Lerp(
                StartingPosition,
                EndPosition,
                elapsed / duration);
            yield return null;
        }
        MainMenuBlackBoxHolder.transform.localPosition=EndPosition;
        MoveMainBoxCoro=null;
    }
    public IEnumerator EnemyAttackMoveMenu(bool point)
    {
        float yendpoint=100f;
        if (!point)
        {
            yendpoint=-1*yendpoint;
        }
        Vector3 StartingPosition=MainMenuBlackBoxHolder.transform.localPosition;
        Vector3 EndPosition=new Vector3 (
            MainMenuBlackBoxHolder.transform.localPosition.x,
            MainMenuBlackBoxHolder.transform.localPosition.y+yendpoint,
            MainMenuBlackBoxHolder.transform.localPosition.z
        );
        const float duration = 0.3f;
        float elapsed = 0f;
        float speedmodifier=1;
        while (elapsed < duration)
        {
            if (elapsed < duration / 4 * 3)
            {
                speedmodifier=0.4f;
            }
            else
            {
                speedmodifier=1.25f;
            }
            elapsed += Time.deltaTime/speedmodifier;
            MainMenuBlackBoxHolder.transform.localPosition=Vector3.Lerp(
                StartingPosition,
                EndPosition,
                elapsed / duration);
            yield return null;
        }
        MainMenuBlackBoxHolder.transform.localPosition=EndPosition;
        
    }
    public void GoToTheSide()
    {
        MovableTPBar.transform.localPosition=new Vector3 (
            -456.4641f,
            78.07851f,
            -6606f
        );
    }
    public void MoveTPBar(bool point)
    {
        if (MoveTPBarCoro != null)
        {
            StopCoroutine(MoveTPBarCoro);
        }
        StartCoroutine(MoveTPBarStart(point));
    }
    System.Collections.IEnumerator MoveTPBarStart(bool point)
    {
        float xendpoint=120f;
        if (!point)
        {
            xendpoint=-120f;
        }
        Vector3 StartingPosition=MovableTPBar.transform.localPosition;
        Vector3 EndPosition=new Vector3 (
            MovableTPBar.transform.localPosition.x+xendpoint,
            MovableTPBar.transform.localPosition.y,
            MovableTPBar.transform.localPosition.z
        );
        const float duration = 0.6f;
        float elapsed = 0f;
        float speedmodifier=1;
        while (elapsed < duration)
        {
            if (elapsed < duration / 4 * 3)
            {
                speedmodifier=0.25f;
            }
            else
            {
                speedmodifier=1.5f;
            }
            elapsed += Time.deltaTime/speedmodifier;
            MovableTPBar.transform.localPosition=Vector3.Lerp(
                StartingPosition,
                EndPosition,
                elapsed / duration);
            yield return null;
        }
        MovableTPBar.transform.localPosition=EndPosition;
        MoveTPBarCoro=null;
    }
    public void TBGD(int i)
    {
        if (DownMovement != null)
        {
            StopCoroutine(DownMovement);
        }
        DownMovement=StartCoroutine(ThatBarGoesDOWN(i));
    }
    public void TBGU(int i)
    {
        if (UPMovement != null)
        {
            StopCoroutine(UPMovement);
        }
        UPMovement=StartCoroutine(ThatBarGoesUP(i));
    }
    public void AllBarsGoDown()
    {
        foreach (GameObject GO in UpperHalf)
        {
            GO.transform.localPosition=new Vector3(
                GO.transform.localPosition.x,
                0,
                GO.transform.localPosition.z
            );
        }
    }
    IEnumerator ThatBarGoesUP(int i)
    {
        Vector3 NewPosition=new Vector3(
            UpperHalf[i].transform.localPosition.x,
            0.33f,
            UpperHalf[i].transform.localPosition.z
        );
        Vector3 OldPosition=UpperHalf[i].transform.localPosition;
        const float duration = 0.5f;
        float elapsed = 0f;
        float speedmodifier=1;
        while (elapsed < duration)
        {
            if (elapsed < duration / 4 * 3)
            {
                speedmodifier=0.25f;
            }
            else
            {
                speedmodifier=1.5f;
            }
            elapsed += Time.deltaTime/speedmodifier;
            UpperHalf[i].transform.localPosition=Vector3.Lerp(
                OldPosition,
                NewPosition,
                elapsed / duration);
            yield return null;
        }
        UpperHalf[i].transform.localPosition=NewPosition;
        DownMovement=null;
    }
    IEnumerator ThatBarGoesDOWN(int i)
    {
        Vector3 NewPosition=new Vector3(
            UpperHalf[i].transform.localPosition.x,
            0f,
            UpperHalf[i].transform.localPosition.z
        );
        Vector3 OldPosition=UpperHalf[i].transform.localPosition;
        const float duration = 0.33f;
        float elapsed = 0f;
        float speedmodifier=1;
        while (elapsed < duration)
        {
            if (elapsed < duration / 4 * 3)
            {
                speedmodifier=0.25f;
            }
            else
            {
                speedmodifier=1.5f;
            }
            elapsed += Time.deltaTime/speedmodifier;
            UpperHalf[i].transform.localPosition=Vector3.Lerp(
                OldPosition,
                NewPosition,
                elapsed / duration);
            yield return null;
        }
        UpperHalf[i].transform.localPosition=NewPosition;
        UPMovement=null;
    }
    public void ChangeTheStatus(int Index, bool TOF)
    {
        if (Index < 0 || Index >= Spawner.Count)
        {
            return;
        }

        (GameObject first, GameObject second) pair = Spawner[Index];
        Spawner firstSpawner = pair.first.GetComponent<Spawner>();
        Spawner secondSpawner = pair.second.GetComponent<Spawner>();

        if (firstSpawner != null)
        {
            firstSpawner.Activate = TOF;
        }

        if (secondSpawner != null)
        {
            secondSpawner.Activate = TOF;
        }
    }
    public void allThingHasToShutDown()
    {
        int i=0;
        foreach(GameObject GO in WhichUiExists)
        {
            TBGD(i);
            ChangeTheStatus(i,false);
        }
        AllBarsGoDown();
        MoveMainBox(false);
        MoveTPBar(false);
    }
    public void DestroyStartCoro(){
 if (FadeEverything != null)
        {
            StopCoroutine(FadeEverything);
        }
        StartCoroutine(WaitJustALittle());
}
System.Collections.IEnumerator DestroyStart(){
    const float duration =0.4f;
    float elapsed = 0f;
    float speed=1f;
    while (elapsed < duration)
        {   
            if (elapsed<duration/4*3){
                speed=3f;
            }else{
                speed=0.5f;
            }
            elapsed += Time.deltaTime*speed;
            foreach(GameObject spawned in SuperLibrary.AMG.Copies)
            {
                SpriteRenderer SP=spawned.GetComponent<SpriteRenderer>();
                Color cl=SP.color;
                cl.a=1f * (1f - elapsed / duration);
                spawned.transform.localPosition=new Vector3(
                    spawned.transform.localPosition.x-(0.015f),
                    spawned.transform.localPosition.y,
                    spawned.transform.localPosition.z
                );
                SP.color=cl;
            }
            yield return null;
        }
    SuperLibrary.AMG.DestroyCopies();
    SuperLibrary.AMG.Activitism=false;
}
public System.Collections.IEnumerator WaitJustALittleAndBattleStart(){
    const float duration =0.25f;
    float elapsed = 0f;
    while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
    StartCoroutine(EnemyAttackMoveMenu(false));
}
System.Collections.IEnumerator WaitJustALittle(){
    const float duration =1.5f;
    float elapsed = 0f;
    while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
    FadeEverything=StartCoroutine(DestroyStart());
}
    }
    

