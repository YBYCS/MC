using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.Mesh;

public class test : MonoBehaviour
{
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();
    [SerializeField]
    GameObject m_BlockPrefab;
    // Source textures.
    Texture2D[] textures;

    // Rectangles for individual atlas textures.
    Rect[] rects;
    GameObject block;

    void Start() {
        textures = new Texture2D[2];
        var tax = Resources.Load<Texture2D>("Blocks/dirt");
        textures[0] = tax;
        tax = Resources.Load<Texture2D>("Blocks/water_overlay");
        textures[1] = tax;
        Texture2D atlas = new Texture2D(2048, 2048);
        Material material = GetComponent<MeshRenderer>().material;
        material.mainTexture = atlas;
        rects = atlas.PackTextures(textures,1, 2048);
        atlas.filterMode = FilterMode.Point;
        //block = Instantiate(m_BlockPrefab);
        Rect rect = rects[0];
        block1(rect);


    }

    // 显示在y轴上方
    void block1(Rect rect) {
        MeshFilter filter = GetComponent<MeshFilter>();

        // 顶点，观察点要顺时针绕才能看的见
        Vector3 v1 = new Vector3(-0.5f, 0.5f, -0.5f);
        Vector3 v2 = new Vector3(-0.5f, 0.5f, 0.5f);
        Vector3 v3 = new Vector3(0.5f, 0.5f, 0.5f);
        Vector3 v4 = new Vector3(0.5f, 0.5f, -0.5f);
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        vertices.Add(v4);

        // 三角网格
        triangles.Add(0);
        triangles.Add(1);
        triangles.Add(2);

        triangles.Add(0);
        triangles.Add(2);
        triangles.Add(3);

        // 贴图UV，这个是贴图的比例
        Vector2[] uvs = new Vector2[4];

        uvs[0] = new Vector2(rect.xMin, rect.yMin);
        uvs[1] = new Vector2(rect.xMax, rect.yMin);
        uvs[2] = new Vector2(rect.xMax, rect.yMax);
        uvs[3] = new Vector2(rect.xMin, rect.yMax);

        // 渲染参数设定
        filter.mesh.Clear();
        filter.mesh.subMeshCount = 1;   //贴图数量只设定为一张
        filter.mesh.vertices = vertices.ToArray();  //设置顶点信息
        filter.mesh.SetTriangles(triangles.ToArray(), 0);   //设置三角网格
        filter.mesh.uv = uvs; //设置uv，贴图对应的顶点
        //filter.mesh.uv = rect; //设置uv，贴图对应的顶点

        // 开始重新渲染
        filter.mesh.RecalculateNormals();
    }

    // 显示在y轴下方
    void block2() {
        MeshFilter filter = GetComponent<MeshFilter>();

        // 顶点
        Vector3 v1 = new Vector3(-0.5f, 0.5f, -0.5f);
        Vector3 v2 = new Vector3(0.5f, 0.5f, -0.5f);
        Vector3 v3 = new Vector3(0.5f, 0.5f, 0.5f);
        Vector3 v4 = new Vector3(-0.5f, 0.5f, 0.5f);
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        vertices.Add(v4);

        // 三角网格
        triangles.Add(0);
        triangles.Add(1);
        triangles.Add(2);

        triangles.Add(0);
        triangles.Add(2);
        triangles.Add(3);

        // 贴图UV
        uvs.Add(new Vector2(0.333f, 0));
        uvs.Add(new Vector2(0.666f, 0));
        uvs.Add(new Vector2(0.666f, 0.333f));
        uvs.Add(new Vector2(0.333f, 0.333f));


        filter.mesh.Clear();
        filter.mesh.subMeshCount = 1;
        filter.mesh.vertices = vertices.ToArray();
        filter.mesh.SetTriangles(triangles.ToArray(), 0);
        filter.mesh.uv = uvs.ToArray();
        filter.mesh.RecalculateNormals();
    }

    // 这个是错误的
    void block3() {
        MeshFilter filter = GetComponent<MeshFilter>();

        // 顶点
        Vector3 v1 = new Vector3(-0.5f, 0.5f, -0.5f);
        Vector3 v2 = new Vector3(0.5f, 0.5f, -0.5f);
        Vector3 v3 = new Vector3(-0.5f, 0.5f, 0.5f);
        Vector3 v4 = new Vector3(0.5f, 0.5f, 0.5f);
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        vertices.Add(v4);

        // 三角网格
        triangles.Add(0);
        triangles.Add(1);
        triangles.Add(2);

        triangles.Add(1);
        triangles.Add(2);
        triangles.Add(3);

        // 贴图UV
        uvs.Add(new Vector2(0.333f, 0));
        uvs.Add(new Vector2(0.666f, 0));
        uvs.Add(new Vector2(0.333f, 0.333f));
        uvs.Add(new Vector2(0.666f, 0.333f));

        filter.mesh.Clear();
        filter.mesh.subMeshCount = 1;
        filter.mesh.vertices = vertices.ToArray();
        filter.mesh.SetTriangles(triangles.ToArray(), 0);
        filter.mesh.uv = uvs.ToArray();
        filter.mesh.RecalculateNormals();
    }
    protected void AddSprite(Transform transform, Sprite sprite) {
        AddSprite(transform, sprite, Color.white);
    }
    protected void AddSprite(Transform transform, Sprite sprite, Color color) {
        var s = transform.GetComponent<SpriteRenderer>();
        s.sprite = sprite;
        s.color = color;
    }
    protected Sprite GetSprite(string path) {
        Texture2D tex = Resources.Load<Texture2D>(path);
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 16);
    }
}
