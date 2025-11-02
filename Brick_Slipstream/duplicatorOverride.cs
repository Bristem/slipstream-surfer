// Fix event_zonebricks not being compatible with duplicator
// Adds a zone event check on plant
package slipstreamDupZonePackage
{
    function ND_Selection::plantBrick(%this, %i, %position, %angleID, %brickGroup, %client, %bl_id)
    {
        //Offset position
        %bPos = $NS[%this, "P", %i];

        //Local angle id
        %bAngle = $NS[%this, "R", %i];

        //Apply mirror effects (ugh)
        %datablock = $NS[%this, "D", %i];

        %mirrX = %this.ghostMirrorX;
        %mirrY = %this.ghostMirrorY;
        %mirrZ = %this.ghostMirrorZ;

        if(%mirrX)
        {
            //Mirror offset
            %bPos = -firstWord(%bPos) SPC restWords(%bPos);

            //Handle symmetries
            switch($ND::Symmetry[%datablock])
            {
                //Asymmetric
                case 0:
                    if(%db = $ND::SymmetryXDatablock[%datablock])
                    {
                        %datablock = %db;
                        %bAngle = (%bAngle + $ND::SymmetryXOffset[%datablock]) % 4;

                        //Pair is made on X, so apply mirror logic for X afterwards
                        if(%bAngle % 2 == 1)
                            %bAngle = (%bAngle + 2) % 4;
                    }
                    else
                    {
                        //Add datablock to list of mirror problems
                        if(!$NS[%client, "MXK", %datablock])
                        {
                            %id = $NS[%client, "MXC"];
                            $NS[%client, "MXC"]++;

                            $NS[%client, "MXE", %id] = %datablock;
                            $NS[%client, "MXK", %datablock] = true;
                        }
                    }

                //Do nothing for fully symmetric

                //X symmetric - rotate 180 degrees if brick is angled 90 or 270 degrees
                case 2:
                    if(%bAngle % 2 == 1)
                        %bAngle = (%bAngle + 2) % 4;

                //Y symmetric - rotate 180 degrees if brick is angled 0 or 180 degrees
                case 3:
                    if(%bAngle % 2 == 0)
                        %bAngle = (%bAngle + 2) % 4;

                //X+Y symmetric - rotate 90 degrees
                case 4:
                    if(%bAngle % 2 == 0)
                        %bAngle = (%bAngle + 1) % 4;
                    else
                        %bAngle = (%bAngle + 3) % 4;

                //X-Y symmetric - rotate -90 degrees
                case 5:
                    if(%bAngle % 2 == 0)
                        %bAngle = (%bAngle + 3) % 4;
                    else
                        %bAngle = (%bAngle + 1) % 4;
            }
        }
        else if(%mirrY)
        {
            //Mirror offset
            %bPos = getWord(%bPos, 0) SPC -getWord(%bPos, 1) SPC getWord(%bPos, 2);

            //Handle symmetries
            switch($ND::Symmetry[%datablock])
            {
                //Asymmetric
                case 0:
                    if(%db = $ND::SymmetryXDatablock[%datablock])
                    {
                        %datablock = %db;
                        %bAngle = (%bAngle + $ND::SymmetryXOffset[%datablock]) % 4;

                        //Pair is made on X, so apply mirror logic for X afterwards
                        if(%bAngle % 2 == 0)
                            %bAngle = (%bAngle + 2) % 4;
                    }
                    else
                    {
                        //Add datablock to list of mirror problems
                        if(!$NS[%client, "MXK", %datablock])
                        {
                            %id = $NS[%client, "MXC"];
                            $NS[%client, "MXC"]++;

                            $NS[%client, "MXE", %id]= %datablock;
                            $NS[%client, "MXK", %datablock] = true;
                        }
                    }

                //Do nothing for fully symmetric

                //X symmetric - rotate 180 degrees if brick is angled 90 or 270 degrees
                case 2:
                    if(%bAngle % 2 == 0)
                        %bAngle = (%bAngle + 2) % 4;

                //Y symmetric - rotate 180 degrees if brick is angled 0 or 180 degrees
                case 3:
                    if(%bAngle % 2 == 1)
                        %bAngle = (%bAngle + 2) % 4;

                //X+Y symmetric - rotate 90 degrees
                case 4:
                    if(%bAngle % 2 == 1)
                        %bAngle = (%bAngle + 1) % 4;
                    else
                        %bAngle = (%bAngle + 3) % 4;

                //X-Y symmetric - rotate -90 degrees
                case 5:
                    if(%bAngle % 2 == 1)
                        %bAngle = (%bAngle + 3) % 4;
                    else
                        %bAngle = (%bAngle + 1) % 4;
            }
        }

        if(%mirrZ)
        {
            //Mirror offset
            %bPos = getWords(%bPos, 0, 1) SPC -getWord(%bPos, 2);

            //Change datablock if asymmetric
            if(!$ND::SymmetryZ[%datablock])
            {
                if(%db = $ND::SymmetryZDatablock[%datablock])
                {
                    %datablock = %db;
                    %bAngle = (%bAngle + $ND::SymmetryZOffset[%datablock]) % 4;
                }
                else
                {
                    //Add datablock to list of mirror problems
                    if(!$NS[%client, "MZK", %datablock])
                    {
                        %id = $NS[%client, "MZC"];
                        $NS[%client, "MZC"]++;

                        $NS[%client, "MZE", %id]= %datablock;
                        $NS[%client, "MZK", %datablock] = true;
                    }
                }
            }
        }

        //Rotate and add offset
        %bAngle = (%bAngle + %angleID) % 4;
        %bPos = vectorAdd(%position, ndRotateVector(%bPos, %angleID));

        switch(%bAngle)
        {
            case 0: %bRot = "1 0 0 0";
            case 1: %bRot = "0 0 1 90.0002";
            case 2: %bRot = "0 0 1 180";
            case 3: %bRot = "0 0 -1 90.0002";
        }

        //Attempt to plant brick
        %brick = new FxDTSBrick()
        {
            datablock = %datablock;
            isPlanted = true;
            client = %client;

            position = %bPos;
            rotation = %bRot;
            angleID = %bAngle;

            colorID = $NS[%this, "CO", %i];
            colorFxID = $NS[%this, "CF", %i];

            printID = $NS[%this, "PR", %i];
        };

        //This will call ::onLoadPlant instead of ::onPlant
        %prev1 = $Server_LoadFileObj;
        %prev2 = $LastLoadedBrick;
        $Server_LoadFileObj = %brick;
        $LastLoadedBrick = %brick;

        //Add to brickgroup
        %brickGroup.add(%brick);

        //Attempt plant
        %error = %brick.plant();

        //Restore variable
        $Server_LoadFileObj = %prev1;
        $LastLoadedBrick = %prev2;

        if(!isObject(%brick))
            return -1;

        if(%error == 2)
        {
            //Do we plant floating bricks?
            if(%this.forcePlant)
            {
                //Brick is floating. Pretend it is supported by terrain
                %brick.isBaseplate = true;

                //Make engine recompute distance from ground to apply it
                %brick.willCauseChainKill();
            }
            else
            {
                %brick.delete();
                return 0;
            }
        }
        else if(%error)
        {
            %brick.delete();
            return -1;
        }

        //Check for trust
        %downCount = %brick.getNumDownBricks();

        if(!%client.isAdmin || !$Pref::Server::ND::AdminTrustBypass2)
        {
            for(%j = 0; %j < %downCount; %j++)
            {
                if(!ndFastTrustCheck(%brick.getDownBrick(%j), %bl_id, %brickGroup))
                {
                    %brick.delete();
                    return -2;
                }
            }

            %upCount = %brick.getNumUpBricks();

            for(%j = 0; %j < %upCount; %j++)
            {
                if(!ndFastTrustCheck(%brick.getUpBrick(%j), %bl_id, %brickGroup))
                {
                    %brick.delete();
                    return -2;
                }
            }
        }
        else if(!%downCount)
            %upCount = %brick.getNumUpBricks();

        //Finished trust check
        if(%downCount)
            %brick.stackBL_ID = %brick.getDownBrick(0).stackBL_ID;
        else if(%upCount)
            %brick.stackBL_ID = %brick.getUpBrick(0).stackBL_ID;
        else
            %brick.stackBL_ID = %bl_id;

        %brick.trustCheckFinished();

        //Apply special settings
        %brick.setRendering(!$NS[%this, "NR", %i]);
        %brick.setRaycasting(!$NS[%this, "NRC", %i]);
        %brick.setColliding(!$NS[%this, "NC", %i]);
        %brick.setShapeFx($NS[%this, "SF", %i]);

        //Apply events
        if(%numEvents = $NS[%this, "EN", %i])
        {
            %brick.numEvents = %numEvents;
            %brick.implicitCancelEvents = 0;

            for(%j = 0; %j < %numEvents; %j++)
            {
                %brick.eventEnabled[%j] = $NS[%this, "EE", %i, %j];
                %brick.eventDelay[%j] = $NS[%this, "ED", %i, %j];

                %inputIdx = $NS[%this, "EII", %i, %j];

                %brick.eventInput[%j] = $NS[%this, "EI", %i, %j];
                %brick.eventInputIdx[%j] = %inputIdx;

                %target = $NS[%this, "ET", %i, %j];
                %targetIdx = $NS[%this, "ETI", %i, %j];

                if(%targetIdx == -1)
                {
                    %nt = $NS[%this, "ENT", %i, %j];
                    %brick.eventNT[%j] = %nt;
                }

                %brick.eventTarget[%j] = %target;
                %brick.eventTargetIdx[%j] = %targetIdx;

                %output = $NS[%this, "EO", %i, %j];
                %outputIdx = $NS[%this, "EOI", %i, %j];

                //Only rotate outputs for named bricks if they are selected
                if(%targetIdx >= 0 || $NS[%this, "HN", %nt])
                {
                    //Rotate fireRelay events
                    switch$(%output)
                    {
                        case "fireRelayUp":    %dir = 0;
                        case "fireRelayDown":  %dir = 1;
                        case "fireRelayNorth": %dir = 2;
                        case "fireRelayEast":  %dir = 3;
                        case "fireRelaySouth": %dir = 4;
                        case "fireRelayWest":  %dir = 5;
                        default: %dir = -1;
                    }

                    if(%dir >= 0)
                    {
                        %rotated = ndTransformDirection(%dir, %angleID, %mirrX, %mirrY, %mirrZ);
                        %outputIdx += %rotated - %dir;

                        switch(%rotated)
                        {
                            case 0: %output = "fireRelayUp";
                            case 1: %output = "fireRelayDown";
                            case 2: %output = "fireRelayNorth";
                            case 3: %output = "fireRelayEast";
                            case 4: %output = "fireRelaySouth";
                            case 5: %output = "fireRelayWest";
                        }
                    }
                }

                %brick.eventOutput[%j] = %output;
                %brick.eventOutputIdx[%j] = %outputIdx;
                %brick.eventOutputAppendClient[%j] = $NS[%this, "EOC", %i, %j];

                //Why does this need to be so complicated?
                if(%targetIdx >= 0)
                    %targetClass = getWord($InputEvent_TargetListfxDtsBrick_[%inputIdx], %targetIdx * 2 + 1);
                else
                    %targetClass = "FxDTSBrick";

                %paramList = $OutputEvent_ParameterList[%targetClass, %outputIdx];
                %paramCount = getFieldCount(%paramList);

                for(%k = 0; %k < %paramCount; %k++)
                {
                    %param = $NS[%this, "EP", %i, %j, %k];

                    //Only rotate outputs for named bricks if they are selected
                    if(%targetIdx >= 0 || $NS[%this, "HN", %nt])
                    {
                        %paramType = getField(%paramList, %k);

                        switch$(getWord(%paramType, 0))
                        {
                            case "vector":
                                //Apply mirror effects
                                if(%mirrX)
                                    %param = -firstWord(%param) SPC restWords(%param);
                                else if(%mirrY)
                                    %param = getWord(%param, 0) SPC -getWord(%param, 1) SPC getWord(%param, 2);

                                if(%mirrZ)
                                    %param = getWord(%param, 0) SPC getWord(%param, 1) SPC -getWord(%param, 2);

                                %param = ndRotateVector(%param, %angleID);

                            case "list":
                                %value = getWord(%paramType, %param * 2 + 1);

                                switch$(%value)
                                {
                                    case "Up":    %dir = 0;
                                    case "Down":  %dir = 1;
                                    case "North": %dir = 2;
                                    case "East":  %dir = 3;
                                    case "South": %dir = 4;
                                    case "West":  %dir = 5;
                                    default: %dir = -1;
                                }

                                if(%dir >= 0)
                                {
                                    switch(ndTransformDirection(%dir, %angleID, %mirrX, %mirrY, %mirrZ))
                                    {
                                        case 0: %value = "Up";
                                        case 1: %value = "Down";
                                        case 2: %value = "North";
                                        case 3: %value = "East";
                                        case 4: %value = "South";
                                        case 5: %value = "West";
                                    }

                                    for(%l = 1; %l < getWordCount(%paramType); %l += 2)
                                    {
                                        if(getWord(%paramType, %l) $= %value)
                                        {
                                            %param = getWord(%paramType, %l + 1);
                                            break;
                                        }
                                    }
                                }
                        }
                    }

                    %brick.eventOutputParameter[%j, %k + 1] = %param;
                }
            }
        }

        setCurrentQuotaObject(getQuotaObjectFromClient(%client));

        if((%tmp = $NS[%this, "NT", %i]) !$= "")
            %brick.setNTObjectName(%tmp);

        if(%tmp = $NS[%this, "LD", %i])
            %brick.setLight(%tmp, %client);

        if(%tmp = $NS[%this, "ED", %i])
        {
            %dir = ndTransformDirection($NS[%this, "ER", %i], %angleID, %mirrX, %mirrY, %mirrZ);

            %brick.emitterDirection = %dir;
            %brick.setEmitter(%tmp, %client);
        }

        if(%tmp = $NS[%this, "ID", %i])
        {
            %pos = ndTransformDirection($NS[%this, "IP", %i], %angleID, %mirrX, %mirrY, %mirrZ);
            %dir = ndTransformDirection($NS[%this, "IR", %i], %angleID, %mirrX, %mirrY, %mirrZ);

            %brick.itemPosition = %pos;
            %brick.itemDirection = %dir;
            %brick.itemRespawnTime = $NS[%this, "IT", %i];
            %brick.setItem(%tmp, %client);
        }

        if(%tmp = $NS[%this, "VD", %i])
        {
            %brick.reColorVehicle = $NS[%this, "VC", %i];
            %brick.setVehicle(%tmp, %client);
        }

        if(%tmp = $NS[%this, "MD", %i])
            %brick.setSound(%tmp, %client);

        %brick.checkForZoneEvents();

        return %brick;
    }
};
if(ForceRequiredAddOn("Tool_NewDuplicator") == $Error::AddOn_NotFound)
{
   error("No duplicator addon to override!");
}
else
{
    activatePackage(slipstreamDupZonePackage);
}