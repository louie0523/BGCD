using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public enum State
{
    Title,
    EnemyWar,
    BossWar,
    Store,
}

[System.Serializable]
public class UseItem
{
    public Item Item;
    public float Weight = 0f;
    public float LifeTIme = 30;
    public Sprite Icon;
    public Color Color;
    public string Id;
}



public class GameManager : MonoBehaviour
{
    public static GameManager Instance;




    public State CurrentState;

    public ItemObject ItemPrefab;
    public List<UseItem> UseItems = new List<UseItem>();
    public int MaxItems = 3;
    public float MaxWeight = 75f;
    public float CurLifetime;

    public float BombCoolitme;
    public float CurrentBoombCooltime;
    public GameObject BombParent;

    public float Parry;

    public GameObject BlackHole;
    public ChromaticAberration ChromaticAberration;
    public ColorGrading ColorGrading;
    public Vignette Vignette;
    public bool TimeFast;

    public float GameTime = 1f;
    public float ProjectileSpeed = 1f;
    public bool MousePointing = false;

    public bool EnemySpawnStop;




    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
        } else
        {
            Destroy(gameObject);
        }
    }
    

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        BombParent = GameObject.Find("폭탄");

        PostProcessVolume volume = Camera.main.GetComponent<PostProcessVolume>();
        volume.profile.TryGetSettings(out ChromaticAberration);
        volume.profile.TryGetSettings(out ColorGrading);
        volume.profile.TryGetSettings(out Vignette);
    }



    public IEnumerator Angae(float time)
    {
        float t = 0f;
        while ( t < 1) { 
            t += Time.deltaTime;
            Vignette.intensity.value = t;
            yield return null;
        }

        Vignette.intensity.value = 1f;

        yield return new WaitForSeconds(time);

        while (t > 0)
        {
            t -= Time.deltaTime;
            Vignette.intensity.value = t;
            yield return null;
        }

        Vignette.intensity.value = 0f;


    }


    private void Update()
    {
        if (CurrentState == State.Store || CurrentState == State.Title)
            return;

        if(CurrentBoombCooltime > 0)
        {
            CurrentBoombCooltime -= Time.deltaTime;
        } 

        if(UseItems.Count > 0 )
        {
            CurLifetime += Time.deltaTime;

            if(CurLifetime >= UseItems[0].LifeTIme)
            {
                UseItems.RemoveAt(0);
                CurLifetime = 0;
            }

        }

        if(Parry > 0)
        {
            Parry -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if(UseItems.Count <= 0)
            {
                UImanager.instance.Message("인벤토리가 비어있습니다.");
                return;
            }

            InUseItem();


        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            if(CurrentBoombCooltime <= 0)
            {
                UseBomb();
            } else
            {
                UImanager.instance.Message("폭탄이 준비되지 않았습니다.");
            }
        }

        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            if(DataManager.instance.Part1Cooltime <= 0)
            {
                UseParts(0);
            } else
            {
                UImanager.instance.Message($"{DataManager.instance.Parts[DataManager.instance.CurrentPart1].Name} 파츠가 준비되지 않았습니다..");
            }
        } else if(Input.GetKeyDown(KeyCode.E))
        {
            if (DataManager.instance.Part2Cooltime <= 0)
            {
                UseParts(1);
            }
            else
            {
                UImanager.instance.Message($"{DataManager.instance.Parts[DataManager.instance.CurrentPart2].Name} 파츠가 준비되지 않았습니다..");
            }
        }


    }

    void UseParts(int num)
    {
        int currentNum = num == 0 ? DataManager.instance.CurrentPart1 : DataManager.instance.CurrentPart2;

        float cool = 0f;

        switch(currentNum)
        {
            case 0:
                Enemy target = AngleEnemy();
                if(target == null)
                {
                    UImanager.instance.Message("타겟이 존재하지 않습니다.");
                    cool = -1f;
                    break;
                } else
                {
                    Player.Instance.UoDo.gameObject.transform.position = target.transform.position;
                    Player.Instance.UoDo.Play();
                    SfxManager.instance.PlaySfx("강제유도");

                    Projectile[] Allprojectiles = AllProjectile();

                    foreach (Projectile p in Allprojectiles)
                    {
                        if(p.type == ProjectileType.Missile)
                        {
                            if (!p.NotChasing)
                            {
                                p.Target = target.transform;
                            } else
                            {
                                p.transform.Rotate(0f, 180f, 0f);
                                p.GetComponent<Rigidbody>().velocity = p.transform.forward * p.Speed;
                            }
                        } else if(p.type == ProjectileType.Boom)
                        {
                            p.GetComponent<Rigidbody>().velocity = Vector3.zero;
                            p.SetTargetPos();
                            p.StartBoomMove();
                        }

                    }
                }
                break;
            case 1:


                StartCoroutine(TimeStopEffect(DataManager.instance.Parts[currentNum].Resoult[1].resoult[DataManager.instance.Parts[currentNum].UpGrade]));
                SfxManager.instance.PlaySfx("시간정지");
                break;
            case 2:
                Parry += DataManager.instance.Parts[currentNum].Resoult[1].resoult[DataManager.instance.Parts[currentNum].UpGrade];
                var main = Player.Instance.Reflect.main;
                main.duration = DataManager.instance.Parts[currentNum].Resoult[1].resoult[DataManager.instance.Parts[currentNum].UpGrade];
                Player.Instance.Reflect.Play();
                SfxManager.instance.PlaySfx("공격반사");
                break;
            case 3:
                SfxManager.instance.PlaySfx("블랙홀");
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Plane groundPlane = new Plane(Vector3.up, Vector3.zero); 

                if (groundPlane.Raycast(ray, out float distance))
                {
                    Vector3 pos = ray.GetPoint(distance);
                    BlackHole blackHole = Instantiate(BlackHole.GetComponent<BlackHole>(), pos, Quaternion.identity);
                    blackHole.LifeTime = DataManager.instance.Parts[currentNum].Resoult[1].resoult[DataManager.instance.Parts[currentNum].UpGrade];
                }

                break;
            case 4:
                SfxManager.instance.PlaySfx("감속장");
                var shape = Player.Instance.GamSuk.shape;
                shape.radius = DataManager.instance.Parts[currentNum].Resoult[1].resoult[DataManager.instance.Parts[currentNum].UpGrade];
                Player.Instance.GamSuk.Play();
                List<Projectile> Nearprojectiles = GetObjectsInRadius<Projectile>(Player.Instance.transform.position, DataManager.instance.Parts[currentNum].Resoult[1].resoult[DataManager.instance.Parts[currentNum].UpGrade]);
                foreach (Projectile p in Nearprojectiles)
                {
                    StartCoroutine(p.SpeedStopCoroutin(3f, 0.5f));
                }

                break;
            case 5:
                SfxManager.instance.PlaySfx("신경과부화");
                StartCoroutine(SandeBistan(1.5f));
                break;
            case 6:
                SfxManager.instance.PlaySfx("이엠피");
                Player.Instance.Emp.Play();

                List<Enemy> NearEnemy = GetObjectsInRadius<Enemy>(Player.Instance.transform.position, 25f);
                foreach(Enemy e in NearEnemy)
                {
                    e.CurrentAttackTime -= DataManager.instance.Parts[currentNum].Resoult[1].resoult[DataManager.instance.Parts[currentNum].UpGrade];
                }
                break;



        }
        

        if (cool >= 0)
            cool = DataManager.instance.Parts[currentNum].Resoult[0].resoult[DataManager.instance.Parts[currentNum].UpGrade];
        else
            cool = 3;

        if(num == 0)
        {
            DataManager.instance.Part1Cooltime = cool;
        } else
        {
            DataManager.instance.Part2Cooltime = cool;
        }
    }

    IEnumerator TimeStopEffect(float time)
    {
        
        float value = 0f;
        while (value < 0.25f)
        {
            value += Time.deltaTime;
            ColorGrading.saturation.value = value * -400;
            ProjectileSpeed = 1 - (value * 4);
            yield return null;
        }
        ProjectileSpeed = 0f;



        yield return new WaitForSeconds(time);

        value = 0.5f;
        while (value > 0)
        {
            value -= Time.deltaTime;
            ColorGrading.saturation.value = -100 * value * 2;
            yield return null;
        }
        ProjectileSpeed = 1f;
    }

    IEnumerator SandeBistan(float time)
    {
        TimeFast = true;
        float result = 0f;
        while(result < 0.25)
        {
            result += Time.deltaTime;
            ChromaticAberration.intensity.value = result * 4;
            yield return null;
        }





        GameTime = 0.1f;



        yield return new WaitForSeconds(time);


        GameTime = 1f;

        result = 0.25f;
        while (result > 0)
        {
            result -= Time.deltaTime;
            ChromaticAberration.intensity.value = result * 4; 
            yield return null;
        }

        ChromaticAberration.intensity.value = 0f;

        TimeFast = false;

    }



    public static List<T> GetObjectsInRadius<T>(Vector3 center, float radius, int maxCount = 200) where T : Component
    {
        Collider[] coll = Physics.OverlapSphere(center, radius);

        List<T> objects = new List<T>();

        foreach(Collider c in coll)
        {
            if (objects.Count >= maxCount)
                break;

            if(c.GetComponent<T>() != null)
            {
                objects.Add(c.GetComponent<T>());
            }
        }


        return objects;
    }


    public Enemy AngleEnemy()
    {
        Collider[] cols = Physics.OverlapBox(Player.Instance.transform.position + Player.Instance.transform.forward * 10f, new Vector3(10f, 5f, 20f), transform.rotation);

        float distance = 999;
        Enemy enemy = null;
        foreach(Collider c in cols)
        {
            if(c.GetComponent<Enemy>() != null && c.CompareTag("Enemy"))
            {
                if(Vector3.Distance(Player.Instance.transform.position, c.transform.position) <  distance)
                {
                    distance = Vector3.Distance(Player.Instance.transform.position, c.transform.position);
                    enemy = c.GetComponent<Enemy>();
                }
            }
        }

        return enemy; 
    }




    void UseBomb()
    {
        CurrentBoombCooltime = BombCoolitme;

        ParticleSystem[] particleSystems = BombParent.transform.GetComponentsInChildren<ParticleSystem>();

        CameraManager.instance.Shake(3f, 0.25f);

        foreach (ParticleSystem particleSystem in particleSystems)
        {
            float x = Random.Range(-35f, 35f);
            float z = Random.Range(-20f, 20f);
            particleSystem.gameObject.transform.position = new Vector3(x, 0, z);
            particleSystem.Play();
            SfxManager.instance.PlaySfx("폭탄사용");
        }

        Invoke("ProjectileDelete", 0.15f);
    }

    void ProjectileDelete()
    {

        Projectile[] projectiles = AllProjectile();

        foreach (Projectile p in projectiles)
        {
            p.OnSignalReceived();
        }
    }

    Projectile[] AllProjectile()
    {
        Projectile[] projectiles = FindObjectsOfType<Projectile>();

        return projectiles;
    }


    void InUseItem()
    {
        switch(UseItems[0].Item)
        {
            case Item.DefenseUp:
                Player.Instance.DefenseSet(15f, 5f);
                break;
            case Item.SpeedUp:
                Player.Instance.SpeedSet(7f, 7f);
                break;
            case Item.Mujeok:
                Player.Instance.DefenseSet(10000f, 4f);
                break;
        }

        UseItems.RemoveAt(0);
        CurLifetime = 0;

    }
    

    public void GetItem(UseItem item)
    {
        if(item.Item == Item.Heal)
        {
            Player.Instance.Heal(40);
            return;
        }

        if(UseItems.Count >= MaxItems + DataManager.instance.InvenUpGrade)
        {
            UImanager.instance.Message("인벤토리가 가득 찼습니다.");
            return;
        }

        if(MaxWeight + (DataManager.instance.InvenUpGrade * 30) < GetWeight() + item.Weight)
        {
            UImanager.instance.Message("획득하려는 아이템이 너무 무겁습니다!");
            return ;
        }

        UseItems.Add(item);
        if(UseItems.Count == 1)
            CurLifetime = 0;
    }

    float GetWeight()
    {
        float result = 0;
        foreach(UseItem i in UseItems)
        {
            result += i.Weight;
        }

        return result;
    }

    public void StateChange(State state)
    {
        CurrentState = state;

        switch(state)
        {
            case State.Title:

                break;
            case State.EnemyWar:

                break;
            case State.BossWar:

                break;
            case State.Store:

                break;
        }
    }
}
