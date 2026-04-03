using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;

    public Boss BossPrefab;
    public string BossName; 

    public List<Transform> SpawnPoints = new List<Transform>();

    public List<GameObject> Enemys = new List<GameObject>();

    public List<Enemy> AliveEnemys = new List<Enemy>();

    public List<int> StartEnemys = new List<int>();

    public List<GameObject> Stones = new List<GameObject>();

    public float SpawnsTimeMin = 5f;
    public float SpawnTimeMax = 10f;
    public float CurrentNeedSpawnTime = 0;
    public float SpawnTIme;

    public float StoneSpawnsTimeMin = 9f;
    public float StoneSpawnTimeMax = 13f;
    public float CurrentNeedStoneSpawnTime = 0;
    public float StoneSpawnTIme;

    public int SpawnEnemysMin = 1;
    public int SpawnEnemysMax = 3;

    public List<int> NeedNextWaveEnemy = new List<int>();
    public int MaxEnemys = 20;

    public int MaxWave = 1;
    public int CurrentWave;

    public bool BossEnemySpawn = false;

    public int FieldEffectNum = -1;
    public List<int> FieldEffectNums = new List<int>();

    public float FieldLifeTIme = 25f;

    public bool Stange;

    

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void SetFieldEffect()
    {
        int Rand = Random.Range(0, FieldEffectNums.Count);

        FieldEffectNum = FieldEffectNums[Rand];

        switch(FieldEffectNum)
        {
            case 0:
                Debug.Log("정전");
                StartCoroutine(UImanager.instance.PlayerUIStop(7f));
                UImanager.instance.Message("정전 - UI가 잠시 비활성화됩니다.", 1);
                break;
            case 1:
                Debug.Log("괴전파");
                UImanager.instance.Message("괴전파 - 파츠들의 쿨타임이 증가합니다.", 1);
                DataManager.instance.Part1Cooltime += 10f;
                DataManager.instance.Part2Cooltime += 10f;
                break;
            case 2:
                Debug.Log("안개");
                UImanager.instance.Message("안개 - 가시거리가 감소합니다.", 1);
                StartCoroutine(GameManager.Instance.Angae(4f));
                break;
            case 3:
                Debug.Log("변형");
                UImanager.instance.Message("변형 파동 - 미사일의 크기가 변합니다.", 1);
                StartCoroutine(StopScale(8f));
                break;

        }
    }

    IEnumerator StopScale(float time)
    {
        Stange = true;

        yield return new WaitForSeconds(time);

        Stange = false;
    }
    private void Start()
    {
        for (int i = 0; i < StartEnemys.Count; i++)
        {
            int randSpawn = Random.Range(0, SpawnPoints.Count);



            Enemy enemy = Instantiate(Enemys[StartEnemys[i]].GetComponent<Enemy>(), SpawnPoints[randSpawn].position, Quaternion.identity, transform);
            AliveEnemys.Add(enemy);
        }

        GameManager.Instance.StateChange(State.EnemyWar);
        GameManager.Instance.EnemySpawnStop = false;

        CurrentNeedSpawnTime = Random.Range(SpawnsTimeMin, SpawnTimeMax);
        CurrentNeedStoneSpawnTime = Random.Range(StoneSpawnsTimeMin, StoneSpawnTimeMax);

        SfxManager.instance.PlayBgm("스테이지");
    }

    private void Update()
    {
        FieldLifeTIme -= Time.deltaTime;

       if(FieldLifeTIme <= 0 && DataManager.instance.Stage >= 1)
        {
            float RandTime = Random.Range(25f, 35f);
            FieldLifeTIme = RandTime;
            SetFieldEffect();
        }

        SpawnTIme += Time.deltaTime;
        if(Stones.Count > 0) 
            StoneSpawnTIme += Time.deltaTime;

        if (SpawnTIme >= CurrentNeedSpawnTime && (GameManager.Instance.CurrentState == State.EnemyWar || (GameManager.Instance.CurrentState == State.BossWar && BossEnemySpawn)) && !GameManager.Instance.EnemySpawnStop)
        {
            SpawnTIme = 0;
            int Rand = Random.Range(SpawnEnemysMin, SpawnEnemysMax + 1);
            EnemySpawn(Rand);
            CurrentNeedSpawnTime = Random.Range(SpawnsTimeMin, SpawnTimeMax);
        }
        
        if(StoneSpawnTIme >= CurrentNeedStoneSpawnTime )
        {
            StoneSpawnTIme = 0;
            int Rand = Random.Range(0, Stones.Count);
            int randSpawn = Random.Range(0, SpawnPoints.Count);
            Instantiate(Stones[Rand], SpawnPoints[randSpawn].position, Quaternion.identity, transform);

            CurrentNeedStoneSpawnTime = Random.Range(StoneSpawnsTimeMin, StoneSpawnTimeMax);


        }
    }

    public void EnemySpawn(int UnitCount)
    {
        for(int i = 0; i < UnitCount; i++)
        {
            if (AliveEnemys.Count >= MaxEnemys)
                return;

            int randSpawn = Random.Range(0, SpawnPoints.Count);
            int randEnemy = Random.Range(0, Enemys.Count);



            Enemy enemy = Instantiate(Enemys[randEnemy].GetComponent<Enemy>(), SpawnPoints[randSpawn].position, Quaternion.identity, transform);
            AliveEnemys.Add(enemy);
        }
    }

    public void NextWavesEnemyCount()
    {
        if (GameManager.Instance.CurrentState != State.EnemyWar || GameManager.Instance.EnemySpawnStop)
            return;

        if (CurrentWave > NeedNextWaveEnemy.Count)
            return;

        NeedNextWaveEnemy[CurrentWave]--;
        if (NeedNextWaveEnemy[CurrentWave] <= 0)
        {
            CurrentWave++;
            if(CurrentWave == MaxWave && GameManager.Instance.CurrentState == State.EnemyWar)
            {
                Debug.Log("스테이지 클리어");
                GameManager.Instance.EnemySpawnStop = true;

                if(!BossEnemySpawn)
                {
                    AllEnemyDeath();
                } else
                {
                    GameManager.Instance.EnemySpawnStop = false;
                }


                StartCoroutine(BossSpawn(3f));
            } else
            {

            }
        }
    }


    IEnumerator BossSpawn(float time)
    {
        yield return new WaitForSeconds(time);
        Boss boss = Instantiate(BossPrefab, BossPrefab.GetComponent<Boss>().MovePoints[0], Quaternion.identity);

        if (DataManager.instance.Stage < 2)
        {
            SfxManager.instance.PlayBgm("보스");
        }
        else if (DataManager.instance.Stage == 2)
        {
            SfxManager.instance.PlayBgm("막보스");
        }

        Vector3 targetpos = boss.MovePoints[0];
        Vector3 StartPos = new Vector3(0, 0, 25f);
        float IntroTime = 0;
        while(IntroTime < 2)
        {
            IntroTime += Time.deltaTime;
            float t = Mathf.Clamp01(IntroTime / 2);
            boss.transform.position = Vector3.Lerp(StartPos, targetpos, t);
        }
        boss.transform.position = targetpos;

        CameraManager.instance.Shake(3f, 1.5f);

        UImanager.instance.BossWaringShow();

        yield return new WaitForSeconds(1.5f);

        UImanager.instance.BossSliderAni();



        GameManager.Instance.StateChange(State.BossWar);


    }
    void AllEnemyDeath()
    {
        Enemy[] Enemys = FindObjectsOfType<Enemy>();
        foreach(Enemy enemy in Enemys)
        {
            if(enemy != null &&  enemy.isAlive) 
                enemy.Damage(9999f);
        }
    }

    public void BossClear()
    {
        Debug.Log("보스 클리어!!");

        UImanager.instance.BossSliderAni(false);

        AllEnemyDeath();

        if(DataManager.instance.Stage + 1 <= 3)
            StartCoroutine(StoreGo(3f));

        DataManager.instance.Stage++;

        if(DataManager.instance.Stage >= 3)
        {
            SceneManager.LoadScene("Ending");
            GameManager.Instance.StateChange(State.Title);
        }

        
    }

    public IEnumerator StoreGo(float time)
    {
        yield return new WaitForSeconds(time);

        SceneManager.LoadScene("Store");

        GameManager.Instance.StateChange(State.Store);
    }


}
