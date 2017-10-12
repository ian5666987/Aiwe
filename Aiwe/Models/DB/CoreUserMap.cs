namespace Aiwe.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CoreUserMap")]
    public partial class CoreUserMap
    {
        [Key]
        public int Cid { get; set; }

        [StringLength(500)]
        public string UserName { get; set; }

        [StringLength(2000)]
        public string EncryptedPassword { get; set; }
    }
}
