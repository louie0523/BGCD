using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{
    public static Player Instance;


    public float Acc;


    public float RotateSpeed;

    public float TiltMax = 45f;
    public float TiltSpeed = 5f;
    public float TiltReturnSpeed = 8f;
    public bool BossAttack;
    public float bossAttackDamage = 50f;
    public bool isKnockback = false;

    [Header("Effects")]
    public ParticleSystem Emp;
    public ParticleSystem UoDo;
    public ParticleSystem Reflect;
    public ParticleSystem GamSuk;

    private Transform child;

    private Vector3 CurrentVelocity;
    private float currentTilt;

    private Camera cam;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    protected override void Start()
    {
        base.Start();

        cam = Camera.main;

        child = transform.GetChild(0);

        material = child.GetComponent<Renderer>().material;

        MaxHp += DataManager.instance.HpUpGrade * 50;
        Hp = MaxHp;

        Speed += DataManager.instance.SpeedUpGrade * 0.7f;

        Depense += DataManager.instance.DependUpGrade * 3;
        
    }

    private void FixedUpdate()
    {
        if(isAlive && !isKnockback)
        HandleMovenet();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (BossAttack)
            return;

        if (collision.gameObject.CompareTag("Boss"))
        {
            Unit boss = collision.gameObject.GetComponent<Unit>();
            boss.Damage(bossAttackDamage);
            StartCoroutine(KnockBack());
        }
    }

    IEnumerator KnockBack()
    {
        isKnockback = true;
        rb.velocity = -transform.forward * 15f; // 뒤로 속도 확 줌
        yield return new WaitForSeconds(0.3f);  // 잠깐 유지
        isKnockback = false;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!BossAttack)
            return;

        if (collision.gameObject.CompareTag("Boss"))
        {
            BossAttack = false;
        }
    }


    void HandleMovenet()
    {

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        float TargetSpeed =
            v > 0 ? Speed :
            v < 0 ? -Speed * 0.25f :
            Speed * 0.5f;

        Vector3 TargetMove = transform.forward * TargetSpeed;

        CurrentVelocity = Vector3.Lerp(CurrentVelocity, TargetMove, Time.fixedDeltaTime * Acc * (h == 0 && v == 0 ? 0.3f : 1f));    

        rb.velocity = new Vector3(CurrentVelocity.x, 0f, CurrentVelocity.z);

        CameraTo();
    }

    void CameraTo()
    {
        Vector3 vp = cam.WorldToViewportPoint(transform.position);

        vp.x = Mathf.Clamp01(vp.x);
        vp.y = Mathf.Clamp01(vp.y);

        Vector3 World = cam.ViewportToWorldPoint(vp);

        if(Vector3.Distance(transform.position, World) > 0.01f)
        {
            CurrentVelocity = Vector3.zero;
            rb.velocity = Vector3.zero;
        }
        transform.position = World;
    }

    private void Update()
    {
        HandleRotate();
        HandleTilt();
    }

    void HandleRotate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        transform.Rotate(0f, h * RotateSpeed * Time.unscaledDeltaTime, 0f);
    }

    void HandleTilt()
    {
        float h = Input.GetAxisRaw("Horizontal");

        float targeTilt = h != 0 ? -h * TiltMax : 0f;

        float tiltSpeed = h != 0 ? TiltSpeed : TiltReturnSpeed;

        currentTilt = Mathf.Lerp(currentTilt, targeTilt, tiltSpeed * Time.unscaledDeltaTime);

        child.localEulerAngles = new Vector3(child.localEulerAngles.x, child.localEulerAngles.y, currentTilt);  
    }

    public override bool Damage(float damage)
    {
        if (Cheat.Instance.Mujuk)
            return false;
        SfxManager.instance.PlaySfx("플레이어피격", 1f);
        CameraManager.instance.Shake(-1.5f, 0.15f);
        return base.Damage(damage);
    }

    public override void Death()
    {
        SfxManager.instance.PlaySfx("패배");
        UImanager.instance.GameOver.SetActive(true);
        Time.timeScale = 0f;
    }
}
