using System.Collections.Generic;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using System.Collections.ObjectModel;

namespace DesktopUI.ViewModels.Charts.Interfaces;

public interface IBarChartViewModel
{
    ObservableCollection<ISeries> Series { get; }
    ObservableCollection<Axis> XAxes { get; }
    ObservableCollection<Axis> YAxes { get; }

    void AddSeries(IEnumerable<object> data);
    void RemoveSeries(IEnumerable<object> data);
}
