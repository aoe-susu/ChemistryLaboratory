using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

namespace DialogBox {
    public class ListDialogContent : MonoBehaviour, IPointerClickHandler
    {
        public static ListDialogContent Select;
        public DataListDialogBox dialogBox;
        public void OnPointerClick(PointerEventData eventData)
        {
            for (int i = 0; i < dialogBox.ViewContent.childCount; i++)
            {
                if (dialogBox.ViewContent.GetChild(i) == transform)
                {
                    dialogBox.SelectContent(i);
                    break;
                }
            }
            
        }
    }
}

