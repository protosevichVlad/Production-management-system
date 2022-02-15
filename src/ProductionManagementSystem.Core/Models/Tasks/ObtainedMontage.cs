﻿using System.ComponentModel.DataAnnotations.Schema;
using ProductionManagementSystem.Core.Models.Components;

namespace ProductionManagementSystem.Core.Models.Tasks
{
    [Table("ObtainedComponents")]
    public class ObtainedMontage : ObtainedBase
    {
        public Montage Montage { get; set; }
    }
}