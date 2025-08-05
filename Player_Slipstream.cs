// IMPORTANT: ITEM_SPORTS FOR SOME REASON BREAKS THINGS, LEAVE DISABLED
// raycast check for airborne just completely breaks and lags the server when sportsballs are enabled. may be some other conflict with modded modter addon but everything fixed when sports werent on
// i dont know why and i dont care to know yet

// Image slot 1 stores emitter for charged drift indicating slingshot readiness

// todo: image slots, 1: drift trail, 2: hats, 3: aura

datablock PlayerData(PlayerBoostArmor : PlayerStandardArmor)
{
   uiName = "Boost Surf Player";

   canJet = false;

   mass = 120;
   boundingBox = "5 5 10.6";

   maxForwardSpeed = 100; // to get BPS, torque units multiplied by two, this max speed is 200
   horizMaxSpeed = 100;
   maxForwardCrouchSpeed = 50;
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
   %obj.driftCounter = 0;
   %obj.slingCooldown = 0;
   %obj.slingReady = false;

   %obj.driftCounterLimit = 50;

}

function Player::surfTick(%this) //shamelessly ripped timer from gamemode surf
{
	cancel(%this.surfTick);

	if (%this.getState() $= "Dead") 
		return;
   
	if (!%this.isSurfing && !%this.startedSurfing && getSimTime() - %this.spawnTime >= 500) {
		if (mAbs(getWord(%this.getVelocity(), 2)) >= 0.01) {
			%this.isSurfing = 1;
			%this.startedSurfing = 1;

			%this.surfStartTime = getSimTime();
		}
	}

	%max = 75;
	%min = 25;

	if (isObject(%this.client)) // HUD
   {
		%speed = %this.getSpeedInBPS();
		%text = "\c6  SPEED <color:FFFFAA> " @ mFloatLength(%speed, 0) SPC "BPS";
      if(%this.hasBoosted)
         %dashcolor = "<color:a0a0a0>"; // dash on cooldown display
      else
         %dashcolor = "\c4";

      if(%this.slingCooldown != 0)
         %slingcolor = "<color:a0a0a0>";
      else 
         %slingcolor = "\c4";

         %text = %text NL "<font:lucida console:20>" SPC %dashcolor SPC "DASH " SPC "\c6/" SPC %slingcolor SPC "SLING";

      if (%this.isSurfing) 
      {
			%time = getSimTime() - %this.surfStartTime;
			%text = %text NL "\c6  TIME  <color:FFFFAA>" @ getTimeString(mFloatLength(%time / 1000, 2));
      }
		%factor = mClampF((%speed - %min) / (%max - %min), 0, 1) * 0.1;
		commandToClient(%this.client, 'SetVignette', 1, %factor SPC %factor SPC %factor SPC %factor);
		commandToClient(%this.client, 'BottomPrint', "<font:lucida console:19>" @ %text, 0.25, 1);
	}

   if (getSimTime() - %this.spawnTime > $Game::PlayerInvulnerabilityTime && getWord(%this.position, 2) < 0.3) // force respawn on ground plane
   {
      %this.kill();
      // if (isObject(%this.client)) 
      // {
      //    %this.player.kill();
      //    //%this.client.instantRespawn();
      // }
      // else 
      // {
      //    %this.kill();
      // }

      return;
	}

	%this.surfTick = %this.schedule(50, surfTick);
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
         if(%obj.isDrifting && !(%obj.isAirborne()) && %obj.slingCooldown == 0)
         {
            %obj.triggerSlingshot();
         }
         return %r;
      }

      %obj.triggerAirDash();
   }


   if(%slot == 3) // crouch 
   {
      
      if (%obj.getMountedImage(3) != 0)
      {
         %auraImage = %obj.getMountedImage(3).getName();
         %hasAura = true;
      }
      else
         %hasAura = false;

      if(%on)
      {
         %obj.isDrifting = true;
         %obj.driftStoredSpeed = %obj.getSpeedInBPS();
         cancel(%obj.driftTick);
         %obj.driftTick();

         if(%hasAura)
         {
            if(%auraImage $= "slipstreamAuraBaseImage")
            {
               %obj.mountImage(slipstreamAuraBaseCrouchImage, 3);
            }
         }
      }
      if(!%on)
      {
         %obj.isDrifting = false;

         if(%hasAura)
         {
            if(%auraImage $= "slipstreamAuraBaseCrouchImage")
            {
               %obj.mountImage(slipstreamAuraBaseImage, 3);
            }
         }
      }
   }

   return %r;
}

