package AnimatedPropItems
{
    function ItemData::onAdd(%this, %obj)
    {
        %ret = parent::onAdd(%this, %obj);

        if (%obj.getDatablock().passiveThread !$= "")
        {
            %obj.playThread(0, %obj.getDatablock().passiveThread);
        }

        return %ret;
    }

    function ItemData::onPickup(%this, %obj, %user, %amount)
    {
        if (%obj.getDataBlock().isProp)
        {
            return;
        }
        return parent::onPickup(%this, %obj, %user, %amount);
    }
};
activatePackage(AnimatedPropItems);