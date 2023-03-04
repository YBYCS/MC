using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LYcommon;
using System;
using Random = System.Random;
using UnityEngine.UIElements;
using static CommonFunction;
using static UnityEditor.PlayerSettings;

public class GameManager : Singleton<GameManager> {
    [SerializeField]
    GameObject m_BlockPrefab;
    [SerializeField]
    GameObject m_MapPrefab;
    Dictionary<(int, int), MapData> mapTest = new();
    enum BlockType {
        dirt = 1,
        water,
        grass,
        cobblestone,
        grass_sn,
        ice,
        //橡树
        oak,
        oak_leaves
    }
    PerlinNoise perlin;
    // Start is called before the first frame update
    void Start() {
        Init();

    }


    Texture2D riverNoise;
    void Init() {
        perlin = new PerlinNoise();
        riverNoise = Resources.Load<Texture2D>("RiverNoise");
        int max = 5;
        for (int i = 0; i < max; i++) {
            for (int j = 0; j < max; j++) {
                mapTest.Add((i, j), CreateMapData(i, j));
            }

        }
        //CreateRiverByDFS(max);
        for (int i = 0; i < max; i++) {
            for (int j = 0; j < max; j++) {
                CreateTree(mapTest[(i, j)]);
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

    readonly int seaLevel = 72;
    MapData CreateMapData(int cx, int cz) {
        var d = new MapData {
            x = cx,
            z = cz
        };
        for (int i = 0; i < 16; i++) {
            for (int j = 0; j < 16; j++) {
                int x = (cx << 4) | i, z = (cz << 4) | j;
                var h = GenerateHeight(x, z);
                d.position[i, 256, j] = (byte)h;
                //注意K才是高度
                for (int k = 0; (k <= h || k <= seaLevel) && k < 256; k++) {
                    if (k < h) {

                        //如果不是空的就生成地块，空的生成洞穴
                        //perlin.OctavePerlin(Map2decimals(x, f), Map2decimals(z, f), Map2decimals(k, f), 3, 0.6) * 10 % 1 < 0.95
                        if (!IsGenerateCave(x,k,z)) {
                            d.position[i, k, j] = GenerateBlockTest(k, (int)h);
                        }
                    }

                    else if (k == h) {
                        //并且如果在水下应该是泥土 TODO 沙子
                        if (h < seaLevel) {
                            d.position[i, k, j] = 1;
                        }
                        else {
                            //生成洞穴
                            if (IsGenerateCave(x,k,z)) {
                                continue;
                            }
                            var t = GetTemperature(x, k, z);
                            var hd = GetHumidity(x, k, z);
                            if (t <= -20 && hd > 0.3f) {
                                d.position[i, k, j] = (byte)BlockType.grass_sn;
                            }
                            else if (h < seaLevel && t < -10 && hd > 0.3f) {
                                d.position[i, k, j] = (byte)BlockType.ice;
                            }
                            else {
                                //草坪
                                d.position[i, k, j] = 3;
                            }
                        }
                    }
                    else if (h < seaLevel) {
                        //海水
                        d.position[i, k, j] = 2;
                    }
                }
            }
        }

        return d;

    }

    bool IsGenerateCave(int x,int y,int z) {
        //var c1 = Worley2D(new Vector2(x, z), false)*0.6;
        //var c2 = Worley2D(new Vector2(x, y), false) *0.2;
        //var c3 = Worley2D(new Vector2(z, y), false) *0.2;
        //var c = c1 + c2 + c3;
        var f = 14;
        ////var c = perlin.OctavePerlin(GenerateRandomBySeed(x), GenerateRandomBySeed(y), GenerateRandomBySeed(z), 3, 0.6);
        //var c = perlin.OctavePerlin(Map2decimals(x, f), Map2decimals(y, f), Map2decimals(z, f), 5, 0.6);
        //return c > 0.95;
        return perlin.OctavePerlin(Map2decimals(x, f), Map2decimals(z, f), Map2decimals(y, f), 3, 0.6) * 10 % 1 >0.95;
    }
    float Worley2D(Vector2 uv, bool revert) {
        float Dist = 16.0f;
        Vector2 intPos = floor(uv);
        Vector2 fracPos = fract(uv);
        //search range
        const int Range = 2;
        for (int X = -Range; X <= Range; X++) {
            for (int Y = -Range; Y <= Range; Y++) {
                float D = Vector2.Distance(hash2d2(intPos +new Vector2(X, Y)) + new Vector2(X, Y), fracPos);
                // take the feature point with the minimal distance
                Dist = MathF.Min(Dist, D);
            }
        }
        //use the distance as output
        if (revert)
            return 1.0f - 2.0f * Dist;
        else
            return Dist;
    }
    Vector2 floor(Vector2 inputVector) {
        float flooredX = Mathf.Floor(inputVector.x);
        float flooredY = Mathf.Floor(inputVector.y);
        return new Vector2(flooredX, flooredY);
    }
    Vector2 fract(Vector2 inputVector) {
        float fractionalX = inputVector.x - Mathf.Floor(inputVector.x);
        float fractionalY = inputVector.y - Mathf.Floor(inputVector.y);
        return new Vector2(fractionalX, fractionalY);
    }
    int hash2d2(int n) {
        n = (n >> 13) ^ n;
        return (n * (n * n * 60493 + 19990303) + 1376312589) & 0x7fffffff;
    }

    Vector2 hash2d2(Vector2 v) {
        int x = (int)v.x;
        int y = (int)v.y;
        return new Vector2(
            (float)hash2d2(x * 193 + y * 241 + 1013) / 1073741824.0f,
            (float)hash2d2(x * 331 + y * 337 + 977) / 1073741824.0f
        );
    }
    /// <summary>
    /// 判断返回草地还是岩石
    /// </summary>
    /// <param name="h"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    byte GenerateBlockTest(int h, int max) {
        if (h <= 55) {
            return 4;
        }
        else if (h >= max - 5) {
            return 1;
        }
        else {
            // 计算h在区间[45, max-5]中的比例
            float proportion = (float)(h - 55) / (max - 50);

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
    /// <summary>
    /// 设置指定位置的方块为指定方块
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="type"></param>
    void SetBlock(int x, int y, int z, BlockType type) {
        var data = GetMapData(x / 16, z / 16);
        if (data == null || x < 0 || z < 0) {
            //TODO 生成对应chunk
            return;
        }
        data.position[x % 16, y, z % 16] = (byte)type;
    }
    void SetBlock(Vector3 position, BlockType type) {
        SetBlock((int)position.x, (int)position.y, (int)position.z, type);
    }

    void CreateTree(MapData map) {
        int cx = map.x;
        int cz = map.z;
        for (int i = 0; i < 16; i++) {
            for (int j = 0; j < 16; j++) {
                int x = (cx << 4) | i, z = (cz << 4) | j;
                int y = map.position[i, 256, j];
                if (y < seaLevel + 4) {
                    continue;
                }
                //var c = ShoullGenerateTree(x, y, z);
                Random rand = new();
                var c = rand.NextDouble();
                if (c > .995) {
                    Vector3 p = new(x, y, z);
                    //建立树基
                    p.y++;
                    SetBlock(p, BlockType.oak);
                    p.y++;
                    SetBlock(p, BlockType.oak);

                    int maxH = (int)(c * 8);
                    for (int k = 0; k < maxH; k++) {
                        var l = GetXOnBezierCurve(new Vector2(0, 0), new Vector2(maxH / 2f , 1),
                            new Vector2(maxH / 3, maxH * 0.75f), new Vector2(0, maxH), k);
                        p.y++;

                        //遍历生成叶子
                        int x0 = (int)(p.x - l);
                        int z0 = (int)(p.z - l);
                        int x1 = (int)(p.x + l);
                        int z1 = (int)(p.z + l);
                        // 遍历正方形内的所有整数点
                        for (int dz = z0; dz <= z1; dz++) {
                            for (int dx = x0; dx <= x1; dx++) {
                                var tp = new Vector3( dx, p.y,  dz);
                                if (GetBlockInfo(tp) == 0) {
                                    SetBlock(tp, BlockType.oak_leaves);
                                }
                            }
                        }
                        //中心点改为树干
                        SetBlock(p, BlockType.oak);
                    }
                    SetBlock(p, BlockType.oak_leaves);
                }
            }
        }
    }
    int GetBlockInfo(Vector3 p) {
        return GetBlockInfo((int)p.x, (int)p.y, (int)p.z);
    }
    int GetBlockInfo(int x, int y, int z) {
        var data = GetMapData(x / 16, z / 16);
        if (data == null || x < 0 || z < 0) {
            //TODO 生成对应chunk
            return 0;
        }
        return data.position[x % 16, y, z % 16];
    }

    float ShoullGenerateTree(int x, int y, int z) {

        float c1 = GetHumidity(x, y, z) * 0.20f;
        float c2 = GetTemperatureIn021(x, y, z) * 0.20f;

        //float c3 = SimplexNoise.Generate(x, z,4,5,2,0.6f)*.35f;
        float c4 = (float)(GenerateRandomBySeed(x + y + z) * .60f);
        return c1 + c2 + c4;
    }



    float GenerateRandomBySeed(int seed) {
        return Fract(Mathf.Sin(seed) * 1000);
    }
    void CreateRiverByDFS(int max) {
        for (int cx = 0; cx < max; cx++) {
            for (int cz = 0; cz < max; cz++) {
                var map = GetMapData(cx, cz);
                for (int i = 0; i < 16; i++) {
                    for (int j = 0; j < 16; j++) {
                        int x = (cx << 4) | i, z = (cz << 4) | j;

                        //如果可以创建河的源头
                        var h = map.position[i, 256, j];
                        if (IsCreateRiver(x, h, z) > 0) {
                            //随机河流强度
                            //DFSRiver(i,,, 5000);
                            //map.position[i, h, j] = (byte)BlockType.water;
                            DFSRiver(map, i, h, j, 10);
                        }
                    }
                }

            }
        }

    }
    static List<Vector2> offPosition = new List<Vector2>{
        new Vector2(0,1),
        new Vector2(0,-1),
        new Vector2(1,0),
        new Vector2(-1,0)
    };
    void DFSRiver(MapData map, int x, int y, int z, int intensity) {
        if (intensity == 0) return;
        //按强度给水流挖地
        for (int i = 0; i < intensity / ZoomSection(64, 128, 2, 5, y); i++) {
            map.position[x, y - i, z] = (byte)BlockType.water;
            Debug.Log(ZoomSection(64, 128, 2, 5, y));
        }
        Vector2 coord = new(x, z);
        for (int i = 0; i < 4; i++) {
            var p = coord + offPosition[i];
            if (p.x < 16 && p.y < 16 && p.x >= 0 && p.y >= 0) {
                var h = map.position[(int)p.x, 256, (int)p.y];
                if (y >= h) {
                    DFSRiver(map, (int)p.x, h, (int)p.y, intensity - 1);
                }
                else {
                    DFSRiver(map, (int)p.x, h, (int)p.y, intensity / 2);
                }
            }
        }


        //int[,] data = new int[2, 2] { { 0, map.position[x,256,y+1] },{ 1, map.position[x, 256, y - 1] } };
        //for (int i = 2; i < 4; i++) {
        //    var p = coord + offPosition[i];
        //    if (map.position[(int)p.x, 256, y] < data[0, 1]) {
        //        data[0, 0] = i; 
        //        data[0,1] = map.position[(int)p.x, 256, y];
        //    }
        //    else if(map.position[(int) p.x, 256, y] < data[1, 1]) {
        //        data[1, 0] = i;
        //        data[1, 1] = map.position[(int) p.x, 256, y];
        //    }
        //}
        //var po = coord + offPosition[data[0, 0]];
        //DFSRiver(map, (int)po.x, map.position[(int)po.x, 256, (int)po.y], (int)po.y, intensity - 1);
        //po = coord + offPosition[data[1, 0]];
        //DFSRiver(map, (int)po.x, map.position[(int)po.x, 256, (int)po.y], (int)po.y, intensity - 1);
    }

    int IsCreateRiver(float x, float y, float z) {
        int width = riverNoise.width;
        int height = riverNoise.height;
        x = Map2decimals(x, 24) * width;
        z = Map2decimals(z, 24) * height;
        Color pixelColor = riverNoise.GetPixel((int)x, (int)z);
        // 将灰度值转换为float类型的0到1之间的值
        float grayValue = pixelColor.grayscale;
        float b = 20;
        if (grayValue > 0.75f) {
            b *= 1 + (64 - y) * (1 - 0.5f) / (64 - 128);
            return (int)b;
        }
        else {
            return 0;
        }

        //int randomNumber = random.Next(101);
        //return randomNumber > 80 && GenerateHumidity(x, z, 0) > 0.75 && y > 128;

    }
    double ZoomSection(double originalMin, double originalMax, double newMin, double newMax, double value) {
        return newMax + (value - originalMin) * (newMax - newMin) / (originalMax - originalMin);
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
    float Map2decimals(float x, int frequency = 7, float excursion = 0, int max = 6737) {
        x += seed + excursion;
        x *= frequency;
        x %= max;
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
    /// <param name="z">y位置</param>
    int GenerateHeight(float x, float z) {
        x = Map2decimals(x);
        z = Map2decimals(z);
        //基本频率
        float bf = 6;
        float rate = 2;
        //var h = 128 * CellularNoise(bf * x % 1, bf * z % 1) + 64 * CellularNoise(rate * bf * x % 1, rate * bf * z % 1) + 32 * CellularNoise(rate * rate * bf * x % 1, rate * rate * bf * z % 1);
        var h = 128 * Mathf.PerlinNoise(bf * x % 1, bf * z % 1) + 64 * Mathf.PerlinNoise(rate * bf * x % 1, rate * bf * z % 1) + 32 * Mathf.PerlinNoise(rate * rate * bf * x % 1, rate * rate * bf * z % 1);
        return (int)h;

    }
    float CellularNoise(float x, float z) {
        int width = riverNoise.width;
        int height = riverNoise.height;
        x *= width;
        z *= height;
        Color pixelColor = riverNoise.GetPixel((int)x, (int)z);
        // 将灰度值转换为float类型的0到1之间的值
        return 1 - pixelColor.grayscale;
    }
    float GetTemperature(float x, float y, float z) {
        //基本频率
        int bf = 17;
        x = Map2decimals(x, bf);
        z = Map2decimals(z, bf);

        var b = Mathf.PerlinNoise(x, z);
        //线性插值返回基础温度
        b = Mathf.Lerp(-35, 55, b);
        b -= y <= 64 ? 0 : (y - 64) * 0.6f;
        return b;
    }

    float GetTemperatureIn021(float x, float y, float z) {
        //基本频率
        int bf = 17;
        x = Map2decimals(x, bf);
        z = Map2decimals(z, bf);

        var b = Mathf.PerlinNoise(x, z);
        return b;
    }

    /// <summary>
    /// 获取湿度
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="h"></param>
    /// <returns></returns>
    float GetHumidity(float x, float y, float z) {
        x = Map2decimals(x);
        z = Map2decimals(z);
        var b = Mathf.PerlinNoise(x, z);
        b += b < 0.9 ? 0.1f : 0;
        return b;
    }


    Texture2D atlas;
    Rect[] rects;
    List<Texture2D> textures;
    void CreateAtlas() {
        //Texture2D[] textures;
        atlas = new Texture2D(2048, 2048);
        //textures = new Texture2D[2048];
        textures = new();
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

        AddTex("Blocks/log_oak_toplog_oak_top");
        AddTex("Blocks/log_oak");
        AddTex("Blocks/log_oak_toplog_oak_top");

        AddTex("Blocks/leaves_oak");
        AddTex("Blocks/leaves_oak");
        AddTex("Blocks/leaves_oak");


        rects = atlas.PackTextures(textures.ToArray(), 1, 2048);
        atlas.filterMode = FilterMode.Point;
    }
    void AddTex(string path) {
        textures.Add(Resources.Load<Texture2D>(path));
    }

    void Show(MapData map) {
        var p = Instantiate(m_MapPrefab);
        p.name = map.x + "-" + map.z;
        p.transform.position = new Vector3(map.x * 16, 0, map.z * 16);
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

                    var isT = map.position[i, j, k] switch {
                        (byte)BlockType.water => true,
                        _ => false,
                    };
                    if (IsVisible(map.x * 16 + i, j + 1, map.z * 16 + k, isT)) {
                        AddFace2Mesh(map.position[i, j, k], new Vector3(i, j, k), vertices, triangles, uvs, Face.Top, count, rects);
                        count += 4;
                    }
                    if (IsVisible(map.x * 16 + i, j - 1, map.z * 16 + k, isT)) {
                        AddFace2Mesh(map.position[i, j, k], new Vector3(i, j, k), vertices, triangles, uvs, Face.Bottom, count, rects);
                        count += 4;
                    }
                    if (IsVisible(map.x * 16 + i - 1, j, map.z * 16 + k, isT)) {
                        AddFace2Mesh(map.position[i, j, k], new Vector3(i, j, k), vertices, triangles, uvs, Face.Left, count, rects);
                        count += 4;
                    }
                    if (IsVisible(map.x * 16 + i + 1, j, map.z * 16 + k, isT)) {
                        AddFace2Mesh(map.position[i, j, k], new Vector3(i, j, k), vertices, triangles, uvs, Face.Right, count, rects);
                        count += 4;
                    }
                    if (IsVisible(map.x * 16 + i, j, map.z * 16 + k - 1, isT)) {
                        AddFace2Mesh(map.position[i, j, k], new Vector3(i, j, k), vertices, triangles, uvs, Face.Front, count, rects);
                        count += 4;
                    }
                    if (IsVisible(map.x * 16 + i, j, map.z * 16 + k + 1, isT)) {
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
    void AddFace2Mesh(int id, Vector3 position, List<Vector3> vertices, List<int> triangles, List<Vector2> uvs, Face face, int count, Rect[] rects) {
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
        id += face switch {
            Face.Top => 0,// 如果 face 为 Top，增量为 0
            Face.Bottom => 2,// 如果 face 为 Bottom，增量为 2
            _ => 1,// 其余情况下增量为 1
        };
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
    //TODO 水的双面材质
    /// <summary>
    /// 是否要显示该面，查询挡住该面的方块是否存在或者透明
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="isTransparencyBlock">要检测的方块是否是透明方块</param>
    /// <returns>true为显示false为不显示</returns>
    public bool IsVisible(int x, int y, int z, bool isTransparencyBlock = false) {

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
        if (d.position[x, y, z] != 0) {
            //如果是透明方块 不接触空气一面返回假,否则判断是否为透明方块决定是否显示
            return !isTransparencyBlock && d.position[x, y, z] == 2;
        }
        return true;
    }
    /// <summary>
    /// 获取对应的地图块数据
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public MapData GetMapData(int x, int y) {
        //TODO
        //if (x < 4&&y<4 ) {
        //    return mapTest[(x, y)];
        //}
        //return mapTest[(0, 0)];
        if (mapTest.TryGetValue((x, y), out MapData value)) {
            return value;
        }
        else {
            return null;
        }
    }
}
