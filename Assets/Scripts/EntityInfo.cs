using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntityInfo{

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

    public Tree<T> GetChild (int index)
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


public class CL_Object
{
    public string Name;
    public int ModelId;
    public float ModelSize=1;
    public List<Operation> Operations=new List<Operation>(); 
    public CL_ObjType Type;
    public float VesselVolume;//容器体积        
    public float Liquidvolume;//溶液体积
    public Color32 LiquidColor = new Color32(255, 255, 255, 0);//溶液颜色
    public float LiquidTemperature = 20;//温度

    public Vector3 Position { get; set; }
    public Vector3 EulerAngle { get; set; }
}



public class Operation
{
    public string Name;
}


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