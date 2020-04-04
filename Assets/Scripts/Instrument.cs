using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public delegate int myDelegate();
public class Instrument : MonoBehaviour,IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler
{
    private Color enterColor;
    private Color clickColor;
    private Color oldColor;

    public UnityAction<int> unityAction;
    private int index;
    
    myDelegate SentIndexDel;
    private void Start()
    {
        index = -1;
        enterColor = new Color(0.50445f, 0.9811321f, 0.9497714f);
        clickColor = new Color(0.3103863f, 0.8773585f, 0.8248454f);
        oldColor = this.GetComponent<Image>().color;
        SentIndexDel = new myDelegate(ReturnIndex);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
       this.GetComponent<Image>().color = clickColor;

        index = SentIndexDel.Invoke();
        Debug.Log(index);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        this.GetComponent<Image>().color = enterColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.GetComponent<Image>().color = oldColor;
    }

    public int ReturnIndex()
    {
        Transform parent = GameObject.Find("Content").transform;
        for (int index = 0; index < parent.childCount; index++)
        {
            Transform child = parent.GetChild(index);
            if (child==this.transform)
            {
                return index;
            }
        }
        return -1;
    }
}
