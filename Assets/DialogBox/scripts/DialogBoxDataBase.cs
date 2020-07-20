using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DialogBox {
    public class DialogBoxDataBase 
    {
        public virtual string GetDialogBoxShowString() {
            return "";
        }
        public virtual void SetValue(string data)
        {          
            
        }

        public virtual object GetValue()
        {
            return null;
        }

    }

   
}
