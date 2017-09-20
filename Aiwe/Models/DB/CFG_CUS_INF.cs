namespace Aibe.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CFG_CUS_INF
    {
        [Key]
        public int Cid { get; set; }

        [Required]
        [StringLength(50)]
        public string CustomerId { get; set; }

        [Required]
        [StringLength(3)]
        public string CustomerSiteId { get; set; }

        [StringLength(100)]
        public string CustomerName { get; set; }

        [StringLength(200)]
        public string CompanyAddress { get; set; }

        [StringLength(600)]
        public string ContactList { get; set; }
    }
}
