using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BlackHole : MonoBehaviour
{
    public float LifeTime = 5;



    private void Update()
    {
        LifeTime -= Time.deltaTime;
        if(LifeTime < 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {

        if(other.CompareTag("Projectile"))
        {

            Destroy(other.gameObject);

        }
    }
}
