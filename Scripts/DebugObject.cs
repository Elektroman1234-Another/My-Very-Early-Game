using UnityEditor.U2D.Aseprite;
using UnityEngine;


public class DebugObject : MonoBehaviour
{
    public SuperLibrary SuperLibrary;
    bool DebugMode=true;
    public Character Alaska;
    public Character Dummy;
    public Character Dummy2;
    public Character Dummy3;
    public GameObject AlaskaCharacter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (DebugMode == true)
        {
            Debug.Log("Дебаг запущен");
            SpawnCharacter(AlaskaCharacter);
            SuperLibrary.CM.AddToParty(Alaska);
            SuperLibrary.MH.SpawnUI();
            SuperLibrary.CM.DamageCharacter("Alaska",90);

        }

    }

    // Update is called once per frame
    void Update()
    {
        if (DebugMode == false)
        {
            return;
        }
        if (Input.GetKeyDown("[1]"))
        {
            AddAlaskaIntoParty();
            Debug.Log("Alaska Added");
        }
        if (Input.GetKeyDown("[2]"))
        {
            Debug.Log("Party Info;");
            CheckParty();
        }
        if (Input.GetKeyDown("[3]"))
        {
            DealDamage();
        }
        if (Input.GetKeyDown("[4]"))
        {
            SpawnUI();

        }
        if (Input.GetKeyDown("[5]"))
        {
            SuperLibrary.MH.StartTurn();
        }
        if (Input.GetKeyDown("[6]"))
        {
            SuperLibrary.MH.ChangeUIActive();
        }
        if (Input.GetKeyDown("[7]"))
        {
            SuperLibrary.MH.StartBattle(null);
        }
        if (Input.GetKeyDown("[9]"))
        {
            SuperLibrary.EM.EndFight();
        }
    }

    void AddAlaskaIntoParty()
    {
        if (DebugMode == false)
        {
            return;
        }
        SuperLibrary.CM.AddToParty(Alaska);
        SuperLibrary.CM.AddToParty(Dummy);
    }
    void CheckParty()
    {
        if (DebugMode == false)
        {
            return;
        }
        SuperLibrary.CM.PrintParty();
    }
    void DealDamage()
    {
        if (DebugMode == false)
        {
            return;
        }
        SuperLibrary.CM.DamageCharacter("Alaska",30);
    }
    void SpawnUI()
    {
        if (DebugMode == false)
        {
            return;
        }
        SuperLibrary.MH.SpawnUI();
    }
    void SpawnCharacter(GameObject Character)
    {
        Instantiate(Character);
        if (Character.CompareTag("AlaskaPlayer"))
        {
            AlaskaScript AlaskaS=Character.GetComponent<AlaskaScript>();
            AlaskaS.Spawn(SuperLibrary);
        }
    }
}
