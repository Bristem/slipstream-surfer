datablock PlayerData(PlayerSurfArmor : PlayerStandardArmor) {
	uiName = "Surf Player";
	isSurfPlayer = 1;

	canJet = false;

	mass = 120;
	boundingBox = "5 5 10.6";

	airControl = 0.5;
	runSurfaceAngle = 20;

	maxForwardSpeed = 15;
	maxBackwardSpeed = 15;
	maxSideSpeed = 200;
};