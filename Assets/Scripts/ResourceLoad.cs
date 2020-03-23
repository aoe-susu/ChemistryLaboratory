using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CLEditor
{
    public class ResourceLoad : MonoBehaviour
    {

        public static ResourceLoad resourceLoad;

        public Dictionary<string, CL_Object> CL_Objects = new Dictionary<string, CL_Object>();

        public List<GameObject> ObjectPrefabs;

        // Use this for initialization
        void Start()
        {
            resourceLoad = this;
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


        public Tree<string> GetCL_ObjectNames()
        {
            var texttree = new Tree<string> { content = "全部物体" };
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

        public CL_Object GetObjectInfoByName(string name)
        {
            if (!CL_Objects.ContainsKey(name)) return null;
            return CL_Objects[name];
        }

        public GameObject GetPrefabsByModelId(int id)
        {
            if (id < 0 || id >= CL_Objects.Count) return null;
            return ObjectPrefabs[id];
        }

    }

}
