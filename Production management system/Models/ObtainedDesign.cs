﻿namespace ProductionManagementSystem.Models
{
    public class ObtainedDesign
    {
        public int Id { get; set; }
        public Task Task { get; set; }
        public Design Design { get; set; }
        public int Obtained { get; set; }
    }
}