using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileType
{
    Missile,
    Boom,
}

public class Projectile : MonoBehaviour
{
    public ProjectileType type;

    public float LifeTime = 20f;
    public Transform Target;
    public Vector3 TargetPos;

    public float Speed = 10f;
    public float Damage = 10f;

    [Header("폭탄")]
    public float DamageRadius = 5f;
    public float arcHeight = 3f;

    [Header("폭탄 속도")]
    public float minSpeed = 0.35f;
    public float maxSpeed = 2f;
    public float referenceDistance = 20f;

    public bool Hit = false;
    public bool NotChasing = false;

    public Rigidbody rb;
    public float CurrentLifeTIme;

    private ParticleSystem Particle;

    // BoomMove 필드
    private Vector3 boomStartPos;
    private Vector3 boomEndPos;
    private float boomDuration;
    private float boomTime = 0f;
    private Vector3 flatDir;
    private float OriginSpeed;
    [SerializeField]private bool reflect = false;
    public GameObject Debug;
    public float DestoryTime;


    public void Init(ProjectileType type, Transform Target, float Speed, float Damage, float DamageRadius = 5f, bool NotChasing = false)
    {
        //Hit = true;
        if (StageManager.Instance.Stange)
        {
            float s = Random.Range(0.5f, 1.3f);

            transform.localScale = new Vector3(s, s, s);
        }

        rb = GetComponent<Rigidbody>();

        Debug = transform.Find("Debug").gameObject;

        this.type = type;
        this.Target = Target;
        this.Speed = Speed;
        OriginSpeed = Speed;
        this.Damage = Damage;
        this.DamageRadius = DamageRadius;
        this.NotChasing = NotChasing;

        if (type == ProjectileType.Boom)
            SetTargetPos();

        if (this.NotChasing)
        {
            if(Target != null)
            {
                Vector3 DirChasing = (Target.position - transform.position).normalized;
                rb.velocity = DirChasing * Speed;

                Quaternion Dirlook = Quaternion.LookRotation(DirChasing);
                transform.rotation = Dirlook;

                StartCoroutine(hitClear());
                return;
            } else
            {
                StartCoroutine(hitClear());
                return;
            }
        }

        Vector3 Dir = new Vector3(0, 0, 0);
        if (type == ProjectileType.Missile && !this.NotChasing)
        {
            if (!this.NotChasing)
            {
                Dir = (Target.position - transform.position).normalized;
            }
        }
        else if (type == ProjectileType.Boom)
        {
            rb.velocity = Vector3.zero;
            StartBoomMove();
            StartCoroutine(hitClear());
            return;
        }

        rb.velocity = Dir * Speed;

        Quaternion look = Quaternion.LookRotation(Dir);
        transform.rotation = look;
        StartCoroutine(hitClear());
    }

    IEnumerator hitClear()
    {
        Hit = true;
        yield return new WaitForSeconds(0.1f);
        Hit = false;
    }

    public void SetTargetPos()
    {
        Particle = transform.GetChild(0).GetComponent<ParticleSystem>();
        float x = Random.Range(-2f, 8f);
        float z = Random.Range(-5f, 5f);
        TargetPos = Target.position + Target.forward * 7f + new Vector3(x, 0f, z);
    }

    public void StartBoomMove()
    {
        boomStartPos = new Vector3(transform.position.x, 0f, transform.position.z);
        boomEndPos = new Vector3(TargetPos.x, 0f, TargetPos.z);
        flatDir = new Vector3(-(boomEndPos - boomStartPos).normalized.z, 0f, (boomEndPos - boomStartPos).normalized.x);

        float dist = Vector3.Distance(boomStartPos, boomEndPos);
        float adjustedSpeed = Mathf.Clamp(dist, minSpeed * Speed, maxSpeed * Speed);
        boomDuration = dist / adjustedSpeed;
        boomTime = 0f;
    }




    public IEnumerator SpeedStopCoroutin(float time, float power)
    {
        Speed *= power;
        yield return new WaitForSeconds(time);
        Speed = OriginSpeed;
    }



