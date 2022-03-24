using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DesktopUI.Models;
using DesktopUI.ViewModels.Charts.Interfaces;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace DesktopUI.ViewModels.Charts;

public class ManagerRowsBarChartVM : IBarChartViewModel
{
    public ObservableCollection<ISeries> Series { get; } = new();
    public ObservableCollection<Axis> XAxes { get; } = new()
    {
        new() { Labels = new List<string> { "Read", "Written" } },
    };
    public ObservableCollection<Axis> YAxes { get; } = new();

    public void AddSeries(IEnumerable<object> data)
    {
        foreach (ManagerDto m in data)
        {
            Series.Add(new ColumnSeries<int?>
            {
                Name = m.NameShort,
                Values = new List<int?> { m.RowsRead, m.RowsWritten }
            });
        }
    }

    public void RemoveSeries(IEnumerable<object> data)
    {
        foreach (ManagerDto m in data)
        {
            if (Series.FirstOrDefault(x => x.Name == m.NameShort) is { } series)
            {
                Series.Remove(series);
            }
        }
    }
}
