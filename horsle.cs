datablock PlayerData(HorseBoostArmor : HorseArmor){
   uiName = "Boost Surf Horse";
   hasBoosted = false;
   isDrifting = false;
   driftStoredSpeed = 0;
   driftCounter = 0;
   slingCooldown = 0;

   maxForwardSpeed = 100; // to get BPS, torque units multiplied by two, this max speed is 200
   horizMaxSpeed = 100; // 
   maxForwardCrouchSpeed = 100;
	maxBackwardSpeed = 80;
	maxSideSpeed = 80;

   airControl = 1;
	runSurfaceAngle = 20;
   runForce = 3500;
   maxStepHeight = 0;
};

function HorseBoostArmor::onNewDataBlock(%this, %obj) {
	%obj.surfTick();
}

function HorseBoostArmor::onTrigger(%this,%obj,%slot,%on)
{
   //Parent::onTrigger(%this, %obj, %slot, %val); 
   
   PlayerBoostArmor::onTrigger(%this,%obj,%slot,%on);
}