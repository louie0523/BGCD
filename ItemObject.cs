using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Item
{
    Heal,
    DefenseUp,
    SpeedUp,
    Mujeok,

}



public class ItemObject : MonoBehaviour 
{
    public int ItemNum;
    public float LifeTime;
    public float currentLifeTIme;
    public bool Get = false;

    public SpriteRenderer Icon;
    public Text ItemName;

    public int Grade = 0;
    public float Weight = 0;

    private void Start()
    {
        SetItem();

    }

    private void Update()
    {
        LifeTime -= Time.deltaTime;
        if(LifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !Get)
        {
            Get = true;
            GameManager.Instance.GetItem(DataManager.instance.item[ItemNum]);
            Destroy(gameObject, 0.1f);
        }
    }

    void SetItem()
    {
        int Rand = 0;
        if(Grade == 0)
        {
            Rand = Random.Range(0, 3);
        } else if(Grade == 1)
        {
            Rand = Random.Range(0, 4);
        }

        ItemNum = Rand;
        Debug.Log($"아이템 번호 : {ItemNum}");   

        LifeTime = DataManager.instance.item[ItemNum].LifeTIme;
        Weight = DataManager.instance.item[ItemNum].Weight;

        Icon.sprite = DataManager.instance.item[ItemNum].Icon;
        ItemName.text = "탐색 결과 : " + DataManager.instance.item[ItemNum].Id;

    }
}
