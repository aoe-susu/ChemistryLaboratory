using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DialogBox
{
    public class MessageDialogBox : MonoBehaviour
    {
        public Text Title;
        public Text Content;
        public UnityAction ConfirmEvent;
        // Use this for initialization
        public void Confirm()
        {
            if (ConfirmEvent != null) ConfirmEvent();
            Destroy(this.gameObject);
        }
    }
}
    
