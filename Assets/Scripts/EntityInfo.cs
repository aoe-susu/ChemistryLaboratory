using DialogBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class EntityInfo
{
    
}

[Serializable]
public class ExperimentalInfo
{
    public string ExperimentTitle = "";
    public List<CL_Object> CL_Objects=new List<CL_Object>();
    public List<Operation> Operations=new List<Operation>();
    public List<CL_Object> SceneObjects=new List<CL_Object>();
    public List<string> EventClassifys=new List<string>();
    public List<List<EventInfo>> EventInfos=new List<List<EventInfo>>();
}


//public class TextListInfo {
//    public string ParentIndex;

//}


public class Tree<T>
{
    public Tree<T> parent;
    public T content;
    private List<Tree<T>> children = new List<Tree<T>>();

    public int ChildrenCount { get { return children.Count; } }

    public Tree<T> GetChild(int index)
    {
        return children[index];
    }

    public void SetParent(Tree<T> parent)
    {
        if (this.parent != null)
        {
            this.parent.children.Remove(this);
        }
        parent.children.Add(this);
        this.parent = parent;
    }
}

//物体基本信息类
//2020-4-3最新更新：给字段添加属性，以供反射
[Serializable]
public class CL_Object
{
    public string Id { get; set; }

    public string Name;
    public int Model;
    public float ModelSize=1;
    public List<Operation> Operations = new List<Operation>();
    public CL_ObjType Type;   
    public float Liquidvolume;
    public float LiquidTemperature=20;  
    public Color32 LiquidColor = new Color32(255, 255, 255, 0);//溶液颜色

   
    public Vector3 Position { get; set; }
    public Vector3 EulerAngle { get; set; }

    public CL_Object(CL_Object clobj)
    {
        this.Name = clobj.Name;
        this.Model = clobj.Model;
        this.ModelSize = clobj.ModelSize;
        this.Operations = new List<Operation>(clobj.Operations);
        this.Type = clobj.Type;
        this.Liquidvolume = clobj.Liquidvolume;
        this.LiquidTemperature = clobj.LiquidTemperature;
        this.LiquidColor = clobj.LiquidColor;
        this.Id = clobj.Id;
        this.Position = clobj.Position;
        this.EulerAngle = clobj.EulerAngle;
    }
    public CL_Object() { }


}

public class DialogOperationData: DialogBoxDataBase
{
    Operation operation;
    public DialogOperationData(Operation oper)
    {
        operation = oper;
    }
    public override string GetDialogBoxShowString()
    {
        return operation.Name;
    }
    public override void SetValue(string data)
    {
        operation = new Operation { Name = data };
    }
    public override object GetValue()
    {
        return operation;
    }
}


public class DialogStrData: DialogBoxDataBase
{
    public DialogStrData(string data)
    {
        strdata = data;
    }

    string strdata="";
    public override string GetDialogBoxShowString()
    {
        return strdata;
    }
    public override void SetValue(string data)
    {
        strdata = data;
    }
    public override object GetValue()
    {
        return strdata;
    }
}
public class DialogFloatData : DialogBoxDataBase
{
    public DialogFloatData(float data)
    {
        floatdata = data;
    }

    float floatdata = 0;
    public override string GetDialogBoxShowString()
    {
        return floatdata.ToString();
    }
    public override void SetValue(string data)
    {
        floatdata = float.Parse(data);
    }
    public override object GetValue()
    {
        return floatdata;
    }
}

[Serializable]
public class Operation
{
    public string Name;
}

//物体类型
public enum CL_ObjType
{
    Common,
    Vessel,
    Custom
}


public enum ObjectControlState
{
    CanPlace,
    NotPlace
}

[Serializable]
public class EventInfo
{
    public string Name;
    public string Remark;
    public CLEventType EventType;
    public CLCondition Condition;
    public CLAction Action=new CLAction();
}

[Serializable]
public enum CLEventType
{
    GameInitialize,
    AnyObjectExecuteOperation,
    AnyObjectSelected,
    AnyObjectCompare,
    AnyVesselLiquidVolumeChanged,
    AnyVesselLiquidColorChanged,
    AnyVesselPourLiquidToOtherVessel
    //AnyObjectModelSizeCompare,
    //AnyObjectPositionCompare,
    //AnyObjectEulerAngleCompare,

    //AnyLiquidColorCompare,
    //AnyLiquidVolumeCompare,
    //AnyLiquidTemperatureCompare

}

[Serializable]
public enum CLConditionType
{
    greater,
    less,
    equal,
    add,
    sub,
    mul,
    div,
    negate,
    and,
    or,
    val,
}

[Serializable]
public class CLValue
{
    public object value;
    public int parameterindex = -1;
    public string id;
    public string name;
    public List<string> parameterreflect=new List<string>();

    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();
        if (value == null)
        {
            builder.Append(name);
            for (int i = 0; i < parameterreflect.Count; i++)
            {
                builder.Append('的');
                builder.Append(parameterreflect[i]);
            }

        }
        else
        {
            builder.Append(value.ToString());
        }
        return builder.ToString();
    }
}
[Serializable]
public class CLCondition
{
    public CLValue value;
    public CLCondition condition1;
    public CLCondition condition2;
    public CLConditionType conditiontype;

    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();      
        if (conditiontype == CLConditionType.val)
        {
            builder.Append(value.ToString());
        }
        else
        {
            builder.Append("(");
            if (condition1 != null) builder.Append(condition1.ToString());
            builder.Append(" ");
            builder.Append(conditiontype.ToString());
            builder.Append(" ");
            if (condition2 != null) builder.Append(condition2.ToString());
            builder.Append(")");
        }      
        return builder.ToString();
    }
}


public class CLAction
{
    public CLValue[] values = new CLValue[2];
    public CLActionType type;
}

public enum CLActionType
{
    ShowTip,
    ChangeLiquidColor,
    ChangeLiquidVolume,
    ChangeBubble,
    ChangeSediment,
    ChangeFog,
    ChangeSmog
}


public delegate void EventDelegate(params object[] T);