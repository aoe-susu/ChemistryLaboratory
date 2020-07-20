using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class LiquidBalance : MonoBehaviour
{
    //public float LiquidVolume {
    //    get {
    //        float res = 0;
    //        var hr = this.liquidMesh.HR;
    //        for (int i = 1; i < hr.Count; i++)
    //        {
    //            float toparea = 0, bomarea = 0;
    //            float h = hr[i].x - hr[i - 1].x;
    //            if (hr[i].x > liquidheight)
    //            {
    //                float h1 = liquidheight - hr[i - 1].x;
    //                float h2 = hr[i].x - liquidheight;
    //                float r = h1 / h * hr[i].y + h2 / h * hr[i - 1].y;
    //                toparea = Mathf.PI * r * r;
    //                h = h1;
    //            }
    //            else
    //            {
    //                h = hr[i].x - hr[i - 1].x;
    //                toparea = Mathf.PI * hr[i].y * hr[i].y;
    //            }
    //            bomarea = Mathf.PI * hr[i - 1].y * hr[i - 1].y;
    //            res += 0.5f * (toparea + bomarea) * h;
    //            if (hr[i].x > liquidheight) break;
    //        }
    //        return res;
    //    }
    //}

    public GameObject PourWaterEffective;
    private ParticleSystem ParticleSystem;
    private CLGameObject cLGameObject = new CLGameObject();
    private Vector3 SpillPoint = new Vector3();
    private float liquidheight = 0.1f;
    private float minheight = float.MaxValue;
    private int SpillPointCount=0;
    private RotatorMesh liquidMesh;
    private List<Vector3> vectors;
    private Vector3 lowpoint;

    // Use this for initialization
    void Awake()
    {
       
        liquidMesh = GetComponent<RotatorMesh>();
        cLGameObject = GetComponentInParent<CLGameObject>();
        liquidMesh.HR.ForEach(s => {
            if (minheight > s.x)
            {
                minheight = s.x;
            }
        });
        LiquidClear();              
        if (liquidMesh == null)
        {
            print("不存在LiquidMesh");
            this.enabled = false;
            return;
        }
        liquidMesh.renderer.material = new Material(liquidMesh.renderer.material);
        ParticleSystem = Instantiate(PourWaterEffective).GetComponent<ParticleSystem>();
        ParticleSystem.GetComponent<Renderer>().material= liquidMesh.renderer.material;
        ParticleSystem.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (cLGameObject.cl_object.Type != CL_ObjType.Vessel) return;
        //如果没液体，不计算
        if (liquidheight == minheight)
        {
            liquidMesh.renderer.enabled = false;
            ParticleSystem.gameObject.SetActive(false);
        }
        else
        {
            liquidMesh.renderer.enabled = true;
            LiquidMesh();
            PourLiquid();
            SetLiquidColor();
        }
        SetLiquidHeight();
    }

    /// <summary>
    /// 往容器倒入液体
    /// </summary>
    /// <param name="volume">倒入的液体体积</param>
    /// <param name="color">倒入的液体颜色</param>
    public void PourInto(float volume,Color32 color)
    {   
        float liqvol = cLGameObject.cl_object.Liquidvolume;
        float totalvol = liqvol + volume;
        cLGameObject.SetLiquidVolume(totalvol);

        float cbl_1 = volume / totalvol;
        float cbl_2 = liqvol / totalvol;
        Color32 color2 = cLGameObject.cl_object.LiquidColor;
        cLGameObject.SetLiquidColor(Tool.ColorCross(color, cbl_1, color2, cbl_2));
    }
    /// <summary>
    /// 倒出液体
    /// </summary>
    /// <returns>倒出的液体体积</returns>
    private float PourOut()
    {       
        var hr = this.liquidMesh.HR;
        //计算容器口的半径的平方
        float pourradiu_2 = hr.Last().y;
        pourradiu_2 *= pourradiu_2;
        //获取容器一圈的顶点数，相当于周长
        int circum = this.liquidMesh.vectexcount_everyheight;
        //获取出口水面积比例
        float bl = SpillPointCount / (float)circum;
        //获取容器口的扇形面积，用扇形面积减去三角形面积获得出水面积
        float totalarea = Mathf.PI * pourradiu_2 * bl;
        float angle = 2 * Mathf.PI * bl;
        float trianarea = 0.5f * pourradiu_2 * Mathf.Sin(angle);
        float pourarea = totalarea - trianarea;
        //出水面积乘以时间间隔再乘以某个常数获取出水体积
        float pourvolume = pourarea * Time.deltaTime;
        //如果当容器倾斜时，倒出相当于溢出]
        //如果当容器倒放时，溢出时溢出面积是容器口面积
        //如果当容器正放时，溢出时溢出面积不是容器口面积
        if (bl == 1&&transform.eulerAngles.x==0&& transform.eulerAngles.z==0)
        {
            pourvolume *= Time.deltaTime;
        }
        if (cLGameObject.cl_object.Liquidvolume > pourvolume)
        {
            cLGameObject.SetLiquidVolume(cLGameObject.cl_object.Liquidvolume-pourvolume);
        }
        else
        {
            pourvolume = cLGameObject.cl_object.Liquidvolume;
            cLGameObject.SetLiquidVolume(0);
        }       
        return pourvolume;
    }

    /// <summary>
    /// 模拟从一个容器往另一个容器倾倒液体
    /// </summary>
    private void PourLiquid()
    {
        if (SpillPointCount == 0)
        {
            ParticleSystem.gameObject.SetActive(false);
            return;
        }
        float volume = this.PourOut();
        if (volume == 0)
        {
            ParticleSystem.gameObject.SetActive(false);
            return;
        }
        Vector3 worldpoint = transform.TransformPoint(SpillPoint);
        ParticleSystem.gameObject.SetActive(true);
        ParticleSystem.transform.position = worldpoint;
        Ray ray = new Ray(worldpoint, Vector3.down);       
        RaycastHit[] raycasts = Physics.RaycastAll(ray);
        foreach (var c in raycasts)
        {
            if (c.transform == this.transform.parent) continue;
            var liq = c.transform.GetComponentInChildren<LiquidBalance>();
            if (liq == null) continue;
            if (liq == this) continue;          
            liq.PourInto(volume, liquidMesh.renderer.material.color);
            cLGameObject.PourOutLiquidToOtherVessel(liq.cLGameObject);
            break;
        }
    }


    private void SetLiquidHeight()
    {
        float volume = cLGameObject.cl_object.Liquidvolume;
        float height = minheight;
        if (volume == 0)
        {
            liquidheight = height;
            return;
        }
        var hr = this.liquidMesh.HR;
        for (int i = 0; i < hr.Count - 1 && volume != 0; i++)
        {
            //计算出第每层的体积，与实际体积volume比较
            //如果实际体积大，则height=height+该层高度，volume=volume-该层体积
            //否则根据该层的参数 以及volume 计算出真实高度 

            //此层高度
            float h = hr[i + 1].x - hr[i].x;
            //此层体积
            float vol = 0.5f * Mathf.PI * (hr[i].y * hr[i].y + hr[i + 1].y * hr[i + 1].y) * h;

            if (volume < vol)
            {
                Vector2 point1 = new Vector2(0, hr[i].y);
                Vector2 point2 = new Vector2(h, hr[i + 1].y);
                LinearFunction linear = new LinearFunction(point1, point2);
                float k = linear.GetK();
                float realh;
                if (k == 0)
                {
                    realh = volume / vol * h;
                }
                else if(k > 0)
                {
                    float th = h - linear.GetX(0);
                    float tv = 1f / 3f * Mathf.PI * hr[i+1].y * hr[i+1].y * th;
                    float v1 = tv - vol + volume;
                    float h1 = Mathf.Pow((3f * v1) / (k * k * Mathf.PI), 1f / 3f);
                    realh = h1 + linear.GetX(0);
                }
                else
                {
                    float th = linear.GetX(0);
                    float tv = 1f / 3f * Mathf.PI * hr[i].y * hr[i].y * th;
                    float v1 = tv - volume;
                    float h1 = Mathf.Pow((3f * v1) / (k * k * Mathf.PI), 1f / 3f);
                    realh = th - h1;
                }
               
               
                height += realh;
                volume = 0;
            }
            else
            {
                if(i == hr.Count - 2)
                {
                    height += volume / vol * h;
                    volume = 0;
                }
                else
                {
                    height += h;
                    volume -= vol;
                }
                
            }

        }
        liquidheight = height;
    }

    /// <summary>
    /// 计算出液体平衡后的mesh
    /// </summary>
    private void LiquidMesh()
    {
        if (vectors == null)
        {
            vectors = liquidMesh.filter.mesh.vertices.ToList();
            return;
        }
        List<List<Vector3>> vec = new List<List<Vector3>>();
        for (int i = 0; i < liquidMesh.HR.Count; i++)
        {
            vec.Add(vectors.GetRange(i * liquidMesh.vectexcount_everyheight, liquidMesh.vectexcount_everyheight));
        }

        float k = liquidMesh.transform.TransformPoint(lowpoint).y + liquidheight;
        SpillPoint = new Vector3(0, float.MaxValue, 0);

        SpillPointCount = 0;
        for (int i = 0; i < liquidMesh.vectexcount_everyheight / 2; i++)
        {
            var otheri = (i + liquidMesh.vectexcount_everyheight / 2) % liquidMesh.vectexcount_everyheight;


            //以竖截面 创建临时变量，方便遍历
            var tempvectors = new List<Vector3>();
            for (int j = vec.Count - 1; j >= 0; j--)
            {
                tempvectors.Add(vec[j][i]);
            }
            for (int j = 0; j < vec.Count; j++)
            {
                tempvectors.Add(vec[j][otheri]);
            }


            //遍历创建的临时集合，写算法
            var firstoverindex = -1;
            for (int j = 0; j < tempvectors.Count; j++)
            {
                var temp = liquidMesh.transform.TransformPoint(tempvectors[j]);
                if (temp.y < liquidMesh.transform.TransformPoint(lowpoint).y) lowpoint = tempvectors[j];
                if (temp.y > k)
                {
                    if (firstoverindex == -1)
                        firstoverindex = j;
                }
                else
                {
                    if (firstoverindex >= 0)
                    {
                        var top = liquidMesh.transform.TransformPoint(tempvectors[j - 1]);
                        var bom = liquidMesh.transform.TransformPoint(tempvectors[j]);
                        var t = (k - bom.y) / (top.y - bom.y);
                        var newpoint = liquidMesh.transform.InverseTransformPoint(t * (top - bom) + bom);
                        for (int l = firstoverindex; l < j; l++)
                        {
                            tempvectors[l] = newpoint;
                        }
                        firstoverindex = -1;
                    }

                }
                if (j == tempvectors.Count - 1 && firstoverindex >= 0)
                {
                    var top = liquidMesh.transform.TransformPoint(tempvectors[firstoverindex]);
                    var bom = liquidMesh.transform.TransformPoint(tempvectors[firstoverindex - 1 < 0 ? 0 : firstoverindex - 1]); ;
                    var t = (k - bom.y) / (top.y - bom.y);
                    var newpoint = liquidMesh.transform.InverseTransformPoint(t * (top - bom) + bom);
                    for (int l = firstoverindex; l < tempvectors.Count; l++)
                    {
                        tempvectors[l] = newpoint;
                    }
                }

                if (j == 0 || j == tempvectors.Count - 1)//寻找溢出点
                {
                    if (firstoverindex == -1)
                    {
                        if (SpillPoint.y > tempvectors[j].y)
                        {
                            SpillPoint = tempvectors[j];
                        }
                        SpillPointCount++;
                    }
                }

            }


            for (int j = 0; j < vec.Count; j++)
            {
                vec[vec.Count - 1 - j][i] = tempvectors[j];
            }
            for (int j = 0; j < vec.Count; j++)
            {
                vec[j][otheri] = tempvectors[j + vec.Count];
            }
        }
        var finalvec = new List<Vector3>();
        foreach (var t1 in vec)
        {
            finalvec.AddRange(t1);
        }
        liquidMesh.filter.mesh.vertices = finalvec.ToArray();
    }

    /// <summary>
    /// 设置液体颜色
    /// </summary>
    private void SetLiquidColor()
    {
        liquidMesh.renderer.material.color = cLGameObject.cl_object.LiquidColor;
    }

    private void LiquidClear()
    {
        liquidheight = minheight;
        LiquidMesh();
    }


}
