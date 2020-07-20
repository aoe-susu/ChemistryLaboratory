using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CLEditor
{
    public class ObjectClassify : MonoBehaviour
    {

        public ScrollViewOptions Classify;
        public ScrollViewOptions Thumbnail;
        public Transform FlexibleTrans;
        // Use this for initialization
        Dictionary<string, List<CL_Object>> typeclassify = new Dictionary<string, List<CL_Object>>();
        public UnityAction<CL_Object> OnSelectClObject;
        public UnityAction<int> OnSelectClassify;

        public CL_Object SelectObj = null;

        private void Start()
        {
            Classify.OnValueChange = ClassifyClick;
            Thumbnail.OnValueChange = ObjectThumbnailClick;
        }
        public void LoadInfo(ResourceLoad resourceLoad)
        {
            SelectObj = null;
            Thumbnail.Clear();
            typeclassify.Clear();
            string[] classifynames = Enum.GetNames(typeof(CL_ObjType));
            Classify.SetOPtions(classifynames);
            Thumbnail.OtherData = resourceLoad;
            foreach (string clfy in classifynames)
            {
                typeclassify[clfy] = resourceLoad.GetObjectInfoByClassify(clfy);
            }
        }

        public void ClassifyClick(int value)
        {
            string typename = Classify.Options[value].ToString();
            Thumbnail.SetOPtions(typeclassify[typename]);
            if (SelectObj != null && (int)SelectObj.Type == value)
            {
                int nowval = Thumbnail.Options.FindIndex(s => ((CL_Object)s).Name == SelectObj.Name);
                Thumbnail.OptionClick(nowval);
            }
            if (OnSelectClassify != null) OnSelectClassify(value);
        }
        public void ObjectThumbnailClick(int value)
        {
            if (value < 0 || value >= Thumbnail.Options.Count)
            {
                SelectObj = null;
                return;
            }
            var clobj = (Thumbnail.Options[value] as CL_Object);
            SelectObj = clobj;
          
            if (OnSelectClObject != null)
                OnSelectClObject(clobj);
        }

       
        public void Flexible()
        {
            if (FlexibleTrans == null) return;
            bool isshow = Classify.gameObject.activeSelf;
            Classify.gameObject.SetActive(!isshow);
            Thumbnail.gameObject.SetActive(!isshow);
            FlexibleTrans.Rotate(new Vector3(0, 0, 180));
            float height = 75;
            transform.localPosition = transform.localPosition + new Vector3(0, isshow ? -height : height, 0);
        }
    }
}

