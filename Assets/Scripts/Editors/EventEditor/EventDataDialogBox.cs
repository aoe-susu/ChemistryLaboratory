using CLEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DialogBox
{
    public class EventDataDialogBox : MonoBehaviour
    {
        public ResourceLoad resourceLoad;
        public RadioOption select;
        public RadioOption var;
        public RadioOption fun;
        public RadioOption val;
        public Text Title;
        public Dropdown Function;
        public Text Pm1;
        public Text FunTip;
        public Text Pm2;
        public InputField valinput;
        public Sprite Selected;
        public Sprite UnSelected;

        public UnityAction<object> ConfirmEvent;
        public UnityAction CancelEvent;
        public CLEventType eventType;
        public object dataBase=null;

        private bool IsCondition = true;
        private List<CL_Object> clobjs = null;
        private List<CL_Object> Clobjs{
            get {
                if (clobjs == null)
                {
                    clobjs = new List<CL_Object>();
                    resourceLoad.SceneObjects.ForEach(s => {
                        var clobj = s.GetComponent<CLGameObject>().cl_object;
                        clobjs.Add(clobj);
                    });
                }
                return clobjs;
            }
        }
        public void Confirm()
        {
            if (!IsCondition)
            {
                if (select == val)
                {
                    var value = new CLValue() { value = valinput.text };
                    dataBase = value;
                }
                else
                {
                    var value = dataBase as CLValue;
                    value.value = null;
                    dataBase = value;
                }
            }
            
            if (ConfirmEvent != null) ConfirmEvent(dataBase);
            Destroy(this.gameObject);
        }
      
        public void SetEventData(CLCondition condition)
        {
            dataBase = new CLCondition();
            Tool.CopyValue(dataBase, condition);
            IsCondition = true;
            Title.text = "编辑值-布尔";
            string[] funstrs = Enum.GetNames(typeof(CLConditionType));
            if (funstrs.Length != Function.options.Count + 1)
            {
                Function.options.Clear();
                for (int i = 0; i < funstrs.Length - 1; i++)
                {
                    if (funstrs[i] == "or" || funstrs[i] == "and" || funstrs[i] == "negate")
                    {
                        Function.options.Add(new Dropdown.OptionData("[布尔]" + funstrs[i]));
                    }
                    else
                    {
                        Function.options.Add(new Dropdown.OptionData("[实数]" + funstrs[i]));
                    }

                    if (funstrs[i] == condition.conditiontype.ToString())
                    {
                        Function.value = i;
                    }
                }
                Function.RefreshShownValue();
                Function.onValueChanged.AddListener(res => {
                    var condi = new CLCondition()
                    {
                        conditiontype = (CLConditionType)res
                    };
                    SetEventData(condi);
                });
            }

            Pm1.text = condition.condition1 == null ? "null" : condition.condition1.ToString();
            FunTip.text = condition.conditiontype.ToString();
            val.gameObject.SetActive(false);
            if (condition.conditiontype== CLConditionType.negate)
            {
                Pm2.gameObject.SetActive(false);
            }
            else
            {
                Pm2.gameObject.SetActive(true);
                Pm2.text = condition.condition2 == null ? "null" : condition.condition2.ToString();
            }
            
            fun.Select();

        }
        public void SetEventData(CLValue value)
        {
            dataBase = new CLValue();
            Tool.CopyValue(dataBase, value);
            IsCondition = false;
            Title.text = "编辑值-数据";
            string objname = "物体";
            string join = null;
            if (value.value == null)
            {
                var templist = new List<string>();
                templist.Add(objname);
                templist.AddRange(value.parameterreflect);
                join = string.Join("的", templist.ToArray());
                fun.Select();
                Pm1.text = value.ToString();
            }
            else
            {
                val.Select();
                valinput.text = value.ToString();
            }
            int index=0;
            List<string> strnames = GetAllTypeValueNameForClobj(objname);
            if (strnames.Count != Function.options.Count)
            {
                Function.options.Clear();
                for (int i = 0; i < strnames.Count; i++)
                {
                    if (join == strnames[i]) index = i;
                    Function.options.Add(new Dropdown.OptionData(strnames[i]));
                }
                Function.value = index;
                Function.RefreshShownValue();
                Function.onValueChanged.AddListener(res => {
                    var clval = new CLValue();
                    var str = Function.options[res].text;
                    List<string> reflect = new List<string>(str.Split('的'));
                    reflect.RemoveAt(0);
                    clval.parameterreflect = reflect;
                    clval.parameterindex = 0;
                    clval.name = "触发物体0";
                    SetEventData(clval);
                });
            }
            FunTip.gameObject.SetActive(false);
            Pm2.gameObject.SetActive(false);
          
        }

        public void Pm1Click()
        {
            if (!IsCondition)
            {
                PmsClick(dataBase as CLValue);
            }
            else
            {
                PmsClick(dataBase as CLCondition,true);
            }
        }
        public void Pm2Click()
        {
            PmsClick(dataBase as CLCondition,false);
        }
        private void PmsClick(CLCondition condition,bool isleft)
        {
            CLCondition con = isleft ? condition.condition1 : condition.condition2;
            if (con == null) con = new CLCondition();
            if (condition.conditiontype== CLConditionType.negate|| 
                condition.conditiontype == CLConditionType.and|| 
                condition.conditiontype == CLConditionType.or)
            {
                DialogBoxManager.dialogBoxManager.EditConditionData(resourceLoad, con, confirm => {
                    if (isleft) condition.condition1 = confirm as CLCondition;
                    else condition.condition2= confirm as CLCondition;
                    SetEventData(condition);
                });
            }
            else
            {
                con.conditiontype = CLConditionType.val;
                CLValue value = con.value;
                if(value==null)
                    value = new CLValue()
                    {
                        parameterindex = 0,
                        name = "触发物体0"
                    };

                DialogBoxManager.dialogBoxManager.EditCLValueData(resourceLoad, value, s => {
                    con.value = s as CLValue;
                    if (isleft) condition.condition1 = con;
                    else condition.condition2 = con;
                    SetEventData(condition);
                });
            }
        }
        private void PmsClick(CLValue value)
        {
            List<DialogBoxDataBase> datas = new List<DialogBoxDataBase>();
            int count = 3;
            for(int i = 0; i < count; i++)
            {
                datas.Add(new DialogStrData("触发物体" + i));

            }
            for (int i=0;i< Clobjs.Count;i++)
            {
                datas.Add(new DialogStrData("场景物体"+i+Clobjs[i].Name));
            }
            int defaultval = datas.FindIndex(s => s.GetDialogBoxShowString() == value.name);
            DialogBoxManager.dialogBoxManager.ListSelect("选择", "物体", defaultval, datas, confirm => {
                if (confirm < count)
                {
                    value.parameterindex = confirm;
                }
                else
                {
                    value.parameterindex = -1;
                    value.id = Clobjs[confirm - count].Id;
                }
                value.name = datas[confirm].GetDialogBoxShowString();
                SetEventData(value);
            });
        }

        private List<string> GetAllTypeValueNameForClobj(string name = "")
        {
            List<string> res = new List<string>();
            Type type = typeof(CL_Object);
            PropertyInfo[] pros = type.GetProperties();
            FieldInfo[] fies = type.GetFields();
            for (int i = 0; i < pros.Length; i++)
            {
                var pro = pros[i];
                var protype = pro.PropertyType;
                string nextname = name + "的" + pro.Name;
                if (protype == typeof(int) || protype == typeof(float) || protype == typeof(string))
                {
                    res.Add(nextname);
                }
                else if (protype == typeof(Vector3))
                {
                    res.Add(nextname + "的x");
                    res.Add(nextname + "的y");
                    res.Add(nextname + "的z");
                }
            }

            for (int i = 0; i < fies.Length; i++)
            {
                var fie = fies[i];
                var fietype = fie.FieldType;
                string nextname = name + '的' + fie.Name;
                if (fietype == typeof(int) || fietype == typeof(float) || fietype == typeof(string))
                {
                    res.Add(name + '的' + fie.Name);
                }
                else if (fietype == typeof(Color32))
                {
                    res.Add(nextname + "的r");
                    res.Add(nextname + "的g");
                    res.Add(nextname + "的b");
                    res.Add(nextname + "的a");
                }
            }
            return res;
        }

     
        public void Cancel()
        {
            if (CancelEvent != null) CancelEvent();
            Destroy(this.gameObject);
        }
    }
}

