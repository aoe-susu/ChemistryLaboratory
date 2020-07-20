using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatorMesh : MonoBehaviour {

    public List<Vector2> HR;
    public int vectexcount_everyheight;
    public Material material;


    public MeshFilter filter;
    public new  MeshRenderer renderer;
    public bool isReady = false;
    //private List<Vector2> HRTemp;
    //private int count_temp;
    private void Awake()
    {
        SetMeshFilter(HR);
        SetMeshRenderer();
    }

    private void OnValidate()
    {
        SetMeshFilter(HR);
        SetMeshRenderer();
    }

    private void Update()
    {
        if (isReady) return;
        try
        {
            SetMeshFilter(HR);
            SetMeshRenderer();
            isReady = true;
        }
        catch
        {

        }
    }

   

    public void SetMeshFilter(List<Vector2> hr)
    {
        if (filter == null) filter = this.GetComponent<MeshFilter>();
        if (filter==null)filter = this.gameObject.AddComponent<MeshFilter>();      
        Mesh mesh=filter.sharedMesh;
        if (mesh==null) mesh = new Mesh();
        List<Vector3> vectors = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        //根据HR高半径，来设置旋转体的定点连线
        for (int i = 0; i < hr.Count; i++)
        {
            for (int count = 0; count < vectexcount_everyheight; count++)
            {
                var h = hr[i].x;
                var r = hr[i].y;
                var newvector = new Vector3
                {
                    x = r * Mathf.Cos(Tool.getradByangle(count * 360 / vectexcount_everyheight)),
                    y = h,
                    z = r * Mathf.Sin(Tool.getradByangle(count * 360 / vectexcount_everyheight))
                };
                vectors.Add(newvector);
                if (i < (hr.Count - 1))
                {
                    triangles.Add(i * vectexcount_everyheight + count);
                    triangles.Add((i + 1) * vectexcount_everyheight + count);
                    triangles.Add(i * vectexcount_everyheight + (count + 1) % vectexcount_everyheight);

                    triangles.Add(i * vectexcount_everyheight + (count + 1) % vectexcount_everyheight);
                    triangles.Add((i + 1) * vectexcount_everyheight + count);
                    triangles.Add((i + 1) * vectexcount_everyheight + (count + 1) % vectexcount_everyheight);
                }
                uvs.Add(new Vector2(count / (float)vectexcount_everyheight, i / (float)hr.Count));
            }
        }


        //封住上下的顶
        for (int count = 0; count < vectexcount_everyheight; count++)
        {
            if (hr[0].y != 0)
            {
                triangles.Add(count);
                triangles.Add((count + 1) % vectexcount_everyheight);
                triangles.Add((vectexcount_everyheight - (count + 1)) % vectexcount_everyheight);
            }
            if (hr[hr.Count - 1].y != 0)
            {
                triangles.Add((hr.Count - 1) * vectexcount_everyheight + count);
                triangles.Add((hr.Count - 1) * vectexcount_everyheight + (vectexcount_everyheight - (count + 1)) % vectexcount_everyheight);
                triangles.Add((hr.Count - 1) * vectexcount_everyheight + (count + 1) % vectexcount_everyheight);
            }

        }


        mesh.vertices = vectors.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        filter.mesh = mesh;         
    }

    void SetMeshRenderer()
    {
        if (renderer == null) renderer = this.GetComponent<MeshRenderer>();
        if (renderer==null)  renderer = this.gameObject.AddComponent<MeshRenderer>();       
        if (material == null)
        {
            material = new Material(Shader.Find("Diffuse"));
            material.SetColor("_Color", Color.white);
        }
        renderer.material = material;
    }


   

}
