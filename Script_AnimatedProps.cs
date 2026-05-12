package AnimatedPropItems
{
    function ItemData::onAdd(%this, %obj)
    {
        %ret = parent::onAdd(%this, %obj);

        if (%obj.getDatablock().isProp)
        {
            %obj.canPickup = false;
        }
        if (%obj.getDatablock().passiveThread !$= "")
        {
            %obj.playThread(0, %obj.getDatablock().passiveThread);
        }

        return %ret;
    }
};
activatePackage(AnimatedPropItems);