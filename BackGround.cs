using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    public Transform Bg1;
    public Transform Bg2;

    public float beHight = 150f;

    public float ScrollSpeed;

    private void Start()
    {   
        Bg1.position = new Vector3(0, transform.position.y, 0);
        Bg2.position = new Vector3(0, transform.position.y, beHight);
    }

    private void Update()
    {
        Bg1.Translate(Vector3.back * ScrollSpeed * Time.deltaTime);
        Bg2.Translate(Vector3.back * ScrollSpeed * Time.deltaTime);

        if(Bg1.position.z <= -beHight)
        {
            Bg1.transform.position = new Vector3(0, transform.position.y, Bg2.position.z + beHight);
        }


        if (Bg2.position.z <= -beHight)
        {
            Bg2.transform.position = new Vector3(0, transform.position.y, Bg1.position.z + beHight);
        }
    }
}
