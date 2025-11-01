// IMPORTANT: ITEM_SPORTS FOR SOME REASON BREAKS THINGS, LEAVE DISABLED
// raycast check for airborne just completely breaks and lags the server when sportsballs are enabled. may be some other conflict with modded modter addon but everything fixed when sports werent on
// i dont know why and i dont care to know yet

// image slots. 0: slingshot boost, 1: drift trail, 2: hats, 3: aura

datablock PlayerData(PlayerBoostArmor : PlayerStandardArmor)
{
   uiName = "Boost Surf Player";

   canJet = false;
   showEnergyBar = true;
   maxEnergy = 100;
   rechargeRate = 0;

   mass = 120;
   boundingBox = "5 5 10.6";

   maxForwardSpeed = 100; // to get BPS, torque units multiplied by two, this max speed is 200
   horizMaxSpeed = 100;
   maxForwardCrouchSpeed = 68; // ok i lied for some reason the factor is not two for crouch? or maybe the one on top. idk it doesnt really matter
	maxBackwardSpeed = 80;
	maxSideSpeed = 80;

   airControl = 1;
	runSurfaceAngle = 20;
   runForce = 3500;
   maxStepHeight = 0;
};

function PlayerBoostArmor::onNewDataBlock(%this, %obj) 
{
	%obj.surfTick();

   %obj.hasBoosted = false;
   %obj.isDrifting = false;
   %obj.driftStoredSpeed = 0;

   %obj.currentAuraImage = "0";
   %obj.trailImage = slipstreamAuraBaseImage;
   %obj.crouchTrailImage = slipstreamAuraBaseCrouchImage;
   %obj.boostTrailImage = slipstreamBoostTrailImage;
   %obj.driftImage = slipstreamDriftImage;
   %obj.longDriftImage = slipstreamLongDriftImage;


   %obj.schedule(1, setEnergyLevel, 0); // should probably find some onspawn function but this works i guess
}

function Player::surfTick(%this) //shamelessly ripped timer from gamemode surf
{
	cancel(%this.surfTick);

	if(%this.getState() $= "Dead") 
		return;

	if(!%this.isSurfing && !%this.startedSurfing && getSimTime() - %this.spawnTime >= 500) {
		if(mAbs(getWord(%this.getVelocity(), 2)) >= 0.01) {
			%this.isSurfing = 1;
			%this.startedSurfing = 1;

			%this.surfStartTime = getSimTime();
		}
	}

	%max = 75;
	%min = 25;

	if(isObject(%this.client)) // HUD
   {
		%speed = %this.getSpeedInBPS();

      if(%speed > 130 && isEventPending(%this.auraTick) == 0)
      { 
          %this.auraTick();
      }

		%text = "\c6  SPEED <color:FFFFAA> " @ mFloatLength(%speed, 0) SPC "BPS";

      if(%this.isSurfing) 
      {
			%time = getSimTime() - %this.surfStartTime;
			%text = %text SPC "\c6   TIME  <color:FFFFAA>" @ getTimeString(mFloatLength(%time / 1000, 2));
      }
		%factor = mClampF((%speed - %min) / (%max - %min), 0, 1) * 0.1;
		commandToClient(%this.client, 'SetVignette', 1, %factor SPC %factor SPC %factor SPC %factor);
		commandToClient(%this.client, 'BottomPrint', "<font:lucida console:19>" @ %text, 0.25, 1);
	}

   if(getSimTime() - %this.spawnTime > $Game::PlayerInvulnerabilityTime && getWord(%this.position, 2) < 0.3) // force death on ground plane
   {
      %this.kill();

      return;
	}

	%this.surfTick = %this.schedule(50, surfTick);
}

function PlayerBoostArmor::onEnterLiquid(%this, %obj, %coverage, %type)
{
   Parent::onEnterLiquid(%data, %obj, %coverage, %type);
   %obj.hasShotOnce = true;
   %obj.invulnerable = false;
   %obj.damage(%obj, %obj.getPosition(), 10000, $DamageType::Lava);
}

