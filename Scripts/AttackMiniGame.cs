using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackMiniGame : MonoBehaviour
{
    public SuperLibrary SuperLibrary;
    public GameObject FightBar;
    public Sprite StartSptie;

    public List<FightMinigameInfoHolder> FMIH = new List<FightMinigameInfoHolder>();

    public List<GameObject> ObjectHolder = new List<GameObject>();
    public List<GameObject> ThatLittleSquare = new List<GameObject>();
    public List<GameObject> ThatBigSquare = new List<GameObject>();
    public List<GameObject> ThatBar = new List<GameObject>();
    public List<GameObject> LittleFaces = new List<GameObject>();
    private int frameSnapshotValue;
    private int frameSnapshotFrame = -1;
    public bool Activitism=false;

    public GameObject HoldAObject;

    public GameObject ToWhatAttach;
    float stepran=1.5f;
    float[] options;
    void Awake()
    {
        options = new float[] {0,stepran,stepran*2,stepran*3 };
    }

    public int counternumber=1;
    public Dictionary<int, int> RemainingPerTier = new Dictionary<int, int>();
    public List<GameObject> AllSpawnedCharacters = new List<GameObject>();
    public List<GameObject> Copies = new List<GameObject>();
    public AudioData AttackSound;
    public void FMIHPrint()
{
    Activitism=true;
    if (FMIH.Count<=0){
        Activitism=false;
    }
    RemainingPerTier.Clear();
    SuperLibrary.BTH.HideButton();
    int i=0;
    int minTier = int.MaxValue; 

    foreach (FightMinigameInfoHolder FM in FMIH)
    {
        foreach (GameObject go in ObjectHolder)
        {
            WhichCharacter wc = go.GetComponent<WhichCharacter>();
            if (FM.Chara.baseData == wc.Ch)
            {
                HoldAObject = go;
                break;
            }
        }

        int index = SuperLibrary.CM.party.FindIndex(rc => rc == FM.Chara);

        GameObject Spawned = Instantiate(HoldAObject, ToWhatAttach.transform);
        AllSpawnedCharacters.Add(Spawned);
        GameObject SpawnedBar = Instantiate(FightBar, ToWhatAttach.transform);
        GameObject faceObject = new GameObject("CharacterFace");
        faceObject.transform.SetParent(ToWhatAttach.transform, false);
        SpriteRenderer faceRenderer = faceObject.AddComponent<SpriteRenderer>();
        faceRenderer.sprite = StartSptie;
        faceRenderer.transform.localScale=faceRenderer.transform.localScale*0.7f;
        Vector3 oldSize=faceRenderer.sprite.bounds.size;
        faceRenderer.sprite = FM.Chara.baseData.Face_Sprite;
        Vector3 newSize=faceRenderer.sprite.bounds.size;
        faceRenderer.sortingOrder = 10;
        faceObject.transform.localScale = new Vector3(
                        faceObject.transform.localScale.x * oldSize.x / newSize.x,
                        faceObject.transform.localScale.y * oldSize.y / newSize.y,
                        faceObject.transform.localScale.z);
        AllSpawnedCharacters.Add(faceObject);
        MoveThisObject moveScript = SpawnedBar.GetComponent<MoveThisObject>();
        float generate = options[UnityEngine.Random.Range(0, options.Length)];
        int tier = Mathf.RoundToInt(generate / stepran);

        if (moveScript != null)
        {
            moveScript.SuperLibrary= SuperLibrary;
            moveScript.numba = i;
            moveScript.realnumber = tier;
            moveScript.AttackSound=AttackSound;
        }

        if (!RemainingPerTier.ContainsKey(tier)) RemainingPerTier[tier] = 0;
        RemainingPerTier[tier]++;

        if (tier < minTier) minTier = tier;

        Spawned.transform.localPosition = new Vector3(0, index * -0.81f, 0);
        faceObject.transform.localPosition=new Vector3(-2.3f,-0.85f +index * -0.81f, 0);
        SpawnedBar.transform.position = new Vector3(Spawned.transform.position.x+stepran*3 + generate, Spawned.transform.position.y - 0.86f, 0);

        GameObject square = Spawned.transform.Find("MainBattleBox2").gameObject;
        GameObject square2 = Spawned.transform.Find("MainBattleBox1").gameObject;

        ThatLittleSquare.Add(square);
        ThatBigSquare.Add(square2);
        ThatBar.Add(SpawnedBar);
        LittleFaces.Add(faceObject);

        i++;
    }

    counternumber = minTier;
}

    public bool TryNextCounter(GameObject go)
{
    if (Time.frameCount != frameSnapshotFrame)
    {
        frameSnapshotFrame = Time.frameCount;
        frameSnapshotValue = counternumber;
    }

    int R = go.GetComponent<MoveThisObject>().realnumber;
    if (R != frameSnapshotValue)
    {
        return false;
    }
    return true;
}

public void AdvanceToNextTier()
{
    int next = int.MaxValue;
    foreach (var kvp in RemainingPerTier)
    {
        if (kvp.Value > 0 && kvp.Key > counternumber && kvp.Key < next)
        {
            next = kvp.Key;
        }
    }

    counternumber = next;
}
public void DestroyCopies(){
    foreach(GameObject spawned in Copies){
        Destroy(spawned);
    }
    Copies.Clear();
}
}
