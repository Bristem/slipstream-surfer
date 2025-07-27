//-----------------------------------------------------------------------------
// Torque Game Engine 
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

// Load dts shapes and merge animations
datablock TSShapeConstructor(HorseDts)
{
	baseShape  = "./horse.dts";
	sequence0  = "./h_root.dsq root";

	sequence1  = "./h_run.dsq run";
	sequence2  = "./h_run.dsq walk";
	sequence3  = "./h_back.dsq back";
	sequence4  = "./h_side.dsq side";

	sequence5  = "./h_root.dsq crouch";
	sequence6  = "./h_run.dsq crouchRun";
	sequence7  = "./h_back.dsq crouchBack";
	sequence8  = "./h_side.dsq crouchSide";

	sequence9  = "./h_look.dsq look";
	sequence10 = "./h_root.dsq headside";
	sequence11 = "./h_root.dsq headUp";

	sequence12 = "./h_jump.dsq jump";
	sequence13 = "./h_jump.dsq standjump";
	sequence14 = "./h_root.dsq fall";
	sequence15 = "./h_root.dsq land";

	sequence16 = "./h_armAttack.dsq armAttack";
	sequence17 = "./h_root.dsq armReadyLeft";
	sequence18 = "./h_root.dsq armReadyRight";
	sequence19 = "./h_root.dsq armReadyBoth";
	sequence20 = "./h_spearReady.dsq spearready";  
	sequence21 = "./h_root.dsq spearThrow";

	sequence22 = "./h_root.dsq talk";  

	sequence23 = "./h_death1.dsq death1"; 
	
	sequence24 = "./h_shiftUp.dsq shiftUp";
	sequence25 = "./h_shiftDown.dsq shiftDown";
	sequence26 = "./h_shiftAway.dsq shiftAway";
	sequence27 = "./h_shiftTo.dsq shiftTo";
	sequence28 = "./h_shiftLeft.dsq shiftLeft";
	sequence29 = "./h_shiftRight.dsq shiftRight";
	sequence30 = "./h_rotCW.dsq rotCW";
	sequence31 = "./h_rotCCW.dsq rotCCW";

	sequence32 = "./h_root.dsq undo";
	sequence33 = "./h_plant.dsq plant";

	sequence34 = "./h_root.dsq sit";

	sequence35 = "./h_root.dsq wrench";

   sequence36 = "./h_root.dsq activate";
   sequence37 = "./h_root.dsq activate2";

   sequence38 = "./h_root.dsq leftrecoil";
};    


datablock AudioProfile(HorseJumpSound)
{
   fileName = "./jumpHorse.wav";
   description = AudioClose3d;
   preload = true;
};


datablock DebrisData( HorseDebris )
{
   explodeOnMaxBounce = false;

   elasticity = 0.15;
   friction = 0.5;

   lifetime = 4.0;
   lifetimeVariance = 0.0;

   minSpinSpeed = 40;
   maxSpinSpeed = 600;

   numBounces = 5;
   bounceVariance = 0;

   staticOnMaxBounce = true;
   gravModifier = 1.0;

   useRadiusMass = true;
   baseRadius = 1;

   velocity = 20.0;
   velocityVariance = 12.0;
};             

