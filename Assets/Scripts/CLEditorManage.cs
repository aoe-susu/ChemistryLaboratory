using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CLEditor
{
    public class CLEditorManage:MonoBehaviour
    {
        public MenuManager MenuManager;
        public SceneEditorManager SceneEditorManager;
        public ObjectEditorManger ObjectEditorManger;
        public EventEditorManger EventEditorManger;

        //private List<CL_Object> PlacedObjectInfos = new List<CL_Object>();

        private void Start()
        {
            MenuManager.CreateExperiment = CreateExperiment;
            MenuManager.OpenExperiment = OpenExperiment;
            MenuManager.SaveExperiment = SaveExperiment;
            MenuManager.SwitchModule = SwitchModule;
        }


        public void CreateExperiment()
        {

        }
        public void OpenExperiment()
        {
            string path = Application.dataPath+@"/Datas";
            var filepath=EditorUtility.OpenFilePanel("打开实验", path, "cl");

        }
        public void SaveExperiment()
        {

        }

        private void SwitchModule(int index)
        {
            if (index == 0)
            {
                SceneEditorManager.gameObject.SetActive(true);
                EventEditorManger.gameObject.SetActive(false);
                ObjectEditorManger.gameObject.SetActive(false);
               
            }
            else if (index == 1)
            {
                SceneEditorManager.gameObject.SetActive(false);
                EventEditorManger.gameObject.SetActive(true);
                ObjectEditorManger.gameObject.SetActive(false);
               
            }
            else if (index == 2)
            {
                SceneEditorManager.gameObject.SetActive(false);
                EventEditorManger.gameObject.SetActive(false);
                ObjectEditorManger.gameObject.SetActive(true);
            }
        }

    }
}
