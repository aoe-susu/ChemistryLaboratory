using System;
using System.Collections.Generic;
using UnityEngine;
public class CLGameObject : MonoBehaviour
{
    private ResourceLoad resourceLoad;
    public CL_Object cl_object = new CL_Object();
    //public string clobjname;
    //public CL_ObjType cL_ObjType;  
    //public float LiquidTemperature = 20;   
    //public LiquidBalance Liquid;
    //private float modelsize = 1;
    private Vector3? defaultsize;


    private void Update()
    {     
        if (resourceLoad == null) return;
        if (resourceLoad.IsEdit)
        {
            CL_Object clobj = resourceLoad.GetObjectInfoByName(cl_object.Name);
            if (clobj != null)
            {
                clobj.Id = cl_object.Id;
                Load(resourceLoad, clobj);
            }
          
        }
        else
        {
            if (EventsManager.AnyObjectCompare != null)
            {
                EventsManager.AnyObjectCompare(cl_object);
            }
        }    
    }

    public void Load(ResourceLoad res,CL_Object clobj)
    {
        resourceLoad = res;
        if (defaultsize == null)
        {
            defaultsize = transform.localScale;
        }
        if (resourceLoad == null) return;
        cl_object = new CL_Object(clobj);
        if (cl_object == null) return;      
        transform.localScale = (Vector3)defaultsize * cl_object.ModelSize;
        cl_object.Position = transform.position;
        cl_object.EulerAngle = transform.eulerAngles;
        if (cl_object.Id == null) cl_object.Id = Guid.NewGuid().ToString();
    }

    public void SetLiquidColor(Color32 color)
    {
        if (!color.Equals(cl_object.LiquidColor))
        {
            cl_object.LiquidColor = color;
            if (EventsManager.AnyVesselLiquidColorChanged != null)
            {
                EventsManager.AnyVesselLiquidColorChanged(cl_object, color);
            }
        }
    }
    public void SetLiquidVolume(float volume)
    {
        if (cl_object.Liquidvolume != volume)
        {
            cl_object.Liquidvolume = volume;
            if (EventsManager.AnyVesselLiquidVolumeChanged != null)
            {
                EventsManager.AnyVesselLiquidVolumeChanged(cl_object, volume);
            }
        }
    }

    public void PourOutLiquidToOtherVessel(CLGameObject vessel)
    {
        if (EventsManager.AnyVesselPourLiquidToOtherVessel != null)
        {
            EventsManager.AnyVesselPourLiquidToOtherVessel(cl_object, vessel.cl_object);
        }
    }

}