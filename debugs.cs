function listAvailableProjectiles() {
  for (%i = 0; %i < DatablockGroup.getCount(); %i++) {
    %db = DatablockGroup.getObject(%i);
    if (%db.getClassName() $= "ProjectileData") {
      echo("Projectile: " @ %db.getName());
    }
  }
}

function testBoostBroomExplode(%obj) {
   %p = new Projectile() {
      dataBlock = boostBroomProjectile;
      initialPosition = %obj.getPosition();
      initialVelocity = "0 0 -1";
   };
   %p.explode();
   announce("Exploded boostBroomProjectile");
}

function listServerEmitters()
{
   echo("---- Particle Emitters ----");

   for (%i = 0; %i < DatablockGroup.getCount(); %i++)
   {
      %db = DatablockGroup.getObject(%i);
      if (%db.getClassName() $= "ParticleEmitterData")
      {
         echo("Emitter: " @ %db.getName());

         // Optional: print associated particles
         for (%p = 0; %p < %db.particleCount; %p++)
         {
            %particle = %db.particle[%p];
            if (isObject(%particle))
               echo("  - Particle[" @ %p @ "]: " @ %particle.getName());
         }
      }
   }

   echo("----------------------------");
}