function Player::getSpeedInBPS(%this, %obj) // bricks per second, thanks to Buddy for this
{
   return vectorLen(%this.getVelocity()) * 2;
}

// ripped impulse jump from tf2 scout pack
// thank you space guy and co

// TODO: speed check and emitter for going fast, scales up, sound effect
// TODO: mount temporary image on air dash and slingshot, plus sound
function PlayerBoostArmor::onTrigger(%this,%obj,%slot,%on) 
{
   %r = Parent::onTrigger(%this,%obj,%slot,%on);

   if(%slot == 4 && %on) // right click / jet
   { 
      if(%obj.hasBoosted)
         return %r;
      if(!%obj.isAirborne())
      {
         if(%obj.isDrifting && %obj.getEnergyPercent() == 1)
         {
            %obj.triggerSlingshot();
         }

         return %r;
      }

      %obj.triggerAirDash();
   }

   if(%slot == 3) // crouch 
   {
      if(%on)
      {
         %obj.isDrifting = true;
         %obj.driftStoredSpeed = %obj.getSpeedInBPS();
         cancel(%obj.driftTick);
         %obj.driftTick();
      }
      if(!%on)
      {
         %obj.isDrifting = false;
      }
   }

   return %r;
}

function Player::triggerAirDash(%this)
{
   %this.hasBoosted = true;
   %this.airDashTick();

   
   // angle difference between velocity and player cam direction
   %velocityVector = vectorNormalize(%this.getHorizontalVelocityVector());
   %forwardVector = vectorNormalize(%this.getForwardVector());
   %angleDiff = angleBetweenVectors(%velocityVector, %forwardVector) * (180 / $pi); // work in degrees pls

   if(%angleDiff > 10 || %this.getSpeedInBPS() < 60)
   {
      %impulse = 25; // using setvelocity: doubling this number gets our target BPS
   }                 // this may change if you mess with runforce or drag or some other thing
   else
   {
      %impulse = vectorLen(%this.getVelocity());
   }
   %boostVector = VectorScale(%this.getForwardVector(), %impulse);
   %this.setVelocity(getWords(%boostVector, 0, 1) SPC 9);

   %this.playThread(3,jump);
   %this.playAudio(0, slipstreamAirdashSound);

   if(isEventPending(%this.auraTick) == 0)
   { 
      %this.auraTick();
   }

   %scaleFactor = getWord(%this.getScale(), 2);
   %data = pushBroomProjectile;
   %p = new Projectile()
   {
      dataBlock = %data;
      initialPosition = %this.getPosition();
      initialVelocity = "0 0 -1";
      sourceObject = %this;
      client = %this.client;
      sourceSlot = 0;
      originPoint = %pos;
   };
   %p.setScale(%scaleFactor * 2 SPC %scaleFactor * 2 SPC %scaleFactor * 2);
   %p.explode();
}

function angleBetweenVectors(%vecA, %vecB)
{
   %dot = vectorDot(%vecA, %vecB);
   return mAcos(%dot);
}

function signedAngleBetweenVectors(%vecA, %vecB)
{
   %angle = angleBetweenVectors(%vecA, %vecB);
   %cross = getWord(vectorCross(%vecA, %vecB), 2);
   if (%cross < 0)
      %angle = -%angle; // immediately regretted using clockwise angle as positive. do not do it.
   return %angle;
}

function Player::triggerSlingshot(%this)
{
   %this.unmountImage(1);
   %this.mountImage(%this.boostTrailImage, 0);
   %this.schedule(1000, "unmountImage", 0);

   if(isEventPending(%this.auraTick) == 0)
   { 
      %this.auraTick();
   }

   %this.playAudio(1, slipstreamSlingshotSound);

   %this.isDrifting = false;
   %this.slingshotCharged = false;
   %this.setEnergyLevel(0);


   if(%this.driftStoredSpeed < 100)
      %scaleFactor = 110 / 2;
   else if(%this.driftStoredSpeed < 150)
      %scaleFactor = 170 / 2;
   else
      %scaleFactor = %this.driftStoredSpeed / 2;
      
   %boostVector = VectorScale(%this.getForwardVector(), %scaleFactor);
   %this.setVelocity(getWords(%boostVector, 0, 1) SPC 3);

   // boost explosion
   %projScale = 1.5;
   %p = new Projectile()
   {
      dataBlock = slipstreamExplosionProjectile;
      initialPosition = %this.getPosition();
      initialVelocity = "0 0 -1";
      sourceObject = %this;
      client = %this.client;
      sourceSlot = 0;
      originPoint = %pos;
   };
   %p.setScale(%projScale SPC %projScale SPC 1);
   %p.explode();
}

