datablock AudioProfile(slipstreamDriftingSound)
{
   filename    = "./drifting.wav";
   description = AudioCloseLooping3d;
   preload = true;
};

datablock AudioProfile(slipstreamSlingReadySound)
{
   filename    = "./driftready.wav";
   description = AudioClose3d;
   preload = true;
};

datablock AudioProfile(slipstreamSlingshotSound)
{
   filename    = "./slingshotBoost.wav";
   description = AudioDefault3d;
   preload = true;
};

datablock AudioProfile(slipstreamSlingshotMiniSound : slipstreamSlingshotSound)
{
   filename    = "./slingshotBoostMini.wav";
};

datablock AudioProfile(slipstreamAirdashSound : slipstreamSlingshotSound)
{
   filename    = "./airdash.wav";
};
