using UnityEngine;
using System.Collections;
public class AfterEffectMove : MonoBehaviour
{
    
    public float TimeOfExistence=2f;
    public float Distance=1f;
    Coroutine Delete;
    SpriteRenderer SR;
    public void SetStart(float time, float distance, string direction)
    {
        TimeOfExistence=time;
        Distance=distance;
        if (direction == "Left")
        {
            Distance=Distance*-1;
        }
        SR=gameObject.GetComponent<SpriteRenderer>();
        MoveDelete();
    }
    public void MoveDelete()
    {
        if (Delete != null)
        {
            StopCoroutine(Delete);
        }
        Delete=StartCoroutine(Move());
    }
    IEnumerator Move()
    {
        float duration = TimeOfExistence;
        float elapsed = 0f;
        float speedmod=1f;
        Vector3 StartPosition=transform.localPosition;
        Vector3 EndPosition=new Vector3(
            transform.localPosition.x+Distance,
            transform.localPosition.y,
            transform.localPosition.z
        );
        while (elapsed < duration)
        {
            if (elapsed < duration / 3)
            {
                speedmod=0.5f;
            }
            else
            {
                speedmod=2f;
            }
            elapsed += Time.deltaTime*speedmod;
            Color color = SR.color;
            color.a = 1f * (1f - elapsed / duration);
            transform.localPosition = Vector3.Lerp(StartPosition, EndPosition, elapsed / duration);
            SR.color = color;
            yield return null;
        }
        transform.localPosition = EndPosition;
        Destroy(gameObject);
    }
}
