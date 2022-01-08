using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopUI.Models;

public class ManagerDto
{
    private string _nameShort = string.Empty;

    public string Name { get; set; } = string.Empty;
    // Takes the last word and removes ',rnd_xxxxxxx' from the end.
    public string NameShort => string.Join('.', 
        Name.Split('.').Last().Split(',').First());
    public int? RowId { get; set; }
    public Dictionary<string, int> RowsReadDict { get; set; } = new();
    public int? RowsRead { get; set; }
    public Dictionary<string, int> RowsWrittenDict { get; set; } = new();
    public int? RowsWritten { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public TimeSpan? Runtime { get; set; }
    public Dictionary<string, int> SqlCostDict { get; set; } = new();
    public Dictionary<string, int> TimeDict { get; set; } = new();
}