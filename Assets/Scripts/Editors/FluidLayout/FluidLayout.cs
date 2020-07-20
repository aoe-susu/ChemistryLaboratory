using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CLEditor
{
    public class FluidLayout : MonoBehaviour
    {
        //private Vector3 defaultposition;

        //private void Start()
        //{
        //    defaultposition = transform.localPosition;
        //}
        // Update is called once per frame
        public Vector2 Pivot;

        void Update()
        {
            AutoHeight(GetComponent<RectTransform>());
        }

        private void OnValidate()
        {           
            AutoHeight(GetComponent<RectTransform>());
        }


        void AutoHeight(RectTransform recttran)
        {
            if (recttran == null) return;
            recttran.pivot = Pivot;
            float totalheight = 0;
            int childcount = recttran.childCount;
            List<RectTransform> children = new List<RectTransform>();
            for (int i = 0; i < childcount; i++)
            {
                var rect = recttran.GetChild(i).GetComponent<RectTransform>();
                if (!rect.gameObject.activeSelf) continue;
                if (rect == null) continue;
                var flu = rect.GetComponent<FluidLayout>();
                if (flu != null)
                {
                    flu.Pivot = Pivot;
                }
                else
                {
                    rect.pivot = Pivot;
                }
                totalheight += rect.rect.height;
                children.Add(rect);

            }
            if (childcount > 0)
            {
                recttran.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalheight);
                //recttran.localPosition = defaultposition + new Vector3(0, totalheight / 2, 0);
            }

            float nowheight = 0;
            foreach (var t in children)
            {
                t.localPosition = new Vector3(t.localPosition.x, 0, t.localPosition.z);//坐标置零
                t.localPosition += new Vector3(0, (1 - t.pivot.y) * totalheight, 0);//根据锚点确定开始位置
                //根据子元素height下移
                //根据子元素锚点确定移动的距离
                t.localPosition -= new Vector3(0,nowheight+t.rect.height* (1 - t.pivot.y), 0);
                nowheight += t.rect.height;
            }
        }

    }

}
