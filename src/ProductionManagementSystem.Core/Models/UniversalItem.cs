using ProductionManagementSystem.Core.Models.PCB;

namespace ProductionManagementSystem.Core.Models
{
    public class UniversalItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }
        public string Manufacture { get; set; }
        public string Category { get; set; }
        public string PartNumber { get; set; }
        public CDBItemType ItemType { get; set; }
        public object Origin { get; set; }
        
        public UniversalItem() {}

        public UniversalItem(object obj)
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

        public UniversalItem(EntityExt entityExt)
        {
            MapFromEntity(entityExt);
        }
        public UniversalItem(Pcb pcb)
        {
            MapFromPcb(pcb);
        }
        public UniversalItem(CompDbDevice device)
        {
            MapFromDevice(device);
        }
        
        private void MapFromPcb(Pcb pcb)
        {
            Id = pcb.Id;
            ImagePath = pcb.ImagePath;
            Name = pcb.Name;
            Description = pcb.Description;
            Manufacture = "";
            Category = "PCB";
            PartNumber = pcb.Variant;
            ItemType = CDBItemType.PCB;
            Origin = pcb;
        }
        
        private void MapFromDevice(CompDbDevice device)
        {
            Id = device.Id;
            ImagePath = device.ImagePath;
            Name = device.Name;
            Description = device.Description;
            Manufacture = "";
            Category = "Device";
            PartNumber = device.Variant;
            ItemType = CDBItemType.Device;
            Origin = device;
        }

        private void MapFromEntity(EntityExt entityExt)
        {
            Id = entityExt.KeyId;
            ImagePath = entityExt.ImageUrl;
            Name = entityExt.Item;
            Description = entityExt.Description;
            Manufacture = entityExt.Manufacturer;
            Category = entityExt.Category;
            PartNumber = entityExt.PartNumber;
            ItemType = CDBItemType.Device;
            Origin = entityExt;
        }
    }
}