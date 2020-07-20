using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using UnityEngine;

/// <summary>
/// 二维线性函数
/// </summary>
public class LinearFunction
{
    float a, b, c;
    float? k;

    /// <summary>
    /// 点斜式
    /// </summary>
    /// <param name="point">点坐标</param>
    /// <param name="k">斜率</param>
    public LinearFunction(Vector2 point, float k)
    {
        this.a = k;
        this.b = -1;
        this.c = -k * point.x + point.y;
        this.k = k;
    }

    /// <summary>
    /// 两点式
    /// </summary>
    /// <param name="point1">点1坐标</param>
    /// <param name="point2">点2坐标</param>
    public LinearFunction(Vector2 point1, Vector2 point2)
    {
        this.a = point2.y - point1.y;
        this.b = point1.x - point2.x;
        this.c = point2.x * point1.y - point1.x * point2.y;
        if (point1.x != point2.x) this.k = (point1.y - point2.y) / (point1.x - point2.x);
    }

    /// <summary>
    /// 一般式
    /// </summary>
    /// <param name="a">参数a</param>
    /// <param name="b">参数b</param>
    /// <param name="c">参数c</param>
    public LinearFunction(float a,float b,float c)
    {
        this.a = a;
        this.b = b;
        this.c = c;
        if (b != 0) this.k = -a / b;
    }

    public float GetX(float y)
    {
        return (-b * y - c) / a;
    }
    public float GetY(float x)
    {
        return (-a * x - c) / b;
    }
    public float GetK()
    {
        return (float)k;
    }
}
