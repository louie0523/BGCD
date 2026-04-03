using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    public Camera Camera;

    public Vector3 OriginPos;

    private Coroutine ShakeCoru;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;

        } else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Camera = GetComponent<Camera>();
        OriginPos = Camera.transform.position;
    }

    public void Shake(float Power, float time)
    {
        if(ShakeCoru != null)
        {
            StopCoroutine(ShakeCoru);
        }

        ShakeCoru = StartCoroutine(ShakeEffect(Power, time));
    }

    IEnumerator ShakeEffect(float Power, float time)
    {
        float curTime = 0f;
        while (curTime < time)
        {
            curTime += Time.deltaTime;

            float x = Random.Range(-Power, Power);
            float z = Random.Range(-Power, Power);

            Camera.transform.position = OriginPos + new Vector3(x, 0f, z);

            yield return null;
        }

        Camera.transform.position = OriginPos;
    }
}