datablock PlayerData(HorseArmor)
{
   renderFirstPerson = false;
   emap = false;
   
   className = Armor;
   shapeFile = "./horse.dts";
   cameraMaxDist = 8;
   cameraTilt = 0.261;//0.174 * 2.5; //~25 degrees
   cameraVerticalOffset = 2.3;
     
   cameraDefaultFov = 90.0;
   cameraMinFov = 5.0;
   cameraMaxFov = 120.0;
   
   //debrisShapeName = "~/data/shapes/player/debris_player.dts";
   //debris = horseDebris;

   aiAvoidThis = true;

   minLookAngle = -1.5708;
   maxLookAngle = 1.5708;
   maxFreelookAngle = 3.0;

   mass = 90;
   drag = 0.1;
   density = 0.7;
   maxDamage = 250;
   maxEnergy =  10;
   repairRate = 0.33;

   rechargeRate = 0.4;

   runForce = 28 * 90;
   runEnergyDrain = 0;
   minRunEnergy = 0;
   maxForwardSpeed = 12;
   maxBackwardSpeed = 6;
   maxSideSpeed = 1;

   maxForwardCrouchSpeed = 12;
   maxBackwardCrouchSpeed = 6;
   maxSideCrouchSpeed = 1;

   maxForwardProneSpeed = 0;
   maxBackwardProneSpeed = 0;
   maxSideProneSpeed = 0;

   maxForwardWalkSpeed = 0;
   maxBackwardWalkSpeed = 0;
   maxSideWalkSpeed = 0;

   maxUnderwaterForwardSpeed = 8.4;
   maxUnderwaterBackwardSpeed = 7.8;
   maxUnderwaterSideSpeed = 7.8;

   jumpForce = 17 * 90; //8.3 * 90;
   jumpEnergyDrain = 0;
   minJumpEnergy = 0;
   jumpDelay = 0;

   minJetEnergy = 0;
	jetEnergyDrain = 0;
	canJet = 0;

   minImpactSpeed = 250;
   speedDamageScale = 3.8;

   boundingBox			= vectorScale("2.5 2.5 2.4", 4);
   crouchBoundingBox	= vectorScale("2.5 2.5 2.4", 4);
   
   pickupRadius = 0.75;
   
   // Foot Prints
   //decalData   = HorseFootprint;
   //decalOffset = 0.25;
	
   jetEmitter = "";
   jetGroundEmitter = "";
   jetGroundDistance = 4;
  
   //footPuffEmitter = LightPuffEmitter;
   footPuffNumParts = 10;
   footPuffRadius = 0.25;

   //dustEmitter = LiftoffDustEmitter;

   splash = PlayerSplash;
   splashVelocity = 4.0;
   splashAngle = 67.0;
   splashFreqMod = 300.0;
   splashVelEpsilon = 0.60;
   bubbleEmitTime = 0.1;
   splashEmitter[0] = PlayerFoamDropletsEmitter;
   splashEmitter[1] = PlayerFoamEmitter;
   splashEmitter[2] = PlayerBubbleEmitter;
   mediumSplashSoundVelocity = 10.0;   
   hardSplashSoundVelocity = 20.0;   
   exitSplashSoundVelocity = 5.0;

   // Controls over slope of runnable/jumpable surfaces
   runSurfaceAngle  = 85;
   jumpSurfaceAngle = 86;

   minJumpSpeed = 20;
   maxJumpSpeed = 30;

   horizMaxSpeed = 68;
   horizResistSpeed = 33;
   horizResistFactor = 0.35;

   upMaxSpeed = 80;
   upResistSpeed = 25;
   upResistFactor = 0.3;
   
   footstepSplashHeight = 0.35;

   //NOTE:  some sounds commented out until wav's are available

   JumpSound			= HorseJumpSound;

   // Footstep Sounds
//   FootSoftSound        = HorseFootFallSound;
//   FootHardSound        = HorseFootFallSound;
//   FootMetalSound       = HorseFootFallSound;
//   FootSnowSound        = HorseFootFallSound;
//   FootShallowSound     = HorseFootFallSound;
//   FootWadingSound      = HorseFootFallSound;
//   FootUnderwaterSound  = HorseFootFallSound;
   //FootBubblesSound     = FootLightBubblesSound;
   //movingBubblesSound   = ArmorMoveBubblesSound;
   //waterBreathSound     = WaterBreathMaleSound;

   //impactSoftSound      = ImpactLightSoftSound;
   //impactHardSound      = ImpactLightHardSound;
   //impactMetalSound     = ImpactLightMetalSound;
   //impactSnowSound      = ImpactLightSnowSound;
   
   impactWaterEasy      = Splash1Sound;
   impactWaterMedium    = Splash1Sound;
   impactWaterHard      = Splash1Sound;
   
   groundImpactMinSpeed    = 10.0;
   groundImpactShakeFreq   = "4.0 4.0 4.0";
   groundImpactShakeAmp    = "1.0 1.0 1.0";
   groundImpactShakeDuration = 0.8;
   groundImpactShakeFalloff = 10.0;
   
   //exitingWater         = ExitingWaterLightSound;

   // Inventory Items
	maxItems   = 10;	//total number of bricks you can carry
	maxWeapons = 5;		//this will be controlled by mini-game code
	maxTools = 5;
	
	uiName = "Horse";
	rideable = true;
		lookUpLimit = 0.6;
		lookDownLimit = 0.2;

	canRide = false;
	showEnergyBar = false;
	paintable = true;

	brickImage = horseBrickImage;	//the imageData to use for brick deployment

   numMountPoints = 1;
   mountThread[0] = "root";
   mountNode[0] = 2;
};



function HorseArmor::onAdd(%this,%obj)
{
   // Vehicle timeout
   %obj.mountVehicle = true;

   // Default dynamic armor stats
   %obj.setRepairRate(0);

}



//called when the driver of a player-vehicle is unmounted
function HorseArmor::onDriverLeave(%obj, %player)
{
	//do nothing
}