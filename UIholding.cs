using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum UIType
{
    AirFrame,
    Part
}

public class UIholding : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UIType Type;

    public List<Text> Texts = new List<Text>();
    public List<string> Labels = new List<string>();
    public int num;

    public void OnPointerEnter(PointerEventData eventData)
    {
        TextSet();
    }

    public void TextSet()
    {
        switch (Type)
        {
            case UIType.AirFrame:
                Texts[0].text = Labels[0];
                break;
            case UIType.Part:
                Texts[0].text = GetPartDes();
                Texts[1].text = GetPartDes(true);
                break;

        }
    }

    public string GetPartDes(bool cool = false)
    {
        string result = DataManager.instance.Parts[num].Description;
        if (cool)
        {
            result = $"COOLTIME : {(int)DataManager.instance.Parts[num].Resoult[0].resoult[DataManager.instance.Parts[num].UpGrade]}s";
        } else
        {
            if (DataManager.instance.Parts[num].Resoult.Count > 1)
                result = result.Replace( "{result1}", DataManager.instance.Parts[num].Resoult[1].resoult[DataManager.instance.Parts[num].UpGrade].ToString("F0"));
  
        }


        return result;
    }



    public void OnPointerExit(PointerEventData eventData)
    {
        foreach (Text t in Texts) {
            t.text = "";
        }
    }
  
}
