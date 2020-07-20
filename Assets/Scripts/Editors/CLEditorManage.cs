using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CLEditor
{
    public class CLEditorManage:MonoBehaviour
    {
        public MenuManager MenuManager;
        public SceneEditorManager SceneEditorManager;       
        public EventEditorManager EventEditorManager;
        public ObjectEditorManager ObjectEditorManager;
        private ResourceLoad resourceLoad;
        //private List<CL_Object> PlacedObjectInfos = new List<CL_Object>();

        private void Awake()
        {
            resourceLoad = GetComponent<ResourceLoad>();
            SceneEditorManager.resourceLoad = resourceLoad;          
            EventEditorManager.resourceLoad = resourceLoad;
            ObjectEditorManager.resourceLoad = resourceLoad;

            MenuManager.CreateExperiment = CreateExperiment;
            MenuManager.OpenExperiment = OpenExperiment;
            MenuManager.SaveExperiment = SaveExperiment;
            MenuManager.SwitchModule = SwitchModule;

            CreateExperiment();
        }

        public void CreateExperiment()
        {
            ExperimentalInfo info = new ExperimentalInfo();
            //烧杯
            info.CL_Objects.Add(new CL_Object
            {
                Name = "烧杯",
                Model = 0,
                ModelSize = 1,
                Operations = new List<Operation>() { new Operation { Name = "Move" } },
                Type = CL_ObjType.Vessel,
                Liquidvolume = 0,
            });
            //玻璃棒
            info.CL_Objects.Add(new CL_Object
            {
                Name = "玻璃棒",
                Model = 1,
                ModelSize = 1,
                Operations = new List<Operation>() { new Operation { Name = "Move" } },
                Type = CL_ObjType.Common

            });
            //量筒
            info.CL_Objects.Add(new CL_Object
            {
                Name = "量筒",
                Model = 2,
                ModelSize = 1,
                Operations = new List<Operation>() { new Operation { Name = "Move" } },
                Type = CL_ObjType.Vessel

            });
            //烧瓶
            info.CL_Objects.Add(new CL_Object
            {
                Name = "烧瓶",
                Model = 3,
                ModelSize = 1,
                Operations = new List<Operation>() { new Operation { Name = "Move" } },
                Type = CL_ObjType.Vessel

            });
            //锥形瓶
            info.CL_Objects.Add(new CL_Object
            {
                Name = "锥形瓶",
                Model = 4,
                ModelSize = 1,
                Operations = new List<Operation>() { new Operation { Name = "Move" } },
                Type = CL_ObjType.Vessel

            });
            //试管
            info.CL_Objects.Add(new CL_Object
            {
                Name = "试管",
                Model = 5,
                ModelSize = 1,
                Operations = new List<Operation>() { new Operation { Name = "Move" } },
                Type = CL_ObjType.Vessel

            });
            info.Operations.Add(new Operation() { Name = "Move" });
            info.Operations.Add(new Operation() { Name = "Hold" });
            info.Operations.Add(new Operation() { Name = "Rota" });
            info.Operations.Add(new Operation() { Name = "Pull" });
            info.Operations.Add(new Operation() { Name = "Other1" });
            info.EventClassifys.Add("默认");
            info.EventInfos.Add(new List<EventInfo>() {
                new EventInfo(){
                    Name ="初始化",
                    Remark="这是一个事件案列",
                    EventType= CLEventType.GameInitialize,
                    Action =new CLAction{
                        type= CLActionType.ShowTip,
                        values=new CLValue[2]{
                            new CLValue(){value="欢迎来到化学实验室"},
                            null
                        }
                    }
                },
            });
            LoadExperiment(info);
        }

        public void LoadExperiment(ExperimentalInfo info)
        {
            resourceLoad.LoadExperimentalInfo(info);

            //初始化所有编辑器
            SceneEditorManager.Init();
            EventEditorManager.Init();
            ObjectEditorManager.Init();
        }
        public void OpenExperiment()
        {
            string path = Application.dataPath+@"/Datas";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            DirectoryInfo root = new DirectoryInfo(path);
            FileInfo[] files = root.GetFiles("*.cl");
            List<DialogBox.DialogBoxDataBase> dataBases = new List<DialogBox.DialogBoxDataBase>();
            foreach(var t in files)
            {
                dataBases.Add(new DialogStrData(t.Name));
            }
            DialogBox.DialogBoxManager.dialogBoxManager.ListSelect("选择实验文件","文件名称",0,dataBases,index=>{
                var f=root.GetFiles("*.cl")[index];
                var filepath = f.FullName;
                var info = Tool.ReadXml<ExperimentalInfo>(filepath);
                SceneEditorManager.ExperimentTitle.text = info.ExperimentTitle;
                LoadExperiment(info);
            });
            //var filepath=EditorUtility.OpenFilePanel("打开实验", path, "cl");
            //if (string.IsNullOrEmpty(filepath)) return;

            //var info= Tool.ReadXml<ExperimentalInfo>(filepath);
            //SceneEditorManager.ExperimentTitle.text = info.ExperimentTitle;
            //LoadExperiment(info);
        }
        public void SaveExperiment(string filepath)
        {
            var info = resourceLoad.GetExperimentalInfo();
            info.ExperimentTitle = SceneEditorManager.ExperimentTitle.text;
            Tool.WriteXml(info, filepath);
        }

        public void SaveExperiment()
        {           
            DialogBox.DialogBoxManager.dialogBoxManager.EditData("保存实验", "实验名称",new DialogStrData(""), res => {
                string path = Application.dataPath + @"/Datas/";
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                var filepath = res.GetValue().ToString();
                if (!filepath.Contains(".cl")) filepath = filepath + ".cl";
                filepath = path + filepath;
                if (File.Exists(filepath))
                {
                    DialogBox.DialogBoxManager.dialogBoxManager.ShowConfirm("警告", "该实验文件已存在，是否覆盖？", () =>
                    {
                        SaveExperiment(filepath);
                    });
                }
                else
                {
                    SaveExperiment(filepath);
                }
                
            });
            //var filepath = EditorUtility.SaveFilePanel("打开实验", path, "新建实验", "cl");
            //if (string.IsNullOrEmpty(filepath)) return;
            ////Debug.Log("Save");
            //var info=resourceLoad.GetExperimentalInfo();
            //info.ExperimentTitle = SceneEditorManager.ExperimentTitle.text;
            //Tool.WriteXml(info, filepath);

        }

        private void SwitchModule(int index)
        {          
            if (index == 0)
            {
                SceneEditorManager.gameObject.SetActive(true);
                EventEditorManager.gameObject.SetActive(false);
                ObjectEditorManager.gameObject.SetActive(false);
               
            }
            else if (index == 1)
            {
                SceneEditorManager.gameObject.SetActive(false);
                EventEditorManager.gameObject.SetActive(true);
                ObjectEditorManager.gameObject.SetActive(false);
               
            }
            else if (index == 2)
            {
                SceneEditorManager.gameObject.SetActive(false);
                EventEditorManager.gameObject.SetActive(false);
                ObjectEditorManager.gameObject.SetActive(true);
            }
        }

        public void ReturnMain()
        {
            DialogBox.DialogBoxManager.dialogBoxManager.ShowConfirm("警告","确认离开编辑器？",()=> {
                SceneManager.LoadScene("Main");
            });          
        }


    }
}
