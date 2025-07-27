//Support_AltDatablock.cs
//Add and remove "alternate" datablocks from players for special abilities

function Player::pushDatablock(%this,%data)
{
	%data = %data.getID();
	
	if(%this.getState() $= "Dead")
		return;
	
	if(fileName(%this.dataBlock.shapeFile) !$= fileName(%data.shapeFile))
		return;
	
	if(%this.altDataID[%data.getID()] !$= "")
		return;
	
	if(%this.altDataNum == 0 || %this.altDataNum == 1)
	{
		%this.altDataNum = 1;
		%this.altData[0] = %this.dataBlock.getID();
		%this.altDataID[%this.dataBlock.getID()] = 0;
	}
	
	%this.altData[%this.altDataNum] = %data.getID();
	%this.altDataID[%data.getID()] = %this.altDataNum;
	%this.altDataNum++;
	
	
	%health = %this.dataBlock.maxDamage - %this.getDamageLevel();
	%flash = %this.getDamageFlash();
	%white = %this.getWhiteOut();
	
	%this.isChangingAltData = 1;
	
	%this.setDatablock(%data);
	%this.setHealth(%health);
	%this.setDamageFlash(%flash);
	%this.setWhiteOut(%white);
	
	%this.isChangingAltData = "";
}

function Player::popDatablock(%this,%data)
{
	if(%this.getState() $= "Dead")
		return;
	
	if(%data $= "")
	{
		%this.altDataNum--;
		
		%health = %this.dataBlock.maxDamage - %this.getDamageLevel();
		%flash = %this.getDamageFlash();
		%white = %this.getWhiteOut();
		
		%this.isChangingAltData = 1;
		
		%this.setDatablock(%this.altData[%this.altDataNum-1]);
		%this.setHealth(%health);
		%this.setDamageFlash(%flash);
		%this.setWhiteOut(%white);
		
		%this.isChangingAltData = "";
		
		%this.altDataID[%this.altData[%this.altDataNum]] = "";
		%this.altData[%this.altDataNum] = "";
	}
	else
	{
		%data = %data.getID();
		
		if(!isObject(%data) || %data.getClassName() !$= "PlayerData")
			return;
		
		if(%this.altDataID[%data] == 0)
			return;
		
		%id = %this.altDataID[%data.getID()];
		
		for(%i=%id;%i<%this.altDataNum;%i++)
		{
			%this.altDataID[%this.altData[%i]]--;
			%this.altData[%i] = %this.altData[%i+1];
		}
		
		%this.altDataID[%data] = "";
		%this.altDataNum--;
		
		if(%this.dataBlock.getID() == %data)
		{
			%health = %this.dataBlock.maxDamage - %this.getDamageLevel();
			%flash = %this.getDamageFlash();
			%white = %this.getWhiteOut();
			
			%this.isChangingAltData = 1;
			
			%this.setDatablock(%this.altData[%this.altDataNum-1]);
			%this.setHealth(%health);
			%this.setDamageFlash(%flash);
			%this.setWhiteOut(%white);
			
			%this.isChangingAltData = "";
		}
	}
}

function Player::getFirstDatablock(%this)
{
	if(%this.altDataNum == 0)
		return %this.dataBlock;
	else
		return %this.altData[0];
}

function Player::resetDatablock(%this)
{
	if(%this.getState() $= "Dead")
		return;
	
	%data = %this.getFirstDatablock();
	if(%data == %this.dataBlock)
		return;
	
	%health = %this.dataBlock.maxDamage - %this.getDamageLevel();
	%flash = %this.getDamageFlash();
	%white = %this.getWhiteOut();
	
	%this.isChangingAltData = 1;
	
	%this.setDatablock(%data);
	%this.setHealth(%health);
	%this.setDamageFlash(%flash);
	%this.setWhiteOut(%white);
	
	%this.isChangingAltData = "";
	
	for(%i=0;%i<%this.altDataNum;%i++)
	{
		%this.altDataID[%this.altData[%i]] = "";
		%this.altData[%i] = "";
	}
	
	%this.altDataNum = 0;
}

package AltDatablocks
{
	function Player::playPain(%this)
	{
		if(%this.isChangingAltData)
			return;
		
		Parent::playPain(%this);
	}
	
	function ShapeBase::setDatablock(%this,%data)
	{
		if(%this.getType() & $TypeMasks::PlayerObjectType && %this.altDataNum > 1 && !%this.isChangingAltData)
		{
			%this.altDataID[%this.dataBlock.getID()] = "";
			%this.altDataID[%data.getID()] = 0;
			%this.altData[0] = %data;
			return;
		}
		
		Parent::setDatablock(%this,%data);
	}
};activatePackage(AltDatablocks);