%error = ForceRequiredAddOn("Weapon_Push_Broom");

if(%error == $Error::AddOn_Disabled)
{
   pushBroomItem.uiName = "";
}


if(%error == $Error::AddOn_NotFound)
{
   error("ERROR: Player_Boost - required add-on Weapon_Push_Broom not found");
}
else
{
   //exec("./Support_AltDatablock.cs");
   exec("./Player_Boost.cs");
   exec("./Horse_Boost.cs");
   exec("./debugs.cs");
   exec("./horsepathcam.cs");
}