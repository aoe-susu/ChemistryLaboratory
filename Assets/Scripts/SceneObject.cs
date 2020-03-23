using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CLEditor
{
    public class SceneObject : MonoBehaviour
    {
        public static SceneObject BeSelected;

        public GameObject SignPrefabs;
        private GameObject Sign;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (BeSelected == null && Sign != null) Sign.SetActive(false);
        }
        public void Select()
        {
            if (BeSelected != null)
            {
                BeSelected.Sign.SetActive(false);                
            }

            BeSelected = this;           
            if (Sign == null) Sign=Instantiate(SignPrefabs, transform);
            Sign.transform.eulerAngles = new Vector3(90, 0, 0);
            Sign.SetActive(true);
            
        }

        public static void ClearSelect()
        {
            BeSelected = null;
        }

       

    }
}

