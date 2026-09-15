using System;
using System.Numerics;
using UnityEngine;

public class AlaskaScript : MonoBehaviour
{
    public SuperLibrary SuperLibrary;
    public bool CanMove=true;
    private float horizontal=0f;
    private float vertical=0f;
    private float speed=0.05f;
    [SerializeField] private Animator AlaskaAnimations;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SuperLibrary.CF.GetTarget(gameObject);
        SuperLibrary.EM.AlaskaPlayer = gameObject;
    }
    void Awake()
    {
       SuperLibrary.CF.GetTarget(gameObject); 
    }
    public void Spawn(SuperLibrary SuperLibraryO)
    {
        SuperLibrary=SuperLibraryO;
        SuperLibrary.CF.GetTarget(gameObject);
        SuperLibrary.EM.AlaskaPlayer=gameObject;
    }
    // Update is called once per frame
    void FixedUpdate()
    {

        HandleMovement();
        HandleAnimation();
        
    }
    private void HandleAnimation()
    {
        FindDirection();
        IsWalking();
    }
    private void FindDirection(){
        if (vertical>0f){
            AlaskaAnimations.SetInteger("Direction",3);
        }
        if (horizontal<0f){
            AlaskaAnimations.SetInteger("Direction",2);
        }
        if (vertical<0f){
            AlaskaAnimations.SetInteger("Direction",0);
        }
        if(horizontal>0f){
            AlaskaAnimations.SetInteger("Direction",1);         
        }
    }
    public void HandleMovement(){
        if (!CanMove)
        {
            return;
        }
        if (Input.GetKey(KeyCode.W))
        {
            vertical=1f;
        }else if (Input.GetKey(KeyCode.S))
        {
            vertical=-1f;
        }else{
            vertical=0f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            horizontal=-1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            horizontal=1f;
        }else{
            horizontal=0f;
        }
        if (horizontal!=0f && vertical !=0f){
            float calc=(float)Math.Sqrt(vertical*vertical+horizontal*horizontal);
            vertical=((vertical/Math.Abs(vertical))*calc)/2;
            horizontal=((horizontal/Math.Abs(horizontal))*calc)/2;
        }
        gameObject.transform.position=new UnityEngine.Vector3(
            gameObject.transform.position.x+(horizontal*speed),
            gameObject.transform.position.y+(vertical*speed),
            gameObject.transform.position.z
        );
    }
    private void IsWalking(){
        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))&& CanMove){
            AlaskaAnimations.SetBool("Walking",true);
        }else{
            AlaskaAnimations.SetBool("Walking",false);
        }
    }
    
}
