using CLEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DialogBox
{
    public class DataListDialogBox : MonoBehaviour
    {
        public Text Title;
        public Text Name;
        public Transform ViewContent;
        public GameObject Template;
        public Color SelectColor=new Color();
        public List<DialogBoxDataBase> TotalDatas;

        public UnityAction<List<DialogBoxDataBase>> ConfirmEvent;
        public UnityAction CancelEvent;

        private List<DialogBoxDataBase> Options=new List<DialogBoxDataBase>();    
        private List<GameObject> contents = new List<GameObject>();
        private int selectedindex=-1;
        public void SelectContent(int index) {
            if (selectedindex != -1)
            {
                contents[selectedindex].GetComponent<Image>().color = new Color(255, 255, 255, 255);
            }
            selectedindex = index;
            if(selectedindex!=-1)
            {
                contents[selectedindex].GetComponent<Image>().color = SelectColor;
            }

        }

        public void SetOptions(List<DialogBoxDataBase> opts)
        {
            Options = opts;
            if (Options == null) Options = new List<DialogBoxDataBase>();
            foreach (var t in contents)
            {
                Destroy(t.gameObject);
            }
            contents.Clear();
            foreach (var t in Options)
            {
                var obj = Instantiate(Template, ViewContent);
                obj.name = "con";
                obj.GetComponent<ListDialogContent>().dialogBox = this;
                obj.GetComponentInChildren<Text>().text = t.GetDialogBoxShowString();
                obj.SetActive(true);
                contents.Add(obj);
            }
        }

        public void Add() {
            DialogBoxManager.dialogBoxManager.ListSelect("选择值-整数索引", Name.text,0, TotalDatas, index =>
            {
                DialogBoxDataBase adddata = TotalDatas[index];

                if (selectedindex == -1)
                {
                    Options.Add(adddata);
                    SetOptions(Options);
                }
                else
                {
                    Options.Insert(selectedindex, adddata);
                    SetOptions(Options);
                    selectedindex++;
                    SelectContent(selectedindex);
                }
            });
            
        }

        public void Edit()
        {
            var database = Options[selectedindex];
            int defaultindex = TotalDatas.IndexOf(database);
            DialogBoxManager.dialogBoxManager.ListSelect("选择值-整数索引", Name.text, defaultindex, TotalDatas, index =>
            {
                DialogBoxDataBase editdata = TotalDatas[index];
                if (selectedindex == -1) return;
                Options[selectedindex] = editdata;
                SetOptions(Options);
                SelectContent(selectedindex);
            });
           
        }
        public void Dele()
        {
            if (selectedindex == -1) return;
            Options.RemoveAt(selectedindex);
            SetOptions(Options);
            if(selectedindex>= Options.Count)
            {
                selectedindex = Options.Count - 1;
            }
            SelectContent(selectedindex);
        }
        public void LastMove()
        {
            if (selectedindex == -1) return;
            if (selectedindex > 0)
            {
                var temp = Options[selectedindex];
                Options[selectedindex] = Options[selectedindex - 1];
                Options[selectedindex - 1] = temp;
                SetOptions(Options);
                selectedindex--;
                SelectContent(selectedindex);
            }
        }
        public void NextMove()
        {
            if (selectedindex == -1) return;
            if (selectedindex <Options.Count-1)
            {
                var temp = Options[selectedindex];
                Options[selectedindex] = Options[selectedindex + 1];
                Options[selectedindex + 1] = temp;
                SetOptions(Options);
                selectedindex++;
                SelectContent(selectedindex);
            }
        }

        public void Confirm()
        {
            if (ConfirmEvent != null) ConfirmEvent(Options);
            Destroy(this.gameObject);
        }
        public void Cancel()
        {
            if (CancelEvent != null) CancelEvent();
            Destroy(this.gameObject);
        }

        // Use this for initialization
        void Start()
        {
            //SetOptions(new List<string>() { "susu", "asdasd","素素","烧杯" });
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
