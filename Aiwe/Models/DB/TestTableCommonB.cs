namespace Aibe.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TestTableCommonB")]
    public partial class TestTableCommonB
    {
        [Key]
        public int Cid { get; set; }

        [StringLength(100)]
        public string PersonInCharge { get; set; }

        [StringLength(100)]
        public string CompanyName { get; set; }

        [StringLength(300)]
        public string JobSite { get; set; }

        public int? WorkHours { get; set; }

        [StringLength(100)]
        public string TeamAssigned { get; set; }
    }
}
