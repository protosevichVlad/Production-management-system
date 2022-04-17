using ProductionManagementSystem.Core.Models;
using ProductionManagementSystem.Core.Models.PCB;

namespace ProductionManagementSystem.WEB.Models.Device
{
    public class CreateDeviceSearchViewModel
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Manufacture { get; set; }
        public string Category { get; set; }
        public string PartNumber { get; set; }
        public string Supplier { get; set; }
        public string Case { get; set; }
        public UsedInDeviceComponentType ComponentType { get; set; }
        
        public CreateDeviceSearchViewModel() {}

        public CreateDeviceSearchViewModel(object obj)
        {
            if (obj is EntityExt entityExt)
            {
                MapFromEntity(entityExt);
            } 
            else if (obj is Pcb pcb)
            {
                MapFromPcb(pcb);
            }
            else if (obj is CompDbDevice device)
            {
                MapFromDevice(device);
            }
        }

        public CreateDeviceSearchViewModel(EntityExt entityExt)
        {
            MapFromEntity(entityExt);
        }
        public CreateDeviceSearchViewModel(Pcb pcb)
        {
            MapFromPcb(pcb);
        }
        public CreateDeviceSearchViewModel(CompDbDevice device)
        {
            MapFromDevice(device);
        }
        
        private void MapFromPcb(Pcb pcb)
        {
            Id = pcb.Id;
            ImageUrl = pcb.ImagePath;
            Name = pcb.Name;
            Description = pcb.Description;
            Manufacture = "";
            Category = "PCB";
            PartNumber = pcb.Variant;
            ComponentType = UsedInDeviceComponentType.PCB;
        }
        
        private void MapFromDevice(CompDbDevice device)
        {
            Id = device.Id;
            ImageUrl = device.ImagePath;
            Name = device.Name;
            Description = device.Description;
            Manufacture = "";
            Category = "Device";
            PartNumber = device.Variant;
            ComponentType = UsedInDeviceComponentType.Device;
        }

        private void MapFromEntity(EntityExt entityExt)
        {
            Id = entityExt.KeyId;
            ImageUrl = entityExt.ImageUrl;
            Name = entityExt.Item;
            Description = entityExt.Description;
            Manufacture = entityExt.Manufacturer;
            Category = entityExt.Category;
            PartNumber = entityExt.PartNumber;
            Case = entityExt.Case;
            Supplier = entityExt.Supplier;
            ComponentType = UsedInDeviceComponentType.Entity;
        }
    }
}