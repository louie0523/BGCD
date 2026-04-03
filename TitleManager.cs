using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{

    public GameObject LoadUI;
    public GameObject SaveDelete;
    public GameObject Rank;

    void Start()
    {

        SfxManager.instance.PlayBgm("타이틀");

        DataManager.instance.LoadRank();

        if (PlayerPrefs.HasKey("Coin") && DataManager.instance.First)
        {
            DataManager.instance.First = false;
            LoadUI.gameObject.SetActive(true);
            SaveDelete.gameObject.SetActive(true);
            return;
        }

    }

    private void Update()
    {

        if (DataManager.instance.Rank.Count > 0)
        {

            for (int i = 0; i < Rank.transform.childCount; i++)
            {
                if (DataManager.instance.Rank.Count > i)
                {
                    GameObject obj = Rank.transform.GetChild(i).gameObject;
                    obj.SetActive(true);
                    Text Name = obj.transform.GetChild(1).GetComponent<Text>();
                    Text Scroe = obj.transform.GetChild(2).GetComponent<Text>();

                    Name.text = DataManager.instance.Rank[i].Name;
                    Scroe.text = DataManager.instance.Rank[i].Score.ToString();
                }
                else
                {
                    GameObject obj = Rank.transform.GetChild(i).gameObject;
                    obj.SetActive(true);
                    Text Name = obj.transform.GetChild(1).GetComponent<Text>();
                    Text Scroe = obj.transform.GetChild(2).GetComponent<Text>();

                    Name.text = "NULL";
                    Scroe.text = "NULL";
                }
            }
        }
        else
        {
            for (int i = 0; i < Rank.transform.childCount; i++)
            {

                GameObject obj = Rank.transform.GetChild(i).gameObject;
                obj.SetActive(true);
                Text Name = obj.transform.GetChild(1).GetComponent<Text>();
                Text Scroe = obj.transform.GetChild(2).GetComponent<Text>();

                Name.text = "NULL";
                Scroe.text = "NULL";


            }
        }
    }

    public void SaveDeleteBtn()
    {
        DataManager.instance.SaveDelete();
    }

    public void LoadDataGo()
    {
        SfxManager.instance.PlaySfx("로드");
        SceneManager.LoadScene("Store");
        GameManager.Instance.StateChange(State.Store);
        DataManager.instance.LoadData();
    }

    public void GameStart()
    {
        SceneManager.LoadScene("Stage1");
        GameManager.Instance.StateChange(State.EnemyWar);
    }

    public void Exit()
    {
        Application.Quit();
    }

    // Update is called once per frame

}
