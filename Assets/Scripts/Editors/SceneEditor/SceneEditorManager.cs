using DialogBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CLEditor
{
    public class SceneEditorManager : MonoBehaviour
    {
        public InputField ExperimentTitle;//实验标题
        public ObjectClassify ObjectClassify;//物体分类
        public ObjectSelectManager selectManager;//物体选择管理
        public Material CanInitMaterial;//可初始化的材质
        public Material NotCanInitMaterial;//不可初始化的材质   
        public Vector3 MapMinLimit;//地图边界的最小值
        public Vector3 MapMaxLimit;//地图边界的最大值
        //用于保存场景中的物体及其对应的CL_Object的名称
       
        public ResourceLoad resourceLoad;
        private ModelInit ModelInit;//初始化模型，红色代表不可初始化，绿色代表可初始化


        //下面字段用于选定物体
        //public GameObject SelectSignPrefab;//选择标志的预设体
        //public LineRenderer SelectLine;//选择的线框
        //private Vector3? SelectPos;//鼠标按下时的映射的世界坐标
        private Vector3 LastMoveDis;//上一次物体移动的距离
        private bool MoveSelectObj=false;//移动被选择的物体
        //private List<GameObject> SelectSignList = new List<GameObject>();//选择标志list
        //private ObjectSelect ObjectSelect;
        private void OnEnable()
        {
            Init();
        }

        public void Init()
        {
            ObjectClassify.Classify.Clear();
            ObjectClassify.LoadInfo(resourceLoad);
            ObjectClassify.OnSelectClObject = SelectObjectThumb;
            selectManager.Init(resourceLoad);
            selectManager.OnStartSelect = ObjectSelect;
            selectManager.OnSelected = ObjectSelected;
            selectManager.OnSelecting = ObjectSelecting;
            CancelModel();
        }

        private void Update()
        {
            if (ModelInit != null)//存在初始化模型，让模型跟随鼠标位置
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Vector3 nowpos = Tool.GetRayPointFromY(ray.origin, ray.direction, 0);
                ModelInit.transform.position = nowpos;//将初始化模型的位置设置为鼠标指向3d坐标
                ModelInit.SetInlimit(InLimit(nowpos));
            }
        }

        

        private void SelectObjectThumb(CL_Object clobj)
        {
            if (ModelInit != null)
            {
                Destroy(ModelInit.gameObject);
            }
            var prefab = resourceLoad.GetPrefabsByModelId(clobj.Model);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 pos = Tool.GetRayPointFromY(ray.origin, ray.direction, 0);
            var obj = Instantiate(prefab, pos, prefab.transform.rotation);
            obj.transform.localScale *= clobj.ModelSize;
            obj.GetComponent<Collider>().isTrigger = true;
            ModelInit = obj.AddComponent<ModelInit>();
            ModelInit.CanInitMaterial = CanInitMaterial;
            ModelInit.NotCanInitMaterial = NotCanInitMaterial;
        }
        private void ObjectSelect(Vector3 presspoint, List<GameObject> sign)
        {
            if (ModelInit != null)
            {
                selectManager.IsSelect = false;
                return;
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;
            //当鼠标指向物体时，并且该物体属于已选则的物体，则可移动所有已选择的物体物体
            if (Physics.Raycast(ray, out raycastHit) && selectManager.ObjectSelected(raycastHit.transform))
            {
                MoveSelectObj = true;
                selectManager.IsSelect = false;
            }
            else
            {
                selectManager.IsSelect = true;
            }
        }

        private void ObjectSelecting(Vector3 presspoint, Vector3 nowpoint, List<GameObject> sign)
        {
            if (ModelInit != null) return;
            
            if (MoveSelectObj)
            {
                Vector3 dec = nowpoint - presspoint;
                foreach (var t in sign)
                {
                    var tran = t.transform.parent;
                    tran.position -= LastMoveDis;
                    tran.position += dec;
                }
                LastMoveDis = dec;
            }          
        }

        private void ObjectSelected(Vector3 presspoint, Vector3 nowpoint,List<GameObject> sign)
        {
            if (MoveSelectObj)//结束移动
            {
                MoveSelectObj = false;
                //超出限定范围的物体提示是否删除
                bool isover = sign.Exists(s => !InLimit(s.transform.parent.position));
                if (isover)//如果存在超出界限
                {
                    DialogBoxManager.dialogBoxManager.ShowConfirm("提示", "确认删除超出界限的物体吗？", () =>
                    {
                        List<GameObject> overlimitobject = new List<GameObject>(sign);
                        foreach (var t in overlimitobject)
                        {
                            var tran = t.transform.parent;
                            if (!InLimit(tran.position))
                            {
                                DeleteObject(tran.gameObject);
                            }
                        }
                        LastMoveDis = new Vector3();

                    }, () =>
                    {
                        foreach (var t in sign)
                        {
                            var tran = t.transform.parent;
                            tran.position -= LastMoveDis;
                        }
                        LastMoveDis = new Vector3();
                    });
                }
                else
                {
                    LastMoveDis = new Vector3();
                }
            }
            if(ModelInit != null)
            {
                if (ModelInit.CanInit)
                {
                    InitModel();
                }
                else
                {
                    CancelModel();
                }
            }
        }
      

         
        private void DeleteObject(GameObject gameObject)
        {
            resourceLoad.SceneObjects.Remove(gameObject);
            selectManager.DeleteSignObject(gameObject.transform);
            Destroy(gameObject);
        }
        public void InitModel()
        {
            if (ObjectClassify.SelectObj==null) return;
            var clobj = ObjectClassify.SelectObj;
            var prefab = resourceLoad.GetPrefabsByModelId(clobj.Model);
            var obj=Instantiate(prefab, ModelInit.transform.position, ModelInit.transform.rotation);
            var objselect = obj.GetComponent<CLGameObject>();
            objselect.Load(resourceLoad, clobj);
            resourceLoad.SceneObjects.Add(obj);
        }
        public void CancelModel()
        {
            if(ModelInit!=null)
                Destroy(ModelInit.gameObject);
            ObjectClassify.Thumbnail.OptionClick(-1);
        }

     
        private bool InLimit(Vector3 pos)
        {
            return 
                pos.x >= MapMinLimit.x && pos.y >= MapMinLimit.y && pos.z >= MapMinLimit.z &&
                pos.x <= MapMaxLimit.x && pos.y <= MapMaxLimit.y && pos.z <= MapMaxLimit.z;
        }

        public void ReNameSceneObject(string oldname,string newname)
        {
            for(int i=0;i< resourceLoad.SceneObjects.Count; i++)
            {
                var temp = resourceLoad.SceneObjects[i];
                var clgameobj = temp.GetComponent<CLGameObject>();
                if (clgameobj.cl_object.Name == oldname)
                {
                    CL_Object clobj = resourceLoad.GetObjectInfoByName(newname);
                    clobj.Id = clgameobj.cl_object.Id;                   
                    clgameobj.Load(resourceLoad, clobj);
                }
            }
           
        }
        













        //    public TextTreeControl ObjectNameTree;
        //    public Material NotPlace;
        //    public Material CanPlace;
        //    public GameObject SignPrefabs;
        //    public Camera MainCamera;
        //    //已放置的物体列表


        //    private ObjectPlaceStateControl slelctedobj;
        //    private List<SceneObject> SceneObjects = new List<SceneObject>();


        //    // Use this for initialization
        //    void Start()
        //    {
        //        ObjectNameTree.TextContentTree = ResourceLoad.resourceLoad.GetCL_ObjectNames();
        //        ObjectNameTree.SelectEvent = SelectObject;
        //    }

        //    // Update is called once per frame
        //    void Update()
        //    {
        //        if (slelctedobj != null)
        //        {
        //            SceneObject.ClearSelect();
        //            SceneViewCameraControl.ClearView();

        //            Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
        //            var pos = Tool.GetRayPointFromY(ray.origin, ray.direction, 0);              
        //            slelctedobj.transform.position = new Vector3(pos.x, 0, pos.z);
        //            SetObjectState(slelctedobj.State);
        //            if (Input.GetMouseButtonDown(1))
        //            {
        //                if (slelctedobj.State == ObjectControlState.CanPlace)
        //                {
        //                    PlaceObject(slelctedobj.gameObject);
        //                }
        //            }
        //        }
        //    }

        //    //从物体列表选择物体，用于放置物体
        //    void SelectObject(string objectname)
        //    {
        //        if (slelctedobj != null) Destroy(slelctedobj.gameObject);
        //        var clobj= ResourceLoad.resourceLoad.GetObjectInfoByName(objectname);
        //        if (clobj == null) return;
        //        var prefab = ResourceLoad.resourceLoad.GetPrefabsByModelId(clobj.ModelId);
        //        if (prefab == null) return;
        //        slelctedobj = Instantiate(prefab).AddComponent<ObjectPlaceStateControl>();
        //        slelctedobj.name = clobj.Name;
        //        SetObjectState(slelctedobj.State);
        //    }      
        //    //设置物体展示的状态，表现该位置是否可以放置本物体
        //    void SetObjectState(ObjectControlState state)
        //    {
        //        var renderers = slelctedobj.GetComponentsInChildren<MeshRenderer>();
        //        for (int i = 0; i < renderers.Length; i++)
        //        {
        //            if (state== ObjectControlState.CanPlace) renderers[i].sharedMaterial = CanPlace;
        //            else if (state == ObjectControlState.NotPlace) renderers[i].sharedMaterial = NotPlace;
        //        }
        //    }


        //    //放置物体，把展示物体放置在场景中
        //    void PlaceObject(GameObject obj)
        //    {
        //        var clobj = ResourceLoad.resourceLoad.GetObjectInfoByName(obj.name);
        //        if (clobj == null) return;
        //        var prefab = ResourceLoad.resourceLoad.GetPrefabsByModelId(clobj.ModelId);
        //        if (prefab == null) return;
        //        var ob = Instantiate(prefab).AddComponent<SceneObject>();
        //        ob.name = clobj.Name;
        //        ob.transform.position = obj.transform.position;
        //        ob.transform.localScale= new Vector3(1,1,1)*clobj.ModelSize;
        //        ob.SignPrefabs = SignPrefabs;
        //        SceneObjects.Add(ob);
        //    }

        //    //左点击，选择场景中的物体
        //    public void OnPointerClick(PointerEventData eventData)
        //    {
        //        if (slelctedobj==null)
        //        {
        //            Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
        //            RaycastHit hitInfo;
        //            if (Physics.Raycast(ray, out hitInfo))
        //            {
        //                var sceneobj = hitInfo.collider.GetComponent<SceneObject>();
        //                if (sceneobj != null)
        //                {
        //                    SceneViewCameraControl.ShowInSceneView(sceneobj.gameObject);
        //                    sceneobj.Select();
        //                }
        //            }
        //        }
        //    }
    }


}
