namespace Aibe.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TestTableCommonE")]
    public partial class TestTableCommonE
    {
        [Key]
        public int Cid { get; set; }

        [StringLength(100)]
        public string NullableString { get; set; }

        public int? NullableInt { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? NullableDateTime { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? NullableDecimal { get; set; }

        [StringLength(1)]
        public string NullableChar { get; set; }

        public short? NullableSmallInt { get; set; }

        [Column(TypeName = "numeric")]
        public decimal DecimalType { get; set; }

        [StringLength(100)]
        public string ILTemplate { get; set; }

        [StringLength(2000)]
        public string ItemList { get; set; }

        [StringLength(100)]
        public string SPTemplate { get; set; }

        [StringLength(2000)]
        public string ServicePerformed { get; set; }

        [StringLength(100)]
        public string CPTemplate { get; set; }

        [StringLength(2000)]
        public string ContactPersons { get; set; }

        [StringLength(2000)]
        public string StaticList { get; set; }
    }
}
