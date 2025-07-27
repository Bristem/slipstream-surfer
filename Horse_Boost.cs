datablock PlayerData(HorseBoostArmor : HorseArmor){
   uiName = "Boost Surf Horse";
   hasBoosted = false;
   isDrifting = false;

   mass = 120;
   maxForwardSpeed = 300;
	maxBackwardSpeed = 15;
	maxSideSpeed = 125;

   airControl = 0.5;
	runSurfaceAngle = 20;
};

function HorseBoostArmor::onNewDataBlock(%this, %obj) {
	%obj.surfTick();
}

function HorseBoostArmor::onTrigger(%this,%obj,%slot,%on)
{
   //Parent::onTrigger(%this, %obj, %slot, %val); 
   
   PlayerBoostArmor::onTrigger(%this,%obj,%slot,%on);
}