using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Unit : MonoBehaviour
{

    public float MaxHp;
    public float Hp;

    public bool isAlive = true;

    public float Speed;
    public float Depense;

    public Material material;

    public Color OriginColor;
    
    public Rigidbody rb;

    public ParticleSystem DeathEffect;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        DeathEffect = transform.Find("DeathEffect").GetComponent<ParticleSystem>();
        isAlive = true;
        Hp = MaxHp;
    }

    public void DefenseSet(float result, float time)
    {
        StartCoroutine(DefenseSetCoru(result, time));
    }

    IEnumerator DefenseSetCoru(float result, float time)
    {
        Depense += result;
        yield return new WaitForSeconds(time);
        Depense -= result;
    }

    public void SpeedSet(float result, float time)
    {
        StartCoroutine(SpeedSetCoru(result, time));
    }

    IEnumerator SpeedSetCoru(float result, float time)
    {
        Speed += result;
        yield return new WaitForSeconds(time);
        Speed -= result;
    }

    public virtual bool Damage(float damage)
    {
        if (!isAlive)
            return false;

        damage *= 1 - (Depense * 0.01f) ;

        Hp -= damage;

        

        if(Hp <= 0)
        {
            isAlive = false;
            Hp = 0;

            Death();

            return true;
        }

        return false;
    }

    public virtual void Heal(float heal)
    {
        if(!isAlive) return;

        Hp += heal;

        if(Hp > MaxHp)
        {
            Hp = MaxHp;
        }
    }

    public virtual void Death()
    {

    }
    

}
