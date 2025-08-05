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

// its torque making explosions client sided
datablock ProjectileData(boostExplosionProjectile) {
   explosion      = boostExplosion;
   muzzleVelocity = 0;
   lifetime       = 10;
};

// drift particles

datablock ParticleEmitterData(boostDriftEmitter : vehicleBubbleEmitter)
{
	lifetimeMS = 0;
	ejectionOffset = 0.5;
	ejectionVelocity = 0.1;
	velocityVariance = 0.1;
};

datablock ShapeBaseImageData(boostDriftImage) 
{
	shapeFile   = "base/data/shapes/empty.dts";
	emap        = false;

	offset      = "-0.1 0 -0.5";
	mountpoint  = 3;

	stateName[0]                  = "Ready";
	stateEmitter[0]               = boostDriftEmitter;
	stateEmitterTime[0]           = 99;
};

datablock ParticleEmitterData(boostLongDriftEmitter : vehicleSplashEmitter)
{
	lifetimeMS = 0;
};

datablock ShapeBaseImageData(boostLongDriftImage : boostDriftImage)
{
	stateEmitter[0]	= boostLongDriftEmitter;
};

// aura particles

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

	colors[0] = "1 1 1 0.15";
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

// boost visuals
datablock ParticleEmitterData (slipstreamBoostTrailEmitter : vehicleBubbleEmitter)
{
	lifetimeMS = 0;
};

datablock ShapeBaseImageData(slipstreamBoostTrailImage : boostDriftImage)
{
	stateEmitter[0]	= slipstreamBoostTrailEmitter;
	offset = "0 1 1.5";
	mountPoint = 8;
};