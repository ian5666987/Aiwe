namespace Aibe.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ORD_CUS_PRD
    {
        [Key]
        public int Cid { get; set; }

        [StringLength(100)]
        public string ProductTypeId { get; set; }

        [StringLength(100)]
        public string DeviceName { get; set; }

        [StringLength(100)]
        public string ProductCode { get; set; }

        [StringLength(500)]
        public string ProductSeriesNo { get; set; }

        [Required]
        [StringLength(50)]
        public string CustomerId { get; set; }

        [StringLength(20)]
        public string CustomerSiteId { get; set; }
    }
}
