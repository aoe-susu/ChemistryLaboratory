using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CLEditor
{
    //资源加载类
    public class ResourceLoad : MonoBehaviour
    {

        public static ResourceLoad resourceLoad;

        //用于储存物体信息的字典类
        public  Dictionary<string, CL_Object> CL_Objects = new Dictionary<string, CL_Object>();
        //用于存储物体实体预设
        public  List<GameObject> ObjectPrefabs; 

        // Use this for initialization
        //2020—4-3最新更新：将start改成awake，数据库初始化应该在最开始
        void Awake()
        {
            resourceLoad = this;    //单例模式
            //烧杯
            CL_Objects.Add("烧杯", new CL_Object
            {
                Name = "烧杯",
                ModelId = 0,
                ModelSize = 1,
                Operations = new List<Operation>() { new Operation { Name = "Move" } },
                Type = CL_ObjType.Vessel,
                VesselVolume = 1,
                Liquidvolume = 0,
            });
            //玻璃棒
            CL_Objects.Add("玻璃棒", new CL_Object
            {
                Name = "玻璃棒",
                ModelId = 1,
                ModelSize = 1,
                Operations = new List<Operation>() { new Operation { Name = "Move" } },
                Type = CL_ObjType.Common

            });
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

        //通过id获得物体预设
        //更改：将方法改成静态方法，通过查找名字的方法选取物体的预设
        public GameObject GetPrefabsByModelId(int id)
        {
            if (id < 0 || id >= CL_Objects.Count) return null;
            return ObjectPrefabs[id];
        }

    }

}
