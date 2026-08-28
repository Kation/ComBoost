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

        public DateTime? OptionalCreatedAt { get; set; }

        public DateTimeOffset OccurredAt { get; set; }

        public DateTimeOffset? OptionalOccurredAt { get; set; }

#if NET6_0_OR_GREATER
        public DateOnly Date { get; set; }

        public DateOnly? OptionalDate { get; set; }

        public TimeOnly Time { get; set; }

        public TimeOnly? OptionalTime { get; set; }
#endif

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
        [Key]
        public string? Name { get; set; }

        //[ExcelExportExpandable]
        public List<SampleOrderLine> Orders { get; set; } = new();

        public int Age { get; set; }
    }

    public class SampleDateTimeExportItem
    {
        [Key]
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? OptionalCreatedAt { get; set; }

        public DateTimeOffset OccurredAt { get; set; }

        public DateTimeOffset? OptionalOccurredAt { get; set; }

#if NET6_0_OR_GREATER
        public DateOnly Date { get; set; }

        public DateOnly? OptionalDate { get; set; }

        public TimeOnly Time { get; set; }

        public TimeOnly? OptionalTime { get; set; }
#endif
    }
}