namespace Aiwe.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CoreActionLog")]
    public partial class CoreActionLog
    {
        [Key]
        public int Cid { get; set; }

        [StringLength(300)]
        public string UserName { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime LogTimeStamp { get; set; }

        [Required]
        [StringLength(100)]
        public string ControllerType { get; set; }

        [Required]
        [StringLength(100)]
        public string ControllerName { get; set; }

        [StringLength(100)]
        public string TableSource { get; set; }

        [Required]
        [StringLength(100)]
        public string UserAction { get; set; }

        [StringLength(3000)]
        public string LogMessage { get; set; }
    }
}
