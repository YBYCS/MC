using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LYcommon;

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
                for (int k = 0; k < 256; k++) {
                    if (k < 64) {
                        d.position[i, k, j] = 1;
                    }
                    else {
                        
                        break;
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
                    switch (testData.position[i, j, k]) {
                        case 1:
                            var block = Instantiate(m_BlockPrefab);
                            var s = block.AddComponent<GrassBlock>();
                            //s.Setposition(testData.x * 16 + i, testData.y * 16 + j, k);
                            s.Init(testData, i, j, k);
                            break;
                    }
                }
            }
        }
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
        Debug.Log(x + " " + y + " " + z);
        if (d.position[x,y,z] != 0) {
            //����ʾ
            return false;
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
