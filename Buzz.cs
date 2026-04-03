using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buzz : Boss
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
        if (State != BossState.Idle)
            return;

        if (CurrentSideBulletCool > SideBulletCool)
        {
            CurrentSideBulletCool = 0;
            float Cool1 = Random.Range(sideBulletCoolMin, sideBulletCoolMax);
            SideBulletCool = Cool1;
            StartCoroutine(SideBullet(0, 0.25f, 12, 35));
            return;
        }

        CurrentSideBulletCool += Time.deltaTime * GameManager.Instance.GameTime;

        if (CurrentAttackTime >= AttackTime)
        {

            CurrentAttackTime = 0;

            StartCoroutine(NormalAttack(0, BulletPos, 0.3f, 15, 15));
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

        float radius = 8f;
        for(int i = 0; i < MaxBullet; i++)
        {
            float angle = (Mathf.PI * 2 / MaxBullet) * i;
            Vector3 dir = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
            Vector3 spawn = Player.Instance.transform.position + dir * radius;
            Projectile projectile = Instantiate(Projectiles[0], spawn, Quaternion.identity);
            projectile.Init(ProjectileType, Target, bulletSpeed, damage, BombRaidus, NoChasing);

            if (ProjectileType == ProjectileType.Missile)
            {
                SfxManager.instance.PlaySfx("적공격", 0.7f);
            }
            else if (ProjectileType == ProjectileType.Boom)
            {
                SfxManager.instance.PlaySfx("폭탄", 0.5f);
            }


            yield return new WaitForSeconds(0.05f);
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
