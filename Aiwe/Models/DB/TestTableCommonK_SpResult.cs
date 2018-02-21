namespace Aiwe.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TestTableCommonK_SpResult
    {
        [Key]
        public int Cid { get; set; }

        public string TextResult { get; set; }
    }
}
