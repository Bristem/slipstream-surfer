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