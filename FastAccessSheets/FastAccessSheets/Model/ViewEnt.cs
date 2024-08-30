using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
