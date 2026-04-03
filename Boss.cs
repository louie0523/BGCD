using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossState
{
    Idle,
    Move,
    PriAttack,
    SkillAttack,
}

public class Boss : Enemy
{

    public static Boss Instance;

    [Header("보스 관련 설정")]
    public List<Vector3> MovePoints = new List<Vector3>();
    public int currentPoints;
    public BossState State;
    public Animator animator;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(gameObject);
        }
    }



    protected override void Start()
    {
        rb = GetComponent<Rigidbody>();
        DeathEffect = transform.Find("DeathEffect").GetComponent<ParticleSystem>();
        isAlive = true;
        Hp = MaxHp;


        Target = Player.Instance.transform;

        material = transform.GetChild(0).GetComponent<Renderer>().material;
        OriginColor = material.color;

        DebugObject = transform.Find("Debug").gameObject;

        animator = transform.GetChild(0).GetComponent<Animator>();

        State = BossState.Idle;

    }

    protected override void FixedUpdate()
    {
        if (isAlive && State == BossState.Move)
            Move();
    }

    protected override void Move()
    {
        Vector3 dir = (MovePoints[currentPoints] - transform.position).normalized;
        if ( Vector3.Distance(MovePoints[currentPoints], transform.position) > 1.5f)
        {
            rb.MovePosition(transform.position + dir * Speed * Time.fixedDeltaTime * GameManager.Instance.GameTime);
            Debug.Log("보스 이동중");
        }
        else
        {
            
            rb.velocity = Vector3.zero;
            State = BossState.Idle;
        }


    }

    public override void Death()
    {
        DeathEffect.Play();
        State = BossState.Idle;

        DataManager.instance.Score += Score;
        DataManager.instance.Coin += 5000;


        StageManager.Instance.NextWavesEnemyCount();

        StageManager.Instance.BossClear();

        SfxManager.instance.PlaySfx("승리");

        Destroy(gameObject, 0.25f);
    }

}
