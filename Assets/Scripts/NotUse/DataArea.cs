using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CLEditor;
using MyTools;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DataArea : MonoBehaviour
{
    //储存数据条预设
    public Color DataBarClickColor;
    public GameObject DataPerfab;
    public GameObject Concent;
    public List<GameObject> DataObjList = new List<GameObject>();
    public int Value = -1;
    public UnityAction<int> DataBarClickEvent;
    //字典
    private Dictionary<string, string> Dic_translations = new Dictionary<string, string>();

    private void Awake()
    {
        Dic_translations["Name"] = "名称";
        Dic_translations["ModelId"] = "模型编号";
        Dic_translations["ModelSize"] = "模型大小";
        Dic_translations["Operations"] = "操作组";
        Dic_translations["Type"] = "类型";
        Dic_translations["VesselVolume"] = "模型体积";
        Dic_translations["Liquidvolume"] = "溶液体积";
        Dic_translations["LiquidTemperature"] = "溶液温度";
        Dic_translations["LiquidColor"] = "溶液颜色";
        Dic_translations["Position"] = "位置";
        Dic_translations["EulerAngle"] = "角度";
    }
    // Start is called before the first frame update
    void Start()
    {
        //UpdateSelectData("玻璃棒");
        //UpdateSelectData("烧杯");       
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 通过名字来选择模型信息
    /// </summary>
    /// <param name="modelId"></param>
    public void UpdateSelectData(string name)
    {
        //销毁物体
        //这里有个问题，物体销毁后，容器没清空
        foreach (GameObject o in DataObjList)
        {
            GameObject.Destroy(o);
        }
        DataObjList.Clear();

        CL_Object cl_Object=new CL_Object();// = ResourceLoad.resourceLoad.GetObjectInfoByName(name);
        FieldInfo[] info = ClassOperation.GetClassFields(cl_Object);

        //容器
        if (cl_Object.Type == CL_ObjType.Vessel)
        {
            foreach (FieldInfo p in info)
            {
                InitDataBar(Dic_translations[p.Name], p.GetValue(cl_Object).ToString());
            }
        }
        //非容器类型
        else 
        {
            foreach (FieldInfo p in info)
            {
                if (p.Name != "VesselVolume"&& p.Name != "Liquidvolume"&&p.Name!= "LiquidTemperature")
                {
                    InitDataBar(Dic_translations[p.Name], p.GetValue(cl_Object).ToString());
                }
            }
        }
    }

    /// <summary>
    /// 初始化数据元
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public void InitDataBar(string name, string value)
    {
        GameObject Data = GameObject.Instantiate(DataPerfab);
        Data.transform.position = Vector2.zero;             //将物品的坐标归零到父容器               
        Data.transform.SetParent(Concent.transform, false); //记得关闭世界坐标        
        var databar=Data.GetComponent<Data_bar>();//初始化数据
        databar.InitData(name, value);
        databar.DataArea = this;
        DataObjList.Add(Data);
    }

    public void ClickDataBar(GameObject obj)
    {
        GameObject o = null;
        if(Value>=0&& Value< DataObjList.Count)
        {
            o = DataObjList[Value];
            o.GetComponent<Image>().color = new Color(255, 255, 255, 255);
        }    
        obj.GetComponent<Image>().color = DataBarClickColor;
        int index=DataObjList.IndexOf(obj);
        Value = index;
        if (DataBarClickEvent != null)       
        {
            DataBarClickEvent(index);
        }
        else
        {
            Debug.Log("当前value为" + Value+ "  未绑定事件");
        }
    }


}
