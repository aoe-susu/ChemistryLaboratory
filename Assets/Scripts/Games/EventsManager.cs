using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EventsManager : MonoBehaviour {
    private ResourceLoad resourceLoad;
    
    public Text TipText;
    public static EventDelegate
        GameInitialize,
        AnyObjectExecuteOperation,
        AnyObjectSelected,
        AnyObjectCompare,
        AnyVesselLiquidVolumeChanged,
        AnyVesselLiquidColorChanged,
        AnyVesselPourLiquidToOtherVessel;
    //AnyObjectModelSizeCompare,
    //AnyObjectPositionCompare,
    //AnyObjectEulerAngleCompare,
    //AnyLiquidColorCompare,
    //AnyLiquidVolumeCompare,
    //AnyLiquidTemperatureCompare;

    private List<CL_Object> clobjs = null;
    private List<CL_Object> Clobjs
    {
        get
        {
            if (clobjs == null)
            {
                clobjs = new List<CL_Object>();
                resourceLoad.SceneObjects.ForEach(s => {
                    var clobj = s.GetComponent<CLGameObject>().cl_object;
                    clobjs.Add(clobj);
                });
            }
            return clobjs;
        }
    }

    public void Init(ResourceLoad res)
    {
        resourceLoad = res;
        Type type = this.GetType();
        foreach (var eventinfos in resourceLoad.EventInfos.Values)
        {
            foreach (var info in eventinfos)
            {
                AnalysisEventInfo(info, type);
            }
        }
        if (GameInitialize != null) GameInitialize();
    }
    
    private void AnalysisEventInfo(EventInfo info,Type type)
    {
        if (info == null) return;
        var field = type.GetField(info.EventType.ToString());
        EventDelegate temp= (pms) =>
        {
            var res = (bool)AnalysisCondition(info.Condition, pms);
            if (res)
            {
                AnalysisAction(info.Action, pms);
            }
        };
        field.SetValue(this, temp);       
    }

    private object AnalysisValue(CLValue value, params object[] objs)
    {
        if (value.value!= null){
            return value.value;
        }
        object obj;
        if (value.parameterindex >= 0){
            obj = objs[value.parameterindex];
        }
        else
        {
            obj = Clobjs.Find(s => s.Id == value.id);
        }
        Type type = obj.GetType();
        foreach(var t in value.parameterreflect)
        {
            var field = type.GetField(t);
            if (field != null)
            {
                obj = field.GetValue(obj);
                type = obj.GetType();
                continue;
            }
            var proper = type.GetProperty(t);
            if (proper != null)
            {
                obj = proper.GetValue(obj, new object[0]);
                type = obj.GetType();
                continue;
            }
            Debug.Log("发生空指针异常");
            return false;
        }
        return obj;
    }

    private object AnalysisCondition(CLCondition condition, params object[] objs)
    {
        if (condition == null) return true;
        if (condition.conditiontype == CLConditionType.val)
            return AnalysisValue(condition.value, objs);
        var type = condition.conditiontype;
        object cond1 = AnalysisCondition(condition.condition1, objs);
        object cond2 = AnalysisCondition(condition.condition2, objs);
        object res = null;
        if (type== CLConditionType.add)
        {
            float c1 = float.Parse(cond1.ToString());
            float c2 = float.Parse(cond2.ToString());
            res = c1 + c2;
        }
        else if (type == CLConditionType.sub)
        {
            float c1 = float.Parse(cond1.ToString());
            float c2 = float.Parse(cond2.ToString());
            res = c1 - c2;
        }
        else if (type == CLConditionType.mul)
        {
            float c1 = float.Parse(cond1.ToString());
            float c2 = float.Parse(cond2.ToString());
            res = c1 * c2;
        }
        else if (type == CLConditionType.div)
        {
            float c1 = float.Parse(cond1.ToString());
            float c2 = float.Parse(cond2.ToString());
            res = c1 / c2;
        }
        else if (type == CLConditionType.greater)
        {
            float c1 = float.Parse(cond1.ToString());
            float c2 = float.Parse(cond2.ToString());
            res = c1 > c2;
        }
        else if (type == CLConditionType.less)
        {
            float c1 = float.Parse(cond1.ToString());
            float c2 = float.Parse(cond2.ToString());
            res = c1 < c2;
        }
        else if (type == CLConditionType.equal)
        {
            res = cond1.ToString() == cond2.ToString();
        }
        else if (type == CLConditionType.negate)
        {
            res = !(bool)cond1;
        }  
        else if (type == CLConditionType.and)
        {
            res = (bool)cond1 && (bool)cond2;
        }
        else if (type == CLConditionType.or)
        {
            res = (bool)cond1 || (bool)cond2;
        }
        else
        {
            res = true;
        }
        return res;
    }
    private void AnalysisAction( CLAction action, params object[] objs)
    {
        var method = this.GetType().GetMethod(action.type.ToString(), BindingFlags.NonPublic | BindingFlags.Instance);
        List<object> pms = new List<object>();
        ParameterInfo[] pmsinfo = method.GetParameters();
        for (int i=0;i< action.values.Length; i++)
        {
            if (action.values[i] == null) break;
            pms.Add(AnalysisValue(action.values[i], objs));
            if (pmsinfo[i].ParameterType == pms[i].GetType())
            {

            }
            else if (pmsinfo[i].ParameterType == typeof(string))
            {
                pms[i] = pms[i].ToString();
            }
            else
            {

            }
        }
        method.Invoke(this, pms.ToArray());
    }


    private void ShowTip(object text)
    {
        TipText.text = text.ToString();
    }
    private void ChangeLiquidColor(object id,object color)
    {
        string colorstr = color.ToString();
        Color32 color32 = new Color32();
        string[] splits = colorstr.Split(',');
        color32.r =byte.Parse(splits[0]);
        color32.g = byte.Parse(splits[1]);
        color32.b = byte.Parse(splits[2]);
        color32.a = byte.Parse(splits[3]);
        foreach(var t in Clobjs)
        {
            if(t.Id== id.ToString())
            {
                t.LiquidColor = color32;
                break;
            }
        }
    }
    private void ChangeLiquidVolume(object id, object volume) { }
    private void ChangeBubble(object id, object level) { }
    private void ChangeSediment(object id, object level) { }
    private void ChangeFog(object id, object level) { }
    private void ChangeSmog(object id, object level) { }
}