// raycasts still cast a far wide net and will end up working on some walls
// strongly considering keeping this as a wall run tech though it feels very spammy
// consider feedback if it ever gets discovered
function Player::isAirborne(%this) // thanks to Eagle517 for the ExtraConsoleMethods
{
   if (!(isFunction(%this.findContact())))
   {
      // talk("Missing ExtraConsoleCommands DLL file!");
   }
   else
   {
      %check = getWord(%this.findContact(), 0); // returns 1 if on walkable or jumpable surface
      if(%check == true)
         return false;
   }
   
   // switch to raycasts if contact info fails
   %pos = %this.getPosition();
   %targets = $TypeMasks::FxBrickAlwaysObjectType | $TypeMasks::PlayerObjectType | $TypeMasks::StaticObjectType | $TypeMasks::TerrainObjectType | $TypeMasks::VehicleObjectType;
   %ray[0] = ContainerRayCast(%pos, vectorAdd(%pos,".627 .627 -0.25"), %targets, %this);
   %ray[1] = ContainerRayCast(%pos, vectorAdd(%pos,".627 -.627 -0.25"), %targets, %this);
   %ray[2] = ContainerRayCast(%pos, vectorAdd(%pos,"-.627 .627 -0.25"), %targets, %this);
   %ray[3] = ContainerRayCast(%pos, vectorAdd(%pos,"-.627 -.627 -0.25"), %targets, %this);
   %ray[4] = ContainerRayCast(%pos, vectorAdd(%pos,"0 0 -0.3"), %targets, %this);

   for(%i = 0; %i < 5; %i++)
   {
      %col = getWord(%ray[%i],0);
      if(isObject(%col) && (%col.getType() & %targets) && %col.isColliding())
      {
         return false;
      } 
   }

   return true;
}

// function Player::airtest(%this)
// {
//    cancel(%this.airtest);
//    if(%this.getState() $= "Dead") 
//    {
//       return;
// 	}
	
//    if(!%this.isAirborne())
//    {
//       talk("Ground");
//    }
//    else
//       talk("Air");
//    %this.airtest = %this.schedule(50, airtest);
// }

function Player::airDashTick(%this)  // tick check to see if we are still airborne after airboost
{
   cancel(%this.airDashTick);
   if(%this.getState() $= "Dead") 
   {
		%this.hasBoosted = false;
      return;
	}
	
   if(!%this.isAirborne())
   {
      %this.hasBoosted = false;
      %this.spawnExplosion(radioWaveProjectile);

      // %p = new Projectile()
      // {
      //    dataBlock = slipstreamAirdashChargeExplosionProjectile;
      //    initialPosition = %this.getPosition();
      //    initialVelocity = "0 0 -0.5";
      //    sourceObject = %this;
      //    client = %this.client;
      //    sourceSlot = 0;
      //    originPoint = %pos;
      // };
      // %p.setScale("1 1 1");
      // %p.explode();

      return;
   }

   %this.airDashTick = %this.schedule(10, airDashTick);
}

