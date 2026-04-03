using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class StoreManager : MonoBehaviour
{
    public static StoreManager instance;

    public GameObject PartsParent;
    public GameObject PartsEqParent;
    public Text PartDes;
    public Text PartCoolText;
    public List<UIholding> uIholdings = new List<UIholding>();

    public List<StoreUI> AirFrames = new List<StoreUI> ();

    public List<StoreUI> Parts = new List<StoreUI> ();

    public List<StoreUI> PartsEq = new List<StoreUI>();

    public GameObject PartsPrefab;

    public GameObject PartsEqPrefab;

    public GameObject BossShadow;

    public GameObject BossText;

    public GameObject FieldEffect;

    public GameObject MapText;

    public bool Use = false;

    public Text Messages;
    private Coroutine MessageCoroutine;

    public Text Coin;

    public void Message(string message, int num = 0)
    {
        if (MessageCoroutine != null)
        {
            StopCoroutine(MessageCoroutine);
        }


        MessageCoroutine = StartCoroutine(MeesageWrite(Messages, message));
    }

    IEnumerator MeesageWrite(Text text, string message)
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


    public void NextScene()
    {
        DataManager.instance.SaveData();
        if (DataManager.instance.Stage > 3)
        {
            SceneManager.LoadScene("Ending");
            GameManager.Instance.StateChange(State.Title);
        } else
        {

            SceneManager.LoadScene("Stage" + (DataManager.instance.Stage + 1));
            GameManager.Instance.StateChange(State.EnemyWar);
        }
    }
    public void Start()
    {
        SfxManager.instance.PlayBgm("상점");

        DataManager.instance.SaveData();

        for (int i = 0; i <= 2; i++)
        {
            if( i == DataManager.instance.Stage)
            {
                BossShadow.transform.GetChild(i).gameObject.SetActive(true);
            } else
            {
                BossShadow.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        for (int i = 0; i <= 2; i++)
        {
            if (i == DataManager.instance.Stage)
            {
                BossText.transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                BossText.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        for (int i = 0; i <= 2; i++)
        {
            if (i == DataManager.instance.Stage)
            {
                FieldEffect.transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                FieldEffect.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        for (int i = 0; i <= 2; i++)
        {
            if (i == DataManager.instance.Stage)
            {
                MapText.transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                MapText.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < DataManager.instance.Parts.Count; i++)
        {
            int index = i;
            GameObject obj = Instantiate(PartsPrefab, PartsParent.GetComponent<RectTransform>().position, Quaternion.Euler(0f, 0f, 0f), PartsParent.GetComponent<RectTransform>());
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localPosition = Vector3.zero;


            StoreUI storeUI = new StoreUI();
            storeUI.Icon = obj.transform.GetChild(0).GetComponent<Image>();
            storeUI.Name = obj.transform.GetChild(1).GetComponent<Text>();
            storeUI.Price = obj.transform.GetChild(2).GetComponent<Text>();

            UIholding uIholding = obj.GetComponent<UIholding>();
            if (uIholding != null)
            {
                uIholding.num = index;
                uIholding.Type = UIType.Part;
                uIholding.Texts.Add(PartDes);
                uIholding.Texts.Add(PartCoolText);
                uIholdings.Add(uIholding);
            }
            else
            {
                Debug.Log("읎어요");
            }


            Parts.Add(storeUI);

            Button objbtn = obj.GetComponent<Button>();
            objbtn.onClick.AddListener(() => UpGradeParts(index));

        }

        for (int i = 0; i < DataManager.instance.Parts.Count; i++)
        {
            int index = i;
            GameObject obj = Instantiate(PartsEqPrefab, PartsEqParent.GetComponent<RectTransform>().position, Quaternion.Euler(0f, 0f, 0f), PartsEqParent.GetComponent<RectTransform>());
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localPosition = Vector3.zero;




            StoreUI storeUI = new StoreUI();
            storeUI.Icon = obj.transform.GetChild(0).GetComponent<Image>();
            storeUI.Name = obj.transform.GetChild(1).GetComponent<Text>();

            Transform grid = obj.transform.Find("grid");

            Button Eq = grid.transform.GetChild(0).GetComponent<Button>();
            Eq.onClick.AddListener(() => PartEquip(index));

            Button UnEq = grid.transform.GetChild(1).GetComponent<Button>();
            UnEq.onClick.AddListener(() => PartUnEquip(index));

            PartsEq.Add(storeUI);

        }


        Use = true;
    }

    public void PartEquip(int num)
    {
        if (DataManager.instance.Parts[num].UpGrade <= 0)
        {
            Message($"{DataManager.instance.Parts[num].Name} 파츠는 아직 구매하지 않으셨습니다.");
            SfxManager.instance.PlaySfx("UI취소");
            return;
        }

        if (DataManager.instance.CurrentPart1 < 0 && DataManager.instance.CurrentPart2 != num)
        {
            DataManager.instance.CurrentPart1 = num;
            Message($"퀵슬롯 1에 {DataManager.instance.Parts[num].Name}을(를) 장착하였습니다.");
            SfxManager.instance.PlaySfx("파츠장착");
            return;
        
        } else if(DataManager.instance.CurrentPart2 < 0 && DataManager.instance.CurrentPart1 != num)
        {

            DataManager.instance.CurrentPart2 = num;
            Message($"퀵슬롯 2에 {DataManager.instance.Parts[num].Name}을(를) 장착하였습니다.");
            SfxManager.instance.PlaySfx("파츠장착");
            return;
        } else
        {
            Message($"해당 파츠는 장착할 수 없습니다..");
            SfxManager.instance.PlaySfx("UI취소");
            return;
        }
    }

    public void PartUnEquip(int num)
    {
        if (DataManager.instance.CurrentPart1 == num)
        {
            DataManager.instance.CurrentPart1 = -1;
            Message($"퀵슬롯 1에 {DataManager.instance.Parts[num].Name}을(를) 장착 해제 하였습니다.");
            SfxManager.instance.PlaySfx("해제");
            return;
        }
        else if (DataManager.instance.CurrentPart2 == num)
        {
            DataManager.instance.CurrentPart2 = -1;
            Message($"퀵슬롯 2에 {DataManager.instance.Parts[num].Name}을(를) 장착 해제 하였습니다.");
            SfxManager.instance.PlaySfx("해제");
            return;
        }
        else
        {
            Message($"퀵슬롯에 장착한 파츠가 없습니다.");
            SfxManager.instance.PlaySfx("UI취소");
            return;
        }
    }


    public void BuyAir(int i)
    {

        switch(i)
        {
            case 0:
                if(DataManager.instance.HpUpGrade < 10)
                {
                    if(DataManager.instance.Coin >= DataManager.instance.HpPrice)
                    {
                        Message($"HP 업그레이드 ! {DataManager.instance.HpUpGrade} -> {++DataManager.instance.HpUpGrade}");
                        DataManager.instance.Coin -= DataManager.instance.HpPrice;
                        SfxManager.instance.PlaySfx("UI기본");
                    } else
                    {
                        Message("골드가 부족합니다.");
                        SfxManager.instance.PlaySfx("UI취소");
                    }
                } else
                {
                    Message("이미 최대 레벨입니다.");
                    SfxManager.instance.PlaySfx("UI취소");
                }
                break;
            case 1:
                if (DataManager.instance.SpeedUpGrade < 10)
                {
                    if (DataManager.instance.Coin >= DataManager.instance.SpeedPrice)
                    {
                        Message($"SPD 업그레이드 ! {DataManager.instance.SpeedUpGrade} -> {++DataManager.instance.SpeedUpGrade}");
                        DataManager.instance.Coin -= DataManager.instance.SpeedPrice;
                        SfxManager.instance.PlaySfx("UI기본");
                    }
                    else
                    {
                        Message("골드가 부족합니다.");
                        SfxManager.instance.PlaySfx("UI취소");
                    }
                }
                else
                {
                    Message("이미 최대 레벨입니다.");
                    SfxManager.instance.PlaySfx("UI취소");
                }
                break;
            case 2:
                if (DataManager.instance.DependUpGrade < 10)
                {
                    if (DataManager.instance.Coin >= DataManager.instance.DependPrice)
                    {
                        Message($"DEF 업그레이드 ! {DataManager.instance.DependUpGrade} -> {++DataManager.instance.DependUpGrade}");
                        DataManager.instance.Coin -= DataManager.instance.DependPrice;
                        SfxManager.instance.PlaySfx("UI기본");
                    }
                    else
                    {
                        Message("골드가 부족합니다.");
                        SfxManager.instance.PlaySfx("UI취소");
                    }
                }
                else
                {
                    Message("이미 최대 레벨입니다.");
                    SfxManager.instance.PlaySfx("UI취소");
                }
                break;
            case 3:
                if (DataManager.instance.InvenUpGrade < 5)
                {
                    if (DataManager.instance.Coin >= DataManager.instance.InventoryPrice)
                    {
                        Message($"INVEN 업그레이드 ! {DataManager.instance.InvenUpGrade} -> {++DataManager.instance.InvenUpGrade}");
                        DataManager.instance.Coin -= DataManager.instance.InventoryPrice;
                        SfxManager.instance.PlaySfx("UI기본");
                    }
                    else
                    {
                        Message("골드가 부족합니다.");
                        SfxManager.instance.PlaySfx("UI취소");
                    }
                }
                else
                {
                    Message("이미 최대 레벨입니다.");
                    SfxManager.instance.PlaySfx("UI취소");
                }
                break;
        }
    }

    public void UpGradeParts(int i)
    {
        Debug.Log(i);
        if (DataManager.instance.Parts[i].UpGrade >= 5)
        {
            Message("해당 파츠는 최대 레벨입니다.");
            SfxManager.instance.PlaySfx("UI취소");
            return;
        }

        if(DataManager.instance.Parts[i].UpGrade == 0)
        {
            if(DataManager.instance.Coin >= DataManager.instance.Parts[i].Price)
            {
                Message($"{DataManager.instance.Parts[i].Name} 파츠를 구매 하였습니다!");
                DataManager.instance.Parts[i].UpGrade++;
                DataManager.instance.Coin -= DataManager.instance.Parts[i].Price;
                SfxManager.instance.PlaySfx("UI기본");
            } else
            {
                Message("골드가 부족합니다.");
                SfxManager.instance.PlaySfx("UI취소");
            }
        } else
        {
            if (DataManager.instance.Coin >= DataManager.instance.Parts[i].UpgradePrice)
            {
                Message($"{DataManager.instance.Parts[i].Name} 파츠를 업그레이드 하였습니다!");
                DataManager.instance.Parts[i].UpGrade++;
                DataManager.instance.Coin -= DataManager.instance.Parts[i].UpgradePrice;
                SfxManager.instance.PlaySfx("UI기본");
            }
            else
            {
                Message("골드가 부족합니다.");
                SfxManager.instance.PlaySfx("UI취소");
            }
        }

        SetTextParts(i);
    }

    private void Update()
    {
        Coin.text = DataManager.instance.Coin + "G";

        if (!Use)
            return;
        AirFrames[0].Name.text = $"HP Lv. {DataManager.instance.HpUpGrade}";
        AirFrames[0].Price.text = $"{DataManager.instance.HpPrice}G";

        AirFrames[1].Name.text = $"SPD Lv. {DataManager.instance.SpeedUpGrade}";
        AirFrames[1].Price.text = $"{DataManager.instance.SpeedPrice}G";


        AirFrames[2].Name.text = $"DEF Lv. {DataManager.instance.DependUpGrade}";
        AirFrames[2].Price.text = $"{DataManager.instance.DependPrice}G";


        AirFrames[3].Name.text = $"INVEN Lv. {DataManager.instance.InvenUpGrade}";
        AirFrames[3].Price.text = $"{DataManager.instance.InventoryPrice}G";


        for (int i = 0; i < DataManager.instance.Parts.Count; i++)
        {
            Parts[i].Icon.sprite = DataManager.instance.Parts[i].Icon ;
            Parts[i].Icon.color = DataManager.instance.Parts[i].Color;
            if(DataManager.instance.Parts[i].UpGrade == 0)
            {
                Parts[i].Price.text = $"BUY {DataManager.instance.Parts[i].Price}G";
                Parts[i].Name.text = DataManager.instance.Parts[i].Name;
            } else
            {
                Parts[i].Price.text = $"UpGrade {DataManager.instance.Parts[i].UpgradePrice}G";
                Parts[i].Name.text = $"{DataManager.instance.Parts[i].Name} LV. {DataManager.instance.Parts[i].UpGrade}";
            }
        }


        for (int i = 0; i < DataManager.instance.Parts.Count; i++)
        {
            PartsEq[i].Icon.sprite = DataManager.instance.Parts[i].Icon;
            PartsEq[i].Icon.color = DataManager.instance.Parts[i].Color;
            if (DataManager.instance.Parts[i].UpGrade == 0)
            {
                PartsEq[i].Name.text = DataManager.instance.Parts[i].Name;
            }
            else
            {
                PartsEq[i].Name.text = $"{DataManager.instance.Parts[i].Name} LV. {DataManager.instance.Parts[i].UpGrade}";
            }
        }

    }

    public void SetTextParts(int num)
    {

            uIholdings[num].TextSet();
        
    }
}
