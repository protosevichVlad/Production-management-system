using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ProductionManagementSystem.Core.Models.Components;

namespace ProductionManagementSystem.Core.Models.Devices
{
    public abstract class ComponentBaseInDevice : BaseEntity
    {
        public int Quantity { get; set; }
        public int DeviceId { get; set; }
        public string Description { get; set; }
        public int ComponentId { get; set; }
        
    }
    
    public class ComponentBaseInDeviceEqualityComparer : IEqualityComparer<ComponentBaseInDevice>
    {
        public bool Equals(ComponentBaseInDevice x, ComponentBaseInDevice y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.DeviceId == y.DeviceId && x.ComponentId == y.ComponentId;
        }

        public int GetHashCode(ComponentBaseInDevice obj)
        {
            return HashCode.Combine(obj.DeviceId, obj.ComponentId);
        }
    }
}