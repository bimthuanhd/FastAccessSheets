using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FastAccessSheets.Model
{
    public class SheetEnt : BaseViewModel
    {
        private string sheetName;
        public string SheetName
        {
            get => sheetName;
            set
            {
                sheetName = value;
                OnPropertyChanged(nameof(sheetName));
            }
        }

        private ViewSheet viewSheet;
        public ViewSheet ViewSheet
        {
            get => viewSheet;
            set
            {
                viewSheet = value;
                OnPropertyChanged(nameof(viewSheet));
            }
        }
        

        private ObservableCollection<ViewEnt> viewEnts;
        public ObservableCollection<ViewEnt> ViewEnts
        {
            get => viewEnts ?? (viewEnts = new ObservableCollection<ViewEnt>());
            set => OnPropertyChanged(nameof(viewEnts));
        }

        ICommand ViewListChangedCommand = new RelayCommand<object>((p) => true, p =>
        {
            TaskDialog.Show("Revit", "");
        });

    }
}
