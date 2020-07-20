using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace CLEditor
{
    public class ScrollViewOptions : ViewOption
    {
        public List<GameObject> initgameobject = new List<GameObject>();
        public Transform Content;
        public ViewOption OptionTemp;
        protected List<object> options = new List<object>();
        public List<object> Options
        {
            get { return options; }          
        }
        public int Value;
        public Color ClickColor = Color.blue;
        public Color NoClickColor = Color.white;
        public UnityAction<int> OnValueChange;
        public UnityAction<object> OtherAction;
        public object OtherData;
        public void SetOPtions<T>(IEnumerable<T> ts)
        {
            if (options == null) options = new List<object>();
            options.Clear();
            foreach (var t in ts)
            {
                options.Add(t);
            }         
            initgameobject.ForEach(s => Destroy(s));
            initgameobject.Clear();
            foreach (var t in options)
            {
                var obj = Instantiate(OptionTemp.gameObject, Content);
                var vop = obj.GetComponent<ViewOption>();
                vop.viewOptions = this;
                vop.SetOption(t);            
                obj.SetActive(true);
                initgameobject.Add(obj);
            }
            Value = -1;
        }

        public void Clear()
        {
            SetOPtions(new List<object>());
        }

        public virtual void OptionClick(GameObject obj)
        {
            ViewOption lastvop = null;
            if (Value >= 0 && Value < initgameobject.Count)
            {
                lastvop = initgameobject[Value].GetComponent<ViewOption>();
                lastvop.SetBackGroundColor(NoClickColor);
            }
            var vop = obj.GetComponent<ViewOption>();
            vop.SetBackGroundColor(ClickColor);
            Value = initgameobject.IndexOf(obj);
            if (OnValueChange != null) OnValueChange(Value);
        }

        public virtual void OptionClick(int val)
        {
           
            ViewOption lastvop = null;
            if (Value >= 0 && Value < initgameobject.Count)
            {
                lastvop = initgameobject[Value].GetComponent<ViewOption>();
                lastvop.SetBackGroundColor(NoClickColor);
            }
            if (val >= 0 &&val < options.Count)
            {
                var vop = initgameobject[val].GetComponent<ViewOption>();
                vop.SetBackGroundColor(ClickColor);
               
            }
            Value = val;
            if (OnValueChange != null) OnValueChange(Value);
        }

    }
}

