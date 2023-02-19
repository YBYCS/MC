using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    GameObject m_BlockPrefab;
    // Start is called before the first frame update
    void Start()
    {
        CreateGrass();
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateGrass() {
        var block = Instantiate(m_BlockPrefab);
        var s = block.AddComponent<GrassBlock>();
        s.Init(0, 0, 0);
    }
}
