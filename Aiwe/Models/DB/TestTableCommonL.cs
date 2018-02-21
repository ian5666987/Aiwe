namespace Aiwe.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TestTableCommonL")]
    public partial class TestTableCommonL
    {
        [Key]
        public int Cid { get; set; }

        public int? TransactionId { get; set; }

        public string Buyer { get; set; }

        public string ItemName { get; set; }

        public decimal? ItemPrice { get; set; }

        public int? ItemCount { get; set; }

        public decimal? ItemTotalPrice { get; set; }
    }
}
