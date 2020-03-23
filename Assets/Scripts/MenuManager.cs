using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CLEditor
{
    public class MenuManager : MonoBehaviour
    {
        public Dropdown Experiment;
        public Dropdown Editor;
        public Dropdown Module;

        public UnityAction CreateExperiment;
        public UnityAction OpenExperiment;
        public UnityAction SaveExperiment;
        public UnityAction<int> SwitchModule;
        // Use this for initialization
        void Start()
        {
            Experiment.onValueChanged.AddListener(ExperimentChange);
            Editor.onValueChanged.AddListener(EditorChange);
            Module.onValueChanged.AddListener(ModuleChange);
        }

        // Update is called once per frame
        public void ExperimentChange(int index)
        {
            if (index == 0)
            {
                if (CreateExperiment != null) CreateExperiment();
            }
            else if (index == 1)
            {
                if (OpenExperiment != null) OpenExperiment();
            }
            else if (index == 2)
            {
                if (SaveExperiment != null) SaveExperiment();
            }
        }
        public void EditorChange(int index)
        {

        }
        public void ModuleChange(int index)
        {
            if (SwitchModule != null) SwitchModule(index);
        }
    }

}
