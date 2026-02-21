using System;
using System.Collections.Generic;
using OpenTrenches.Common.Combat;
using OpenTrenches.Common.Contracts;
using OpenTrenches.Common.Contracts.DTO;
using OpenTrenches.Common.Contracts.DTO.UpdateModel;
using OpenTrenches.Server.Scripting.Adapter;

namespace OpenTrenches.Core.Scripting.Combat;

public class FirearmSlot : EquipmentSlot<FirearmStats>, IReadOnlyFirearmSlot
{
    public float ReloadCooldown { get; set; }

    public float FireCooldown { get; set; }

    public int AmmoLoaded { get; set; }

    public int AmmoStored { get; set; }


    public FirearmSlot(EquipmentType<FirearmStats>? equipment) : base(EquipmentCategory.Firearm, equipment)
    {
    }

    //* Updates

    public void Update(FirearmSlotUpdateDTO update)
    {
        switch (update.Attribute)
        {
            case FirearmSlotAttribute.Reload:
                ReloadCooldown = Serialization.Deserialize<float>(update.Payload);
                break;
            case FirearmSlotAttribute.Cooldown:
                FireCooldown = Serialization.Deserialize<float>(update.Payload);
                break;
            case FirearmSlotAttribute.AmmoLoaded:
                AmmoLoaded = Serialization.Deserialize<int>(update.Payload);
                break;
            case FirearmSlotAttribute.AmmoStored:
                AmmoStored = Serialization.Deserialize<int>(update.Payload);
                break;
        }
    }
}