    void BoomMove()
    {


        boomTime += Time.fixedDeltaTime * GameManager.Instance.GameTime * GameManager.Instance.ProjectileSpeed;
        float t = Mathf.Clamp01(boomTime / boomDuration);

        Vector3 pos = Vector3.Lerp(boomStartPos, boomEndPos, t);
        pos += flatDir * arcHeight * t * (1 - t) * 4; 
        pos.y = 0f;

        Vector3 moveDir = pos - rb.position; // 실제 이동 방향
        if (moveDir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(moveDir);

        rb.MovePosition(pos);

        if (Vector2.Distance(new Vector2(pos.x, pos.z), new Vector2(boomEndPos.x, boomEndPos.z)) <= 0.1f)
        {
            OnBoomHit();
            return;
        }

        
    }

    void OnBoomHit()
    {
        if(Hit)
            return;

        Hit = true;
        List<Unit> Targets = RadiusUnits(transform.position, DamageRadius);
        foreach (Unit target in Targets)
        {
            if(!target.gameObject.CompareTag("Boss"))
                target.Damage(Damage);
        }


        StartCoroutine(DestoryProejctile(0.35f));
    }



    private void FixedUpdate()
    {
        if (Hit)
            DestoryTime += Time.fixedDeltaTime;

        if(DestoryTime >= 0.2f)
            Destroy(gameObject);

            Debug.SetActive(Cheat.Instance.DebugMode);

        Move();

        CurrentLifeTIme += Time.fixedDeltaTime;

        if (CurrentLifeTIme > LifeTime)
        {
            if(type == ProjectileType.Missile)
            {
                Hit = true;
                Destroy(gameObject, 0f);
            } else if(type == ProjectileType.Boom){
                OnBoomHit();
            }
        }
    }

    void Move()
    {
        if (type == ProjectileType.Boom)
        {
            BoomMove();
            return;
        }

        if (NotChasing)
        {
            rb.velocity = transform.forward * Speed * GameManager.Instance.GameTime * GameManager.Instance.ProjectileSpeed;
            return;
        }

        if (type == ProjectileType.Missile)
        {
            if (Target != null)
            {
                Vector3 Dir = (Target.position - transform.position).normalized;
                rb.velocity = Dir * Speed * GameManager.Instance.GameTime * GameManager.Instance.ProjectileSpeed;

                if (Speed > 0)
                {
                    Quaternion look = Quaternion.LookRotation(Dir);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, look, 360f * Time.fixedDeltaTime);
                }
            }
            else
            {
                rb.velocity = transform.forward * Speed * GameManager.Instance.GameTime * GameManager.Instance.ProjectileSpeed; 
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (Hit)
            return;

        Unit unit = other.GetComponent<Unit>();

        if (unit == null && other.transform.parent != null)
            unit = other.transform.parent.GetComponent<Unit>();

        if (unit == null || other.CompareTag("Boss"))
            return;

        if (other.gameObject.CompareTag("Player") && GameManager.Instance.Parry > 0 && !reflect)
        {
            reflect = true;

            if (type == ProjectileType.Missile)
            {

                Target = null;
                Vector3 DirChasing = -transform.forward;
                rb.velocity = DirChasing * Speed * GameManager.Instance.GameTime * GameManager.Instance.ProjectileSpeed;

                Quaternion Dirlook = Quaternion.LookRotation(DirChasing);
                transform.rotation = Dirlook;
                NotChasing = true;
            }
            else if (type == ProjectileType.Boom)
            {

                flatDir = new Vector3(-(boomEndPos - boomStartPos).normalized.z, 0f, (boomEndPos - boomStartPos).normalized.x);

                float dist = Vector3.Distance(boomStartPos, boomEndPos);
                float adjustedSpeed = Mathf.Clamp(dist, minSpeed * Speed * GameManager.Instance.GameTime * GameManager.Instance.ProjectileSpeed, maxSpeed * Speed * GameManager.Instance.GameTime * GameManager.Instance.ProjectileSpeed);
                boomDuration = -dist / adjustedSpeed;
                boomTime = 0f;
            }

            return;
        }

        if (other.gameObject.CompareTag("Player") && reflect)
            return;

        if (type == ProjectileType.Missile)
        {
            Hit = true;
            unit.Damage(Damage);
        }
        else if (type == ProjectileType.Boom)
        {
            Hit = true;
            OnBoomHit();
            return;
        }

        if (Hit)
            StartCoroutine(DestoryProejctile(0f));
    }



    List<Unit> RadiusUnits(Vector3 pos, float Rad)
    {
        Collider[] Col = Physics.OverlapSphere(pos, Rad);
        List<Unit> units = new List<Unit>();

        foreach (Collider col in Col)
        {
            Unit unit = col.GetComponent<Unit>();
            if (unit != null && !units.Contains(unit))
                units.Add(unit);
        }

        Particle.Play();

        return units;
    }

    public void OnSignalReceived()
    {
        if (GetComponent<Renderer>().isVisible)
        {
            Destroy(gameObject, 0.1f);
        }
    }

    IEnumerator DestoryProejctile(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}