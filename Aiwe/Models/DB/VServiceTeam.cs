namespace Aibe.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("VServiceTeam")]
    public partial class VServiceTeam
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(256)]
        public string UserName { get; set; }

        public string WorkingRole { get; set; }

        public string Team { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(3)]
        public string CustomerSiteId { get; set; }
    }
}
