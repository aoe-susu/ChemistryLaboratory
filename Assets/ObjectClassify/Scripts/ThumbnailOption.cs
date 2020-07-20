using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace CLEditor
{
    public class ThumbnailOption : ViewOption
    {

        public override void SetOption(object obj)
        {
            ResourceLoad resource = viewOptions.OtherData as ResourceLoad;
            var clobj = obj as CL_Object;
            transform.Find("Name").GetComponent<Text>().text = clobj.Name;
            var prefabs = resource.GetPrefabsByModelId(clobj.Model);
            Texture2D texture = resource.ObjectPrefabsTexture2D[clobj.Model];
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 128, 128), Vector2.zero);
            GetComponent<Image>().sprite = sprite;
        }
    }
}

