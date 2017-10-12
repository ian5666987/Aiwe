namespace Aiwe.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class LOG_USR_ACT
    {
        [Key]
        public int Cid { get; set; }

        [Required]
        [StringLength(100)]
        public string UserId { get; set; }

        [StringLength(50)]
        public string FunctionName { get; set; }

        [StringLength(30)]
        public string DeviceId { get; set; }

        public DateTime AccessDatetime { get; set; }
    }
}
