namespace Aibe.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CFG_TEM_DRP
    {
        [Key]
        public int Cid { get; set; }

        [StringLength(100)]
        public string TemplateName { get; set; }

        [StringLength(2000)]
        public string TemplateValue { get; set; }
    }
}
