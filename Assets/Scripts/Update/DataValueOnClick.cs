using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataValueOnClick : MonoBehaviour
{
    private Image background;

    // Start is called before the first frame updateS
    void Start()
    {
        background = GetComponent<Image>();
        //添加点击事件
        EventTriggerListener.Get(this.gameObject).onClick = OnClickHandler;
    }

    //点击事件委托方法
    private void OnClickHandler(GameObject Obj)
    {
        background.color = Color.red;
        Debug.Log("监听到了点击事件");
        
    }
}
