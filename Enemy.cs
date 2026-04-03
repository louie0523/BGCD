using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Unit
{
    public Transform Target;
    public float StopRange;

    [Header("아이템 드롭 설정")]
    public int hwak = 20;
    public int ItemGrade = 0;
    public int Score = 5;

    [Header("공격 설정")]
    public List<Transform> BulletPos = new List<Transform>();
    public List<Projectile> Projectiles = new List<Projectile>();
    public float Range = 10f;
    public int MaxBullet;
    public float AttackTime = 3f;
    public float NextAttackDelay = 0f;
    public bool Osoe = false;

    [Header("미사일/폭탄 설정")]
    public ProjectileType ProjectileType;
    public float damage;
    public float bulletSpeed;
    public float BombRaidus;
    public bool NoChasing = false;

    public float CurrentAttackTime;
    public Coroutine HitCoroutine;
    public GameObject DebugObject;
    public bool isKnockback = false;
    public bool imOut = false;

    public bool hitDamage = false;
    public Slider HpSlider;

    public bool Boss = false;



    protected override void Start()
    {
        base.Start();


        Target = Player.Instance.transform;

        material = GetComponent<Renderer>().material;
        OriginColor = material.color;

        DebugObject = transform.Find("Debug").gameObject;

        HpSlider.gameObject.SetActive(false);



    }

    protected virtual void Update()
    {
        if (isAlive)
            Fire();

        DebugObject.SetActive(Cheat.Instance.DebugMode);
        if(HpSlider != null)    
            HpSlider.value = Hp / MaxHp;
    }

    protected virtual void FixedUpdate()
    {
        if(isAlive &&  !isKnockback)
            Move();
    }

    protected virtual void Fire()
    {
        if(Vector3.Distance(Target.position, transform.position) <= Range)
        {
            if(CurrentAttackTime >= AttackTime)
            {
                CurrentAttackTime = 0;
                StartCoroutine(BulletFire());
            }
        }

        CurrentAttackTime += Time.deltaTime * GameManager.Instance.GameTime;
    }

    public IEnumerator BulletFire()
    {
        for(int i = 0; i <MaxBullet; i++)
        {
            
            Projectile projectile = Instantiate(Projectiles[0], BulletPos[i].transform.position, Quaternion.identity);
            if(!NoChasing)
                projectile.Init(ProjectileType, Target, bulletSpeed, damage, BombRaidus);
            else
                projectile.Init(ProjectileType, Target, bulletSpeed, damage, BombRaidus, NoChasing);

            if (Osoe && !isKnockback)
            {
                StartCoroutine(KnockBack(10f));
            }

            if(ProjectileType == ProjectileType.Missile)
            {
                SfxManager.instance.PlaySfx("적공격", 0.7f);
            } else if(ProjectileType == ProjectileType.Boom)
            {
                SfxManager.instance.PlaySfx("폭탄", 0.5f);
            }

            yield return new WaitForSeconds(NextAttackDelay);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isKnockback)
        {
            StartCoroutine(KnockBack(3f));
        }
    }

    IEnumerator KnockBack(float power)
    {
       
        isKnockback = true;
        rb.velocity = -transform.forward * power * GameManager.Instance.GameTime; // 뒤로 속도 확 줌
        yield return new WaitForSeconds(0.3f);  // 잠깐 유지
        isKnockback = false;
    }


    protected virtual void Move()
    {
        CameraTo();
        Vector3 dir = (Target.position - transform.position).normalized;
        if (Vector3.Distance(Target.position, transform.position) > StopRange || imOut)
        {
            rb.velocity = dir * Speed * GameManager.Instance.GameTime;
        }
        else
        {
            rb.velocity = Vector3.zero;
        }

        Quaternion lookRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRot, 360f * Time.fixedDeltaTime * GameManager.Instance.GameTime);

    }

    public override bool Damage(float damage)
    {

        if (HitCoroutine != null)
        {
            StopCoroutine(HitCoroutine);
        }
        HitCoroutine = StartCoroutine(HitEffect(0.5f * GameManager.Instance.GameTime));

        if(!hitDamage)
        {
            hitDamage = true;
            if(HpSlider != null) 
                HpSlider.gameObject.SetActive(true);
        }

        if(!Boss)
            SfxManager.instance.PlaySfx("피격", 0.4f);
        else
            SfxManager.instance.PlaySfx("보스피격");

        return base.Damage(damage); 
    }
    public void CameraTo()
    {
        Vector3 vp = Camera.main.WorldToViewportPoint(transform.position);
        vp.x = Mathf.Clamp01(vp.x);
        vp.y = Mathf.Clamp01(vp.y);

        Vector3 World = Camera.main.ViewportToWorldPoint(vp);

        if (Vector3.Distance(transform.position, World) > 0.01f)
        {
            imOut = true;
        } else
        {
            imOut = false;
        }

    }

    public override void Death()
    {
        DeathEffect.Play();
        SfxManager.instance.PlaySfx("적사망", 0.4f);
        rb.velocity = Vector3.zero;

        DataManager.instance.Score += Score;
        DataManager.instance.Coin += (int)(Score * 1.2);

        int Rand = Random.Range(1, 101);
        if(Rand <= hwak)
        {
            ItemObject item = Instantiate(GameManager.Instance.ItemPrefab, transform.position, Quaternion.identity, StageManager.Instance.transform);
            item.Grade = ItemGrade;
        }

        StageManager.Instance.AliveEnemys.Remove(this);
        StageManager.Instance.NextWavesEnemyCount();

        StartCoroutine(DeathDestory());
    }

    IEnumerator DeathDestory()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
        HpSlider.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.6f);
        Destroy(gameObject);
    }

    IEnumerator HitEffect(float time)
    {

        material.color = Color.red;
        yield return new WaitForSeconds(time);
        material.color = OriginColor;
    }

}
