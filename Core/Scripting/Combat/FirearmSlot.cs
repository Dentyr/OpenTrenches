using System;
using System.Collections.Generic;
using OpenTrenches.Common.Combat;
using OpenTrenches.Common.Contracts;
using OpenTrenches.Common.Contracts.DTO;
using OpenTrenches.Common.Contracts.DTO.UpdateModel;
using OpenTrenches.Server.Scripting.Adapter;

namespace OpenTrenches.Core.Scripting.Combat;

public class FirearmSlot : EquipmentSlot<FirearmStats>
{
    public float Reload { get; set; }

    public float Cooldown { get; set; }

    public float AmmoLoaded { get; set; }

    public float AmmoStored { get; set; }


    public FirearmSlot(EquipmentCategory category, EquipmentType<FirearmStats> equipment) : base(category, equipment)
    {
    }

    //* Updates

    public void Update(FirearmSlotUpdateDTO update)
    {
        switch (update.Attribute)
        {
            case FirearmSlotAttribute.Reload:
                Reload = Serialization.Deserialize<float>(update.Payload);
                break;
            case FirearmSlotAttribute.Cooldown:
                Cooldown = Serialization.Deserialize<float>(update.Payload);
                break;
            case FirearmSlotAttribute.AmmoLoaded:
                AmmoLoaded = Serialization.Deserialize<float>(update.Payload);
                break;
            case FirearmSlotAttribute.AmmoStored:
                AmmoStored = Serialization.Deserialize<float>(update.Payload);
                break;
        }
    }
}