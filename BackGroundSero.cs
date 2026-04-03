using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundSero : MonoBehaviour
{
    public Transform Bg1;
    public Transform Bg2;

    public float beHight = 150f;

    public float ScrollSpeed;

    private void Start()
    {
        Bg1.localPosition = new Vector3(0f, 0f, 0f);
        Bg2.localPosition = new Vector3(0f, 0f, beHight);
    }

    private void Update()
    {
        Bg1.Translate(Vector3.left * ScrollSpeed * Time.deltaTime);
        Bg2.Translate(Vector3.left * ScrollSpeed * Time.deltaTime);

        if (Bg1.localPosition.z <= -beHight)
        {
            Bg1.localPosition = new Vector3(0f, 0f, Bg2.localPosition.z + beHight);
        }

        if (Bg2.localPosition.z <= -beHight)
        {
            Bg2.localPosition = new Vector3(0, 0f, Bg1.localPosition.z + beHight);
        }
    }
}
