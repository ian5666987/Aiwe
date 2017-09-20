namespace Aibe.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ORD_CUS_CON
    {
        [Key]
        public int Cid { get; set; }

        [Required]
        [StringLength(50)]
        public string CustomerId { get; set; }

        [StringLength(20)]
        public string CustomerSiteId { get; set; }

        [StringLength(100)]
        public string DeviceName { get; set; }

        [Required]
        [StringLength(100)]
        public string ContractId { get; set; }

        [StringLength(10)]
        public string JobTypeId { get; set; }

        public DateTime? ContractStartDate { get; set; }

        public DateTime? ContractEndDate { get; set; }

        [StringLength(10)]
        public string CurrentStatus { get; set; }

        public int? Qty { get; set; }
    }
}
