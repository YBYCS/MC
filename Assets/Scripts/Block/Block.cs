using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public abstract class Block : MonoBehaviour {
    public abstract string blockName { get; }
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

    public void Init(int x, int y, int z) {
        this.x = x;
        this.y = y;
        this.z = z;
        ShowBlock();
    }

    //TODO
    public virtual void ShowBlock() {
        var t = transform.Find("Left");
        AddSprite(t, LeftSprite);
        t = transform.Find("Right");
        AddSprite(t, RightSprite);
        t = transform.Find("Top");
        AddSprite(t, TopSprite);
        t = transform.Find("Bottom");
        AddSprite(t, BottomSprite);
        t = transform.Find("Front");
        AddSprite(t, FrontSprite);
        t = transform.Find("Back");
        AddSprite(t, BackSprite);
    
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
