using Unity.VisualScripting;
using UnityEngine;

public class AttackableEnemy : MonoBehaviour
{
    public EnemyGang WhoSpawns;
    public SuperLibrary SuperLibrary;
    [Header("Sound")]
    public AudioData BattleEnter;
    public GameObject Character;
    Vector3 OriginalPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("AlaskaPlayer")){
            Debug.Log("Alaska Entered");
            SuperLibrary.AudioM.PlaySFX(BattleEnter);
            AlaskaScript Alas=other.gameObject.GetComponent<AlaskaScript>();
            Alas.CanMove=false;
            OriginalPosition=other.gameObject.transform.position;
            
            SuperLibrary.MH.StartBattle(WhoSpawns);
            Instantiate(Character, SuperLibrary.CF.gameObject.transform.position, SuperLibrary.CF.gameObject.transform.rotation);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
       if (other.gameObject.CompareTag("AlaskaPlayer")){
            Debug.Log("Alaska Left Bruh");
        }
    }
}
