using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntityInfo
{

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
public class CL_Object
{
    private string name;
    private int modelId;
    private float modelSize = 1;
    private float vesselVolume;//容器体积        
    private float liquidvolume;//溶液体积
    private float liquidTemperature = 20;//温度  

    public List<Operation> Operations = new List<Operation>();
    public CL_ObjType Type;
    public Color32 LiquidColor = new Color32(255, 255, 255, 0);//溶液颜色


   
    public string Name { get { return name; } set { name = value; } }
    public int ModelId { get { return modelId; } set { modelId = value; } }
    public float ModelSize { get { return modelSize; } set { modelSize = value; } }
    public float VesselVolume { get { return vesselVolume; } set { vesselVolume = value; } }
    public float Liquidvolume { get { return liquidvolume; } set { liquidvolume = value; } }
    public float LiquidTemperature { get { return liquidTemperature; } set { liquidTemperature = value; } }
    public Vector3 Position { get; set; }
    public Vector3 EulerAngle { get; set; }
}



public class Operation
{
    public string Name;
}

//物体类型
public enum CL_ObjType
{
    Common,
    Vessel,
    Special,
    Custom
}


public enum ObjectControlState
{
    CanPlace,
    NotPlace
}