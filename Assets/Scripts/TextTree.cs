using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CLEditor
{
    public class TextTree : MonoBehaviour, IPointerClickHandler
    {
        public Text Text;
        public Transform Icon;
        public Transform List;
        public Image Background;
        public bool Expansion;
        public TextTreeControl TextTreeControl;

        public void ExpansionClick()
        {
            Expansion = !Expansion;
            if (Expansion)
            {
                Icon.localEulerAngles = new Vector3();
            }
            else
            {
                Icon.localEulerAngles = new Vector3(0, 0, 90);
            }
            TextTreeControl.Expansion();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            TextTreeControl.SelectTextTree(this);
        }

        // Use this for initialization
        void Start()
        {

        }


        // Update is called once per frame
        void Update()
        {

        }
    }

}

