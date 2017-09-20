namespace Aibe.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class JOB_CAL_INF
    {
        [Key]
        public int Cid { get; set; }

        [Required]
        [StringLength(100)]
        public string SerialNo { get; set; }

        [StringLength(100)]
        public string Party { get; set; }

        [StringLength(100)]
        public string Supplier { get; set; }

        [StringLength(100)]
        public string ServicePerson { get; set; }

        [StringLength(100)]
        public string CertificateNo { get; set; }

        [StringLength(100)]
        public string EqpDescription { get; set; }

        public DateTime? DueDate { get; set; }

        public DateTime? CalibrationDate { get; set; }

        [StringLength(10)]
        public string JobStatus { get; set; }

        [Required]
        [StringLength(10)]
        public string CalibrationInt { get; set; }

        [StringLength(100)]
        public string Remarks { get; set; }
    }
}
