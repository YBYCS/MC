
/// <summary>
/// 水 id = 2 
/// </summary>				
public class Block2 : Block {
    public override int id => 2;

    public override string blockName => "水";

    public override bool isTransparency => true;

    protected override string LeftSpritePath => "Blocks/water_overlay";

    protected override string RightSpritePath => "Blocks/water_overlay";

    protected override string TopSpritePath => "Blocks/water_overlay";

    protected override string BottomSpritePath => "Blocks/water_overlay";

    protected override string FrontSpritePath => "Blocks/water_overlay";

    protected override string BackSpritePath => "Blocks/water_overlay";

}

