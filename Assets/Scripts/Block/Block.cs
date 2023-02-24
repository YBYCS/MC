using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mesh;

public abstract class Block : MonoBehaviour {
    public abstract string blockName { get; }
    public abstract int id { get; }
    public abstract bool isTransparency { get; }
    public int x, y, z;
    protected abstract string LeftSpritePath { get; }
    protected abstract string RightSpritePath { get; }
    protected abstract string TopSpritePath { get; }
    protected abstract string BottomSpritePath { get; }
    protected abstract string FrontSpritePath { get; }
    protected abstract string BackSpritePath { get; }

    protected Sprite LeftSprite {
        get {
            return GetSprite(LeftSpritePath);
        }
    }

    protected Sprite RightSprite {
        get {
            return GetSprite(RightSpritePath);
        }
    }

    protected Sprite TopSprite {
        get {
            return GetSprite(TopSpritePath);
        }
    }

    protected Sprite BottomSprite {
        get {
            return GetSprite(BottomSpritePath);
        }
    }

    protected Sprite FrontSprite {
        get {
            return GetSprite(FrontSpritePath);
        }
    }

    protected Sprite BackSprite {
        get {
            return GetSprite(BackSpritePath);
        }
    }

    public virtual void Init(MapData data,int i,int j,int k) {
        Setposition(data.x * 16 + i, data.z * 16 + j, k);
        ShowBlock(data,  i, j, k);
    
    }
    public void Setposition(int x, int y, int z) {
        this.x = x;
        this.y = y;
        this.z = z;
        transform.position = new Vector3(x, y, z);
        
    }

    //TODO
    public virtual void ShowBlock(MapData data, int i, int j, int k) {

        
        var t = transform.Find("Left");
        var gm = GameManager.Instance;
        if (gm.IsVisible(x - 1, y, z)) {
            AddSprite(t, LeftSprite);
        }
        if (gm.IsVisible(x + 1, y, z)) {
            t = transform.Find("Right");
            AddSprite(t, RightSprite);
        }
        if (gm.IsVisible(x , y+1, z)) {
            t = transform.Find("Top");
            AddSprite(t, TopSprite);
        }
        if (gm.IsVisible(x, y-1, z)) {
            t = transform.Find("Bottom");
            AddSprite(t, BottomSprite);
        }
        if (gm.IsVisible(x, y, z-1)) {
            t = transform.Find("Front");
            AddSprite(t, FrontSprite);
        }
        if (gm.IsVisible(x, y, z + 1)) {
            t = transform.Find("Back");
            AddSprite(t, BackSprite);
        }


    
    }
    protected void AddSprite(Transform transform,Sprite sprite) {
        AddSprite(transform, sprite, Color.white);
    }
    protected void AddSprite(Transform transform,Sprite sprite,Color color) {
        var s = transform.GetComponent<SpriteRenderer>();
        s.sprite = sprite;
        s.color = color;
    }
    protected Sprite GetSprite(string path) {
        Texture2D tex = Resources.Load<Texture2D>(path);
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 16);
    }
}
