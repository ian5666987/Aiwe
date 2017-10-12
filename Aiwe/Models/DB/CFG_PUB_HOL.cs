namespace Aiwe.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CFG_PUB_HOL
    {
        [Key]
        public int Cid { get; set; }

        [Required]
        [StringLength(3)]
        public string Country { get; set; }

        [Column(TypeName = "date")]
        public DateTime? HolidayDate { get; set; }

        [StringLength(100)]
        public string HolidayDesc { get; set; }
    }
}
