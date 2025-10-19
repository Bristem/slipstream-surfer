if(ForceRequiredAddOn("Weapon_Push_Broom") == $Error::AddOn_NotFound)
{
   error("ERROR: Player_Slipstream - required add-on Weapon_Push_Broom not found");
}
else if(ForceRequiredAddOn("Item_Skis") == $Error::AddOn_NotFound)
{
   error("ERROR: Player_Slipstream - required add-on Item_Skis not found");
}
else if(ForceRequiredAddOn("Vehicle_Pirate_Cannon") == $Error::AddOn_NotFound)
{
   error("ERROR: Player_Slipstream - required add-on Vehicle_Pirate_Cannon not found");
}
else if(ForceRequiredAddOn("Weapon_Rocket_Launcher") == $Error::AddOn_NotFound)
{
   error("ERROR: Player_Slipstream - required add-on Weapon_Rocket_Launcher not found");
}
else
{
   exec("./temptimer.cs");
   exec("./Player_Slipstream/server.cs");
}