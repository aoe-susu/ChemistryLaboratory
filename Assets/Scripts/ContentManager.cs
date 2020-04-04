using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContentManager : MonoBehaviour
{
    [Header("Hide")]
    public GameObject scrollView;
    public Text hideText;
    private bool isHide;

    [Header("Reload")]
    public GameObject content;
    public GameObject instrumentPrefab;

    void Start()
    {
        isHide = false;
    }

    public void HideScrollView()
    {
        if (isHide)
        {
            scrollView.SetActive(true);
            hideText.text = "隐藏";
            isHide = !isHide;
        }
        else
        {
            scrollView.SetActive(false);
            hideText.text = "展开";
            isHide = !isHide;
        }
    }

    public void ClearContent()
    {
        for (int i = 0; i < content.transform.childCount; i++)
        {
            Destroy(content.transform.GetChild(i).gameObject);
        }
    }

    public void ReloadFolder1()
    {
        ClearContent();
        GameObject newInstrument1 = Instantiate(instrumentPrefab, content.transform);
        GameObject newInstrument2 = Instantiate(instrumentPrefab, content.transform);
        GameObject newInstrument3 = Instantiate(instrumentPrefab, content.transform);
        newInstrument1.GetComponent<Image>().sprite = Resources.Load("Image/Beaker", typeof(Sprite)) as Sprite;
        newInstrument2.GetComponent<Image>().sprite = Resources.Load("Image/Beaker", typeof(Sprite)) as Sprite;
        newInstrument3.GetComponent<Image>().sprite = Resources.Load("Image/TextTub", typeof(Sprite)) as Sprite;
    }

    public void ReloadFolder2()
    {
        ClearContent();
        GameObject newInstrument1 = Instantiate(instrumentPrefab, content.transform);
        GameObject newInstrument2 = Instantiate(instrumentPrefab, content.transform);
        GameObject newInstrument3 = Instantiate(instrumentPrefab, content.transform);
        newInstrument1.GetComponent<Image>().sprite = Resources.Load("Image/MeasuringCylinder", typeof(Sprite)) as Sprite;
        newInstrument2.GetComponent<Image>().sprite = Resources.Load("Image/Beaker", typeof(Sprite)) as Sprite;
        newInstrument3.GetComponent<Image>().sprite = Resources.Load("Image/MeasuringCylinder", typeof(Sprite)) as Sprite;
    }

    public void ReloadFolder3()
    {
        ClearContent();
        GameObject newInstrument1 = Instantiate(instrumentPrefab, content.transform);
        GameObject newInstrument2 = Instantiate(instrumentPrefab, content.transform);
        GameObject newInstrument3 = Instantiate(instrumentPrefab, content.transform);
        newInstrument1.GetComponent<Image>().sprite = Resources.Load("Image/GlassBar", typeof(Sprite)) as Sprite;
        newInstrument2.GetComponent<Image>().sprite = Resources.Load("Image/ConicalFlask", typeof(Sprite)) as Sprite;
        newInstrument3.GetComponent<Image>().sprite = Resources.Load("Image/GlassBar", typeof(Sprite)) as Sprite;
    }

    public void ReloadFolder4()
    {
        ClearContent();
        GameObject newInstrument1 = Instantiate(instrumentPrefab, content.transform);
        GameObject newInstrument2 = Instantiate(instrumentPrefab, content.transform);
        GameObject newInstrument3 = Instantiate(instrumentPrefab, content.transform);
        newInstrument1.GetComponent<Image>().sprite = Resources.Load("Image/RoundFlask", typeof(Sprite)) as Sprite;
        newInstrument2.GetComponent<Image>().sprite = Resources.Load("Image/RoundFlask", typeof(Sprite)) as Sprite;
        newInstrument3.GetComponent<Image>().sprite = Resources.Load("Image/ConicalFlask", typeof(Sprite)) as Sprite;
    }
}
