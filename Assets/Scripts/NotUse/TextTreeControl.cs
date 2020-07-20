using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace CLEditor
{
    public class TextTreeControl : MonoBehaviour
    {
        public GameObject Temp;
        public Tree<string> TextContentTree;
        public TextTree Select;
        private Color32 SelectedColor = new Color32(200, 200, 255, 255);
        public UnityAction<string> SelectEvent;

        private TextTree First;
        // Use this for initialization
        void Start()
        {

            if (TextContentTree == null)
            {
                TextContentTree = new Tree<string>();
                TextContentTree.content = "测试用例s";
                for (int i = 0; i < 4; i++)
                {

                    var t = new Tree<string>() { content = "测试用例" + i };
                    t.SetParent(TextContentTree);
                    if (i >= 2) continue;
                    for (int j = 0; j < 2; j++)
                    {
                        var p = new Tree<string>() { content = "测试用例" + i + j };
                        p.SetParent(t);
                        for (int k = 0; k < 2; k++)
                        {
                            var l = new Tree<string>() { content = "测试用例" + i + j + k };
                            l.SetParent(p);
                        }
                    }
                }
            }


            LoadTextTree();
        }


        public void LoadTextTree()
        {
            Dest();
            First = Init(TextContentTree, transform);
            var rtf = First.GetComponent<RectTransform>();
            rtf.localPosition = new Vector3(rtf.rect.width / 2, -rtf.rect.height / 2, 0);
            First.gameObject.SetActive(true);
        }

        public void Expansion()
        {
            var rtf = LoadTextTreePos(First);
            GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rtf.rect.height);
            rtf.localPosition = new Vector3(rtf.rect.width / 2, -rtf.rect.height / 2, 0);
            First.gameObject.SetActive(true);

        }

        private TextTree Init(Tree<string> tree, Transform parent)
        {
            TextTree newtree = Instantiate(Temp, parent).GetComponent<TextTree>();
            newtree.TextTreeControl = this;
            newtree.Text.text = tree.content;
            var childrencount = tree.ChildrenCount;
            if (childrencount == 0)
            {
                newtree.Icon.gameObject.SetActive(false);
                newtree.Text.transform.localPosition -= new Vector3(12.5f, 0, 0);
            }
            //float PosY = 0;
            for (int i = 0; i < childrencount; i++)
            {
                var t = Init(tree.GetChild(i), newtree.List).GetComponent<RectTransform>();
                //t.localPosition = new Vector3(0, PosY- t.rect.height/2, 0);
                //PosY = PosY - t.rect.height;                     
            }
            //var recttran = newtree.GetComponent<RectTransform>();
            //recttran.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, recttran.rect.height- PosY);

            return newtree;
        }

        private void Dest()
        {
            if (First != null)
                Destroy(First.gameObject);
        }

        private RectTransform LoadTextTreePos(TextTree texttree)
        {
            var recttran = texttree.transform.GetComponent<RectTransform>();
            recttran.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 15);
            var childrencount = texttree.List.childCount;
            float PosY = 0;
            if (texttree.Expansion)
            {
                for (int i = 0; i < childrencount; i++)
                {
                    var child = texttree.List.GetChild(i);
                    child.gameObject.SetActive(true);
                    var t = LoadTextTreePos(child.GetComponent<TextTree>());
                    t.localPosition = new Vector3(0, PosY - t.rect.height / 2, 0);
                    PosY = PosY - t.rect.height;
                }
            }
            else
            {
                for (int i = 0; i < childrencount; i++)
                {
                    var child = texttree.List.GetChild(i);
                    child.gameObject.SetActive(false);
                    var t = LoadTextTreePos(child.GetComponent<TextTree>());
                }
            }
            recttran.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, recttran.rect.height - PosY);
            return recttran;
        }

        public void SelectTextTree(TextTree textTree)
        {
            if (Select != null)
            {
                Select.Background.color = new Color32(255, 255, 255, 0);
            }
            Select = textTree;
            Select.Background.color = SelectedColor;
            if (SelectEvent != null) SelectEvent(textTree.Text.text);
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}

