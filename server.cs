%addon1 = ForceRequiredAddOn("Weapon_Push_Broom");
%addon2 = ForceRequiredAddOn("Item_Skis");

if(%addon1 == $Error::AddOn_Disabled || %addon2 == $Error::AddOn_Disabled)
{
   pushBroomItem.uiName = "";
   skisItem.uiName = "";
}


if(%addon1 == $Error::AddOn_NotFound)
{
   error("ERROR: Player_Boost - required add-on Weapon_Push_Broom not found");
}
else if(%addon2 == $Error::AddOn_NotFound)
{
   error("ERROR: Player_Boost - required add-on Item_Skis not found");
}
else
{
   //exec("./Support_AltDatablock.cs");
   exec("./trails.cs");
   exec("./Player_Boost.cs");
   exec("./Horse_Boost.cs");
}