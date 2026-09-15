using System;
using Unity.Mathematics;
using UnityEngine;
using System.Collections.Generic;

public class MoveThisObject : MonoBehaviour
{
    public SuperLibrary SuperLibrary;
    public AfterImage AF;
    public int numba;
    public int realnumber=-1;
    public float Times=0.2f;
     Bounds bigBounds;
     Bounds barBounds;
     Bounds ExactSpot;
     bool xOverlap;
     bool yOverlap;
     bool intersects2D;
     float Distance;
     [Header("Sound")]
    public AudioData AttackSound;



void FixedUpdate()
    {
        SuperLibrary.AMG.ThatBar[numba].transform.localPosition = new Vector3(
            SuperLibrary.AMG.ThatBar[numba].transform.localPosition.x - 0.12f,
            SuperLibrary.AMG.ThatBar[numba].transform.localPosition.y,
            SuperLibrary.AMG.ThatBar[numba].transform.position.z
        );
        SuperLibrary.AMG.ThatBar[numba].transform.position = new Vector3(
            SuperLibrary.AMG.ThatBar[numba].transform.position.x,
            SuperLibrary.AMG.ThatBar[numba].transform.position.y,
            0
        );
        bigBounds = SuperLibrary.AMG.ThatBigSquare[numba].GetComponent<Collider2D>().bounds;
        barBounds = SuperLibrary.AMG.ThatBar[numba].GetComponent<Collider2D>().bounds;
        ExactSpot = SuperLibrary.AMG.ThatLittleSquare[numba].GetComponent<Collider2D>().bounds;
        Distance=barBounds.center.x-ExactSpot.center.x;
        xOverlap =barBounds.min.x <= bigBounds.max.x &&barBounds.max.x >= bigBounds.min.x;
        yOverlap =barBounds.min.y <= bigBounds.max.y &&barBounds.max.y >= bigBounds.min.y;
        intersects2D =xOverlap && yOverlap;
        Distance=barBounds.center.x-ExactSpot.center.x;
        
    }
    void Update()
{
    AfterImageSpawn();
    if (Distance <= -0.5)
        {
            FullyDestroy();
        }
    if (Input.GetKeyDown(KeyCode.Return)||Input.GetKeyDown(KeyCode.Space))
    {
        if (intersects2D)
        {
                if (!SuperLibrary.AMG.TryNextCounter(SuperLibrary.AMG.ThatBar[numba]))
                {
                    return;
                }
            int intu=SuperLibrary.AMG.FMIH[numba].EnemyH.place;
            int Damage=CalculateDamage(SuperLibrary.AMG.FMIH[numba].Chara);
            SuperLibrary.EM.DealDamage(intu,SuperLibrary.AMG.FMIH[numba].Chara,Damage,PrecisionDecision(Distance));
            SuperLibrary.AudioM.PlaySFX(AttackSound);
            FullyDestroy();
            return;
        }
        else
        {
            //Debug.Log("NO!!!");
        }
    }
}
    void FullyDestroy(){
    int tier = realnumber;

    GameObject bar = SuperLibrary.AMG.ThatBar[numba];
    GameObject copy1 = Instantiate(SuperLibrary.AMG.ThatBigSquare[numba], SuperLibrary.AMG.ThatBigSquare[numba].transform.position, SuperLibrary.AMG.ThatBigSquare[numba].transform.rotation, SuperLibrary.AMG.ToWhatAttach.transform);
    GameObject copy2 = Instantiate(SuperLibrary.AMG.ThatLittleSquare[numba], SuperLibrary.AMG.ThatLittleSquare[numba].transform.position, SuperLibrary.AMG.ThatLittleSquare[numba].transform.rotation, SuperLibrary.AMG.ToWhatAttach.transform);
    GameObject copy3 = Instantiate(SuperLibrary.AMG.LittleFaces[numba], SuperLibrary.AMG.LittleFaces[numba].transform.position, SuperLibrary.AMG.LittleFaces[numba].transform.rotation, SuperLibrary.AMG.ToWhatAttach.transform);
    SuperLibrary.AMG.Copies.Add(copy1);
    SuperLibrary.AMG.Copies.Add(copy2);
    SuperLibrary.AMG.Copies.Add(copy3);
    Destroy(bar);
    SuperLibrary.AMG.ThatBar.RemoveAt(numba);
    SuperLibrary.AMG.ThatBigSquare.RemoveAt(numba);
    SuperLibrary.AMG.ThatLittleSquare.RemoveAt(numba);
    SuperLibrary.AMG.LittleFaces.RemoveAt(numba);
    SuperLibrary.AMG.FMIH.RemoveAt(numba);
    StartIncreaseAndDespawn();

    for (int i = numba; i < SuperLibrary.AMG.ThatBar.Count; i++)
    {
        MoveThisObject move = SuperLibrary.AMG.ThatBar[i].GetComponent<MoveThisObject>();
        if (move != null)
        {
            move.numba = i;
        }
    }

        SuperLibrary.AMG.RemainingPerTier[tier]--;
    if (SuperLibrary.AMG.RemainingPerTier[tier] <= 0){
        SuperLibrary.AMG.AdvanceToNextTier();
    }
    if (SuperLibrary.AMG.ThatBar.Count == 0)
        {
            Debug.Log("Fucking Empty");
            DoSomethingWhenEmpty();
            SuperLibrary.MH.DestroyStartCoro();
        }
        enabled = false;
    }
    int CalculateDamage(RuntimeCharacter Ch)
    {
        int calcdamag;
        if (!Ch.baseData.HoldW)
        {
            Debug.Log(Ch.baseData.StandartAttack);
            calcdamag=Ch.baseData.StandartAttack;
        }
        else
        {
            Debug.Log("He holds");
            calcdamag=Ch.baseData.StandartAttack+Ch.baseData.HoldW.Damage;
        }
        return calcdamag;
    }
    float PrecisionDecision(float precision)
    {
        precision=Mathf.Round(precision * 10f) / 10f;
        if (precision < -0.2)
        {
            return 0.7f;
        }else if(precision <= 0.2)
        {
            return 1.5f;
        }else if(precision < 1f)
        {
            return 1f;
        }else if (precision < 3f)
        {
            return Remap(precision,1.1f,3f,0.9f,0.6f);
        }

        return precision;
    }

