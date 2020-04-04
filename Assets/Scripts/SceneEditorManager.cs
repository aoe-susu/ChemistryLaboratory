using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CLEditor
{
    public class SceneEditorManager : MonoBehaviour, IPointerClickHandler
    {

        public TextTreeControl ObjectNameTree;
        public Material NotPlace;
        public Material CanPlace;
        public GameObject SignPrefabs;
        public Camera MainCamera;
        //已放置的物体列表


        private ObjectPlaceStateControl slelctedobj;
        private List<SceneObject> SceneObjects = new List<SceneObject>();


        // Use this for initialization
        void Start()
        {
            //ObjectNameTree.TextContentTree = ResourceLoad.resourceLoad.GetCL_ObjectNames();
            //ObjectNameTree.SelectEvent = SelectObject;
        }

        // Update is called once per frame
        void Update()
        {
            if (slelctedobj != null)
            {
                SceneObject.ClearSelect();
                SceneViewCameraControl.ClearView();

                Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
                var pos = Tool.GetRayPointFromY(ray.origin, ray.direction, 0);              
                slelctedobj.transform.position = new Vector3(pos.x, 0, pos.z);
                SetObjectState(slelctedobj.State);
                if (Input.GetMouseButtonDown(1))
                {
                    if (slelctedobj.State == ObjectControlState.CanPlace)
                    {
                        PlaceObject(slelctedobj.gameObject);
                    }
                }
            }
        }

        //从物体列表选择物体，用于放置物体
        void SelectObject(string objectname)
        {
            if (slelctedobj != null) Destroy(slelctedobj.gameObject);
            var clobj= ResourceLoad.resourceLoad.GetObjectInfoByName(objectname);
            if (clobj == null) return;
            var prefab = ResourceLoad.resourceLoad.GetPrefabsByModelId(clobj.ModelId);
            if (prefab == null) return;
            slelctedobj = Instantiate(prefab).AddComponent<ObjectPlaceStateControl>();
            slelctedobj.name = clobj.Name;
            SetObjectState(slelctedobj.State);
        }      
        //设置物体展示的状态，表现该位置是否可以放置本物体
        void SetObjectState(ObjectControlState state)
        {
            var renderers = slelctedobj.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                if (state== ObjectControlState.CanPlace) renderers[i].sharedMaterial = CanPlace;
                else if (state == ObjectControlState.NotPlace) renderers[i].sharedMaterial = NotPlace;
            }
        }


        //放置物体，把展示物体放置在场景中
        void PlaceObject(GameObject obj)
        {
            var clobj = ResourceLoad.resourceLoad.GetObjectInfoByName(obj.name);
            if (clobj == null) return;
            var prefab = ResourceLoad.resourceLoad.GetPrefabsByModelId(clobj.ModelId);
            if (prefab == null) return;
            var ob = Instantiate(prefab).AddComponent<SceneObject>();
            ob.name = clobj.Name;
            ob.transform.position = obj.transform.position;
            ob.transform.localScale= new Vector3(1,1,1)*clobj.ModelSize;
            ob.SignPrefabs = SignPrefabs;
            SceneObjects.Add(ob);
        }

        //左点击，选择场景中的物体
        public void OnPointerClick(PointerEventData eventData)
        {
            if (slelctedobj==null)
            {
                Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo))
                {
                    var sceneobj = hitInfo.collider.GetComponent<SceneObject>();
                    if (sceneobj != null)
                    {
                        SceneViewCameraControl.ShowInSceneView(sceneobj.gameObject);
                        sceneobj.Select();
                    }
                }
            }
        }
    }


}
