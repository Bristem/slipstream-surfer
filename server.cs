%addon1 = ForceRequiredAddOn("Weapon_Push_Broom");
%addon2 = ForceRequiredAddOn("Item_Skis");
%addon2 = ForceRequiredAddOn("Vehicle_Pirate_Cannon");

if(%addon1 == $Error::AddOn_Disabled || %addon2 == $Error::AddOn_Disabled || %addon3 == $Error::AddOn_Disabled)
{
   pushBroomItem.uiName = "";
   skisItem.uiName = "";
}


if(%addon1 == $Error::AddOn_NotFound)
{
   error("ERROR: Player_Slipstream - required add-on Weapon_Push_Broom not found");
}
else if(%addon2 == $Error::AddOn_NotFound)
{
   error("ERROR: Player_Slipstream - required add-on Item_Skis not found");
}
else if(%addon3 == $Error::AddOn_NotFound)
{
   error("ERROR: Player_Slipstream - required add-on Vehicle_Pirate_Cannon not found");
}
else
{
   //exec("./Support_AltDatablock.cs");
   exec("./trails.cs");
   exec("./sounds.cs");
   exec("./Player_Slipstream.cs");
   //exec("./horsle.cs");
}
BrickTreasureChestData.isSurfTrack = 1;
BrickTreasureChestOpenData.isSurfTrack = 1;

package TempGameModePackage
{
   function FxDTSBrick::onPlayerTouch(%this, %obj)
   {
      if (%obj.isSurfing && %this.getDataBlock().isSurfTrack) 
      {
			%cl = %obj.client;
         %obj.isSurfing = 0;
			%time = getSimTime() - %obj.surfStartTime;

			%name = %cl.getPlayerName();
			%formatted = getTimeString(mFloatLength(%time / 1000, 2));

			%message = "<bitmap:base/client/ui/ci/star> \c3" @ %name;
			%message = %message SPC "\c6completed the track in \c3" @ %formatted @ "\c6.";

			messageAll('', %message);
		}

		Parent::onPlayerTouch(%this, %obj);
	}
};

activatePackage("TempGameModePackage");