    void DoSomethingWhenEmpty()
{
    foreach (GameObject spawned in SuperLibrary.AMG.AllSpawnedCharacters)
    {
        if (spawned != null)
        {
            Destroy(spawned); // also destroys its child squares automatically
        }
    }
    SuperLibrary.AMG.AllSpawnedCharacters.Clear();

    SuperLibrary.AMG.FMIH.Clear();
    SuperLibrary.AMG.HoldAObject = null;
    SuperLibrary.AMG.RemainingPerTier.Clear();
}

    public void Init()
    {
        bigBounds = SuperLibrary.AMG.ThatBigSquare[numba].GetComponent<Collider2D>().bounds;
        barBounds = SuperLibrary.AMG.ThatBar[numba].GetComponent<Collider2D>().bounds;
        ExactSpot = SuperLibrary.AMG.ThatLittleSquare[numba].GetComponent<Collider2D>().bounds;
        xOverlap =barBounds.min.x <= bigBounds.max.x &&barBounds.max.x >= bigBounds.min.x;
        yOverlap =barBounds.min.y <= bigBounds.max.y &&barBounds.max.y >= bigBounds.min.y;
        intersects2D =xOverlap && yOverlap;
        Distance=barBounds.center.x-ExactSpot.center.x;
    }
    float Remap(float value, float inMin, float inMax, float outMin, float outMax)
{
    float t = (value - inMin) / (inMax - inMin);
    return outMin + t * (outMax - outMin);
}
    public void AfterImageSpawn()
    {
        Times=Times+Time.deltaTime;
        if (Times>=0.2f){
        GameObject AMC = Instantiate(gameObject, SuperLibrary.AMG.ToWhatAttach.transform);
        Destroy(AMC.GetComponent<MoveThisObject>());
        AfterImage afterImage = AMC.AddComponent<AfterImage>();
        afterImage.SetStart(1f,0.75f,0.1f);
        Times=0f;
        }
    }
    public void StartIncreaseAndDespawn(){
        GameObject AMC = Instantiate(gameObject, SuperLibrary.AMG.ToWhatAttach.transform);
        Destroy(AMC.GetComponent<MoveThisObject>());
        AfterImage afterImage = AMC.AddComponent<AfterImage>();
        afterImage.SetStart(0.2f,2f,0.5f);
    }
}
