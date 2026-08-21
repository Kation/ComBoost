using System.ComponentModel.DataAnnotations;

namespace Wodsoft.ComBoost.ExcelExport.Test.Models
{
    public class SampleExportItem
    {
        public string? Name { get; set; }

        public int Age { get; set; }

        public int? Score { get; set; }

        public bool Active { get; set; }

        public long LongValue { get; set; }

        public double DoubleValue { get; set; }

        public decimal DecimalValue { get; set; }

        public DateTime CreatedAt { get; set; }

        public SampleStatus Status { get; set; }
    }

    public enum SampleStatus
    {
        [Display(Name = "草稿")]
        Draft = 0,

        [Display(Name = "已发布")]
        Published = 1,

        Archived = 2
    }

    public class SampleOrderLine
    {
        public int Id { get; set; }

        public int Qty { get; set; }
    }

    public class SampleOrderExportItem
    {
        public string? Name { get; set; }

        //[ExcelExportExpandable]
        public List<SampleOrderLine> Orders { get; set; } = new();

        public int Age { get; set; }
    }
}