using System;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace DesktopUI.Models
{
    public enum ReconciliationResult
    {
        Ok = 0,
        Disabled = 1,
        Failed = 2,
        FailMismatch = 4,
    }

    public class ReconciliationDto : ObservableObject
    {
        private bool _isSelected;
        private bool _isExpanded;

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        public Guid Id { get; set; } = Guid.Empty;
        public DateTime Date { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public ReconciliationResult Result { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? RunJob { get; set; }
        public int? ToolkitId { get; set; }

        public string Manager { get; set; } = string.Empty;
        public string? Context { get; set; }

        public int? SrcCount { get; set; }
        public string? SrcSqlString { get; set; }
        public int? SrcSqlCost { get; set; }
        public int? SrcSqlTime { get; set; }

        public int? DstCount { get; set; }
        public int? DstSqlCost { get; set; }
        public string? DstSqlString { get; set; }
        public int? DstSqlTime { get; set; }

        public int? CustomCount { get; set; }
        public int? CustomSqlCost { get; set; }
        public string? CustomSqlString { get; set; }
        public int? CustomSqlTime { get; set; }
    }
}
