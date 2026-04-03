using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    public Vector3 TargetPos;
    public float Speed;

    private Rigidbody rb;

    public float LifeTime = 20f;
    public float currentLifeTIme = 0f;

    public GameObject Debug;

    private void Start()
    {
        TargetPos = Player.Instance.transform.position;

        Debug = transform.Find("Debug").gameObject;

        rb = GetComponent<Rigidbody>();

        Vector3 Dir = (TargetPos - transform.position).normalized;

        rb.velocity = Dir * Speed;

        Quaternion look = Quaternion.LookRotation(Dir);
        transform.rotation = look;

    }

    private void Update()
    {

        Debug.SetActive(Cheat.Instance.DebugMode);


        currentLifeTIme += Time.deltaTime;
        
        if(currentLifeTIme >= LifeTime)
        {
            Destroy(gameObject);
        }
    }

   
}
