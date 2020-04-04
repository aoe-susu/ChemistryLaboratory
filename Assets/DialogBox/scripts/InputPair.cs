using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DialogBox
{

    public class InputPair : MonoBehaviour
    {
        public Text Name;
        public InputField Value;

        public void SetValueInputType(InputField.ContentType contentType) {
            Value.contentType = contentType;
        }

        // Use this for initialization
        public void SetName(string name) {
            Name.text = name + "：";
        }
        //private void Start()
        //{
        //    Debug.Log(typeof(string));
        //    Debug.Log(typeof(String));
        //}
        public int GetIntValue()
        {
            return int.Parse(Value.text);
        }
        public float GetFloatValue()
        {
            return float.Parse(Value.text);
        }
        public string GetStringValue()
        {
            return Value.text;
        }
    }

}
