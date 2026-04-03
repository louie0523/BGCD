using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheat : MonoBehaviour
{
    public static Cheat Instance;

    public bool DebugMode = false;
    public bool Mujuk = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1))
        {
            DebugMode = !DebugMode;
        } else if (Input.GetKeyDown(KeyCode.F2))
        {
            Mujuk = !Mujuk;
        } else if(Input.GetKeyDown(KeyCode.F3))
        {
            Enemy[] Enemys = FindObjectsOfType<Enemy>();
            foreach (Enemy enemy in Enemys)
            {
                if (enemy != null && enemy.isAlive)
                    enemy.Damage(9999f);
            }
        } else if(Input.GetKeyDown(KeyCode.F4))
        {
            DataManager.instance.Coin += 100000;
        } else if (Input.GetKeyDown(KeyCode.F5))
        {
            Enemy[] Enemys = FindObjectsOfType<Enemy>();
            foreach (Enemy enemy in Enemys)
            {
                if (enemy != null && enemy.isAlive)
                    enemy.Damage(9999f);
            }
        }
    }
}
