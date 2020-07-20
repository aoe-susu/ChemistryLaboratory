using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DialogBox
{
    public class ListSelectDialogBox : MonoBehaviour
    {
        public Text Title;
        public Text Name;
        public Dropdown Options;
        public UnityAction<int> ConfirmEvent;
        public UnityAction CancelEvent;
        // Use this for initialization

        public void SetOption(List<DialogBoxDataBase> opts,int defaultval=-1) {
            Options.ClearOptions();
            foreach (var t in opts) {
                Options.options.Add(new Dropdown.OptionData(t.GetDialogBoxShowString()));
            }
            if (opts.Count > 0)
            {
                Options.value = defaultval;
                Options.RefreshShownValue();
                //Options.captionText.text = opts[0].GetDialogBoxShowString();
            }
        }

        public void Confirm()
        {
            if (ConfirmEvent != null) ConfirmEvent(Options.value);
            Destroy(this.gameObject);
        }
        public void Cancel()
        {
            if (CancelEvent != null) CancelEvent();
            Destroy(this.gameObject);
        }
    }
}