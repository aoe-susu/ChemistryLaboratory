using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CLEditor
{
    public class EventClassifyOption : ScrollViewOptions
    {
        public Sprite Openico;
        public Sprite Closeico;
        public Image Background;
        public Text Name;
        public Button Flexible;
        public Image Ico;
        private bool open=true;

        public override void SetOption(object obj)
        {         
            var eventinfos = (KeyValuePair<string, List<EventInfo>>)obj;
            Name.text = eventinfos.Key;
            SetOPtions(eventinfos.Value);        
            FlexibleClick();
            if(viewOptions != null) OtherAction = viewOptions.OtherAction;
        }
        public override void SetBackGroundColor(Color color)
        {
            //base.SetBackGroundColor(color);
            Background.color = color;
        }

        public override void OptionClick(GameObject obj)
        {
            viewOptions.OptionClick(-1);
            //foreach (var t in viewOptions.initgameobject)
            //{
            //    t.GetComponent<EventClassifyOption>().OptionClick(-1);
            //}
            base.OptionClick(obj);
            if (OtherAction != null)
            {
                int index = viewOptions.initgameobject.IndexOf(this.gameObject);
                var clissifyname = viewOptions.initgameobject[index].GetComponent<EventClassifyOption>().Name.text;
                OtherAction(new KeyValuePair<string, int>(clissifyname, Value));
            }
            
        }

        public void FlexibleClick()
        {
            if (open)
            {
                Ico.sprite = Closeico;
                Flexible.transform.Find("Text").GetComponent<Text>().text = "+";
                initgameobject.ForEach(s => s.SetActive(false));
                open = false;
            }
            else
            {
                Ico.sprite = Openico;
                Flexible.transform.Find("Text").GetComponent<Text>().text = "-";
                initgameobject.ForEach(s => s.SetActive(true));
                open = true;
            }
        }
    }

}
