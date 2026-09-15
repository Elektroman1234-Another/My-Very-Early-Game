using System;
using Unity.VisualScripting;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float FollowSpeed =2f;
    public Transform target;
    public string FollowRule;
    public float OffSetx=5f;
    public float OffSety=5;
    public bool IgnoreRule=false;
    public bool CameraCanGoUP=true;
    public bool CameraCanGoDown=true;
    public bool CameraCanGoLeft=true;
    public bool CameraCanGoRight=true;
    public bool CameraShouldMovealways=true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // Update is called once per frame
    void FixedUpdate()
    {
        CameraHitBounds();
        cameraMove();
    }
    void cameraMove()
    {
        if (target == null)
        {
            return;
        }
        if (CameraShouldMovealways!=true){
            return;
        }
        float tarposx=target.position.x;
        float tarposy=target.position.y;
        float distancex=tarposx-transform.position.x;
        float distancey=tarposy-transform.position.y;
        tarposx=CameraAllowMovement(tarposx,distancex,"x");
        tarposy=CameraAllowMovement(tarposy,distancey,"y");
        Vector3 newPos=new Vector3(tarposx,tarposy,-10f);
        if (FollowRule == "Smooth")
        {
            transform.position = Vector3.Lerp(transform.position,newPos,1f - Mathf.Exp(-FollowSpeed * Time.deltaTime)); 
        }
        if (FollowRule == "InMoment")
        {
            transform.position=newPos;
        }
    }
    public void GetTarget(GameObject GO)
    {
        target=GO.transform;
    }
    bool OffSetCalculation(Vector3 Oldpos, Vector3 NewPos)
    {
        if(!(Oldpos.x + OffSetx > NewPos.x && Oldpos.x + (-1*OffSetx) < NewPos.x))
        {
            return false;
        }
        return true;
    }
    void CameraHitBounds()
    {
        SetEverythingToTrue();
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            box.bounds.center,
            box.bounds.size,
            transform.eulerAngles.z);

        foreach (Collider2D hit in hits)
        {
            // Ignore the camera's own BoxCollider2D.
            if (hit == box)
            {
                continue;
            }

            // Compare the tag of the object that the box overlaps.
            if (hit.CompareTag("CameraStop"))
            {
                Vector2 NearestPoint=hit.ClosestPoint(transform.position);
                float DistanceX=transform.position.x-NearestPoint.x;
                float DistanceY=transform.position.y-NearestPoint.y;
                Camera cam = GetComponent<Camera>();
                float cameraHeight = cam.orthographicSize;
                float cameraWidth = cameraHeight * cam.aspect;
                CalculationOfDistance(cameraHeight, cameraWidth, DistanceX,DistanceY);
            }else{
                
            }
        }
    }
    void CalculationOfDistance(float cameraHeight, float cameraWidth, float DistanceX, float DistanceY)
    {
        const float zeroTolerance = 0.001f;
        if (Mathf.Abs(DistanceX) < zeroTolerance)
        {
            DistanceX = 0f;
        }
        if (Mathf.Abs(DistanceY) < zeroTolerance)
        {
            DistanceY = 0f;
        }

        if (DistanceY > 0)
        {
            if (DistanceY <= cameraHeight)
            {
                CameraCanGoDown=false;
            }
        }
        else if (DistanceY < 0)
        {
            if (DistanceY >= -cameraHeight)
            {
                CameraCanGoUP=false;
            }
        }
        if (DistanceX > 0)
        {
            if (DistanceX <= cameraWidth)
            {
                CameraCanGoLeft=false;
            }
        }else if (DistanceX < 0)
        {
            if (DistanceX >= -cameraWidth)
            {
                CameraCanGoRight=false;
            }
        }
    }
    float CameraAllowMovement(float xydpos, float xydistance, string position)
    {
        if (position == "x")
        {
            if (xydistance > 0 && !CameraCanGoRight)
            {
                return transform.position.x;
            }
            if (xydistance < 0 && !CameraCanGoLeft)
            {
                return transform.position.x;
            }
            
        }else if(position == "y")
        {
            if(xydistance>0 && !CameraCanGoUP)
            {
                return transform.position.y;
            }
            if(xydistance<0 && !CameraCanGoDown)
            {
                return transform.position.y;
            }
        }
        return xydpos;
    }
    void SetEverythingToTrue()
    {
        CameraCanGoRight=true;
        CameraCanGoLeft=true;
        CameraCanGoUP=true;
        CameraCanGoDown=true;
    }
    }

