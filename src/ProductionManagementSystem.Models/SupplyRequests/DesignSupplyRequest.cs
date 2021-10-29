﻿using System.ComponentModel.DataAnnotations.Schema;
using ProductionManagementSystem.Models.Components;

namespace ProductionManagementSystem.Models.SupplyRequests
{
    [Table("DesignsSupplyRequests")]
    public class DesignSupplyRequest : SupplyRequest
    {
        [NotMapped]
        public Design Design { get; set; }
    }
}