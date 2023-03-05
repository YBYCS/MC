public class GrassBlock : Block {
    public override string Name => "ฒÝตุ";

    public override int ID => 3;

    public override bool IsTransparent => false;

    protected override string LeftSpritePath => "Blocks/grass_side";

    protected override string RightSpritePath => "Blocks/grass_side";

    protected override string TopSpritePath => "Blocks/grass_top";

    protected override string BottomSpritePath => "Blocks/dirt";

    protected override string FrontSpritePath => "Blocks/grass_side";

    protected override string BackSpritePath => "Blocks/grass_side";


}
