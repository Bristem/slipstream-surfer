if(!isFunction("PathCamera", "onDriverLeave"))
	eval("function PathCamera::onDriverLeave(){}"); //console error fix

datablock PathCameraData(KQ_PathCamera : Observer)
{
	maxNodes = 20; //path cameras only work with up to 20 nodes

	defaultPath = "Spline";
	defaultType = "Normal";
	defaultSpeed = 7;
};

$s0 = 50;
$s1 = $s0 / 2;
for(%I = 0; %I < 80; %I++)
{
	$KQ::Node[%I] = -441 + getRandom()*$s0-$s1 SPC 270 + getRandom()*$s0-$s1 SPC getRandom()*$s1;
}

$KQ::NodeCount = 80;

function mFloatModulo(%num,%mod)
{
	return %num - mFloor(%num/%mod)*%mod;
}

//$sshape1 = staticShape_simple($sshape1, $KQ::Node[0], "2 2 1", "0 0 1 1");
//$sshape = staticShape_simple($sshape, $KQ::Node[1], "2 2 1", "1 0 0 1");

function KQ_PathCamera::onNode(%this, %camera, %node)
{
	//bottomprintall(getSimTime() NL %camera.curNode @" >= "@ $KQ::NodeCount, 1, 1);
	%type = "Normal";
	%path = "Spline";

	%curNode = $KQ::Node[%camera.curNode] SPC eulerToAxis($nextaxis SPC $nextaxis SPC $nextaxis);

	%camera.curNode = (%camera.curNode + 1) % $KQ::NodeCount;

	%nextNode = $KQ::Node[%camera.curNode];

	%defaultSpeed = vectorDist(%nextNode, %curNode);

	%nextNode = %nextNode SPC eulerToAxis($nextaxis SPC $nextaxis SPC $nextaxis);
	$nextaxis = ($nextaxis + %defaultSpeed * 3) % 360;

    %camera.popFront();
	//%camera.reset(1);
    %cam.nodeCount++;

	if(isObject($playerr))
		%nextNode = $playerr.getTransform();
	%defaultSpeed = vectorDist(%nextNode, %curNode);

	$nodeline = staticLine_simple($nodeline, %curNode, %nextNode, 0.5, "1 1 1 0.5");

	%camera.pushBack(%nextNode, %defaultSpeed, %type, %path);

	%camera.setPosition(1);
}

function KQ_PathCamera::onNode(%this, %camera, %node)
{
	//%player = %camera.client.player;
	%player = fpn($player);
	if(!isObject(%player))
		return;

	//%footCross = vectorCross("0 0" SPC 5 * firstWord(%player.getScale()), %player.getForwardVector());
	//%nextNode = vectorAdd(%player.getEyePoint(), %footCross);
	//%nextNode = vectorAdd(%nextNode, "0 0 1") SPC getWords(%player.getEyeTransform(), 3, 6);

	%nextNode = %player.getHackPosition();
	%angle = %camera.angle = mFloatModulo(%camera.angle + 0.2, $pi * 10);

	%rotation = vectorScale(mCos(%angle) SPC mSin(%angle) SPC 0, 5);
	%nextNode = vectorAdd(%nextNode, %rotation);

	%type = "Normal";
	%path = "Spline";
	%defaultSpeed = 7 + vectorLen(%player.getVelocity());
	%defaultSpeed = getMax(vectorDist(%nextNode, %camera.getPosition()), %defaultSpeed);

	%nextNode = %nextNode SPC eulerToAxis($nextaxis SPC $nextaxis SPC $nextaxis);
	$nextaxis = ($nextaxis + %defaultSpeed * 3) % 360;

	%camera.popFront();
	%camera.pushBack(%nextNode, %defaultSpeed, %type, %path);

	%camera.setPosition(1);
}

function yes(%name)
{
	%cl = findclientbyname(%name);
	test(%cl);

    %cam = %cl.pathCam;

	%cam.schedule(1000, setState, "forward");
    %cam.schedule(1000, mountobject, $horse, 0);

    //$horse.setPlayerScale($horseScale); //vectorScale("1 1 1", 1 / $pathCamScale));
    //%cam.setScale($pathCamScale SPC $pathCamScale SPC $pathCamScale);
}

function test(%this)
{
	%datablock = KQ_PathCamera;
	if(isObject(%this.pathCam))
		%this.pathCam.delete();

	%cam = %this.pathCam = new PathCamera(mypathcam)
	{
		dataBlock = %datablock;
		client = %this;
		//scale = "1 1 1";
		curNode = 0;

		numNodes = 0;
	};
	missionCleanup.add(%cam);

	%defaultSpeed = 14;
	%type = "Normal";
	%path = "Spline";


	// APPLY SPLINE
	// PREVIOUS NODE
	%transform = $KQ::Node[$KQ::NodeCount - 1];
	%speed = %defaultSpeed;
	%cam.pushBack(%transform, %speed, %type, %path);
	%cam.numNodes++;

	//START NODE
	%transform = $KQ::Node[0];
	%speed = %defaultSpeed;
	%cam.pushBack(%transform, %speed, %type, %path);
	%cam.numNodes++;

	//NEXT NODE
	%transform = $KQ::Node[1];
	%speed = %defaultSpeed;
	%cam.pushBack(%transform, %speed, %type, %path);
	%cam.numNodes++;

	%cam.curNode = 0;
	%cam.setState("stop");
	%cam.setPosition(1);
	%cam.popFront();
	%cam.numNodes--;

	%cg = nameToID("clientGroup");
	%cgC = %cg.getCount();

	for(%I = 0; %I < %cgC; %I++)
		%cam.scopeToClient(%cg.getObject(%I));

	%cam.setScale("1 1 1");
	//%cam.schedule(100, )
	//$sshape = staticShape_simple($sshape, $KQ::Node[0], "2 2 1", "0.2 0.2 0.2 1");

	missionGroup.add(%cam);
	$horse.PC_fixRenderTransform();
}

function ShapeBase::PC_fixRenderTransform(%this)
{
	cancel(%this.fixRenderTransformSchedule);

	//yep
	%this.setTransform(getWords(%this.getTransform(), 0, 2) SPC "0 0 1 0");

	//if(!isObject(%this.getObjectMount()))
	//	return;

	//%this.getObjectMount().mountObject(%this, %slot);

	%this.fixRenderTransformSchedule = %this.schedule(32, PC_fixRenderTransform);
}
