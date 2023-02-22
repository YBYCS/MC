using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBlock : Block {
    public override string blockName => "ˮ";

    public override int id => 2;

    public override bool isTransparency => true;
    

    protected override string LeftSpritePath => "Blocks/water_overlay";

    protected override string RightSpritePath => "Blocks/water_overlay";

    protected override string TopSpritePath => "Blocks/water_overlay";

    protected override string BottomSpritePath => "Blocks/water_overlay";

    protected override string FrontSpritePath => "Blocks/water_overlay";

    protected override string BackSpritePath => "Blocks/water_overlay";
}
