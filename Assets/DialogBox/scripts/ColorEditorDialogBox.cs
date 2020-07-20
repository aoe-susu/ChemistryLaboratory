using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DialogBox
{
    public class ColorEditorDialogBox : MonoBehaviour
    {
        public Text Title;
        public Text Name;
        public InputField R;
        public InputField G;
        public InputField B;
        public InputField A;
        public Image FinallyShow;
        public UnityAction<Color32> ConfirmEvent;
        public UnityAction CancelEvent;

        private void Update()
        {
            R.onValueChanged.AddListener(ModefyColor);
            G.onValueChanged.AddListener(ModefyColor);
            B.onValueChanged.AddListener(ModefyColor);
            A.onValueChanged.AddListener(ModefyColor);
        }

        private void ModefyColor(string str)
        {
            FinallyShow.color = GetColor();
        }

        public void Confirm()
        {
            var data = GetColor();       
            if (ConfirmEvent != null) ConfirmEvent(data);
            Destroy(this.gameObject);
        }
        public void SetValue(Color32 data)
        {
            R.text = data.r.ToString();
            G.text = data.g.ToString();
            B.text = data.b.ToString();
            A.text = data.a.ToString();
            FinallyShow.color = data;
        }
        public void Cancel()
        {
            if (CancelEvent != null) CancelEvent();
            Destroy(this.gameObject);
        }
        private Color32 GetColor()
        {
            byte r = (byte)(R.text == "" ? 0 : int.Parse(R.text)%256);
            byte g = (byte)(G.text == "" ? 0 : int.Parse(G.text) % 256);
            byte b = (byte)(B.text == "" ? 0 : int.Parse(B.text) % 256);
            byte a = (byte)(A.text == "" ? 0 : int.Parse(A.text) % 256);
            Color32 res = new Color32(r,g,b,a);
            return res;
        }
    }
}
