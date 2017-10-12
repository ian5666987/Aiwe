namespace Aiwe.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TestTableSpecificA")]
    public partial class TestTableSpecificA
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EquipmentId { get; set; }

        [StringLength(100)]
        public string EquipmentName { get; set; }

        [StringLength(100)]
        public string VendorName { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? BoughtDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? LastMaintenance { get; set; }
    }
}
