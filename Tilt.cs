using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Tilt : MonoBehaviour
{
    public float h = 1;
    public float TiltMax = 45f;
    public float TiltSpeed = 5f;
    public float TiltReturnSpeed = 8f;

    private Rigidbody rb;
    public float currentTilt;
    public Vector3 CurrentVelcoity;
    public float hSpeed = 1f;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {

        h = Mathf.Sin(Time.time * hSpeed);



        float targetTilt = h != 0 ? -h * TiltMax : 0f;
        float tiltSpeed = h != 0 ? TiltSpeed : TiltReturnSpeed;
        currentTilt = Mathf.Lerp(currentTilt, targetTilt, tiltSpeed * Time.unscaledDeltaTime);
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, currentTilt);
    }
}
