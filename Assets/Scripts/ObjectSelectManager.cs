using DialogBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// 物体选择管理，可以用线框圈中物体，从而选择一个或多个物体
/// </summary>
public class ObjectSelectManager : MonoBehaviour,IPointerDownHandler,IPointerUpHandler {

    public ResourceLoad resourceLoad;
    /// <summary>
    /// 选择标志的预设体
    /// </summary>
    public GameObject SelectSignPrefab;
    /// <summary>
    /// 选择的线框
    /// </summary>
    public LineRenderer SelectLine;
    /// <summary>
    /// 鼠标按下时的映射的世界坐标
    /// </summary>
    private Vector3? SelectPos;
    /// <summary>
    /// 选择标志list
    /// </summary>
    private List<GameObject> SelectSignList = new List<GameObject>();
    /// <summary>
    /// 开始选择物体，参数一presspoint，参数二选中标志列表
    /// </summary>
    public UnityAction<Vector3, List<GameObject>> OnStartSelect = null;
    /// <summary>
    /// 选择物体中，参数一presspoint，参数二nowpoint，参数三选中标志列表
    /// </summary>
    public UnityAction<Vector3, Vector3,List<GameObject>> OnSelecting = null;
    /// <summary>
    /// 选择物体完成，参数一presspoint，参数二nowpoint，参数三选中标志列表
    /// </summary>
    public UnityAction<Vector3, Vector3, List<GameObject>> OnSelected = null;
    /// <summary>
    /// 是否选择物体，为false只计算点的信息不选择，两个委托函数仍运行
    /// </summary>
    public bool IsSelect=true;
    private void Update()
    {
       
        SelectObjects();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="resourceLoad">资源加载实例</param>
    public void Init(ResourceLoad resourceLoad)
    {
        this.resourceLoad = resourceLoad;
        foreach (var obj in SelectSignList)
        {
            Destroy(obj);
        }
        SelectSignList.Clear();
        SelectPos = null;
        SelectLine.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        SelectLine.gameObject.SetActive(true);
        var pos = Tool.GetRayPointFromY(ray.origin, ray.direction, 0);
        if (OnStartSelect != null) OnStartSelect(pos,SelectSignList);
        SelectPos = pos;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SelectLine.gameObject.SetActive(false);       
        if (OnSelected != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 nowpos = Tool.GetRayPointFromY(ray.origin, ray.direction, 0);
            OnSelected((Vector3)SelectPos,nowpos,SelectSignList);
        }
        SelectPos = null;
    }

    /// <summary>
    /// 判断一个物体是否被选择
    /// </summary>
    /// <param name="obj">要判断的物体</param>
    /// <returns></returns>
    public bool ObjectSelected(Transform obj)
    {
        return SelectSignList.Exists(s => s.transform.parent == obj);
    }

    /// <summary>
    /// 根据标志的父物体删除标志
    /// </summary>
    /// <param name="gameObject">标志的父物体</param>
    public void DeleteSignObject(Transform parent)
    {
        var sign=SelectSignList.Find(s=>s.transform.parent==parent);
        SelectSignList.Remove(sign);
        Destroy(sign);
    }
    /// <summary>
    ///选择物体，实时计算线框位置，然后选中物体
    /// </summary>
    private void SelectObjects()
    {
        if (SelectPos == null) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 nowpos = Tool.GetRayPointFromY(ray.origin, ray.direction, 0);
        Vector3 lastpos = (Vector3)SelectPos; ;
        if (OnSelecting != null)
        {
            OnSelecting(lastpos, nowpos , SelectSignList);
        }
        if (IsSelect)
        {
            
            Vector3[] vecs = new Vector3[5];
            vecs[0] = lastpos;
            vecs[1] = new Vector3(nowpos.x, 0, lastpos.z);
            vecs[2] = nowpos;
            vecs[3] = new Vector3(lastpos.x, 0, nowpos.z);
            vecs[4] = lastpos;
            SelectLine.SetPositions(vecs);
            SetSelectSign(lastpos, nowpos);
            if (!SelectLine.gameObject.activeSelf)
                SelectLine.gameObject.SetActive(true);
        }
        else
        {
            SelectLine.gameObject.SetActive(false);
        }
        
    }
    /// <summary>
    /// 根据两个点，计算出在规则矩形的物体，并生成标志物体
    /// </summary>
    /// <param name="lastpos">上一个点</param>
    /// <param name="nowpos">现在的点</param>
    private void SetSelectSign(Vector3 lastpos, Vector3 nowpos)
    {
        float x0, x1, z0, z1;
        if (nowpos.x < lastpos.x)
        {
            x0 = nowpos.x;
            x1 = lastpos.x;
        }
        else
        {
            x1 = nowpos.x;
            x0 = lastpos.x;
        }
        if (nowpos.z < lastpos.z)
        {
            z0 = nowpos.z;
            z1 = lastpos.z;
        }
        else
        {
            z1 = nowpos.z;
            z0 = lastpos.z;
        }
        foreach (var sign in SelectSignList)
        {
            if (sign != null)
            {
                sign.transform.parent.GetComponent<Rigidbody>().isKinematic = resourceLoad.IsEdit;
                Destroy(sign);
            }
        }
        SelectSignList.Clear();
        if (x1 - x0 + z1 - z0 < 0.01)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit))
            {
                var obj = raycastHit.collider.gameObject;
                if (resourceLoad.SceneObjects.Contains(obj))
                {
                    GameObject sign = Instantiate(SelectSignPrefab, obj.transform);
                    sign.transform.localPosition = SelectSignPrefab.transform.localPosition;
                    sign.transform.localEulerAngles = SelectSignPrefab.transform.localEulerAngles;
                    sign.transform.parent.GetComponent<Rigidbody>().isKinematic = !resourceLoad.IsEdit;          
                    SelectSignList.Add(sign);
                }

            }
        }
        else
        {
            foreach (var obj in resourceLoad.SceneObjects)
            {
                var pos = obj.transform.position;
                if (pos.x >= x0 && pos.x <= x1 && pos.z >= z0 && pos.z <= z1)
                {
                    GameObject sign = Instantiate(SelectSignPrefab, obj.transform);
                    sign.transform.localPosition = SelectSignPrefab.transform.localPosition;
                    sign.transform.localEulerAngles = SelectSignPrefab.transform.localEulerAngles;
                    sign.transform.parent.GetComponent<Rigidbody>().isKinematic = false;
                    SelectSignList.Add(sign);
                }
            }

        }

    }

}
