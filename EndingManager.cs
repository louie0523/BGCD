using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndingManager : MonoBehaviour
{
    public GameObject Rank;
    public GameObject RankInput;

    public InputField InputField;

    public Text Messages;
    private Coroutine MessageCoroutine;

    public bool NickPlese = false;

    public int CurrentScore = 0;

    public GameObject fly;

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


    private void Start()
    {

        SfxManager.instance.PlayBgm("엔딩");
        InputField.characterLimit = 8;
        InputField.onEndEdit.AddListener(RankUp);

        int Bonus = 0;
        if(DataManager.instance.ClearTime < 400f)
        {
            Bonus = (int)(DataManager.instance.Score * 0.3f);
        } else if(DataManager.instance.ClearTime < 700f)
        {
            Bonus = (int)(DataManager.instance.Score * 0.15f);
        } else if (DataManager.instance.ClearTime < 1000f)
        {
            Bonus = (int)(DataManager.instance.Score * 0.1f);
        }
        CurrentScore = (int)(DataManager.instance.Score + Bonus);

        if (DataManager.instance.Rank.Count < 5)
        {
            NickPlese = true;
            RankInput.gameObject.SetActive(true);
            return;
        }

        for (int i = 0; i < DataManager.instance.Rank.Count; i++)
        {
            if (DataManager.instance.Rank[i].Score < CurrentScore)
            {
                NickPlese = true;
                RankInput.gameObject.SetActive(true);
                return;
            }
        }

        StartCoroutine(EndEvnet());

    }

    IEnumerator EndEvnet()
    {
        Animator animator = GetComponent<Animator>();
        animator.SetTrigger("Show");
        yield return new WaitForSeconds(1f);
        fly.GetComponent<Move>().enabled = true;
        fly.GetComponent<Tilt>().enabled = true;
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
        } else
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

    public void RankUp(string value)
    {
        if(value.Length <= 0)
        {
            Message("닉네임이 비어있습니다!");
            return;
        } else
        {
            DataManager.instance.RankAdd(value, CurrentScore);
            Message("랭크에 등록되었습니다!");
            RankInput.gameObject.SetActive(false );
            StartCoroutine(EndEvnet());
        }

    }

    public void GoTitle()
    {
        SceneManager.LoadScene("Title");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
