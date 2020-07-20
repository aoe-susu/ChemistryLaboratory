using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Data_bar : MonoBehaviour, IPointerClickHandler
{
    public DataArea DataArea;
    public Text nametext;
    public Text valuetext;
  
    /// <summary>
    /// 初始化数据
    /// </summary>
    /// <param name="strname"></param>
    /// <param name="strvalue"></param>
    public void InitData(string strname,string strvalue)
    {
        nametext.text = strname;
        valuetext.text = strvalue;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        DataArea.ClickDataBar(this.gameObject);
    }
}