// TODO : progress drift energy charge, with charge emitters (star orbits?). adjust existing emitter logics.
// consider slow charge on non-turn
function Player::driftTick(%this) // drift cooldown and timer, applying emitter logic
{
   cancel(%this.driftTick);
   
   if(%this.getState() $= "Dead") 
   {
      %this.setEnergyLevel(0);
      %this.slingshotCharged = false;
      %this.stopAudio(2);

      return;
	}
	
   if(!(%this.isDrifting))
   {
      %this.setEnergyLevel(0);
      %this.slingshotCharged = false;
      %this.stopAudio(2);
      %this.unmountImage(1);
      return;
   }

   if(%this.driftStoredSpeed < 25)
   {
      %this.stopAudio(2);
      return;
   }

   %isInAir = %this.isAirborne();
   if(%isInAir)
   {
      %this.setEnergyLevel(0);
      %this.slingshotCharged = false;
      %this.stopAudio(2);
      %this.unmountImage(1);
   }
   else
   {
      %this.playAudio(2, slipstreamDriftingSound);

      if(!%this.slingshotCharged)
      {
         if (%this.getEnergyPercent() == 1)
         {
            %this.playAudio(1, slipstreamSlingReadySound);
            %this.slingshotCharged = true;
         }
         else
         {
            %this.slingshotCharged = false;
         }
      }

      
   }

   // drift turning
   if(!%isInAir)
   {
      %velVector = vectorNormalize(%this.getHorizontalVelocityVector());
      %forwardVector = %this.getForwardVector();
      %angleDiff = signedAngleBetweenVectors(%velVector, %forwardVector) * (180 / $pi); // In degreeees

      %turning = true;
      if(%angleDiff < -10) // if player aim is clockwise to velocity, aka when turning right
      {
         %forceVector = getWord(%velVector, 1) SPC getWord(%velVector, 0) * -1 SPC "0";
      }
      else if(%angleDiff > 10)
      {
         %forceVector = getWord(%velVector, 1) * -1 SPC getWord(%velVector, 0) SPC "0";
      }
      else
      {
         %forceVector = "0 0 0";
         %turning = false;
         %this.unmountImage(1);
      }

      if(mAbs(%angleDiff) > 120)
      {
         %forceVector = vectorScale(%forwardVector, 6);
      }
      else
      {
         %forceVector = vectorScale(%forceVector, 2);
      }

      if(mAbs(%angleDiff) > 80 && mAbs(%angleDiff) < 140)
      {
         %this.setEnergyLevel((%this.getEnergyPercent() * 100) + 4);
         %this.mountImage(%this.longDriftImage, 1);
      }
      else if (%turning)
      {
         %this.setEnergyLevel((%this.getEnergyPercent() * 100) + 3);
         %this.mountImage(%this.longDriftImage, 1);
      }
      else
      {
         %this.mountImage(%this.driftImage, 1);
      }

      %this.AddVelocity(%forceVector);
   }

   %this.driftTick = %this.schedule(40, driftTick);
}

function Player::getHorizontalVelocityVector(%this)
{
   return getWord(%this.getVelocity(), 0) SPC getWord(%this.getVelocity(), 1) SPC " 0";
}

function Player::auraTick(%this)
{
   cancel(%this.auraTick);
   if(%this.getState() $= "Dead") 
   {
      %this.unmountImage(3);
      return;
	}
	
   if(%this.getSpeedInBPS() < 30 && !(%this.isDrifting))
   {
      %this.unmountImage(3);
      %this.currentAuraImage = "0";
      return;
   }
   if(%this.isCrouched())
   {
      if(!(%this.currentAuraImage $= %this.crouchTrailImage))
      {
         %this.mountImage(%this.crouchTrailImage, 3);
      }
   }
   else
   {
      if(!(%this.currentAuraImage $= %this.trailImage))
      {
         %this.mountImage(%this.trailImage, 3);
      }
   }
   
   %this.currentAuraImage = %this.getMountedImage(3).getName();
   %this.auraTick = %this.schedule(50, auraTick);
}

// TODO: add stomp emitter, make smoother, probably dont allow it on ground
package SlipstreamLightOverridePackage
{
   function servercmdLight(%cl) // light key to use stomp ability
   {
      %pl = %cl.player;
      if(isObject(%pl))
      {
         %armorName = %pl.getDataBlock().getName();
         if(strstr(%armorName, "Boost") != -1)
         {
            %pl.setVelocity("0 0 -30");
         }
         else Parent::servercmdLight(%cl);
      }
   }
};
activatePackage("SlipstreamLightOverridePackage");