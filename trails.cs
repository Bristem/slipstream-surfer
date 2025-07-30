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
datablock ExplosionData(boostBroomExplosion : tumbleImpactAExplosion)
{
   soundProfile = "";
   lifetimeMS = 35;
};

datablock ProjectileData(boostBroomProjectile : tumbleImpactAProjectile)
{
   explosion = boostBroomExplosion;
};

//placeholder slingshot charge emitter, replace later with road drif particles r something
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