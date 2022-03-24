using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using DesktopUI.Models;
using DesktopUI.ViewModels.Charts.Interfaces;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;

namespace DesktopUI.ViewModels.Charts;

public class ManagerRuntimeBarChartVM : IBarChartViewModel
{
    public ObservableCollection<ISeries> Series { get; } = new()
    {
        new ColumnSeries<long>
        {
            Values = new ObservableCollection<long>(),
            TooltipLabelFormatter = d => TimeSpan.FromTicks((long)d.PrimaryValue).ToString()
        }
    };
    public ObservableCollection<Axis> XAxes { get; } = new();
    public ObservableCollection<Axis> YAxes { get; } = new()
    {
        new()
        {
            Labels = new List<string>(),
            LabelsRotation = -90
        }
    };

    public void AddSeries(IEnumerable<object> data)
    {
        foreach (ManagerDto m in data)
        {
            YAxes[0].Labels?.Add(m.NameShort);
            ((IList)Series[0].Values).Add(m.Runtime.GetValueOrDefault().Ticks);

            //Series.Add(new RowSeries<ObservablePoint>
            //{
            //    Name = m.NameShort,
            //    Values = new List<ObservablePoint>
            //    {
            //        new(2, 5)
            //    },
            //    DataLabelsFormatter = d => m.NameShort + " " + d.SecondaryValue.ToString(),
            //});
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
