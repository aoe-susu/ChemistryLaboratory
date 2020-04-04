using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Data_bar : MonoBehaviour
{
    public GameObject nameObj;
    public GameObject valueObj;
  
    /// <summary>
    /// 初始化数据
    /// </summary>
    /// <param name="strname"></param>
    /// <param name="strvalue"></param>
    public void InitData(string strname,string strvalue)
    {
        nameObj.GetComponent<Text>().text = strname;
        valueObj.GetComponent<Text>().text = strvalue;
    }
}
