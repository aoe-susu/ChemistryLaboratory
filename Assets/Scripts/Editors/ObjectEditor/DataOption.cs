using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace CLEditor
{
    public class DataOption : ViewOption
    {
        public override void SetOption(object obj)
        {
            KeyValuePair<string,object> keyValue=(KeyValuePair<string, object>)obj;
            transform.Find("Name").GetComponent<Text>().text = keyValue.Key;
            var value = transform.Find("Value").GetComponent<Text>();
            if (keyValue.Key == "Operations")
            {
                List<Operation> list = keyValue.Value as List<Operation>;
                StringBuilder builder = new StringBuilder();
                list.ForEach(s => builder.Append(s.Name).Append(' '));
                value.text = builder.ToString();
            }
            else if(keyValue.Key == "Model")
            {
                ResourceLoad resourceLoad = viewOptions.OtherData as ResourceLoad;
                value.text = resourceLoad.GetPrefabsByModelId((int)keyValue.Value).name;
            }
            else value.text = keyValue.Value.ToString();
        }
    }
}

