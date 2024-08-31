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
    public class ViewEnt : BaseViewModel
    {
        private string viewName;
        public string ViewName
        {
            get => viewName;
            set
            {
                viewName = value;
                OnPropertyChanged(nameof(viewName));
            }
        }

        private Element view;
        public Element View
        {
            get => view;
            set
            {
                view = value;
                OnPropertyChanged(nameof(view));
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
        public ICommand OpenViewCommand { get; set; }

        #endregion

        #region Method
        public ViewEnt()
        {
            OpenViewCommand = new RelayCommand(OpenView);
        }
        public void OpenView(object parameter)
        {
            if (View is View3D)
            {
                Data.Instance.uiApp.ActiveUIDocument.ActiveView = View as View3D;
            }
            else if (View is ViewPlan)
            {
                Data.Instance.uiApp.ActiveUIDocument.ActiveView = View as ViewPlan;
            }
            else if (View is ViewSection)
            {
                Data.Instance.uiApp.ActiveUIDocument.ActiveView = View as ViewSection;
            }
            else if (View is ViewSheet)
            {
                Data.Instance.uiApp.ActiveUIDocument.ActiveView = View as ViewSheet;
            }
            else if (View is ViewDrafting)
            {
                Data.Instance.uiApp.ActiveUIDocument.ActiveView = View as ViewDrafting;
            }
            else if (View is ViewSchedule)
            {
                Data.Instance.uiApp.ActiveUIDocument.ActiveView = View as ViewSchedule;
            }

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
