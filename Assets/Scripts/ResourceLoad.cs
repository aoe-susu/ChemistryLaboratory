using CLEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//资源加载类
public class ResourceLoad : MonoBehaviour
{
    //public CLEditorManage cLEditorManage;
    //public static ResourceLoad resourceLoad;
    /// <summary>
    /// 是否为编辑模式
    /// </summary>
    public bool IsEdit = true;
    //用于储存物体信息的字典类
    public Dictionary<string, CL_Object> CL_Objects = new Dictionary<string, CL_Object>();
    //用于存储物体实体预设
    public List<GameObject> ObjectPrefabs = new List<GameObject>();
    //用于存储实体的缩略图
    public List<Texture2D> ObjectPrefabsTexture2D = new List<Texture2D>();
    //用于存储对物体的操作
    public List<Operation> Operations = new List<Operation>();
    //用于储存场景物体的信息
    public List<GameObject> SceneObjects = new List<GameObject>();
    //用于储存事件信息
    public Dictionary<string, List<EventInfo>> EventInfos = new Dictionary<string, List<EventInfo>>();

    // Use this for initialization
    //2020—4-3最新更新：将start改成awake，数据库初始化应该在最开始
    void Awake()
    {
        //cLEditorManage = GetComponent<CLEditorManage>();
        //resourceLoad = this;    //单例模式
        //烧杯
        
    }

    //获得物体名
    public Tree<string> GetCL_ObjectNames()
    {
        var texttree = new Tree<string> { content = "全部物体" };
        //反射获取物体类型名
        string[] types = Enum.GetNames(typeof(CL_ObjType));

        Dictionary<string, Tree<string>> parents = new Dictionary<string, Tree<string>>();

        foreach (var t in types)
        {
            var tree = new Tree<string>() { content = t };
            parents[t] = tree;
            tree.SetParent(texttree);
        }
        foreach (var t in CL_Objects)
        {
            var tree = new Tree<string>() { content = t.Key };
            tree.SetParent(parents[t.Value.Type.ToString()]);
        }
        return texttree;
    }

    //通过名字获取物体信息
    public CL_Object GetObjectInfoByName(string name)
    {
        if (!CL_Objects.ContainsKey(name)) return null;
        return CL_Objects[name];
    }

    //根据分类获取物体信息组
    public List<CL_Object> GetObjectInfoByClassify(string type)
    {
        List<CL_Object> list = new List<CL_Object>();
        foreach (var t in CL_Objects)
        {
            if (t.Value.Type.ToString() == type) list.Add(t.Value);
        }
        return list;
    }

    //通过id获得物体预设
    //更改：将方法改成静态方法，通过查找名字的方法选取物体的预设
    public GameObject GetPrefabsByModelId(int id)
    {
        if (id < 0 || id >= CL_Objects.Count) return null;
        return ObjectPrefabs[id];
    }

    public List<string> GetPrefabsName()
    {
        List<string> res = new List<string>();
        foreach (var t in ObjectPrefabs)
        {
            res.Add(t.name);
        }
        return res;
    }

    public void LoadSceneObjects(List<CL_Object> sceneobjinfo)
    {
        foreach (var t in SceneObjects)
        {
            Destroy(t);
        }
        SceneObjects.Clear();
        //SelectSignList.Clear();
        foreach (var clobj in sceneobjinfo)
        {
            var prefab = GetPrefabsByModelId(clobj.Model);
            var obj = Instantiate(prefab);
            obj.transform.position = clobj.Position;
            obj.transform.eulerAngles = clobj.EulerAngle;
            var clgameobj = obj.GetComponent<CLGameObject>();
            clgameobj.Load(this, clobj);
            var rig = obj.GetComponent<Rigidbody>();
            if (!IsEdit)
            {
                rig.constraints = RigidbodyConstraints.FreezeRotationY;
                rig.isKinematic = false;
            }
            SceneObjects.Add(obj);
        }
    }

    public void LoadExperimentalInfo(ExperimentalInfo info)
    {
        //加载模板物体
        CL_Objects.Clear();
        foreach (var t in info.CL_Objects)
        {
            CL_Objects.Add(t.Name, t);
        }
        //加载操作
        Operations = info.Operations;

        //加载场景信息
        LoadSceneObjects(info.SceneObjects);

        //加载事件
        EventInfos.Clear();
        for (int i = 0; i < info.EventClassifys.Count; i++)
        {
            EventInfos.Add(info.EventClassifys[i], info.EventInfos[i]);
        }
    }

    public ExperimentalInfo GetExperimentalInfo()
    {
        ExperimentalInfo info = new ExperimentalInfo();
        info.CL_Objects = new List<CL_Object>(CL_Objects.Values);
        info.Operations = Operations;
        foreach (var t in SceneObjects)
        {
            CLGameObject clg = t.GetComponent<CLGameObject>();
            info.SceneObjects.Add(clg.cl_object);
        }
        info.EventClassifys = new List<string>(EventInfos.Keys);
        info.EventInfos = new List<List<EventInfo>>(EventInfos.Values);
        return info;
    }


}
