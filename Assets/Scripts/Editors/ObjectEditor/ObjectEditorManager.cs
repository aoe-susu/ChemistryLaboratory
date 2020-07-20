using DialogBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CLEditor
{
    public class ObjectEditorManager : MonoBehaviour
    {
        public ObjectClassify ObjectClassify;
        public ScrollViewOptions ObjectDataArea;
        public ResourceLoad resourceLoad;
        //private CL_Object SelectObj = null;
        // Use this for initialization
        public void Init()
        {
            ObjectClassify.LoadInfo(resourceLoad);
            ObjectClassify.OnSelectClObject = showDataView;
            ObjectDataArea.OnValueChange = DataAreaSelect;
            ObjectDataArea.OtherData = resourceLoad;
            ObjectDataArea.Clear();
        }
        private void OnEnable()
        {
            Init();
        }
       

        private void showDataView(CL_Object clobj)
        {
            //SelectObj = clobj;
            var keyvals = Tool.GetFieldInfo(clobj);
            if (keyvals["Type"].ToString() == "Common")
            {
                keyvals.Remove("VesselVolume");
                keyvals.Remove("Liquidvolume");
                keyvals.Remove("LiquidTemperature");
                keyvals.Remove("LiquidColor");
            }
            int val = ObjectDataArea.Value;
            ObjectDataArea.SetOPtions(keyvals);
            ObjectDataArea.OptionClick(val);
        }
       

        private void DataAreaSelect(int value)
        {
           
        }

        public void AddCL_Object()
        {
            int classifyval = ObjectClassify.Classify.Value;
            if (classifyval == -1) classifyval = (int)CL_ObjType.Custom;
            string clobjname = "自定义物体";
            for (int i=1; resourceLoad.CL_Objects.ContainsKey(clobjname);i++)
            {
                clobjname = "自定义物体" + i;
            }
            CL_Object SelectObj = new CL_Object() {
                Name= clobjname,
                Type= (CL_ObjType)classifyval,
                Model = 0,
                ModelSize=1               
            };           
            resourceLoad.CL_Objects[SelectObj.Name] = SelectObj;
            ObjectClassify.LoadInfo(resourceLoad);
            ObjectClassify.Classify.OptionClick((int)SelectObj.Type);
            int nowval = ObjectClassify.Thumbnail.Options.FindIndex(s => ((CL_Object)s).Name == SelectObj.Name);
            ObjectClassify.Thumbnail.OptionClick(nowval);
        }
        public void DelCL_Object()
        {
            var SelectObj = ObjectClassify.SelectObj;
            if (SelectObj != null)
            {
                DialogBoxManager.dialogBoxManager.ShowConfirm("删除确认", "确认删除" + SelectObj.Name + "吗？", () =>
                {
                    resourceLoad.CL_Objects.Remove(SelectObj.Name);
                    ObjectClassify.LoadInfo(resourceLoad);
                    ObjectClassify.Classify.OptionClick((int)SelectObj.Type);
                    ObjectDataArea.Clear();
                });

            }
        }
        public void ModefyField()
        {
            int val = ObjectDataArea.Value;
            if (val == -1) return;
            KeyValuePair<string, object> keyValue = (KeyValuePair<string, object>)ObjectDataArea.Options[val];
            var diabox = DialogBoxManager.dialogBoxManager;
            var SelectObj = ObjectClassify.SelectObj;
            if (keyValue.Key == "Name")
            {
                DialogStrData dataBase = new DialogStrData(keyValue.Value.ToString());
                diabox.EditData("编辑值-字符串", "仪器名称", dataBase, data =>
                {
                    
                    if (SelectObj == null) throw new Exception("SelectObj为null");
                    if (resourceLoad.CL_Objects.ContainsKey(data.GetValue().ToString()))
                    {
                        DialogBoxManager.dialogBoxManager.ShowMessage("修改错误", "名称为"+ data.GetValue().ToString() + "的物体已存在，无法修改");
                        return;
                    }
                    string OldName = SelectObj.Name;
                    string NewName= data.GetValue().ToString();
                    resourceLoad.CL_Objects.Remove(OldName);
                    SelectObj.Name = NewName;
                    resourceLoad.CL_Objects.Add(NewName, SelectObj);
                    //修改名字时，也要把场景编辑器里的名字替换掉
                    var sceneEditor = resourceLoad.GetComponent<CLEditorManage>().SceneEditorManager;
                    sceneEditor.ReNameSceneObject(OldName, NewName);

                   
                    ObjectClassify.LoadInfo(resourceLoad);
                    ObjectClassify.Classify.OptionClick((int)SelectObj.Type);
                    int nowval = ObjectClassify.Thumbnail.Options.FindIndex(s => ((CL_Object)s).Name == SelectObj.Name);
                    ObjectClassify.Thumbnail.OptionClick(nowval);
                });
            }
            else if (keyValue.Key == "Model")
            {
                List<string> temp = resourceLoad.GetPrefabsName();
                List<DialogBoxDataBase> datas = new List<DialogBoxDataBase>();
                foreach (var t in temp)
                {
                    datas.Add(new DialogStrData(t));
                }
                diabox.ListSelect("选择值-整数索引", "选择模型", SelectObj.Model, datas, index => {
                    if (SelectObj == null) throw new Exception("SelectObj为null");
                    SelectObj.Model = index;
                    resourceLoad.CL_Objects[SelectObj.Name] = SelectObj;
                    ObjectClassify.LoadInfo(resourceLoad);
                    ObjectClassify.Classify.OptionClick((int)SelectObj.Type);
                    int nowval = ObjectClassify.Thumbnail.Options.FindIndex(s => ((CL_Object)s).Name == SelectObj.Name);
                    ObjectClassify.Thumbnail.OptionClick(nowval);
                });
            }
            else if (keyValue.Key == "Operations")
            {
                List<Operation> temp = resourceLoad.Operations;
                List<DialogBoxDataBase> totaldatas = new List<DialogBoxDataBase>();
                foreach (var t in temp)
                {
                    totaldatas.Add(new DialogOperationData(t));
                }
                temp = resourceLoad.GetObjectInfoByName(SelectObj.Name).Operations;
                List<DialogBoxDataBase> defaultdatas = new List<DialogBoxDataBase>();
                foreach (var t in temp)
                {
                    defaultdatas.Add(new DialogOperationData(t));
                }
                diabox.EditDataList("编辑值-操作数组", "操作数组", defaultdatas, totaldatas, reslist => {
                    if (SelectObj == null) throw new Exception("SelectObj为null");
                    SelectObj.Operations.Clear();
                    reslist.ForEach(s => SelectObj.Operations.Add((Operation)s.GetValue()));
                    resourceLoad.CL_Objects[SelectObj.Name] = SelectObj;
                    ObjectClassify.LoadInfo(resourceLoad);
                    ObjectClassify.Classify.OptionClick((int)SelectObj.Type);
                    int nowval = ObjectClassify.Thumbnail.Options.FindIndex(s => ((CL_Object)s).Name == SelectObj.Name);
                    ObjectClassify.Thumbnail.OptionClick(nowval);
                });
            }
            else if (keyValue.Key == "Type")
            {
                string[] temp = Enum.GetNames(typeof(CL_ObjType));
                List<DialogBoxDataBase> datas = new List<DialogBoxDataBase>();
                foreach (var t in temp)
                {
                    datas.Add(new DialogStrData(t));
                }
                diabox.ListSelect("选择值-整数索引", "仪器类型", (int)SelectObj.Type, datas, index => {
                    if (SelectObj == null) throw new Exception("SelectObj为null");
                    SelectObj.Type = (CL_ObjType)index;
                    resourceLoad.CL_Objects[SelectObj.Name] = SelectObj;
                    ObjectClassify.LoadInfo(resourceLoad);
                    ObjectClassify.Classify.OptionClick((int)SelectObj.Type);
                    int nowval = ObjectClassify.Thumbnail.Options.FindIndex(s => ((CL_Object)s).Name == SelectObj.Name);
                    ObjectClassify.Thumbnail.OptionClick(nowval);
                });
            }
            else if (keyValue.Key == "LiquidColor")
            {
                Color32 data = (Color32)keyValue.Value;
                diabox.EditColor("编辑值-颜色", keyValue.Key, data, res =>
                {
                    if (SelectObj == null) throw new Exception("SelectObj为null");
                    SelectObj.LiquidColor = res;
                    resourceLoad.CL_Objects[SelectObj.Name] = SelectObj;
                    ObjectClassify.LoadInfo(resourceLoad);
                    ObjectClassify.Classify.OptionClick((int)SelectObj.Type);
                    int nowval = ObjectClassify.Thumbnail.Options.FindIndex(s => ((CL_Object)s).Name == SelectObj.Name);
                    ObjectClassify.Thumbnail.OptionClick(nowval);
                });
            }
            else
            {
                DialogFloatData dataBase = new DialogFloatData((float)keyValue.Value);
                diabox.EditData("编辑值-浮点", keyValue.Key, dataBase, data =>
                {
                    if (SelectObj == null) throw new Exception("SelectObj为null");
                    var field= SelectObj.GetType().GetFields()[ObjectDataArea.Value];
                    field.SetValue(SelectObj, data.GetValue());
                    resourceLoad.CL_Objects[SelectObj.Name] = SelectObj;
                    ObjectClassify.LoadInfo(resourceLoad);
                    ObjectClassify.Classify.OptionClick((int)SelectObj.Type);
                    int nowval = ObjectClassify.Thumbnail.Options.FindIndex(s => ((CL_Object)s).Name == SelectObj.Name);
                    ObjectClassify.Thumbnail.OptionClick(nowval);
                });
            }
        }

    }

}
