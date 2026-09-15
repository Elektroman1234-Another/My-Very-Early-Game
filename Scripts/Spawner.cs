using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    float Times=0f;
    public GameObject Attach;
    public bool Activate;
    public string Direction;
    public Menu_Handler MH;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void Update()
    {
        if (Activate)
        {
            SpawnCopy();   
        }
    }
    void SpawnCopy()
    {
        Times=Times+Time.deltaTime;
        if (Times>=0.5f){
        if (Attach == null)
        {
            Debug.LogError("Spawner needs an Attach object.", this);
            return;
        }
        GameObject AMC = Instantiate(gameObject, transform.position, transform.rotation);
        AMC.transform.SetParent(Attach.transform, true);
        Vector3 parentScale = Attach.transform.lossyScale;
        Vector3 originalScale = transform.lossyScale;
        AMC.transform.localScale = new Vector3(
            originalScale.x / parentScale.x,
            originalScale.y / parentScale.y,
            originalScale.z / parentScale.z);
        Destroy(AMC.GetComponent<Spawner>());
        AfterEffectMove MHEFM=AMC.AddComponent<AfterEffectMove>();
        MHEFM.SetStart(1f,1f,Direction);
        MH.SpawnedObject.Add(AMC);
        Times=0f;
        }
    }
}   
