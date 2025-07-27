datablock PlayerData(PlayerBoostArmor : PlayerStandardArmor)
{
   uiName = "Boost Surf Player";
   canJet = false;
   hasBoosted = false;
   isDrifting = false;
   driftStoredSpeed = 0;
   driftCounter = 0;
   driftCooldown = 0;

   mass = 120;
   boundingBox = "5 5 10.6";

   maxForwardSpeed = 300;
	maxBackwardSpeed = 15;
	maxSideSpeed = 125;

   airControl = 0.5;
	runSurfaceAngle = 20;
   runForce = 3500;
   maxStepHeight = 0;
};

// placeholder slingshot explosion boost
datablock ExplosionData(boostExplosion : vehicleFinalExplosion)
{
   directDamage        = 0;
   radiusDamage        = 0;
   damageRadius        = 0;

   camShakeDuration = 0.3;
   camShakeFalloff = 2;
   camShakeRadius = 5;
};

// im going to find whoever made this necessary
// why does the function called spawnexplosion
// NOT WORK WITH EXPLOSIONS
datablock ProjectileData(boostExplosionProjectile) {
   explosion = boostExplosion;
   muzzleVelocity = 0;
   lifetime = 10;
};

// broom particles
datablock ExplosionData(boostBroomExplosion : pushBroomExplosion)
{
   camShakeDuration = 0;
   uiName = "Boost Broom";
};

datablock ProjectileData(boostBroomProjectile : pushBroomProjectile)
{
   explosion = boostBroomExplosion;
   uiName = "Boost Broom";
};

//placeholder slingshot emitter, replace later with road drif particles r something
datablock ShapeBaseImageData(boostFuseImage) 
{
   uiName = "Boost Fuse";

   shapeFile = "base/data/shapes/empty.dts";
	emap = false;

   offset = "0 0 -1.8"; // slightly below player feet
   mountpoint = 2;
   rotation = eulerToMatrix("180 0 0");

	stateName[0]                  = "Ready";
	stateEmitter[0]               = cannonFuseCEmitter;
	stateEmitterTime[0]           = 99;
};

function PlayerBoostArmor::onNewDataBlock(%this, %obj) //shamelessly ripped timer from gamemode surf
{
	%obj.surfTick();
}

function Player::surfTick(%this) 
{
	cancel(%this.surfTick);

	if (%this.getState() $= "Dead") 
		return;

	%max = 75;
	%min = 25;

	if (isObject(%this.client)) 
   {
		%speed = %this.getSpeedInBPS();
		%text = "\c6  SPEED <color:FFFFAA> " @ mFloatLength(%speed, 0) SPC "BPS";
      if(%this.hasBoosted)
         %dashcolor = "<color:a0a0a0>";
      else
         %dashcolor = "\c4";

      if(%this.driftCooldown != 0)
         %driftcolor = "<color:a0a0a0>";
      else 
         %driftcolor = "\c4";

         %text = %text NL "<font:lucida console:20>" SPC %dashcolor SPC "DASH " SPC "\c6/" SPC %driftcolor SPC "SLING";

		%factor = mClampF((%speed - %min) / (%max - %min), 0, 1) * 0.1;

		commandToClient(%this.client, 'SetVignette', 1, %factor SPC %factor SPC %factor SPC %factor);
		commandToClient(%this.client, 'BottomPrint', "<font:lucida console:19>" @ %text, 0.25, 1);
	}

	%this.surfTick = %this.schedule(50, surfTick);
}

function Player::getSpeedInBPS(%this) // bricks per second, thanks to Buddy for this
{
   return vectorLen(%this.getVelocity()) * 2;
}

// ripped impulse jump from tf2 scout pack
// thank you space guy and co

function PlayerBoostArmor::onTrigger(%this,%obj,%slot,%on) 
{
   %r = Parent::onTrigger(%this,%obj,%slot,%on);

   if(%slot == 4 && %on) // air dash logic
   { 
      if(%obj.hasBoosted)
         return %r;
      if(!%obj.isAirborne())
         return %r;
      
      %obj.hasBoosted = true;
      %obj.airBoostTick();
      %speed = %obj.getSpeedInBPS();
      if(%speed < 60) // speed limiter
         %impulse = 60;
      else if(%speed > 85)
         %impulse = 85;
      else %impulse = %speed;
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
   if(%slot == 3) // crouch slingshot logic (IT WAS CALLED DRIFT BEFORE OK)
   {
      if(%on)
      {
         %obj.isDrifting = true;
         %obj.driftStoredSpeed = %obj.getSpeedInBPS();
         %obj.driftTick();
      }
      if(!%on)
      {
         %obj.unmountImage(2);
         if(%obj.isDrifting && !(%obj.isAirborne()) && %obj.driftCooldown == 0)
         {
            if(%obj.driftCounter < 18) // 540 ms given 30ms ticks
            {
               %obj.driftStoredSpeed -= 30; // reduce speed if the drift is too low
            }
            %boostVector = VectorScale(%obj.getEyeVector(), %obj.driftStoredSpeed * 60);
            %obj.setVelocity("0 0 0");
            %obj.applyImpulse("0 0 0", getWord(%boostVector, 0) SPC getWord(%boostVector, 1) SPC 15);

            %scaleFactor = (%obj.driftStoredSpeed / 135) * (mPow(%obj.driftCounter / 18, 2)); // scale explosion from a total of max speed possible and max drift
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

            %obj.driftCooldown = 40;
            %obj.driftCooldownTick();
            
         }
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

function Player::driftCooldownTick(%this) // loop check to get drift back after cooldown
{
   cancel(%this.driftCooldownTick);
   if (%this.getState() $= "Dead") 
   {
      %this.driftCooldown = 0;
      return;
	}
   if(%this.driftCooldown <= 0)
   {
      return;
   }
   %this.driftCooldown -= 1;

   %this.driftCooldownTick = %this.schedule(30, driftCooldownTick);
}


function Player::driftTick(%this) // incrementing counter as we drift and attaching appropriate emitters
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

   if(%this.isAirborne())
   {
      %this.driftCounter = 0;
      %this.driftStoredSpeed = %this.getSpeedInBPS();
   }
   else
   {
      if(%this.driftCooldown > 0)
      {
         %this.driftCooldown -= 1;
      }
      
      if(%this.driftCounter < 18) // at low drift time
      {
         %this.driftCounter += 1;

         %scaleFactor = getWord(%this.getScale(), 2) * 2;
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
      else if(%this.driftStoredSpeed > 70) // at high drift time AND high enough speed
      {
         %this.mountImage(boostFuseImage, 2);
      }
   }
   

   %this.driftTick = %this.schedule(30, driftTick);
}

// TODO: add stomp emitter, make smoother, probably dont allow it on ground
function servercmdLight(%cl) // stomp ability onto light
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
      Parent::servercmdLight(%cl);
   }
}