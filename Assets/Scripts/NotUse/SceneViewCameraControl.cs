using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CLEditor
{
    public class SceneViewCameraControl : MonoBehaviour
    {
        private static GameObject ShowObj;
        public Vector3 InitPos;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (ShowObj != null)
            {
                transform.position = InitPos + ShowObj.transform.position;
            }

        }

        public static void ShowInSceneView(GameObject obj)
        {
            ClearView();
            ShowObj = obj;
            SetAllLayout(ShowObj, 8);
        }

        public static void ClearView()
        {
            if (ShowObj != null) SetAllLayout(ShowObj, 0);
            ShowObj = null;
        }

        private static void SetAllLayout(GameObject gameObject, int i)
        {
            var trans = gameObject.GetComponentsInChildren<Transform>();
            foreach (var tran in trans)
            {
                tran.gameObject.layer = i;
            }
        }

    }

}
