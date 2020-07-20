using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CLEditor
{
    public class ModelInit: MonoBehaviour
    {
        public Material CanInitMaterial;
        public Material NotCanInitMaterial;
        private bool iscolloider = false;
        public bool CanInit {
            get {
                return inlimit&&!iscolloider;
            }         
        }

        private bool inlimit = true;

        public void SetInlimit(bool val)
        {
            bool lastcaninit = CanInit;
            inlimit = val;
            //if (lastcaninit == CanInit) return;
            if (CanInit)
            {
                Tool.SetObjMaterial(this.gameObject, CanInitMaterial);
            }
            else
            {
                Tool.SetObjMaterial(this.gameObject, NotCanInitMaterial);
            }
        }


       
        private void OnTriggerEnter(Collider other)
        {
            iscolloider = true;
        }

       

        private void OnTriggerExit(Collider other)
        {
            iscolloider = false; 
        }
    }
}