using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using FastAccessSheets.ViewModel;
using SingleData;
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
            set
            {
                viewEnts = value;
                OnPropertyChanged(nameof(viewEnts));
            }
        }

        private List<IndependentTag> tags;
        public List<IndependentTag> Tags
        {
            get => tags ?? (tags = new List<IndependentTag>());
            set
            {
                tags = value;
                OnPropertyChanged(nameof(tags));
            }
        }

        private List<TextNote> textNotes;
        public List<TextNote> TextNotes
        {
            get => textNotes ?? (textNotes = new List<TextNote>());
            set
            {
                textNotes = value;
                OnPropertyChanged(nameof(textNotes));
            }
        }

        #region Command
        public ICommand OpenSheetCommand { get; set; }

        #endregion

        #region Method
        public SheetEnt()
        {
            OpenSheetCommand = new RelayCommand(OpenSheet);
        }
        public void OpenSheet(object parameter)
        {
            Data.Instance.uiApp.ActiveUIDocument.ActiveView = ViewSheet;

            var ids = new List<ElementId>();
            string strSearch = (FormData.Instance.Form.DataContext as FastAccessVM).TextTagSearch;
            if (strSearch == string.Empty)
            {
                ids.AddRange(Tags.Select(x => x.Id));
                ids.AddRange(TextNotes.Select(x => x.Id));
            }
            else
            {
                ids.AddRange(Tags.Where(x => x.TagText.ToUpper().Contains(strSearch.ToUpper())).ToList().Select(x => x.Id).ToList());
                ids.AddRange(TextNotes.Where(x => x.Text.ToUpper().Contains(strSearch.ToUpper())).ToList().Select(x => x.Id).ToList());
            }
            Data.Instance.sel.SetElementIds(ids);
        }

        #endregion

    }
}