function Player::triggerAirDash(%this)
{
   %this.hasBoosted = true;
   %this.airBoostTick();
   %speed = %this.getSpeedInBPS();
   //if(%speed < 60) // speed limiter
   //%impulse = 60;
   //else if(%speed > 85)
   //%impulse = 85;
   //else %impulse = %speed;
   %impulse = 45;
   %boostVector = VectorScale(%this.getEyeVector(), %impulse * 60); // 60 roughly matches given speed in BPS
   %this.setVelocity("0 0 0");
   %this.applyImpulse("0 0 0", getWord(%boostVector, 0) SPC getWord(%boostVector, 1) SPC 1300);
   %this.playThread(3,jump);
   %this.playAudio(0, slipstreamAirdashSound);
   %this.mountImage(slipstreamAuraBaseImage, 3);

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

function Player::triggerSlingshot(%this)
{
   if(%this.driftCounter < %this.driftCounterLimit && %this.driftStoredSpeed > 70)
   {
      %this.driftStoredSpeed /= 3; // reduce speed if the drift is too low
   }
   %this.unmountImage(1);
   %this.mountImage(slipstreamBoostTrailImage, 0);
   %this.schedule(2000, unmountImage(0));
   %this.playAudio(1, slipstreamSlingshotSound);
   %this.isDrifting = false;
   %this.slingReady = false;
   %boostVector = VectorScale(%this.getEyeVector(), %this.driftStoredSpeed * 60);
   %this.setVelocity("0 0 0");
   %this.applyImpulse("0 0 0", getWord(%boostVector, 0) SPC getWord(%boostVector, 1) SPC 15);

   %scaleFactor = (%this.driftStoredSpeed / 135) * (mPow(%this.driftCounter / %this.driftCounterLimit, 2)); // scale explosion from a total of max speed possible and max drift time
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
   %p.setScale(%scaleFactor SPC %scaleFactor SPC %scaleFactor);
   %p.explode();

   %this.slingCooldown = 166; 
   %this.slingCooldownTick();
}

// Do not touch the ground plane with this active it does not like that
// 2 lazy to find the typemask for whatever contain that
// edit  ok it just echoes errors out in console theyre harmless i think
function Player::isAirborne(%this) // thank you space guy
{
      %pos = %this.getPosition();
      %targets = $TypeMasks::FxBrickAlwaysObjectType | $TypeMasks::PlayerObjectType | $TypeMasks::StaticObjectType | $TypeMasks::TerrainObjectType | $TypeMasks::VehicleObjectType;
      %ray = ContainerRayCast(%pos, vectorAdd(%pos,"0 0 -0.627"), %targets, %this);
      %col = getWord(%ray,0);

      if(isObject(%col) && (%col.getType() & %targets) && %col.isColliding())
      {
         return false;
      }
      return true;
}

function Player::airBoostTick(%this)  // tick check to see if we are still airborne after airboost
{
   cancel(%this.airBoostTick);
   if (%this.getState() $= "Dead") 
   {
		%this.hasBoosted = false;
      return;
	}
	
   if(!%this.isAirborne())
   {
      %this.hasBoosted = false;
      return;
   }

   %this.airBoostTick = %this.schedule(30, airBoostTick);
}

function Player::slingCooldownTick(%this) // schedule loop to decrement sling cooldown
{
   cancel(%this.slingCooldownTick);
   if (%this.getState() $= "Dead") 
   {
      %this.slingCooldown = 0;
      return;
	}
   if(%this.slingCooldown <= 0)
   {
      return;
   }
   %this.slingCooldown -= 1;

   %this.slingCooldownTick = %this.schedule(30, slingCooldownTick);
}

function Player::driftTick(%this) // drift cooldown and timer, applying emitter logic
{
   cancel(%this.driftTick);
   
   if (%this.getState() $= "Dead") 
   {
      %this.driftCounter = 0;
      %this.slingReady = false;
      %this.stopAudio(2);
      return;
	}
	
   if(!(%this.isDrifting))
   {
      %this.driftCounter = 0;
      %this.slingReady = false;
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
      %this.stopAudio(2);
      %this.driftCounter = 0;
      %this.driftStoredSpeed = %this.getSpeedInBPS();
      %this.unmountImage(1);
      %this.slingReady = false;
   }
   else
   {
      %isInAir = false;
      %scaleFactor = 0.4;
      %this.playAudio(2, slipstreamDriftingSound);
      if(%this.slingCooldown > 0)
      {
         %this.slingCooldown -= 1;
      }
      
      if(%this.driftCounter < %this.driftCounterLimit) // at low drift time
      {
         %this.driftCounter += 1;
      }
      else if(%this.driftStoredSpeed > 50 && !%this.slingReady) // at high drift time AND high enough speed
      {
         %this.slingReady = true;
         %this.mountImage(slipstreamLongDriftImage, 1);
         %scaleFactor = 0.9;
         %this.playAudio(1, slipstreamSlingReadySound);
      }

      if(!%this.slingReady)
         %this.mountImage(slipstreamDriftImage, 1);
   }

   // drift turning
   if(!%isInAir && %this.slingCooldown < 140)
   {
      %velVector = %this.getHorizontalVelocityVector();
      %velSpeed = vectorLen(%velVector) * 2;
      %speedCap = 105;
      if(%velSpeed > %this.driftStoredSpeed || %velSpeed > %speedCap)
      {
         %velVector = %this.getHorizontalVelocityVector();
         %this.setVelocity(vectorScale(%velVector, 0.90)); // decay speed if going too fast
      }
      %force = vectorScale(%this.getEyeVector(), 200);
      %this.applyImpulse("0 0 0", getWord(%force, 0) SPC getWord(%force, 1) SPC "0");
   }


   %this.driftTick = %this.schedule(40, driftTick);
}

function Player::getHorizontalVelocityVector(%this)
{
   return getWord(%this.getVelocity(), 0) SPC getWord(%this.getVelocity(), 1) SPC " 0";
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
         if (strstr(%armorName, "Boost") != -1)
         {
            %pl.setVelocity("0 0 0");
            %pl.applyImpulse("0 0 0", "0 0 -5000");
         }
         else Parent::servercmdLight(%cl);
      }
   }
};
activatePackage("SlipstreamLightOverridePackage");