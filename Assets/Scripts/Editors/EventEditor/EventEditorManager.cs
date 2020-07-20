using DialogBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace CLEditor
{
    public class EventEditorManager : MonoBehaviour
    {
        public ScrollViewOptions EventClassify;
        public GameObject DataArea;
        public InputField Remark;
        public Dropdown EventType;
        public Text Condition;
        public Dropdown Action;
        public Text Pm1;
        public Text Pm2;
        public ResourceLoad resourceLoad;

        private KeyValuePair<string, int> classify_event;
        // Use this for initialization
        //写一个监控脚本，专门监控所有的触发事件，看其有没有发生
        //事件编辑器编写的EventInfo会以list的方式存储再监控脚本内，
        //监控脚本会循环读取eventinfo内的信息
        //触发事件内的信息有决定哪个触发函数的枚举，以及对应的参数
        //触发事件内的信息有 栈【决定哪个条件函数的枚举，以及对应的参数】，会以递归的方式层层嵌套
        //触发事件内的信息有 决定哪个响应函数的枚举，以及对应的参数
        //触发的事件，游戏内写好一些函数，一旦这些函数被执行，然后检查条件
        //1.任意(指定)物体任意(指定)属性变化 ObjectPropertyChange(object obj,PropertyType val)    
        //2.时间变化
        //3.任意(指定)物体进行了任意(指定)操作 ObjectOperator(object obj,operator op)
        //4.两任意(指定)物体任意(指定)属性变化ObjectPropertyChange(PropertyType val1,PropertyType val2)
        //7.任意物体被选择事件
        //8.实验初始化
        //筛选的条件，条件默认为true&&其他条件，为bool类型，
        //如果条件满足，执行下面的回调函数，即响应事件
        //1.哪个物体
        //2.满足哪个类型的物体
        //3.指定物体的哪个属性
        //4.数学运算 + - * / > < !
        //5.且
        //6.并
        //7.无
        //响应的事件
        //符合条件的所有物体的某项属性设置为多少实数
        

        public void Init()
        {
            EventClassify.OnValueChange = eventClassifySelect;
            EventClassify.OtherAction = eventSelect;
            EventClassify.SetOPtions(resourceLoad.EventInfos);
            
        }
        private void OnEnable()
        {
            Init();
        }

        void Start()
        {
            Remark.onValueChanged.AddListener(s => {
                resourceLoad.EventInfos[classify_event.Key][classify_event.Value].Remark = s;
            });
            EventType.onValueChanged.AddListener(s =>
            {
                resourceLoad.EventInfos[classify_event.Key][classify_event.Value].EventType = (CLEventType)s;
            });
            Condition.GetComponent<Button>().onClick.AddListener(() => {
                CLCondition con = resourceLoad.EventInfos[classify_event.Key][classify_event.Value].Condition;
                if (con == null) con = new CLCondition();
                DialogBoxManager.dialogBoxManager.EditConditionData(resourceLoad, con, confirm => {
                    resourceLoad.EventInfos[classify_event.Key][classify_event.Value].Condition = confirm as CLCondition;
                    LoadEventInfo(resourceLoad.EventInfos[classify_event.Key][classify_event.Value]);
                });
            });
            Action.onValueChanged.AddListener(s =>
            {
                resourceLoad.EventInfos[classify_event.Key][classify_event.Value].Action.type = (CLActionType)s;
            });
            Pm1.GetComponent<Button>().onClick.AddListener(()=>{
                CLValue value = resourceLoad.EventInfos[classify_event.Key][classify_event.Value].Action.values[0];
                if (value == null) value = new CLValue();
                DialogBoxManager.dialogBoxManager.EditCLValueData(resourceLoad, value, confirm => {
                    resourceLoad.EventInfos[classify_event.Key][classify_event.Value].Action.values[0] = confirm as CLValue;
                    LoadEventInfo(resourceLoad.EventInfos[classify_event.Key][classify_event.Value]);
                });
            });
            Pm2.GetComponent<Button>().onClick.AddListener(() => {
                CLValue value = resourceLoad.EventInfos[classify_event.Key][classify_event.Value].Action.values[1];
                if (value == null) value = new CLValue();
                DialogBoxManager.dialogBoxManager.EditCLValueData(resourceLoad, value, confirm => {
                    resourceLoad.EventInfos[classify_event.Key][classify_event.Value].Action.values[1] = confirm as CLValue;
                    LoadEventInfo(resourceLoad.EventInfos[classify_event.Key][classify_event.Value]);
                });
            });
        }

        void eventClassifySelect (int val)
        {
            //if (val == -1) return;
            foreach (var t in EventClassify.initgameobject)
            {
                t.GetComponent<EventClassifyOption>().OptionClick(-1);
            }
            DataArea.SetActive(false);
        }
        void eventSelect(object val)
        {
            classify_event = (KeyValuePair<string, int>)val;
            var eventinfo= resourceLoad.EventInfos[classify_event.Key][classify_event.Value];
            LoadEventInfo(eventinfo);
        }

        public void AddClassify()
        {
            DialogBoxManager.dialogBoxManager.EditData("添加类别", "类别名称",new DialogStrData("新建类别"), confirm => {
                string na = confirm.GetValue().ToString();
                if (resourceLoad.EventInfos.ContainsKey(na))
                {
                    DialogBoxManager.dialogBoxManager.ShowMessage("添加失败", "该类别名称已存在");
                }
                else
                {
                    resourceLoad.EventInfos.Add(na, new List<EventInfo>());
                    DialogBoxManager.dialogBoxManager.ShowMessage("添加成功", "该类别已添加成功");
                    Init();
                }
            });
        }
        public void AddEvent()
        {
            DialogBoxManager.dialogBoxManager.EditData("添加事件", "类别名称", new DialogStrData("新建事件"), confirm => {
                string na = confirm.GetValue().ToString();
                var val = EventClassify.Value;
                if (val < 0) val = EventClassify.Options.Count - 1;
                string classname = ((KeyValuePair<string, List<EventInfo>>)EventClassify.Options[val]).Key;
                resourceLoad.EventInfos[classname].Add(new EventInfo()
                {
                    Name = na,
                    EventType = CLEventType.GameInitialize,
                    Remark = "这里可以写一些备注来说明这个事件的用途"
                });
                DialogBoxManager.dialogBoxManager.ShowMessage("添加成功", "该事件已添加成功");
                Init();
            });
        }

        public void LoadEventInfo(EventInfo eventInfo)
        {
            DataArea.SetActive(true);
            Remark.text = eventInfo.Remark;
            SetDropDownOption(EventType, eventInfo.EventType);
            Condition.text = eventInfo.Condition==null? "": eventInfo.Condition.ToString();
            SetDropDownOption(Action, eventInfo.Action.type);
            if(eventInfo.Action.values[0]!=null)
            {
                Pm1.text = eventInfo.Action.values[0].ToString();
            }
            else
            {
                Pm1.text = "";
            }
            if (eventInfo.Action.values[1] != null)
            {
                Pm2.text = eventInfo.Action.values[1].ToString();
            }
            else
            {
                Pm2.text = "";
            }
        }
        private  void SetDropDownOption<TEnum>(Dropdown dropdown,TEnum e)
        {
            dropdown.ClearOptions();
            string[] typenames = Enum.GetNames(typeof(TEnum));
            int index = 0;
            for (int i = 0; i < typenames.Length; i++)
            {
                dropdown.options.Add(new Dropdown.OptionData(typenames[i]));
                if (e.ToString() == typenames[i])
                {
                    index = i;
                }
            }
            dropdown.value = index;
            dropdown.RefreshShownValue();
        }

    }

}
