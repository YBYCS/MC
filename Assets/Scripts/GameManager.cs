using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LYcommon;
using System;

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    GameObject m_BlockPrefab;
    [SerializeField]
    GameObject m_MapPrefab;
    MapData testData;
    Dictionary<(int, int), MapData> mapTest = new Dictionary<(int, int), MapData>();
    // Start is called before the first frame update
    void Start()
    {
        Init();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Init() {
        for (int i = 0; i < 4; i++) {
            
                mapTest.Add((i, 0), CreateMapData(i, 0));
            
        }
        for (int i = 0; i < 4; i++) {

            //Show(mapTest[(i, 0)]);
            ShowNew(mapTest[(i, 0)]);
        }
    }

    void CreateGrass() {
        var block = Instantiate(m_BlockPrefab);
        var s = block.AddComponent<GrassBlock>();
        s.Init(testData,0, 0, 0);
    }

    MapData CreateMapData(int x,int y) {
        var d = new MapData();
        d.x = x;
        d.z = y;
        float max = 23;
        for (int i = 0; i < 16; i++) {
            for (int j = 0; j < 16; j++) {
                var h = 128 * Mathf.PerlinNoise((i+x*16) / max, (j+y*16) / max) + 64 * Mathf.PerlinNoise((i + x * 16) / max, (j + y * 16 )/ max);
                //+ 32 * Mathf.PerlinNoise(i / 16, j / 16);
                //+ 64 * Mathf.PerlinNoise( i / 16, j / 16) + 32 * Mathf.PerlinNoise( i / 16,  j / 16) ;
                //Debug.Log(new Vector3(i,j,h));
                //Debug.Log(new Vector2(4 * i, 4 * j));

                //Debug.Log(h);
                for (int k = 0; (k < h||k<64)&&k<256; k++) {
                    if (k < h) {
                        //改为泥土
                        d.position[i, k, j] = 1;
                    }
                    else if(k == h) {
                        //并且如果在水下应该是泥土
                        if (h < 64) {
                            d.position[i, k, j] = 1;
                        }
                        else {
                            //草坪
                            d.position[i, k, j] = 3;
                        }
                    }
                    else if (h < 64) {
                        //海水
                        d.position[i, k, j] = 2;
                    }

                }
            }
        }
        return d;

    }

    void Show(MapData map) {
        var p = new GameObject(map.x+"-"+map.z);
        for (int i = 0; i < 16; i++) {
            for (int j = 0; j < 256; j++) {
                for (int k = 0; k < 16; k++) {
                    if (map.position[i, j, k] == 0) {
                        continue;
                    }
                    //switch (testData.position[i, j, k]) {
                    //    case 1:
                    //        //泥土
                    //        break;
                    //    case 2:
                    //        //TODO 海水
                    //        break;
                    //    case 3:
                    //        //草地
                    //        CreateBlock<GrassBlock>(testData, i, j, k);
                    //        break;
                    //}
                    CreateBlock(map.position[i, j, k], map, i, j, k,p);
                }
            }
        }
    }
    void ShowNew(MapData map) {
        var p = Instantiate(m_MapPrefab);
        p.name = map.x + "-" + map.z;
        p.transform.position = new Vector3(map.x*16, 0, map.z*16);
        var filter = p.GetComponent<MeshFilter>();
        var renderer = p.GetComponent<MeshRenderer>();
        Material material = p.GetComponent<MeshRenderer>().material;
        Mesh mesh = new Mesh();
        int count = 0;
        List<Vector3> vertices = new();
        List<int> triangles = new();
        List<Vector2> uvs = new();
        Texture2D[] textures;
        textures = new Texture2D[2];
        var tax = Resources.Load<Texture2D>("Blocks/dirt");
        textures[0] = tax;
        tax = Resources.Load<Texture2D>("Blocks/water_overlay");
        textures[1] = tax;
        Texture2D atlas = new Texture2D(2048, 2048);
        material.mainTexture = atlas;
        Rect[] rects = atlas.PackTextures(textures, 1, 2048);
        atlas.filterMode = FilterMode.Point;
        for (int i = 0; i < 16; i++) {
            for (int j = 0; j < 256; j++) {
                for (int k = 0; k < 16; k++) {
                    if (map.position[i, j, k] == 0) {
                        continue;
                    }
                    //生成面
                    if (IsVisible(map.x*16+i, j + 1, map.z*16+k)) {
                        var position = new Vector3(i, j, k);
                        Vector3 v1 = new Vector3(-0.5f, 0.5f, -0.5f)+ position;
                        Vector3 v2 = new Vector3(-0.5f, 0.5f, 0.5f) + position;
                        Vector3 v3 = new Vector3(0.5f, 0.5f, 0.5f) + position;
                        Vector3 v4 = new Vector3(0.5f, 0.5f, -0.5f) + position;
                        vertices.Add(v1);
                        vertices.Add(v2);
                        vertices.Add(v3);
                        vertices.Add(v4);
                        triangles.Add(0+count);
                        triangles.Add(1 + count);
                        triangles.Add(2 + count);

                        triangles.Add(0 + count);
                        triangles.Add(2 + count);
                        triangles.Add(3 + count);
                        //取顶面
                        //var rect = rects[map.position[i, j, k]%3];
                        var rect = rects[0];
                        uvs.Add(new Vector2(rect.xMin, rect.yMin));
                        uvs.Add(new Vector2(rect.xMax, rect.yMin));
                        uvs.Add(new Vector2(rect.xMax, rect.yMax));
                        uvs.Add(new Vector2(rect.xMin, rect.yMax));
                        count += 4;
                    }



                    //switch (testData.position[i, j, k]) {
                    //    case 1:
                    //        //泥土
                    //        break;
                    //    case 2:
                    //        //TODO 海水
                    //        break;
                    //    case 3:
                    //        //草地
                    //        CreateBlock<GrassBlock>(testData, i, j, k);
                    //        break;
                    //}

                    //CreateBlockNew(map.position[i, j, k], map, i, j, k, p);
                }
            }
        }
        // 渲染参数设定
        filter.mesh.vertices = vertices.ToArray();  //设置顶点信息
        filter.mesh.SetTriangles(triangles.ToArray(), 0);   //设置三角网格
        filter.mesh.uv = uvs.ToArray(); //设置uv，贴图对应的顶点
        //filter.mesh.uv = rect; //设置uv，贴图对应的顶点

        // 开始重新渲染
        filter.mesh.RecalculateNormals();


    }
    void CreateBlock<T>(MapData mapData,int x,int y,int z) where T:Block{
        var block = Instantiate(m_BlockPrefab);
        var s = block.AddComponent<T>();
        //s.Setposition(testData.x * 16 + i, testData.y * 16 + j, k);
        s.Init(testData, x, y, z);
    }
    //这里有bug
    void CreateBlock(int id, MapData mapData, int x, int y, int z,GameObject parent) {
        var block = Instantiate(m_BlockPrefab);
        block.transform.SetParent(parent.transform);
        string cn = "Block" + id;
        Type type = Type.GetType(cn);  // 获取类型
        var s = block.AddComponent(type);
        var blockComponent = (Block)s;
        blockComponent.Init(mapData, x, y, z);

    }
    void CreateBlockNew(int id, MapData mapData, int x, int y, int z, GameObject parent) {

    }

    /// <summary>
    /// 是否要显示该面，查询挡住该面的方块是否存在或者透明
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns>true为显示false为不显示</returns>
    public bool IsVisible(int x,int y,int z) {

        var i = x / 16;
        var j = z / 16;
        if (x < 0) {
            x += 16 * (-i+1);
        }
        if (z < 0) {
            z += 16 * (-j+1);
        }
        if (y < 0) {
            return true;
        }
        if (y > 256) {
            return true;
        }
        x %= 16;
        z %= 16;
        var d = GetMapData(i, j);
        //Debug.Log(x + " " + y + " " + z);
        if (d.position[x,y,z] != 0 ) {
            //string cn = "Block" + d.position[x, y, z];
            //Type type = Type.GetType(cn);  // 获取类型
            //Block cb = (Block)Activator.CreateInstance(type);
            //return cb.isTransparency;
            return d.position[x,y,z] == 2;
        }
        return true;
    }
    /// <summary>
    /// 获取对应的地图块数据
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public MapData GetMapData(int x,int y) {
        //TODO
        if (x < 4 ) {
            return mapTest[(x, 0)];
        }
        return mapTest[(0, 0)];
    }
}
