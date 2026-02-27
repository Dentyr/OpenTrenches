using System;
using System.Collections.Generic;
using OpenTrenches.Common.Combat;
using OpenTrenches.Common.Contracts;
using OpenTrenches.Common.Contracts.DTO;
using OpenTrenches.Common.Contracts.DTO.UpdateModel;
using OpenTrenches.Server.Scripting.Adapter;

namespace OpenTrenches.Core.Scripting.Combat;

public interface IReadOnlyFirearmState
{
    int AmmoLoaded { get;  }
    int AmmoStored { get;  }
}

public class FirearmState : IReadOnlyFirearmState
{
    public int AmmoLoaded { get; set; }

    public int AmmoStored { get; set; }
    
    public float Recoil { get; set; }



    //* Updates

    public void Update(FirearmSlotUpdateDTO update)
    {
        switch (update.Attribute)
        {
            case FirearmSlotAttribute.AmmoLoaded:
                AmmoLoaded = Serialization.Deserialize<int>(update.Payload);
                break;
            case FirearmSlotAttribute.AmmoStored:
                AmmoStored = Serialization.Deserialize<int>(update.Payload);
                break;
            case FirearmSlotAttribute.Recoil:
                Recoil = Serialization.Deserialize<float>(update.Payload);
                break;
        }
    }
}