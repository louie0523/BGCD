using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UImanager : MonoBehaviour
{
    public static UImanager instance;

    [Header("보스")]
    public GameObject BossUI;
    public Slider BossSlider;
    public Text BossText;
    public Animator BossWaring;



    [Header("기초 UI")]
    public List<Text> Messeges = new List<Text>();
    private Coroutine MessageCoroutine;
    public Text Score;

    [Header("Plyar")]
    public Slider PlayerHpSlider;
    public GameObject GameOver;

    [Header("인벤토리")]
    public GameObject InventoryParent;
    public Image CurrentItemIcon;
    public Text CurrentItemName;
    public Image CurrentLifeTimeImage;
    public Text CurrentItemLifeTime;
    public Image NextItemIcon;

    [Header("장착중 파츠")]
    public GameObject PartParent;
    public Image PartIcon1;
    public Text PartName1;
    public Image PartCool1;
    public Text PartCoolTime1;
    public Image PartIcon2;
    public Text PartName2;
    public Image PartCool2;
    public Text PartCoolTime2;

    [Header("플레이어 폭탄")]
    public Image BombCool;
    public Text BombCoolText;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
        }
    }

    public void RetryStageLoad()
    {
        SfxManager.instance.PlaySfx("로드");
        SceneManager.LoadScene("Store");
        GameManager.Instance.StateChange(State.Store);
        DataManager.instance. LoadData();
        Time.timeScale = 1f;
    }

    public IEnumerator PlayerUIStop(float time)
    {
        PlayerHpSlider.gameObject.SetActive(false);
        InventoryParent.SetActive(false);
        PartParent.SetActive(false);

        yield return new WaitForSeconds(time);

        PlayerHpSlider.gameObject.SetActive(true);
        InventoryParent.SetActive(true);
        PartParent.SetActive(true);

    }

    private void Update()
    {
        PlayerHpSlider.value = Player.Instance.Hp / Player.Instance.MaxHp;

        Score.text =  $"SCORE : {DataManager.instance.Score}";

        if (Boss.Instance != null)
        {
            BossSlider.value = Boss.Instance.Hp / Boss.Instance.MaxHp;
            BossText.text = StageManager.Instance.BossName;
        }

        if(GameManager.Instance.CurrentBoombCooltime > 0)
        {
            BombCool.gameObject.SetActive(false);
            BombCool.fillAmount = GameManager.Instance.CurrentBoombCooltime / GameManager.Instance.BombCoolitme;
            BombCoolText.text = $"{(int)GameManager.Instance.CurrentBoombCooltime}s";
        } else
        {
            BombCool.gameObject.SetActive(false);
        }

        if(GameManager.Instance.UseItems.Count > 0)
        {
            CurrentItemIcon.sprite = GameManager.Instance.UseItems[0].Icon;
            CurrentItemIcon.color = GameManager.Instance.UseItems[0].Color;
            CurrentItemName.text = GameManager.Instance.UseItems[0].Id;
            CurrentItemName.color = GameManager.Instance.UseItems[0].Color;
            CurrentLifeTimeImage.gameObject.SetActive(true);
            CurrentLifeTimeImage.fillAmount = GameManager.Instance.CurLifetime / GameManager.Instance.UseItems[0].LifeTIme;
            CurrentItemLifeTime.text = $"{(GameManager.Instance.UseItems[0].LifeTIme - GameManager.Instance.CurLifetime).ToString("F0")}s";
            if(GameManager.Instance.UseItems.Count > 1)
            {
                NextItemIcon.gameObject.SetActive(true);
                NextItemIcon.sprite = GameManager.Instance.UseItems[1].Icon;
                NextItemIcon.color = GameManager.Instance.UseItems[1].Color;
            }
        } else
        {
            CurrentItemIcon.color = new Color(0f, 0f, 0f, 0f);
            CurrentItemName.color = new Color(0f, 0f, 0f, 0f);
            NextItemIcon.gameObject.SetActive(false);
            CurrentItemLifeTime.transform.parent.gameObject.SetActive(false);
        }

        if(DataManager.instance.CurrentPart1 >= 0)
        {
            PartIcon1.sprite = DataManager.instance.Parts[DataManager.instance.CurrentPart1].Icon;
            PartIcon1.color = DataManager.instance.Parts[DataManager.instance.CurrentPart1].Color;
            PartName1.text = DataManager.instance.Parts[DataManager.instance.CurrentPart1].Name;
            PartName1.color = DataManager.instance.Parts[DataManager.instance.CurrentPart1].Color;
            if(DataManager.instance.Part1Cooltime > 0)
            {
                PartCool1.gameObject.SetActive(true);
                PartCool1.fillAmount = DataManager.instance.Part1Cooltime / DataManager.instance.Parts[DataManager.instance.CurrentPart1].Resoult[0].resoult[DataManager.instance.Parts[DataManager.instance.CurrentPart1].UpGrade];
                PartCoolTime1.text = $"{(int)DataManager.instance.Part1Cooltime}s";
            } else
            {
                PartCool1.gameObject.SetActive(false);
            }
        } else
        {
            PartIcon1.color = new Color(0f, 0f, 0f, 0f);
            PartName1.color = new Color(0f, 0f, 0f, 0f);
            PartCool1.gameObject.SetActive(false);
        }

        if (DataManager.instance.CurrentPart2 >= 0)
        {
            PartIcon2.sprite = DataManager.instance.Parts[DataManager.instance.CurrentPart2].Icon;
            PartIcon2.color = DataManager.instance.Parts[DataManager.instance.CurrentPart2].Color;
            PartName2.text = DataManager.instance.Parts[DataManager.instance.CurrentPart2].Name;
            PartName2.color = DataManager.instance.Parts[DataManager.instance.CurrentPart2].Color;
            if (DataManager.instance.Part2Cooltime > 0)
            {
                PartCool2.gameObject.SetActive(true);
                PartCool2.fillAmount = DataManager.instance.Part2Cooltime / DataManager.instance.Parts[DataManager.instance.CurrentPart2].Resoult[0].resoult[DataManager.instance.Parts[DataManager.instance.CurrentPart2].UpGrade];
                PartCoolTime2.text = $"{(int)DataManager.instance.Part2Cooltime}s";
            }
            else
            {
                PartCool2.gameObject.SetActive(false);
            }
        }
        else
        {
            PartIcon2.color = new Color(0f, 0f, 0f, 0f);
            PartName2.color = new Color(0f, 0f, 0f, 0f);
            PartCool2.gameObject.SetActive(false);
        }


        if (GameManager.Instance.CurrentBoombCooltime > 0)
        {
            BombCool.gameObject.SetActive(true);
            BombCool.fillAmount = GameManager.Instance.CurrentBoombCooltime / GameManager.Instance.BombCoolitme;
            BombCoolText.text = $"{(int)GameManager.Instance.CurrentBoombCooltime}s";
        }
        else
        {
            BombCool.gameObject.SetActive(false);
        }

    }

    public void BossWaringShow()
    {
        BossWaring.SetTrigger("Show");
    }

    public void BossSliderAni(bool Start = true)
    {
        BossSlider.gameObject.SetActive(true);
        string Trigger = "Show";
        if (!Start)
        {
            Trigger = "Hide";
        }

        BossSlider.GetComponent<Animator>().SetTrigger(Trigger);

        
    }

    public void Message(string message, int num = 0)
    {
        if(MessageCoroutine != null)
        {
            StopCoroutine(MessageCoroutine);
        }


        MessageCoroutine = StartCoroutine(MeesageWrite(Messeges[num], message));
    }

    IEnumerator MeesageWrite(Text text,string message)
    {
        text.color = Color.white;
        text.text = message;
        yield return new WaitForSeconds(2.5f);
        float time = 0f;
        while (time < 0.5f)
        {
            time += Time.deltaTime;
            text.color = new Color(1f, 1f, 1f, 1f - time / 0.5f);
            yield return null;
        }
        text.color = new Color(1f, 1f, 1f, 0f);
        text.text = "";
    }
}
