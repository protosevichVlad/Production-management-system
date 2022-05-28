﻿using System;
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
        public string Supplier { get; set; }
        public string Case { get; set; }
        public int Quantity { get; set; }
        public string Path { get; set; }

        public UniversalItem() {}

        public UniversalItem(object obj)
        {
            if (obj == null) return;
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

        public UniversalItem(EntityExt entityExt) : this((object)entityExt)
        {
        }
        public UniversalItem(Pcb pcb) : this((object)pcb)
        {
        }
        public UniversalItem(CompDbDevice device) : this((object)device)
        {
        }
        
        private void MapFromPcb(Pcb pcb)
        {
            Id = pcb.Id;
            ImagePath = pcb.ImagePath;
            Name = pcb.Name;
            Quantity = pcb.Quantity;
            Description = pcb.Description;
            Manufacture = "";
            Category = "PCB";
            PartNumber = pcb.Variant;
            ItemType = CDBItemType.PCB;
            Case = "";
            Supplier = "";
            Path = "/pcb/details/" + pcb.Id;
            Origin = pcb;
        }
        
        private void MapFromDevice(CompDbDevice device)
        {
            Id = device.Id;
            ImagePath = device.ImagePath;
            Name = device.Name;
            Quantity = device.Quantity;
            Description = device.Description;
            Manufacture = "";
            Category = "Device";
            PartNumber = device.Variant;
            ItemType = CDBItemType.Device;
            Case = "";
            Supplier = "";
            Path = "/CompDbDevices/details/" + device.Id;
            Origin = device;
        }

        private void MapFromEntity(EntityExt entityExt)
        {
            Id = entityExt.KeyId;
            ImagePath = entityExt.ImageUrl;
            Name = entityExt.Item;
            Quantity = entityExt.Quantity;
            Description = entityExt.Description;
            Manufacture = entityExt.Manufacture;
            Category = entityExt.Category;
            PartNumber = entityExt.PartNumber;
            ItemType = CDBItemType.Entity;
            Case = entityExt.Case;
            Supplier = entityExt.Supplier;
            Path = "/entities/details/" + entityExt.KeyId;
            Origin = entityExt;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as UniversalItem);
        }

        public bool Equals(UniversalItem other)
        {
            return Id == other?.Id && ItemType == other?.ItemType;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() ^ ItemType.GetHashCode();
        }

        public override string ToString()
        {
            return Category + " " + Name + " " + PartNumber;
        }
    }
}