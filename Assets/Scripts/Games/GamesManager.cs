using CLEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamesManager : MonoBehaviour {

    ResourceLoad resourceLoad;
    public ObjectSelectManager SelectManager;
    public OperationManager OperationManager;
    public EventsManager EventsManager;
    public Text ExperimentTitle;   
    private Vector3 LastPos;
    private Vector3 ThisPos;
    // Use this for initialization
    void Start () {

        OpenExperiment();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void SelectObjectStart(Vector3 pos,List<GameObject> sign)
    {
        SelectManager.IsSelect = true;
        Operation oper = OperationManager.OperationMode;
        if (oper == null) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycastHit;
        //当鼠标指向物体时，并且该物体属于已选则的物体，会按照已选择的操作模式进行操作
        if (Physics.Raycast(ray, out raycastHit) && SelectManager.ObjectSelected(raycastHit.transform))
        {
            SelectManager.IsSelect = false;
            ThisPos = Tool.GetRayPointFromY(ray.origin, ray.direction, raycastHit.transform.position.y);
        }
    }
    void SelectObjecting(Vector3 lastpos,Vector3 nowpos, List<GameObject> sign)
    {
        if (SelectManager.IsSelect) return;
        Operation oper = OperationManager.OperationMode;
        if (oper == null) return;
        
        LastPos = ThisPos;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        ThisPos = Tool.GetRayPointFromY(ray.origin, ray.direction, LastPos.y);
        foreach (var t in sign)
        {
            OperationDeal(oper,t.transform.parent);
        }
        

    }
    void SelectObjectSuccess(Vector3 lastpos, Vector3 nowpos, List<GameObject> sign)
    {
        if(SelectManager.IsSelect)
        {
            SetOperationList(sign);
            return;
        }




    }
    
    void SetOperationList(List<GameObject> sign)
    {       
        List<Operation> opers = new List<Operation>();
        if (sign.Count != 0) opers.AddRange(resourceLoad.Operations);
        foreach (var t in sign)
        {
            var select = t.GetComponentInParent<CLGameObject>();
            if (select == null) { Debug.Log("select为空"); continue; }
            if (EventsManager.AnyObjectSelected != null) EventsManager.AnyObjectSelected(select.cl_object);
            var clobj = resourceLoad.GetObjectInfoByName(select.cl_object.Name);
            List<Operation> temp = new List<Operation>();
            foreach (var s in clobj.Operations)
            {
                if (opers.Exists(s1=>s1.Name==s.Name))
                {
                    temp.Add(s);
                }
            }
            opers.Clear();
            opers = new List<Operation>(temp);
        }     
        OperationManager.SetOperations(opers);
    }

    void OperationDeal(Operation operation,Transform select)
    {
        if (EventsManager.AnyObjectExecuteOperation != null)
            EventsManager.AnyObjectExecuteOperation(select.GetComponent<CLGameObject>().cl_object, operation);
        if (operation.Name == "Move")
        {
            select.position += ThisPos - LastPos;
        }
        else if (operation.Name == "Hold")
        {          
            Vector3 screen = Camera.main.WorldToScreenPoint(select.position);
            Vector3 lastpos = Camera.main.WorldToScreenPoint(LastPos);
            Vector3 thispos = Camera.main.WorldToScreenPoint(ThisPos);
            screen.y += thispos.y - lastpos.y;
            Ray ray = Camera.main.ScreenPointToRay(screen);
            //根据点斜式求高度
            var k = (select.position.x-ray.origin.x) / ray.direction.x;
            float height = k * ray.direction.y + ray.origin.y;
            if (height < 0) height = 0;
            select.position = new Vector3(select.position.x, height, select.position.z);
            
        }
        else if (operation.Name == "Rota")
        {
            Vector3 screen = Camera.main.WorldToScreenPoint(select.position);
            Vector3 lastpos = Camera.main.WorldToScreenPoint(LastPos);
            Vector3 thispos = Camera.main.WorldToScreenPoint(ThisPos);
            float dis = lastpos.x - thispos.x;
            select.Rotate(new Vector3(0,dis,0));
        }
        else if (operation.Name == "Pull")
        {
            Vector3 screen = Camera.main.WorldToScreenPoint(select.position);
            Vector3 lastpos = Camera.main.WorldToScreenPoint(LastPos);
            Vector3 thispos = Camera.main.WorldToScreenPoint(ThisPos);
            float dis = lastpos.x - thispos.x;
            Ray ray = Camera.main.ScreenPointToRay(screen);
            Vector3 dir = ray.direction;
            dir.y = 0;
            select.RotateAround(select.position, dir, dis);
        }
        else if (operation.Name == "Other1")
        {

        }
    }

    void OpenExperiment()
    {
        string path = Application.dataPath + @"/Datas";
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        DirectoryInfo root = new DirectoryInfo(path);
        FileInfo[] files = root.GetFiles("*.cl");
        List<DialogBox.DialogBoxDataBase> dataBases = new List<DialogBox.DialogBoxDataBase>();
        foreach (var t in files)
        {
            dataBases.Add(new DialogStrData(t.Name));
        }
        DialogBox.DialogBoxManager.dialogBoxManager.ListSelect("选择实验文件", "文件名称", 0, dataBases, index => {
            var f = root.GetFiles("*.cl")[index];
            var filepath = f.FullName;
            var info = Tool.ReadXml<ExperimentalInfo>(filepath);
            resourceLoad = GetComponent<ResourceLoad>();
            resourceLoad.LoadExperimentalInfo(info);
            ExperimentTitle.text = info.ExperimentTitle;
            SelectManager.Init(resourceLoad);
            SelectManager.OnStartSelect = SelectObjectStart;
            SelectManager.OnSelecting = SelectObjecting;
            SelectManager.OnSelected = SelectObjectSuccess;
            EventsManager = GetComponent<EventsManager>();
            EventsManager.Init(resourceLoad);
        });
        //var filepath=EditorUtility.OpenFilePanel("打开实验", path, "cl");
        //if (string.IsNullOrEmpty(filepath)) return;

        //var info= Tool.ReadXml<ExperimentalInfo>(filepath);
        //SceneEditorManager.ExperimentTitle.text = info.ExperimentTitle;
        //LoadExperiment(info);
    }

    public void ReturnMain()
    {
        DialogBox.DialogBoxManager.dialogBoxManager.ShowConfirm("警告", "确认离开实验室？", () => {
            SceneManager.LoadScene("Main");
        });
    }

}
