using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Diagnostics.CodeAnalysis;

namespace VirtualUI.Models
{

    [ExcludeFromCodeCoverage]
    public class Files
    {
        [StringLength(50)]
        public virtual string Id { get; set; }

        [StringLength(50)]
        public virtual string Name { get; set; }

        [StringLength(10)]
        public virtual string Extension { get; set; }
    }
}
