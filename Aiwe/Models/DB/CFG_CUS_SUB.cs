namespace Aibe.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CFG_CUS_SUB
    {
        [Key]
        public int Cid { get; set; }

        [Required]
        [StringLength(50)]
        public string CustomerId { get; set; }

        [Required]
        [StringLength(100)]
        public string SubconId { get; set; }
    }
}
