namespace Aibe.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TestTableCommonH")]
    public partial class TestTableCommonH
    {
        [Key]
        public int Cid { get; set; }

        [StringLength(100)]
        public string TemplateName { get; set; }

        [StringLength(2000)]
        public string TemplateDefaultValue { get; set; }

        [StringLength(2000)]
        public string TemplateCheckValue { get; set; }
    }
}
