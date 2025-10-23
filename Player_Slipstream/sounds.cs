datablock AudioProfile(slipstreamDriftingSound)
{
   filename    = "Add-Ons/Server_Slipstream/assets/sounds/drifting.wav";
   description = AudioCloseLooping3d;
   preload = true;
};

datablock AudioProfile(slipstreamSlingReadySound)
{
   filename    = "Add-Ons/Server_Slipstream/assets/sounds/driftready.wav";
   description = AudioClose3d;
   preload = true;
};

datablock AudioProfile(slipstreamSlingshotSound)
{
   filename    = "Add-Ons/Server_Slipstream/assets/sounds/slingshotBoost.wav";
   description = AudioDefault3d;
   preload = true;
};

datablock AudioProfile(slipstreamSlingshotMiniSound : slipstreamSlingshotSound)
{
   filename    = "Add-Ons/Server_Slipstream/assets/sounds/slingshotBoostMini.wav";
};

datablock AudioProfile(slipstreamAirdashSound : slipstreamSlingshotSound)
{
   filename    = "Add-Ons/Server_Slipstream/assets/sounds/airdash.wav";
};
