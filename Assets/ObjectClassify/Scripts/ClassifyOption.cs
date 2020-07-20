using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace CLEditor
{
    public class ClassifyOption : ViewOption
    {

        public override void SetOption(object obj)
        {
            string typename = obj.ToString();
            transform.Find("TypeName").GetComponent<Text>().text = typename;
        }

    }
}

