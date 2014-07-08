using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Portable.Samples.Spreadsheet;

namespace Metro
{
    public class SpreadsheetViewModel
    {
        private Spreadsheet spreadsheet;
        private Dictionary<string, CellViewModel> cells = new Dictionary<string, CellViewModel>();

        public List<RowViewModel> Rows { get; private set; }
        public List<string> Headers { get; private set; }


        public string SourceCode
        {
            get
            {
                return @"
type Spreadsheet(height : int, width : int) = 
    
    do 
        if height <= 0 then failwith ""Height should be greater than zero""
        if width <= 0 || width > 26 then failwith ""Width should be greater than zero and lesser that 26""

    let rowNames = [| for i = 0 to height - 1 do yield string (i + 1)|]
    let colNames = [| for i = 0 to (width - 1) do yield string (char (int 'A' + i)) |]

    let isValidReference (s : string) = 
        if s.Length < 2 then false
        else
        let c = s.[0..0]
        let r = s.[1..]
        (Array.exists ((=)c) colNames) && (Array.exists ((=)r) rowNames)

    let dependencies = Dependencies.Graph()
    let formulas = Dictionary<_, Expression>()

    let values = Dictionary()
    let rawValues = Dictionary()

    let setError cell text = 
        values.[cell] <- EvalResult.E text

    let getValue reference = 
        match values.TryGetValue reference with
        | true, v -> v
        | _ -> EvalResult.S 0.0

    let deleteValue reference = 
        values.Remove(reference)
        |> ignore

    let deleteFormula cell = 
        match formulas.TryGetValue cell with
        | true, expr ->
            dependencies.Delete(cell, expr.GetReferences())
            formulas.Remove(cell) 
            |> ignore
        | _ -> ()
";
            }
        }

        public SpreadsheetViewModel(Spreadsheet spreadsheet)
        {
            this.spreadsheet = spreadsheet;
            Rows = new List<RowViewModel>();
            foreach(var rowRef in spreadsheet.GetRowReferences())
            {
                var rowvm = new RowViewModel {Index = rowRef.Name, Cells = new List<CellViewModel>() };

                foreach(var reference in rowRef.Cells)
                {
                    var cell = new CellViewModel(this, reference);
                    cells.Add(reference, cell);
                    rowvm.Cells.Add(cell);
                }
                Rows.Add(rowvm);
                
            }
            Headers = new[] { "  " }.Concat(spreadsheet.Headers).ToList();
        }

        public void SetCellValue(string reference, string newText)
        {
            var affectedCells = spreadsheet.SetValue(reference, newText);
            foreach (var cell in affectedCells)
            {
                var cellVm = cells[cell.Reference];
                cellVm.RawValue = cell.RawValue;

                if (cell.HasError)
                {
                    cellVm.Value = "#ERROR";
                    cellVm.Tooltip = cell.Value; // will contain error
                }
                else
                {
                    cellVm.Value = cell.Value;
                    cellVm.Tooltip = cell.RawValue;
                }
            }
        }
    }

    public class RowViewModel
    {
        public string Index { get; set; }
        public List<CellViewModel> Cells { get; set; }
    }

    public class CellViewModel : INotifyPropertyChanged
    {
        private SpreadsheetViewModel spreadsheet;

        private string rawValue;
        private string value;
        private string reference;
        private string tooltip;

        public CellViewModel(SpreadsheetViewModel spreadsheet, string reference)
        {
            this.spreadsheet = spreadsheet;
            this.reference = reference;
        }

        public string RawValue
        {
            get
            {
                return rawValue;
            }
            set
            {
                var changed = rawValue != value;
                rawValue = value;
                if (changed) RaisePropertyChanged("RawValue");
            }
        }
        public string Value
        {
            get
            {
                return value;
            }
            set
            {
                var changed = this.value != value;
                this.value = value;
                if (changed) RaisePropertyChanged("Value");
            }
        }
        public string Tooltip
        {
            get
            {
                return tooltip;
            }
            set
            {
                var changed = this.tooltip != value;
                this.tooltip = value;
                if (changed)
                {
                    RaisePropertyChanged("Tooltip");
                    RaisePropertyChanged("TooltipVisibility");
                }
            }
        }

        public Visibility TooltipVisibility
        {
            get { return string.IsNullOrEmpty(tooltip) ? Visibility.Collapsed : Visibility.Visible; }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public void SetCellValue(string newValue)
        {
            spreadsheet.SetCellValue(reference, newValue);
        }

        private void RaisePropertyChanged(string name)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
