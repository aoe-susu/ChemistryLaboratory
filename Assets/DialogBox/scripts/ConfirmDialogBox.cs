using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DialogBox
{
    public class ConfirmDialogBox : MonoBehaviour
    {
        public Text Title;
        public Text Content;
        public UnityAction ConfirmEvent;
        public UnityAction CancelEvent;
        // Use this for initialization
        public void Confirm()
        {
            if (ConfirmEvent != null) ConfirmEvent();
            Destroy(this.gameObject);
        }
        public void Cancel()
        {
            if (CancelEvent != null) CancelEvent();
            Destroy(this.gameObject);
        }
    }
}
    
