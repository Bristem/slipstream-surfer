//DATABLOCKS
// datablock AudioProfile(SpringSound)
// {
// 	description = "AudioClosest3D";
// 	fileName = "Add-Ons/Server_Slipstream/assets/sounds/booster.wav";
// 	preload = true;
// };

datablock fxDTSBrickData(BrickSpringVerticalData)
{
	brickFile = "Add-Ons/Server_Slipstream/assets/bricks/springVertical.blb";
	// iconName = "Add-Ons/Server_Slipstream/assets/bricks/icon_booster";

	category = "Slipstream";
	subCategory = "Springs";
	uiName = "Spring Vertical";

	springType = "Vertical";
};

//PACKAGE
// TODO just put stuff under one function, events unreliable for teleport for som reason
// add sound
package slipstreamSpringVerticalPlantPackage
{
	function fxDTSBrick::onPlant(%obj)
	{
		%data = %obj.getDataBlock();
		%isVertSpring = %data.springType $= "Vertical";
		if(%isVertSpring)
		{
			%obj.eventDelay0 = 0;
			%obj.eventEnabled0 = 1;
			%obj.eventInput0 = "onPlayerEnterBrick";
			%obj.eventInputIdx0 = inputEvent_GetInputEventIdx("onPlayerEnterBrick");
			%obj.eventOutput0 = "doPlayerTeleport";
			%obj.eventOutputAppendClient0 = 1;
			%obj.eventOutputIdx0 = outputEvent_GetOutputEventIdx("fxDTSBrick","doPlayerTeleport");
			%obj.eventOutputParameter0_1 = "SELF";
			%obj.eventOutputParameter0_2 = 0;
			%obj.eventOutputParameter0_3 = 0;
			%obj.eventOutputParameter0_4 = 0;
			%obj.eventTargetIdx0 = 0;

			%obj.eventDelay1 = 1;
			%obj.eventEnabled1 = 1;
			%obj.eventInput1 = "onPlayerEnterBrick";
			%obj.eventInputIdx1 = inputEvent_GetInputEventIdx("onPlayerEnterBrick");
			%obj.eventOutput1 = "setVelocity";
			%obj.eventOutputAppendClient1 = 0;
			%obj.eventOutputIdx1 = outputEvent_GetOutputEventIdx("Player","setVelocity");
			%obj.eventOutputParameter1_1 = "0 0 40";
			%obj.eventTarget1 = "Player";
			%obj.eventTargetIdx1 = 1;

			%obj.numEvents = 2;
			%obj.setColliding(0);
		}
		parent::onPlant( %obj );
		if(%isVertSpring)
		{
			%obj.setColorFx(4); // < some other fx maybe
		}
	}
};
activatePackage(slipstreamSpringVerticalPlantPackage);