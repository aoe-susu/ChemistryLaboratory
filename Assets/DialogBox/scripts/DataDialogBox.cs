using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DialogBox
{
    public class DataDialogBox: MonoBehaviour
    {
        public Text Title;
        public Text Name;
        public InputField Value;
        public UnityAction<DialogBoxDataBase> ConfirmEvent;
        public UnityAction CancelEvent;
        public DialogBoxDataBase dataBase=null;
        public void Confirm()
        {
            var data = dataBase;
            if (data == null) data = new DialogBoxDataBase();
            data.SetValue(Value.text);
            if (ConfirmEvent != null) ConfirmEvent(data);
            Destroy(this.gameObject);
        }
        public void Cancel()
        {
            if (CancelEvent != null) CancelEvent();
            Destroy(this.gameObject);
        }
    }
}