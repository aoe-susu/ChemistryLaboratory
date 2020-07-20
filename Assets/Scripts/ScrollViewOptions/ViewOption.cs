using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace CLEditor
{
    public class ViewOption : MonoBehaviour, IPointerClickHandler
    {

        public ScrollViewOptions viewOptions;
        public virtual void SetOption(object obj)
        {

        }
        public virtual void SetBackGroundColor(Color color)
        {
            var img = GetComponent<Image>();
            if (img != null) img.color = color;
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            if(viewOptions!=null)
                viewOptions.OptionClick(this.gameObject);
        }

    }
}
