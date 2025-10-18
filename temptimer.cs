BrickTreasureChestData.isSurfTrack = 1;
BrickTreasureChestOpenData.isSurfTrack = 1;

package TempSlipstreamTimerPackage
{
   function FxDTSBrick::onPlayerTouch(%this, %obj)
   {
      if (%obj.isSurfing && %this.getDataBlock().isSurfTrack) 
      {
			%cl = %obj.client;
         %obj.isSurfing = 0;
			%time = getSimTime() - %obj.surfStartTime;

			%name = %cl.getPlayerName();
			%formatted = getTimeString(mFloatLength(%time / 1000, 2));

			%message = "<bitmap:base/client/ui/ci/star> \c3" @ %name;
			%message = %message SPC "\c6completed the track in \c3" @ %formatted @ "\c6.";

			messageAll('', %message);
		}

		Parent::onPlayerTouch(%this, %obj);
	}
};

activatePackage("TempSlipstreamTimerPackage");