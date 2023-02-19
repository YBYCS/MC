using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassBlock : Block {
    public override string blockName => "ฒÝตุ";

    protected override string LeftSpritePath => "Blocks/grass_side";

    protected override string RightSpritePath => "Blocks/grass_side";

    protected override string TopSpritePath => "Blocks/grass_top";

    protected override string BottomSpritePath => "Blocks/dirt";

    protected override string FrontSpritePath => "Blocks/grass_side";

    protected override string BackSpritePath => "Blocks/grass_side";


}
