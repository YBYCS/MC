
/// <summary>
/// 水 id = 2 
/// </summary>				
public class Block2 : Block {
    public override int ID => 2;

    public override string Name => "水";

    public override bool IsTransparent => true;

    protected override string LeftSpritePath => "Blocks/water_overlay";

    protected override string RightSpritePath => "Blocks/water_overlay";

    protected override string TopSpritePath => "Blocks/water_overlay";

    protected override string BottomSpritePath => "Blocks/water_overlay";

    protected override string FrontSpritePath => "Blocks/water_overlay";

    protected override string BackSpritePath => "Blocks/water_overlay";

}

