namespace Aiwe.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CFG_JOB_INT
    {
        [Key]
        public int Cid { get; set; }

        [Required]
        [StringLength(10)]
        public string JobTypeId { get; set; }

        [StringLength(100)]
        public string JobTypeDescription { get; set; }

        public int JobInterval { get; set; }
    }
}
