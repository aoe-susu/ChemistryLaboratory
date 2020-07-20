using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using UnityEngine;

public class Tool
{
    public static Vector3 GetRayPointFromY(Vector3 knownpoint, Vector3 dir, float y)
    {
        var res = new Vector3(0, y, 0);
        res.x = (y - knownpoint.y) / dir.y * dir.x + knownpoint.x;
        res.z = (y - knownpoint.y) / dir.y * dir.z + knownpoint.z;
        return res;
    }
    public static Dictionary<string, object> GetFieldInfo<T>(T entity)
    {
        Type type=entity.GetType();
        FieldInfo[] fieldInfos= type.GetFields();
        Dictionary<string, object> keyValues = new Dictionary<string, object>();
        foreach( var t in fieldInfos)
        {
            keyValues.Add(t.Name, t.GetValue(entity));
        }
        return keyValues;
    }

    public static void SetObjMaterial(GameObject gameObject, Material material)
    {
        var renders = gameObject.GetComponentsInChildren<MeshRenderer>();
        foreach (var t in renders)
        {
            t.sharedMaterial = material;
        }
    }

    public static T ReadXml<T>(string filepath)
    {
        FileStream fs = new FileStream(filepath, FileMode.Open);
        XmlSerializer xmlSer = new XmlSerializer(typeof(T));
        T info = (T)xmlSer.Deserialize(fs);
        fs.Close();
        return info;
    }

    public static void WriteXml<T>(T info,string filepath)
    {
        //指定流文件(创建XML的目录)
        FileInfo fileinfo = new FileInfo(filepath);

        StreamWriter sw;  //流写入器对象，，
        if (!fileinfo.Exists) //判断路径是否存在
        {
            //不存在创建
            sw = fileinfo.CreateText();
        }
        else
        {
            //存在就删除，再创建
            //DialogBox.DialogBoxManager.dialogBoxManager.ShowConfirm()
            fileinfo.Delete();
            sw = fileinfo.CreateText();
        }
        //实例化对象，并 指定序列化的类型
        XmlSerializer ser = new XmlSerializer(typeof(T));
        //序列化方法，，（流写入器，实验信息）
        ser.Serialize(sw, info);
        sw.Close();  //关闭流
    }

    public static float CalculateSumVolume(MeshFilter meshFilter)
    {
        Vector3 scale = meshFilter.transform.lossyScale;
        Vector3[] arrVertices = meshFilter.mesh.vertices;
        int[] arrTriangles = meshFilter.mesh.triangles;
        float sum = 0.0f;

        for (int i = 0; i < meshFilter.mesh.subMeshCount; i++)
        {
            int[] arrIndices = meshFilter.mesh.GetTriangles(i);
            for (int j = 0; j < arrIndices.Length; j += 3)
                sum += CalculateVolume(scale
                                        , arrVertices[arrIndices[j]]
                                        , arrVertices[arrIndices[j + 1]]
                                        , arrVertices[arrIndices[j + 2]]);
        }
        return Mathf.Abs(sum);
    }
    private static float CalculateVolume(Vector3 scale, Vector3 pt0, Vector3 pt1, Vector3 pt2)
    {
        pt0 = new Vector3(pt0.x * scale.x, pt0.y * scale.y, pt0.z * scale.z);
        pt1 = new Vector3(pt1.x * scale.x, pt1.y * scale.y, pt1.z * scale.z);
        pt2 = new Vector3(pt2.x * scale.x, pt2.y * scale.y, pt2.z * scale.z);

        float v321 = pt2.x * pt1.y * pt0.z;
        float v231 = pt1.x * pt2.y * pt0.z;
        float v312 = pt2.x * pt0.y * pt1.z;
        float v132 = pt0.x * pt2.y * pt1.z;
        float v213 = pt1.x * pt0.y * pt2.z;
        float v123 = pt0.x * pt1.y * pt2.z;

        return (1.0f / 6.0f) * (-v321 + v231 + v312 - v132 - v213 + v123);
    }

    public static float getradByangle(float angle)
    {
        return angle / 180 * 3.14159f;
    }

    public static bool EqualsList<T>(List<T> list1, List<T> list2)
    {
        if (list1.Count != list2.Count) return false;
        for (int i = 0; i < list1.Count; i++)
        {
            if (!list1[i].Equals(list2[i])) return false; ;
        }
        return true;
    }


    /// <summary>
    /// 颜色混合
    /// </summary>
    /// <param name="color1">颜色1</param>
    /// <param name="pro1">颜色1所占比重</param>
    /// <param name="color2">颜色2</param>
    /// <param name="pro2">颜色2所占比重</param>
    /// <returns>混合后的颜色</returns>
    public static Color ColorCross(Color color1,float bili1,Color color2, float bili2)
    {
        Color color = color1 * bili1 + color2 * bili2;
        return color;
    }

    public static void CopyValue<T>(T origin, T target)
    {
        PropertyInfo[] properties = typeof(T).GetProperties();
        FieldInfo[] fields = (target.GetType()).GetFields();
        for (int i = 0; i < properties.Length; i++)
        {
            properties[i].SetValue(origin, properties[i].GetValue(target,new object[0]), new object[0]);
        }
        for (int i = 0; i < fields.Length; i++)
        {
            fields[i].SetValue(origin, fields[i].GetValue(target));
        }        
    }

    static int index = 1;
    public static void SaveTexture2D(Texture2D texture)
    {
        index++;
        byte[] bytes = texture.EncodeToPNG();
        string path = Application.dataPath + @"/MyTexture/"+index+ ".png";
        FileStream file = File.Open(path, FileMode.Create);
        BinaryWriter writer = new BinaryWriter(file);
        writer.Write(bytes);
        file.Close();
    }

}

