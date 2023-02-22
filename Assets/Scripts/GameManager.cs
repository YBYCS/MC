using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LYcommon;
using System;

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    GameObject m_BlockPrefab;
    MapData testData;
    // Start is called before the first frame update
    void Start()
    {
        Init();
        Show();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Init() {
        CreateMapData();

    }

    void CreateGrass() {
        var block = Instantiate(m_BlockPrefab);
        var s = block.AddComponent<GrassBlock>();
        s.Init(testData,0, 0, 0);
    }

    void CreateMapData() {
        var d = new MapData();
        d.x = 0;
        d.y = 0;
        for (int i = 0; i < 16; i++) {
            for (int j = 0; j < 16; j++) {
                var h = 128 * Mathf.PerlinNoise(i / 16f, j / 16f)+ 64 * Mathf.PerlinNoise(i / 16, j / 16);
                //+ 32 * Mathf.PerlinNoise(i / 16, j / 16);
                //+ 64 * Mathf.PerlinNoise( i / 16, j / 16) + 32 * Mathf.PerlinNoise( i / 16,  j / 16) ;
                //Debug.Log(new Vector3(i,j,h));
                //Debug.Log(new Vector2(4 * i, 4 * j));

                Debug.Log(h);
                for (int k = 0; (k < h||k<64)&&k<256; k++) {
                    if (k < h) {
                        //��Ϊ����
                        d.position[i, k, j] = 1;
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
        testData = d;

    }

    void Show() {
        for (int i = 0; i < 16; i++) {
            for (int j = 0; j < 256; j++) {
                for (int k = 0; k < 16; k++) {
                    if (testData.position[i, j, k] == 0) {
                        continue;
                    }
                    //switch (testData.position[i, j, k]) {
                    //    case 1:
                    //        //����
                    //        break;
                    //    case 2:
                    //        //TODO ��ˮ
                    //        break;
                    //    case 3:
                    //        //�ݵ�
                    //        CreateBlock<GrassBlock>(testData, i, j, k);
                    //        break;
                    //}
                    CreateBlock(testData.position[i, j, k], testData, i, j, k);
                }
            }
        }
    }

    void CreateBlock<T>(MapData mapData,int x,int y,int z) where T:Block{
        var block = Instantiate(m_BlockPrefab);
        var s = block.AddComponent<T>();
        //s.Setposition(testData.x * 16 + i, testData.y * 16 + j, k);
        s.Init(testData, x, y, z);
    }
    //������bug
    void CreateBlock(int id, MapData mapData, int x, int y, int z) {
        var block = Instantiate(m_BlockPrefab);
        string cn = "Block" + id;
        Type type = Type.GetType(cn);  // ��ȡ����
        var s = block.AddComponent(type);
        var blockComponent = (Block)s;
        blockComponent.Init(mapData, x, y, z);

    }
    /// <summary>
    /// �Ƿ�Ҫ��ʾ���棬��ѯ��ס����ķ����Ƿ���ڻ���͸��
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns>trueΪ��ʾfalseΪ����ʾ</returns>
    public bool IsShouldShow(int x,int y,int z) {

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
            //Type type = Type.GetType(cn);  // ��ȡ����
            //Block cb = (Block)Activator.CreateInstance(type);
            //return cb.isTransparency;
            return d.position[x,y,z] == 2?true:false;
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
        return testData;
    }
}
