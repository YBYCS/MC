using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapData 
{
    public int x, z;
    public byte [,,] position = new byte [16,257,16];//257最后一位是放最高高度的。

}
