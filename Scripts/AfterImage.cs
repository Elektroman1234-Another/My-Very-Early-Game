using UnityEngine;
using System;
using System.Collections;
public class AfterImage : MonoBehaviour
{
    public float TimeOfExistence=0.25f;
    public float ChangeOfSide=0.9f;
    public float StartingAlpha=0.1f;
    Coroutine Delete;
    SpriteRenderer SR;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void SetStart(float Time, float Size, float Alpha)
    {
     TimeOfExistence=Time;
     SR=gameObject.GetComponent<SpriteRenderer>();
     ChangeOfSide=Size;
     StartingAlpha=Alpha;
     DeleteObject();  
    }
    public void DeleteObject()
    {
        if (Delete != null)
        {
            StopCoroutine(RemoveObject());
        }
        Delete=StartCoroutine(RemoveObject());
    }
    IEnumerator RemoveObject()
    {
        float duration = TimeOfExistence;
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            Color color = SR.color;
            color.a = StartingAlpha * (1f - elapsed / duration);
            transform.localScale = Vector3.Lerp(startScale, startScale * ChangeOfSide, elapsed / duration);
            SR.color = color;
            yield return null;
        }
        Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
