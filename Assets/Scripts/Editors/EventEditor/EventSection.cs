using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CLEditor
{
    public class EventSection : ViewOption
    {
        public Text Name;
        public override void SetOption(object obj)
        {
            var eventinfo = obj as EventInfo;
            Name.text = eventinfo.Name;
            //base.SetOption(obj);
        }


    }
}

