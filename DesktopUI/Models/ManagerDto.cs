using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrWPF.Windows.Data;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace DesktopUI.Models;

public class ManagerDto : ObservableObject
{
    private int? _rowsRead;
    private int? _rowsWritten;
    private DateTime? _startTime;
    private DateTime? _endTime;
    private TimeSpan? _runtime;

    public string Name { get; set; } = string.Empty;
    // Takes the last word and removes ',rnd_xxxxxxx' from the end.
    public string NameShort => string.Join('.',
        Name.Split('.').Last().Split(',').First());
    public int? RowId { get; set; }
    public ObservableDictionary<string, int> RowsReadDict { get; set; } = new(); // TODO - Consider alternatives to ObservableDictionary (3rd party code)
    public int? RowsRead { get => _rowsRead; set => SetProperty(ref _rowsRead, value); }
    public ObservableDictionary<string, int> RowsWrittenDict { get; set; } = new();
    public int? RowsWritten { get => _rowsWritten; set => SetProperty(ref _rowsWritten, value); }
    public DateTime? StartTime { get => _startTime; set => SetProperty(ref _startTime, value); }
    public DateTime? EndTime { get => _endTime; set => SetProperty(ref _endTime, value); }
    public TimeSpan? Runtime { get => _runtime; set => SetProperty(ref _runtime, value); }
    public ObservableDictionary<string, int> SqlCostDict { get; set; } = new();
    public ObservableDictionary<string, int> TimeDict { get; set; } = new();
}