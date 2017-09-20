namespace Aibe.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CFG_SVG_ACT
    {
        [Key]
        public int Cid { get; set; }

        [Required]
        [StringLength(50)]
        public string ActionName { get; set; }

        [StringLength(1000)]
        public string SPList { get; set; }
    }
}
