// IMPORTANT: ITEM_SPORTS FOR SOME REASON BREAKS THINGS, LEAVE DISABLED
// raycast check for airborne just completely breaks and lags the server when sportsballs are enabled. may be some other conflict with modded modter addon but everything fixed when sports werent on
// i dont know why and i dont care to know yet

// Image slot 1 stores emitter for charged drift indicating slingshot readiness

datablock PlayerData(PlayerBoostArmor : PlayerStandardArmor)
{
   uiName = "Boost Surf Player";
   
   hasBoosted = false;
   isDrifting = false;
   driftStoredSpeed = 0;
   driftCounter = 0;
   slingCooldown = 0;
   driftBaseMomentumVector = "0 0 0";

   canJet = false;

   mass = 120;
   boundingBox = "5 5 10.6";

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

function PlayerBoostArmor::onNewDataBlock(%this, %obj) 
{
	%obj.surfTick();
}

function Player::surfTick(%this) //shamelessly ripped timer from gamemode surf
{
	cancel(%this.surfTick);

	if (%this.getState() $= "Dead") 
		return;

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

		%factor = mClampF((%speed - %min) / (%max - %min), 0, 1) * 0.1;

		commandToClient(%this.client, 'SetVignette', 1, %factor SPC %factor SPC %factor SPC %factor);
		commandToClient(%this.client, 'BottomPrint', "<font:lucida console:19>" @ %text, 0.25, 1);
	}

   if (getSimTime() - %this.spawnTime > $Game::PlayerInvulnerabilityTime && getWord(%this.position, 2) < 0.3) // force respawn on ground
   {
      if (isObject(%this.client)) 
      {
         %this.client.instantRespawn();
      }
      else 
      {
         %this.kill();
      }

      return;
	}

	%this.surfTick = %this.schedule(50, surfTick);
}

function Player::getSpeedInBPS(%this) // bricks per second, thanks to Buddy for this
{
   return vectorLen(%this.getVelocity()) * 2;
}

// ripped impulse jump from tf2 scout pack
// thank you space guy and co

