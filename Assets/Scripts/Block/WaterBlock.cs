public class WaterBlock : Block {
    public override string Name => "ˮ";

    public override int ID => 2;

    public override bool IsTransparent => true;
    

    protected override string LeftSpritePath => "Blocks/water_overlay";

    protected override string RightSpritePath => "Blocks/water_overlay";

    protected override string TopSpritePath => "Blocks/water_overlay";

    protected override string BottomSpritePath => "Blocks/water_overlay";

    protected override string FrontSpritePath => "Blocks/water_overlay";

    protected override string BackSpritePath => "Blocks/water_overlay";
}
