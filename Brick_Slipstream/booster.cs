// Sound assets, placeholder model, and a lot of functionality ripped from Ottosparks and Armageddon's platformer bricks

//DATABLOCKS
datablock AudioProfile(BoosterSound)
{
	description = "AudioClosest3D";
	fileName = "Add-Ons/Server_Slipstream/assets/sounds/booster.wav";
	preload = true;
};

datablock fxDTSBrickData(BrickBoosterData)
{
	brickFile = "Add-Ons/Server_Slipstream/assets/bricks/booster.blb";
	iconName = "Add-Ons/Server_Slipstream/assets/bricks/icon_booster";

	category = "Slipstream";
	subCategory = "Dash Panels";
	uiName = "Dash Panel";

	orientationFix = 3;

	isBooster = 1;
	boosterPower = 60; 
};


//CLASS FUNCTIONS
function fxDTSBrick::Booster(%this, %player, %power, %do) //Makes boosters boost. Gotta go fast.
{
	if(isEventPending(%player.booster))
		cancel(%player.booster);

	if(!isObject(%player))
		return;

	%db = %this.getDatablock();
	if(%do)
	{
		if(%power > 0)
			%vel = %power;
		else
			%vel = %db.boosterPower;
		
		// TODO add check for angle being close enough to booster's orientation, probably same as airdash
		if(%player.getSpeedInBPS() / 2 > %vel)
		{
			%vel = %player.getSpeedInBPS() / 2;
		}

		%ang = %this.getAngleID() + 1;
		if(%ang > 3)
			%ang -= 4;

		%vel = %vel SPC "0 0";
		%vel = rotateVector(%vel, "0 0 0", %ang);

		%player.setVelocity(%vel);
		// ServerPlay3D(BoosterSound, %this.getPosition());
		%player.stopAudio(3);
		%player.playAudio(3, BoosterSound);
		%player.lastBoost = getSimTime();
	}
}

function fxDTSBrick::doBooster(%this, %power, %client) //The event that allows for custom power. Gotta go faster.
{
	if(!isObject(%client.player))
		return;

	%this.Booster(%client.player, %power, true);
}

function fxDTSBrick::onBooster(%this, %player)
{
	$InputTarget_["Self"] = %this;
	$InputTarget_["Player"] = %player;
	$InputTarget_["Client"] = %player.client;

	%clientMini = getMinigameFromObject(%player.client);
	%selfMini = getMinigameFromObject(%this);
	if($Server::Lan)
		$InputTarget_["MiniGame"] = %clientMini;
	else
	{
		if(%clientMini == %selfMini)
			$InputTarget["MiniGame"] = %selfMini;
		else
			$InputTarget["MiniGame"] = 0;
	}
	%this.processInputEvent("onBooster", %player.client);
}

//PACKAGE

package slipstreamBoosterPlantPackage
{
	function fxDTSBrick::onPlant(%obj)
	{
		%data = %obj.getDataBlock();
		
		//apply events
		if(%data.isBooster)
		{
			%obj.eventDelay0 = 0;
			%obj.eventEnabled0 = 1;
			%obj.eventInput0 = "onPlayerEnterBrick";
			%obj.eventInputIdx0 = inputEvent_GetInputEventIdx("onPlayerEnterBrick");
			%obj.eventOutput0 = "doBooster";
			%obj.eventOutputAppendClient0 = 1;
			%obj.eventOutputIdx0 = outputEvent_GetOutputEventIdx("fxDTSBrick","doBooster");//37;
			%obj.eventOutputParameter0_1 = 60;
			%obj.eventTarget0 = "Self";
			%obj.eventTargetIdx0 = 0;
			%obj.numEvents = 1;
			%obj.setColliding(0);
		}
		parent::onPlant( %obj );
		// %obj.setItem("HammerItem");
	}
};
activatePackage(slipstreamBoosterPlantPackage);

//EVENT REGISTRATION
registerInputEvent(fxDTSBrick, "onBooster", "Self fxDTSBrick\tPlayer Player\tClient GameConnection\tMiniGame MiniGame");
registerOutputEvent(fxDTSBrick, "doBooster", "int 0 200 0", true);

//DEPENDENCY
function rotateVector(%vector, %axis, %val) //Rotates a vector around the axis by an angleID.
{
	if(%val < 0)
		%val += 4;
	if(%val > 3)
		%val -= 4;
	switch(%val)
	{
		case 1:
			%nX = getWord(%axis, 0) + (getWord(%vector, 1) - getWord(%axis, 1));
			%nY = getWord(%axis, 1) - (getWord(%vector, 0) - getWord(%axis, 0));
			%new = %nX SPC %nY SPC getWord(%vector, 2);
		case 2:
			%nX = getWord(%axis, 0) - (getWord(%vector, 0) - getWord(%axis, 0));
			%nY = getWord(%axis, 1) - (getWord(%vector, 1) - getWord(%axis, 1));
			%new = %nX SPC %nY SPC getWord(%vector, 2);
		case 3:
			%nX = getWord(%axis, 0) - (getWord(%vector, 1) - getWord(%axis, 1));
			%nY = getWord(%axis, 1) + (getWord(%vector, 0) - getWord(%axis, 0));
			%new = %nx SPC %nY SPC getWord(%vector, 2);
		default: %new = vectorAdd(%vector, %axis);
	}
	return %new;
}