// TODO: speed check and emitter for going fast, scales up, sound effect
// TODO: mount temporary image on air dash and slingshot, plus sound
// look into obj.playAudio(slot, sound)
function PlayerBoostArmor::onTrigger(%this,%obj,%slot,%on) 
{
   %r = Parent::onTrigger(%this,%obj,%slot,%on);

   if(%slot == 4 && %on) // jet air dash logic
   { 
      if(%obj.hasBoosted)
         return %r;
      if(!%obj.isAirborne()) // sling shot logic
      {
         if(%obj.isDrifting && !(%obj.isAirborne()) && %obj.slingCooldown == 0)
         {
            if(%obj.driftCounter < 18 && %obj.driftStoredSpeed > 70) // eighteen 30ms ticks, 520 ms
            {
               %obj.driftStoredSpeed /= 3; // reduce speed if the drift is too low
            }
            %obj.unmountImage(1);
            %boostVector = VectorScale(%obj.getEyeVector(), %obj.driftStoredSpeed * 60);
            %obj.setVelocity("0 0 0");
            %obj.applyImpulse("0 0 0", getWord(%boostVector, 0) SPC getWord(%boostVector, 1) SPC 15);

            %scaleFactor = (%obj.driftStoredSpeed / 135) * (mPow(%obj.driftCounter / 18, 2)); // scale explosion from a total of max speed possible and max drift time
            %p = new Projectile()
            {
               dataBlock = boostExplosionProjectile;
               initialPosition = %obj.getPosition();
               initialVelocity = "0 0 -1";
               sourceObject = %obj;
               client = %obj.client;
               sourceSlot = 0;
               originPoint = %pos;
            };
            %p.setScale(%scaleFactor SPC %scaleFactor SPC %scaleFactor);
            %p.explode();

            %obj.slingCooldown = 166; 
            %obj.slingCooldownTick();
            
         }
         return %r;  
      }

      %obj.hasBoosted = true;
      %obj.airBoostTick();
      %speed = %obj.getSpeedInBPS();
      //if(%speed < 60) // speed limiter
         //%impulse = 60;
      //else if(%speed > 85)
         //%impulse = 85;
      //else %impulse = %speed;
      %impulse = 45;
      %boostVector = VectorScale(%obj.getEyeVector(), %impulse * 60); // 60 roughly matches given speed in BPS
      %obj.setVelocity("0 0 0");
      %obj.applyImpulse("0 0 0", getWord(%boostVector, 0) SPC getWord(%boostVector, 1) SPC 1300);
      %obj.playThread(3,jump);

      %scaleFactor = getWord(%obj.getScale(), 2);
      %data = pushBroomProjectile;
      %p = new Projectile()
      {
         dataBlock = %data;
         initialPosition = %obj.getPosition();
         initialVelocity = "0 0 -1";
         sourceObject = %obj;
         client = %obj.client;
         sourceSlot = 0;
         originPoint = %pos;
      };
      %p.setScale(%scaleFactor * 2 SPC %scaleFactor * 2 SPC %scaleFactor * 2);
      %p.explode();
   }
   if(%slot == 3) // crouch drift
   {
      if(%on)
      {
         %obj.isDrifting = true;
         %obj.driftStoredSpeed = %obj.getSpeedInBPS();
         //%obj.driftBaseDirection = vectorNormalize(%obj.getEyeVector());
         //announce(%obj.driftBaseDirection);
         //announce(%obj.getEyeVector());
         %obj.driftTick();
      }
      if(!%on)
      {
         %obj.unmountImage(1);
         %obj.isDrifting = false;
      }
   }

   return %r;
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
      return;
	}
	
   if(!(%this.isDrifting))
   {
      %this.driftCounter = 0;
      return;
   }

   if(%this.driftStoredSpeed < 25)
   {
      return;
   }

   if(%this.isAirborne())
   {
      %this.driftCounter = 0;
      %this.driftStoredSpeed = %this.getSpeedInBPS();
      %this.unmountImage(1);
      %isInAir = true;
   }
   else
   {
      %isInAir = false;
      %scaleFactor = 0.4;
      if(%this.slingCooldown > 0)
      {
         %this.slingCooldown -= 1;
      }
      
      if(%this.driftCounter < 18) // at low drift time
      {
         %this.driftCounter += 1;
         

      }
      else if(%this.driftStoredSpeed > 50) // at high drift time AND high enough speed
      {
         %this.mountImage(boostFuseImage, 1);
         %scaleFactor = 0.9;
      }
      //%scaleFactor = getWord(%this.getScale(), 2) * 2;
      %data = boostBroomProjectile;
      %p = new Projectile()
      {
         dataBlock = %data;
         initialPosition = %this.getPosition();
         initialVelocity = "0 0 -1";
         sourceObject = %this;
         client = %this.client;
         sourceSlot = 0;
         originPoint = %this.getPosition();
      };
      %p.setScale(%scaleFactor SPC %scaleFactor SPC %scaleFactor);
      %p.explode();
   }

   // drift turning
   if(!%isInAir && %this.slingCooldown < 140)
   {
      %velVector = getWord(%this.getVelocity(), 0) SPC getWord(%this.getVelocity(), 1) SPC " 0"; // adjust vector to be purely horizontal
      %velSpeed = vectorLen(%velVector) * 2;
      //announce("INITIAL" SPC %vel);
      %speedCap = 105;
      if(%velSpeed > %this.driftStoredSpeed)
      {
         %this.setVelocity(vectorScale(%velVector, 0.95)); // decay speed if going too fast
         %velVector = getWord(%this.getVelocity(), 0) SPC getWord(%this.getVelocity(), 1) SPC " 0";
      }
      if (%velSpeed > %speedCap)
      {
         %hit = %velVector;
         //%vel = vectorScale(vectorNormalize(%velVector), %speedCap / 2); 
         //announce("INITIAL" SPC %hit SPC "REVISED" SPC %vel);
         //%this.setVelocity(%vel);
         %this.setVelocity(vectorScale(%velVector, 0.95));
      }
      %force = vectorScale(%this.getEyeVector(), 150);
      %this.applyImpulse("0 0 0", getWord(%force, 0) SPC getWord(%force, 1) SPC "0");
   }


   %this.driftTick = %this.schedule(30, driftTick);
}

// TODO: add stomp emitter, make smoother, probably dont allow it on ground
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
      else
      {
         Parent::servercmdLight(%cl);
      }
   }
   else
   {
      Parent::servercmdLight(%cl);
   }
}