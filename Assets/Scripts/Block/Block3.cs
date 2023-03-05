
using UnityEngine;
/// <summary>
/// �ݵ� id = 3 
/// </summary>				
public class Block3 : Block {
    public override int ID => 3;

    public override string Name => "�ݵ�";

    public override bool IsTransparent => false;

    protected override string LeftSpritePath => "Blocks/grass_side";

    protected override string RightSpritePath => "Blocks/grass_side";

    protected override string TopSpritePath => "Blocks/grass_top";

    protected override string BottomSpritePath => "Blocks/dirt";

    protected override string FrontSpritePath => "Blocks/grass_side";

    protected override string BackSpritePath => "Blocks/grass_side";

    //�޸Ķ�����ɫ
    public override void ShowBlock(MapData data, int i, int j, int k) {
        base.ShowBlock(data, i, j, k);
        var t = transform.Find("Top");
        var s = transform.GetComponent<SpriteRenderer>();
        Color color;
        ColorUtility.TryParseHtmlString("#91bd59", out color);
        s.color = color;
    }

}

