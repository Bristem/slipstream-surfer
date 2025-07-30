// placeholder slingshot explosion boost
datablock ExplosionData(boostExplosion : vehicleFinalExplosion)
{
   directDamage      = 0;
   radiusDamage      = 0;
   damageRadius      = 0;

   camShakeDuration  = 0.3;
   camShakeFalloff   = 2;
   camShakeRadius    = 5;
};

// im going to find whoever made this necessary
// why does the function called spawnexplosion
// NOT WORK WITH EXPLOSIONS
datablock ProjectileData(boostExplosionProjectile) {
   explosion      = boostExplosion;
   muzzleVelocity = 0;
   lifetime       = 10;
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
   uiName      = "Boost Fuse";

   shapeFile   = "base/data/shapes/empty.dts";
	emap        = false;

   offset      = "0 0 -1.8"; // slightly below player feet
   mountpoint  = 2;

	stateName[0]                  = "Ready";
	stateEmitter[0]               = cannonFuseCEmitter;
	stateEmitterTime[0]           = 99;
};

datablock ParticleData(boostAuraBaseParticle) {
	textureName = "./auraA";

	dragCoefficient = 0;
	gravityCoefficient = 0;
	inheritedVelFactor = 0;
	windCoefficient = 0;

	constantAcceleration = 0;
	useInvAlpha = 1;
	spinSpeed = 0;

	lifetimeMS = 300;
	lifetimeVarianceMS = 0;

	spinRandomMin = 0;
	spinRandomMax = 0;

	colors[0] = "1 1 1 0.3";
	colors[1] = "1 1 1 0";
	colors[2] = "1 1 1 0";

	sizes[0] = 3;
	sizes[1] = 2;
	sizes[2] = 0;

	times[0] = 0;
	times[1] = 0.9;
	times[2] = 1;
};

datablock ParticleEmitterData(boostAuraBaseEmitter)
{
   uiName = "Boost Aura Base";
   
   ejectionPeriodMS = 15;
	periodVarianceMS = 0;

	ejectionVelocity = 0;
	ejectionOffset = 0;

	velocityVariance = 0;
	overrideAdvance = 0;

	thetaMin = 0;
	thetaMax = 0;

	phiReferenceVel = 0;
	phiVariance = 0;

   particles = boostAuraBaseParticle;
};

datablock ShapeBaseImageData(boostAuraBaseImage) {
	shapeFile = "base/data/shapes/empty.dts";
	emap = false;

	mountPoint = 8;
	offset = "0 0 1.25";

	stateName[0] = "Emit";
	stateEmitter[0] = boostAuraBaseEmitter;
	stateEmitterTime[0] = 1;
	stateWaitForTimeout[0] = 1;
	stateTransitionOnTimeout[0] = "Loop";

	stateName[1] = "Loop";
	stateTimeoutValue[1] = 0.3;
	stateTransitionOnTimeout[1] = "Emit";
};

datablock ShapeBaseImageData(boostAuraBaseCrouchImage : boostAuraBaseImage) {
	offset = "0 0 -0.5";
};