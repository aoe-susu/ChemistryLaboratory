using DialogBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CLEditor
{
    public class RadioOption : MonoBehaviour, IPointerClickHandler
    {

        public string RadioName;
        public Sprite Selected;
        public Sprite UnSelected;
        public Image Sign;
        public EventDataDialogBox dialogBox;
        // Use this for initialization
        

        public void OnPointerClick(PointerEventData eventData)
        {
            Select();
        }

        public void Select()
        {
            if(dialogBox==null) dialogBox = GetComponentInParent<EventDataDialogBox>();
            if (dialogBox.select != this)
            {
                if (dialogBox.select != null)
                    dialogBox.select.Sign.sprite = UnSelected;
                this.Sign.sprite = Selected;
                dialogBox.select = this;
            }
        }
    }

}

