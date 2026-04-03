using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Brain : Boss
{
    public float sideBulletCoolMax;
    public float sideBulletCoolMin;
    public float SideBulletCool;
    public float CurrentSideBulletCool;


    protected override void Start()
    {
        base.Start();

        float Cool1 = Random.Range(sideBulletCoolMin, sideBulletCoolMax);
        SideBulletCool = Cool1;

    }

    protected override void Fire()
    {
        if(State != BossState.Idle)
            return;

        if(CurrentSideBulletCool > SideBulletCool)
        {
            CurrentSideBulletCool = 0;
            float Cool1 = Random.Range(sideBulletCoolMin, sideBulletCoolMax);
            SideBulletCool = Cool1;
            StartCoroutine(SideBullet(0, 0.25f, 15, 20));
            return;
        }

        CurrentSideBulletCool += Time.deltaTime * GameManager.Instance.GameTime;

        if (CurrentAttackTime >= AttackTime)
        {
               
            CurrentAttackTime = 0;

            StartCoroutine(NormalAttack(0, BulletPos, 0.4f, 10, 12));
        }
        

        CurrentAttackTime += Time.deltaTime * GameManager.Instance.GameTime;
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
            if(RandXnum == 0)
            {
                RandX = Random.Range(-37f, -47f);
            } else
            {
                RandX = Random.Range(37f, 47f);
                rotate = -90f;
            }

            float RandZ = Random.Range(-7f, 7f);

            Vector3 bulletPoins = Player.Instance.transform.position + new Vector3(RandX, 0f, RandZ);


            Projectile projectile = Instantiate(Projectiles[num], bulletPoins, Quaternion.Euler(0f, rotate, 0f));
            projectile.Init(ProjectileType, null, bulletSpeed, damage, BombRaidus, NoChasing);

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

    IEnumerator NormalAttack(int num , List<Transform> bulletPoins, float nextAttack, float damage, int MaxBullet)
    {
        State = BossState.PriAttack;
        animator.SetBool("Attack", true);
        int cp = 0;
        for (int i = 0; i < MaxBullet; i++)
        {
            Projectile projectile = Instantiate(Projectiles[num], BulletPos[cp].transform.position, Quaternion.identity);
            cp++;
            if(cp > bulletPoins.Count - 1)
            {
                cp = 0;
            }
            projectile.Init(ProjectileType, Target, bulletSpeed, damage, BombRaidus, NoChasing);

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



}
