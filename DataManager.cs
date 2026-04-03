using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class PartInfo
{
    public string Name;
    [TextArea]
    public string Description;
    public Sprite Icon;
    public Color Color;
    public int Price;
    public int UpGrade;
    public int UpgradePrice;
    [SerializeField]
    public List<DoubleList> Resoult = new List<DoubleList>();
}


[System.Serializable]
public class DoubleList
{
    public string Name;
    public List<float> resoult = new List<float>();
}

[System.Serializable]
public class RankData
{
    public string Name;
    public int Score;

    public RankData(string name, int score)
    {
        Name = name;
        Score = score;
    }
}

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    [SerializeField]
    public List<RankData> Rank = new List<RankData>();

    public Slider BgmSlider;
    public Slider SfxSlider;
    

    public float ClearTime;
    public int Coin;
    public int Score;
    public int Stage;
    public int CurrentPart1;
    public int CurrentPart2;
    public float Part1Cooltime;
    public float Part2Cooltime;

    public int HpPrice;
    public int SpeedPrice;
    public int DependPrice;
    public int InventoryPrice;

    public int HpUpGrade;
    public int SpeedUpGrade;
    public int DependUpGrade;
    public int InvenUpGrade;


    [Header("아이템 정보")]
    [SerializeField]
    public List<UseItem> item = new List<UseItem>();

    [Header("파츠 정보")]
    [SerializeField]
    public List<PartInfo> Parts = new List<PartInfo>();

    public bool First = true;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }

    

    private void Update()
    {
        if(Part1Cooltime > 0)
        {
            Part1Cooltime -= Time.deltaTime;
        }

        if (Part2Cooltime > 0)
        {
            Part2Cooltime -= Time.deltaTime;
            
        } 

        if(GameManager.Instance.CurrentState == State.EnemyWar || GameManager.Instance.CurrentState == State.BossWar)
        {
            ClearTime += Time.deltaTime;
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            transform.GetChild(0).gameObject.SetActive(!transform.GetChild(0).gameObject.activeSelf);
        }

        
    }

    private void Start()
    {
        SfxSlider.value = PlayerPrefs.GetFloat("Sfx", 1f);
        BgmSlider.value = PlayerPrefs.GetFloat("Bgm", 1f);

        SfxSlider.onValueChanged.AddListener(SliderValueSet);
    }

    public void SliderValueSet(float value)
    {
        PlayerPrefs.SetFloat("Sfx", SfxSlider.value);
        PlayerPrefs.SetFloat("Bgm", BgmSlider.value);
        PlayerPrefs.Save();
    }

    

    public void RankAdd(string name, int Score)
    {
        RankData rank = new RankData(name, Score);

        Rank.Add(rank);

        Rank = RankSet();
    }

    public void LoadRank()
    {
        List<RankData> ranks = new List<RankData>();

        for(int i = 0; i < 5; i++)
        {
            if(PlayerPrefs.HasKey("Rank_n" + i))
            {
                ranks.Add(new RankData(PlayerPrefs.GetString("Rank_n" + i), PlayerPrefs.GetInt("Rank_s" + i)));
            }
        }

        Rank = ranks;

        Rank = RankSet();
    }


    public List<RankData> RankSet()
    {
        List<RankData> ranks = new List<RankData>(Rank);

        ranks.Sort((a, b) => b.Score.CompareTo(a.Score));

        if (ranks.Count > 5)
        {
            ranks.RemoveRange(5, ranks.Count - 5);
        }

        for(int i= 0; i < ranks.Count; i++)
        {
            PlayerPrefs.SetString("Rank_n" + i, ranks[i].Name);
            PlayerPrefs.SetInt("Rank_s" + i, ranks[i].Score);
        }

        PlayerPrefs.Save();

        Debug.Log("랭크 정렬 완료");

        return ranks;
    }

    public void SaveData()
    {
        PlayerPrefs.SetInt("Coin", Coin);
        PlayerPrefs.SetInt("Stage", Stage);
        PlayerPrefs.SetInt("Score", Score);
        PlayerPrefs.SetFloat("ClearTime", ClearTime);
        PlayerPrefs.SetInt("Inven", InvenUpGrade);



        for(int i = 0; i < GameManager.Instance.UseItems.Count; i++)
        {
            string name = $"UseItem" + i;
            PlayerPrefs.SetInt(name, (int)GameManager.Instance.UseItems[i].Item);
        }

        PlayerPrefs.SetInt("Part1", CurrentPart1);
        PlayerPrefs.SetInt("Part2", CurrentPart2);

        for (int i = 0; i < Parts.Count; i++)
        {
            string name = $"Parts" + i;
            PlayerPrefs.SetInt(name, Parts[i].UpGrade);
        }

        for (int i = 0; i < 3; i++)
        {
            string name = $"Status" + i;
            switch(i)
            {
                case 0: PlayerPrefs.SetInt(name, DataManager.instance.HpUpGrade); break;
                case 1: PlayerPrefs.SetInt(name, DataManager.instance.SpeedUpGrade); break;
                case 2: PlayerPrefs.SetInt(name, DataManager.instance.DependUpGrade); break;
            }
        }

        PlayerPrefs.Save();

        Debug.Log("데이터 저장 완료");
    }


    public void SaveDelete()
    {
        PlayerPrefs.DeleteAll();

        Rank = new List<RankData>();
    }

    public void LoadData()
    {
        Coin = PlayerPrefs.GetInt("Coin");
        Stage = PlayerPrefs.GetInt("Stage");
        ClearTime = PlayerPrefs.GetFloat("ClearTime");
        Score = PlayerPrefs.GetInt("Score");
        InvenUpGrade = PlayerPrefs.GetInt("Inven");
        
        for(int i = 0; i < 10; i++)
        {
            if (PlayerPrefs.HasKey("UseItem" + i))
            {
                GameManager.Instance.UseItems.Add(item[PlayerPrefs.GetInt("UseItem" + i)]);
            }
        }

        CurrentPart1 = PlayerPrefs.GetInt("Part1");
        CurrentPart2 = PlayerPrefs.GetInt("Part2");

        for (int i = 0; i < Parts.Count; i++)
        {
            if (PlayerPrefs.HasKey("Parts" + i))
            {
                Parts[i].UpGrade = PlayerPrefs.GetInt("Parts" + i);

            }
        }

        for (int i = 0; i < 3; i++)
        {
            if (PlayerPrefs.HasKey("Status" + i))
            {
                switch (i)
                {
                    case 0: HpUpGrade = PlayerPrefs.GetInt("Status" + i); break;
                    case 1: SpeedUpGrade = PlayerPrefs.GetInt("Status" + i); break;
                    case 2: DependUpGrade = PlayerPrefs.GetInt("Status" + i); break;
                }

            }
        }


        Debug.Log("데이터 로드 완료");
    }

}
