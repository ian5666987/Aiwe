namespace Aibe.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CFG_SVG_TYP
    {
        [Key]
        public int Cid { get; set; }

        [Required]
        [StringLength(30)]
        public string ServiceTypeId { get; set; }

        [StringLength(100)]
        public string TypeDescription { get; set; }
    }
}
