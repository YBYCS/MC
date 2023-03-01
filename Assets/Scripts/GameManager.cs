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
    enum BlockType {
        dirt = 1,
        water,
        grass,
        cobblestone,
        grass_sn,
        ice
    }
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
        int max = 5;
        for (int i = 0; i < max; i++) {
            for (int j = 0; j < max; j++) {
                mapTest.Add((i, j), CreateMapData(i, j));

            }
            
        }
        CreateAtlas();
        for (int i = 0; i < max; i++) {
            for (int j = 0; j < max; j++) {
                //Show(mapTest[(i, 0)]);
                Show(mapTest[(i, j)]);
            }
        }
    }


    MapData CreateMapData(int cx,int cz) {
        var d = new MapData();
        d.x = cx;
        d.z = cz;
        float max = 113;
        for (int i = 0; i < 16; i++) {
            for (int j = 0; j < 16; j++) {
                int x = (cx<<4)|i, z = (cz<<4)|j;
                var h = GetHeight(x, z);
                d.position[i, 256, j] = (byte)h;
                //注意K才是高度
                for (int k = 0; (k <= h||k<=64)&&k<256; k++) {
                    if (k < h) {
                        //改为泥土
                        d.position[i, k, j] = GetBlockTest(k, (int)h);
                    }
                    
                    else if(k == h) {
                        //并且如果在水下应该是泥土 TODO 沙子
                        if (h < 64) {
                            d.position[i, k, j] = 1;
                        }
                        else {
                            var t = GetTemperature(x, k, z);
                            var hd = GetHumidity(x, k, z);
                            //下雪
                            if (t <= 0 && hd > 0.3f) {
                                d.position[i,k,j] = (byte)BlockType.grass_sn;
                            }
                            else if(h<64&& t < 0 && hd > 0.3f) {
                                d.position[i, k, j] = (byte)BlockType.ice;
                            }
                            else {
                                //草坪
                                d.position[i, k, j] = 3;
                            }
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
    /// <summary>
    /// 判断返回草地还是岩石
    /// </summary>
    /// <param name="h"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    byte GetBlockTest(int h, int max) {
        if (h <= 45) {
            return 4;
        }
        else if (h >= max - 5) {
            return 1;
        }
        else {
            // 计算h在区间[45, max-5]中的比例
            float proportion = (float)(h - 45) / (max - 50);

            // 随机生成0到1之间的浮点数
            float randomValue = UnityEngine.Random.value;

            // 根据比例和随机数决定返回1或4
            if (randomValue <= proportion) {
                return 1;
            }
            else {
                return 4;
            }
        }
    }

    int seed = 114514;
    /// <summary>
    /// 将一个数，映射为0-1的数
    /// </summary>
    /// <param name="x">映射值</param>
    /// <param name="frequency">频率</param>
    /// <param name="excursion">偏移值</param>
    /// <param name="max">默认除以的</param>
    /// <returns></returns>
    float map2decimals(float x,int frequency=7, float excursion = 0,int max = 6737) {
        x += seed + excursion;
        x *= frequency;
        x%=max;
        x /= max;
        //x += customSend;
        x = Fract(x);
        return x;
    }
    float Fract(float x) {
        return x - Mathf.Floor(x);
    }

    /// <summary>
    /// 返回对应高度
    /// </summary>
    /// <param name="x">x位置</param>
    /// <param name="y">y位置</param>
    int GetHeight(float x, float y) {
        x = map2decimals(x);
        y = map2decimals(y);
        //基本频率
        float bf = 12;
        float rate = 2;
        var h = 128 * Mathf.PerlinNoise(bf * x%1, bf * y % 1) + 64 * Mathf.PerlinNoise(rate * bf*x % 1, rate * bf *y % 1) + 32 * Mathf.PerlinNoise(rate * rate * bf *x % 1, rate * rate * bf *y % 1);
        return (int)h;

    }
    float GetTemperature(float x,float y,float z) {
        //基本频率
        int bf = 17;
        x = map2decimals(x,bf);
        z = map2decimals(z,bf);
        
        var b = Mathf.PerlinNoise( x, z);
        //线性插值返回基础温度
        b = Mathf.Lerp(-35, 55, b);
        Debug.Log(b);
        b -= y <= 64 ? 0 : (y - 64) * 0.6f;
        return b;
    }
    /// <summary>
    /// 获取湿度
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="h"></param>
    /// <returns></returns>
    float GetHumidity(float x, float y, int h) {
        x = map2decimals(x);
        y = map2decimals(y);
        var b = Mathf.PerlinNoise( x, y );
        b += b < 0.9 ? 0.1f : 0;
        return b;
    }

    Texture2D atlas;
    Rect[] rects;
    void CreateAtlas() {
        //Texture2D[] textures;
        atlas = new Texture2D(2048, 2048);
        //textures = new Texture2D[2048];
        List<Texture2D> textures = new();
        textures.Add(Resources.Load<Texture2D>("Blocks/dirt"));
        textures.Add(Resources.Load<Texture2D>("Blocks/dirt"));
        textures.Add(Resources.Load<Texture2D>("Blocks/dirt"));

        textures.Add(Resources.Load<Texture2D>("Blocks/water_overlay"));
        textures.Add(Resources.Load<Texture2D>("Blocks/water_overlay"));
        textures.Add(Resources.Load<Texture2D>("Blocks/water_overlay"));

        textures.Add(Resources.Load<Texture2D>("Blocks/grass_top"));
        textures.Add(Resources.Load<Texture2D>("Blocks/grass_side"));
        textures.Add(Resources.Load<Texture2D>("Blocks/grass_side"));

        textures.Add(Resources.Load<Texture2D>("Blocks/cobblestone"));
        textures.Add(Resources.Load<Texture2D>("Blocks/cobblestone"));
        textures.Add(Resources.Load<Texture2D>("Blocks/cobblestone"));


        textures.Add(Resources.Load<Texture2D>("Blocks/snow"));
        textures.Add(Resources.Load<Texture2D>("Blocks/grass_side_snowed"));
        textures.Add(Resources.Load<Texture2D>("Blocks/dirt"));

        textures.Add(Resources.Load<Texture2D>("Blocks/ice"));
        textures.Add(Resources.Load<Texture2D>("Blocks/ice"));
        textures.Add(Resources.Load<Texture2D>("Blocks/ice"));
        rects = atlas.PackTextures(textures.ToArray(), 1, 2048);
        atlas.filterMode = FilterMode.Point;
    }


    void Show(MapData map) {
        var p = Instantiate(m_MapPrefab);
        p.name = map.x + "-" + map.z;
        p.transform.position = new Vector3(map.x*16, 0, map.z*16);
        var filter = p.GetComponent<MeshFilter>();
        Material material = p.GetComponent<MeshRenderer>().material;
        int count = 0;
        List<Vector3> vertices = new();
        List<int> triangles = new();
        List<Vector2> uvs = new();

        material.mainTexture = atlas;
        
        for (int i = 0; i < 16; i++) {
            for (int j = 0; j < 256; j++) {
                for (int k = 0; k < 16; k++) {
                    if (map.position[i, j, k] == 0) {
                        continue;
                    }
                    //生成面部分
                    //判断是否为透明方块
                    bool isT;
                    switch (map.position[i, j, k]) {
                        case (byte)BlockType.water:
                            isT = true;
                            break;
                        default:
                            isT = false;
                            break;
                    }
                    if (IsVisible(map.x*16+i, j + 1, map.z*16+k,isT)) {
                        AddFace2Mesh(map.position[i, j, k],new Vector3(i,j,k), vertices, triangles, uvs,Face.Top, count, rects);
                        count += 4;
                    }
                    if(IsVisible(map.x * 16 + i, j-1, map.z * 16 + k, isT)) {
                        AddFace2Mesh(map.position[i, j, k], new Vector3(i, j, k), vertices, triangles, uvs, Face.Bottom, count, rects);
                        count += 4;
                    }
                    if (IsVisible(map.x * 16 + i-1, j, map.z * 16 + k, isT)) {
                        AddFace2Mesh(map.position[i, j, k], new Vector3(i, j, k), vertices, triangles, uvs, Face.Left, count, rects);
                        count += 4;
                    }
                    if (IsVisible(map.x * 16 + i+1, j, map.z * 16 + k, isT)) {
                        AddFace2Mesh(map.position[i, j, k], new Vector3(i, j, k), vertices, triangles, uvs, Face.Right, count, rects);
                        count += 4;
                    }
                    if (IsVisible(map.x * 16 + i, j, map.z * 16 + k-1, isT)) {
                        AddFace2Mesh(map.position[i, j, k], new Vector3(i, j, k), vertices, triangles, uvs, Face.Front, count, rects);
                        count += 4;
                    }
                    if (IsVisible(map.x * 16 + i, j, map.z * 16 + k+1, isT)) {
                        AddFace2Mesh(map.position[i, j, k], new Vector3(i, j, k), vertices, triangles, uvs, Face.Back, count, rects);
                        count += 4;
                    }
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
    enum Face {
        Top,
        Bottom,
        Left,
        Right,
        Front,
        Back
    }
    void AddFace2Mesh(int id,Vector3 position, List<Vector3> vertices, List<int> triangles, List<Vector2> uvs,Face face,int count, Rect[] rects ) {
        Vector3[] v = GetFaceVertices(face);
        Vector3 v1 = v[0] + position;
        Vector3 v2 = v[1] + position;
        Vector3 v3 = v[2] + position;
        Vector3 v4 = v[3] + position;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        vertices.Add(v4);
        triangles.Add(0 + count);
        triangles.Add(1 + count);
        triangles.Add(2 + count);

        triangles.Add(0 + count);
        triangles.Add(2 + count);
        triangles.Add(3 + count);
        //取顶面
        //var rect = rects[map.position[i, j, k]%3];

        //TODO
        id--;
        id *= 3;
        switch (face) {
            case Face.Top:
                // 如果 face 为 Top，增量为 0
                id += 0;
                break;
            case Face.Bottom:
                // 如果 face 为 Bottom，增量为 2
                id += 2;
                break;
            default:
                // 其余情况下增量为 1
                id += 1;
                break;
        }
        var rect = rects[id];
        uvs.Add(new Vector2(rect.xMin, rect.yMin));
        uvs.Add(new Vector2(rect.xMax, rect.yMin));
        uvs.Add(new Vector2(rect.xMax, rect.yMax));
        uvs.Add(new Vector2(rect.xMin, rect.yMax));

    }
    //TODO 可以优化
    Vector3[] GetFaceVertices(Face face) {
        Vector3[] vertices = new Vector3[4];

        switch (face) {
            case Face.Top:
                vertices[0] = new Vector3(-0.5f, 0.5f, -0.5f);
                vertices[1] = new Vector3(-0.5f, 0.5f, 0.5f);
                vertices[2] = new Vector3(0.5f, 0.5f, 0.5f);
                vertices[3] = new Vector3(0.5f, 0.5f, -0.5f);
                break;
            case Face.Bottom:
                vertices[0] = new Vector3(-0.5f, -0.5f, -0.5f);
                vertices[1] = new Vector3(0.5f, -0.5f, -0.5f);
                vertices[2] = new Vector3(0.5f, -0.5f, 0.5f);
                vertices[3] = new Vector3(-0.5f, -0.5f, 0.5f);
                break;
            case Face.Back:
                vertices[0] = new Vector3(-0.5f, -0.5f, 0.5f);
                vertices[1] = new Vector3(0.5f, -0.5f, 0.5f);
                vertices[2] = new Vector3(0.5f, 0.5f, 0.5f);
                vertices[3] = new Vector3(-0.5f, 0.5f, 0.5f);
                break;
            case Face.Front:
                vertices[0] = new Vector3(0.5f, -0.5f, -0.5f);
                vertices[1] = new Vector3(-0.5f, -0.5f, -0.5f);
                vertices[2] = new Vector3(-0.5f, 0.5f, -0.5f);
                vertices[3] = new Vector3(0.5f, 0.5f, -0.5f);
                break;
            case Face.Left:
                vertices[0] = new Vector3(-0.5f, -0.5f, -0.5f);
                vertices[1] = new Vector3(-0.5f, -0.5f, 0.5f);
                vertices[2] = new Vector3(-0.5f, 0.5f, 0.5f);
                vertices[3] = new Vector3(-0.5f, 0.5f, -0.5f);
                break;
            case Face.Right:
                vertices[0] = new Vector3(0.5f, -0.5f, 0.5f);
                vertices[1] = new Vector3(0.5f, -0.5f, -0.5f);
                vertices[2] = new Vector3(0.5f, 0.5f, -0.5f);
                vertices[3] = new Vector3(0.5f, 0.5f, 0.5f);
                break;
        }

        return vertices;
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
    //TODO 水的双面材质
    /// <summary>
    /// 是否要显示该面，查询挡住该面的方块是否存在或者透明
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="isTransparencyBlock">要检测的方块是否是透明方块</param>
    /// <returns>true为显示false为不显示</returns>
    public bool IsVisible(int x,int y,int z,bool isTransparencyBlock = false) {

        var i = x / 16;
        var j = z / 16;
        if (x < 0) {
            return false;
        }
        if (z < 0) {
            return false;
        }
        if (y < 0) {
            return true;
        }
        if (y >= 256) {
            return true;
        }
        x %= 16;
        z %= 16;
        var d = GetMapData(i, j);
        if (d == null) return false;
        //Debug.Log(x + " " + y + " " + z);
        //Debug.Log(new Vector3(x, y, z));
        if (d.position[x,y,z] != 0 ) {
            //如果是透明方块 不接触空气一面返回假,否则判断是否为透明方块决定是否显示
            return isTransparencyBlock?false: d.position[x, y, z] == 2;
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
        //if (x < 4&&y<4 ) {
        //    return mapTest[(x, y)];
        //}
        //return mapTest[(0, 0)];
        if(mapTest.TryGetValue((x, y), out MapData value)) {
            return value;
        }
        else {
            return null;
        }
    }
}
