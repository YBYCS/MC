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
            for (int j = 0; j < 4; j++) {
                mapTest.Add((i, j), CreateMapData(i, j));

            }
            
        }
        CreateAtlas();
        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < 4; j++) {
                //Show(mapTest[(i, 0)]);
                Show(mapTest[(i, j)]);
            }
        }
    }


    MapData CreateMapData(int x,int y) {
        var d = new MapData();
        d.x = x;
        d.z = y;
        float max = 113;
        for (int i = 0; i < 16; i++) {
            for (int j = 0; j < 16; j++) {
                var h = GetHeight(i + x * 16, j + y * 16);

                //ע��K���Ǹ߶�
                for (int k = 0; (k <= h||k<64)&&k<256; k++) {
                    if (k < h) {
                        //��Ϊ����
                        d.position[i, k, j] = GetBlockTest(k, (int)h);
                    }
                    
                    else if(k == h) {
                        //���������ˮ��Ӧ��������
                        if (h < 64) {
                            d.position[i, k, j] = 1;
                        }
                        else {
                            //��ƺ
                            d.position[i, k, j] = 3;
                        }
                    }
                    else if (h < 64) {
                        //��ˮ
                        d.position[i, k, j] = 2;
                    }
                }
            }
        }
        return d;

    }
    byte GetBlockTest(int h, int max) {
        if (h <= 45) {
            return 4;
        }
        else if (h >= max - 5) {
            return 1;
        }
        else {
            // ����h������[45, max-5]�еı���
            float proportion = (float)(h - 45) / (max - 50);

            // �������0��1֮��ĸ�����
            float randomValue = UnityEngine.Random.value;

            // ���ݱ������������������1��4
            if (randomValue <= proportion) {
                return 1;
            }
            else {
                return 4;
            }
        }
    }

    /// <summary>
    /// ���ض�Ӧ�߶�
    /// </summary>
    /// <param name="x">xλ��</param>
    /// <param name="y">yλ��</param>
    int GetHeight(float x, float y) {
        int max = 6000000;
        int send = 114514;
        x += send;
        y += send;
        x /= max;
        y /= max;
        //����Ƶ��
        float bf = 30000;
        float rate = 3;
        var h = 128 * Mathf.PerlinNoise(bf * x%1, bf * y % 1) + 64 * Mathf.PerlinNoise(rate * bf*x % 1, rate * bf *y % 1) + 32 * Mathf.PerlinNoise(rate * rate * bf *x % 1, rate * rate * bf *y % 1);
        //Debug.Log(h);
        return (int)h;

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
                    //������
                    if (IsVisible(map.x*16+i, j + 1, map.z*16+k)) {
                        AddFace2Mesh(map.position[i, j, k],new Vector3(i,j,k), vertices, triangles, uvs,Face.Top, count, rects);
                        count += 4;
                    }
                    if(IsVisible(map.x * 16 + i, j-1, map.z * 16 + k)) {
                        AddFace2Mesh(map.position[i, j, k], new Vector3(i, j, k), vertices, triangles, uvs, Face.Bottom, count, rects);
                        count += 4;
                    }
                    if (IsVisible(map.x * 16 + i-1, j, map.z * 16 + k)) {
                        AddFace2Mesh(map.position[i, j, k], new Vector3(i, j, k), vertices, triangles, uvs, Face.Left, count, rects);
                        count += 4;
                    }
                    if (IsVisible(map.x * 16 + i+1, j, map.z * 16 + k)) {
                        AddFace2Mesh(map.position[i, j, k], new Vector3(i, j, k), vertices, triangles, uvs, Face.Right, count, rects);
                        count += 4;
                    }
                    if (IsVisible(map.x * 16 + i, j, map.z * 16 + k-1)) {
                        AddFace2Mesh(map.position[i, j, k], new Vector3(i, j, k), vertices, triangles, uvs, Face.Front, count, rects);
                        count += 4;
                    }
                    if (IsVisible(map.x * 16 + i, j, map.z * 16 + k+1)) {
                        AddFace2Mesh(map.position[i, j, k], new Vector3(i, j, k), vertices, triangles, uvs, Face.Back, count, rects);
                        count += 4;
                    }
                }
            }
        }
        // ��Ⱦ�����趨
        filter.mesh.vertices = vertices.ToArray();  //���ö�����Ϣ
        filter.mesh.SetTriangles(triangles.ToArray(), 0);   //������������
        filter.mesh.uv = uvs.ToArray(); //����uv����ͼ��Ӧ�Ķ���
        //filter.mesh.uv = rect; //����uv����ͼ��Ӧ�Ķ���

        // ��ʼ������Ⱦ
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
        //ȡ����
        //var rect = rects[map.position[i, j, k]%3];

        //TODO
        id--;
        id *= 3;
        switch (face) {
            case Face.Top:
                // ��� face Ϊ Top������Ϊ 0
                id += 0;
                break;
            case Face.Bottom:
                // ��� face Ϊ Bottom������Ϊ 2
                id += 2;
                break;
            default:
                // �������������Ϊ 1
                id += 1;
                break;
        }
        var rect = rects[id];
        uvs.Add(new Vector2(rect.xMin, rect.yMin));
        uvs.Add(new Vector2(rect.xMax, rect.yMin));
        uvs.Add(new Vector2(rect.xMax, rect.yMax));
        uvs.Add(new Vector2(rect.xMin, rect.yMax));

    }
    //TODO �����Ż�
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
    //������bug
    void CreateBlock(int id, MapData mapData, int x, int y, int z,GameObject parent) {
        var block = Instantiate(m_BlockPrefab);
        block.transform.SetParent(parent.transform);
        string cn = "Block" + id;
        Type type = Type.GetType(cn);  // ��ȡ����
        var s = block.AddComponent(type);
        var blockComponent = (Block)s;
        blockComponent.Init(mapData, x, y, z);

    }
    void CreateBlockNew(int id, MapData mapData, int x, int y, int z, GameObject parent) {

    }

    /// <summary>
    /// �Ƿ�Ҫ��ʾ���棬��ѯ��ס����ķ����Ƿ���ڻ���͸��
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns>trueΪ��ʾfalseΪ����ʾ</returns>
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
        if (y >= 256) {
            return true;
        }
        x %= 16;
        z %= 16;
        var d = GetMapData(i, j);
        if (d == null) return true;
        //Debug.Log(x + " " + y + " " + z);
        //Debug.Log(new Vector3(x, y, z));
        if (d.position[x,y,z] != 0 ) {
            //string cn = "Block" + d.position[x, y, z];
            //Type type = Type.GetType(cn);  // ��ȡ����
            //Block cb = (Block)Activator.CreateInstance(type);
            //return cb.isTransparency;
            return d.position[x,y,z] == 2;
        }
        return true;
    }
    /// <summary>
    /// ��ȡ��Ӧ�ĵ�ͼ������
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
