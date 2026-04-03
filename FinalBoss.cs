using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBoss : Boss
{
    public List<Transform> MissilePos = new List<Transform>();
    public float sideBulletCoolMax;
    public float sideBulletCoolMin;
    public float SideBulletCool;
    public float CurrentSideBulletCool;

    public float CricleBulletCool = 8f;
    public float CurrentCircleBulletCool;
    public int Ban = 6;



    protected override void Start()
    {
        base.Start();

        float Cool1 = Random.Range(sideBulletCoolMin, sideBulletCoolMax);
        SideBulletCool = Cool1;

    }

    protected override void Fire()
    {
        if (State != BossState.Idle)
            return;

        if (CurrentSideBulletCool > SideBulletCool)
        {
            CurrentSideBulletCool = 0;
            float Cool1 = Random.Range(sideBulletCoolMin, sideBulletCoolMax);
            SideBulletCool = Cool1;
            StartCoroutine(SideBullet(1, 0.25f, 12, 35));
            return;
        }

        if(CurrentCircleBulletCool > CricleBulletCool && Ban >= 1)
        {
            Ban--;
            if(Ban <= 0)
            {
                CurrentCircleBulletCool = 0;
                Ban = 6;
            }
            StartCoroutine(Circle(1, 0f, 5, 12));
           
            return;
        }

        CurrentSideBulletCool += Time.deltaTime * GameManager.Instance.GameTime;
        CurrentCircleBulletCool += Time.deltaTime * GameManager.Instance.GameTime;

        if (CurrentAttackTime >= AttackTime)
        {

            CurrentAttackTime = 0;

            StartCoroutine(NormalAttack1(0, BulletPos, 0.3f, 3, 20));
            StartCoroutine(NormalAttack(1, MissilePos, 0.15f, 2, 40));
        }


        CurrentAttackTime += Time.deltaTime * GameManager.Instance.GameTime;
    }

    IEnumerator Circle(int num, float nextAttack, float damage, int MaxBullet)
    {
        State = BossState.PriAttack;
        animator.SetBool("Attack", true);


            float radius = 15f;
            for (int i = 0; i < MaxBullet; i++)
            {
                float angle = (Mathf.PI * 2 / MaxBullet) * i;
                Vector3 dir = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
                Vector3 spawn = transform.position + dir * radius;
                Projectile projectile = Instantiate(Projectiles[2], spawn, Quaternion.identity);
                projectile.Init(ProjectileType.Missile, Target, bulletSpeed, damage, BombRaidus, NoChasing);

                if (ProjectileType == ProjectileType.Missile)
                {
                    SfxManager.instance.PlaySfx("적공격", 0.7f);
                }
                else if (ProjectileType == ProjectileType.Boom)
                {
                    SfxManager.instance.PlaySfx("폭탄", 0.5f);
                }


                yield return new WaitForSeconds(nextAttack);
            }

            
        


        State = BossState.Idle;
        animator.SetBool("Attack", false);
        SetMove();
    }


    void SetMove()
    {
        int rand = Random.Range(0, MovePoints.Count);
        if (currentPoints == rand)
            return;
        currentPoints = rand;
        State = BossState.Move;
    }

    IEnumerator SideBullet(int num, float nextAttack, float damage, int MaxBullet)
    {
        State = BossState.PriAttack;
        animator.SetBool("Attack", true);
        for (int i = 0; i < MaxBullet; i++)
        {
            float RandX = 0f;
            int RandXnum = Random.Range(0, 2);
            float rotate = 90f;
            if (RandXnum == 0)
            {
                RandX = Random.Range(-37f, -47f);
            }
            else
            {
                RandX = Random.Range(37f, 47f);
                rotate = -90f;
            }

            float RandZ = Random.Range(-7f, 7f);

            Vector3 bulletPoins = Player.Instance.transform.position + new Vector3(RandX, 0f, RandZ);


            Projectile projectile = Instantiate(Projectiles[num], bulletPoins, Quaternion.Euler(0f, rotate, 0f));
            projectile.Init(ProjectileType.Missile, null, bulletSpeed, damage, BombRaidus, NoChasing);

            if (ProjectileType == ProjectileType.Missile)
            {
                SfxManager.instance.PlaySfx("적미사일", 1);
            }
            else if (ProjectileType == ProjectileType.Boom)
            {
                SfxManager.instance.PlaySfx("폭탄", 1f);
            }



            yield return new WaitForSeconds(nextAttack);
        }
        State = BossState.Idle;
        animator.SetBool("Attack", false);
        SetMove();
    }

    IEnumerator NormalAttack(int num, List<Transform> bulletPoins, float nextAttack, float damage, int MaxBullet)
    {
        State = BossState.PriAttack;
        animator.SetBool("Attack", true);
        int cp = 0;
        for (int i = 0; i < MaxBullet; i++)
        {
            Projectile projectile = Instantiate(Projectiles[num], BulletPos[cp].transform.position, Quaternion.identity);
            cp++;
            if (cp > bulletPoins.Count - 1)
            {
                cp = 0;
            }
            projectile.Init(ProjectileType.Missile, Target, bulletSpeed, damage, BombRaidus, NoChasing);


            if (ProjectileType == ProjectileType.Missile)
            {
                SfxManager.instance.PlaySfx("적미사일", 1f);
            }
            else if (ProjectileType == ProjectileType.Boom)
            {
                SfxManager.instance.PlaySfx("폭탄", 0.5f);
            }


            yield return new WaitForSeconds(nextAttack);
        }
        State = BossState.Idle;
        animator.SetBool("Attack", false);
        SetMove();
    }

    IEnumerator NormalAttack1(int num, List<Transform> bulletPoins, float nextAttack, float damage, int MaxBullet)
    {
        State = BossState.PriAttack;
        animator.SetBool("Attack", true);
        int cp = 0;
        for (int i = 0; i < MaxBullet; i++)
        {
            Projectile projectile = Instantiate(Projectiles[num], BulletPos[cp].transform.position, Quaternion.identity);
            cp++;
            if (cp > bulletPoins.Count - 1)
            {
                cp = 0;
            }
            projectile.Init(ProjectileType, Target, bulletSpeed, damage, BombRaidus, NoChasing);


            if (ProjectileType == ProjectileType.Missile)
            {
                SfxManager.instance.PlaySfx("적미사일", 1f);
            }
            else if (ProjectileType == ProjectileType.Boom)
            {
                SfxManager.instance.PlaySfx("폭탄", 0.5f);
            }


            yield return new WaitForSeconds(nextAttack);
        }
        State = BossState.Idle;
        animator.SetBool("Attack", false);
        SetMove();
    }

}
