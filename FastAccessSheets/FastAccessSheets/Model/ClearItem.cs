using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FastAccessSheets.Model.Enums;

namespace FastAccessSheets.Model
{
    public class ClearItem : BaseViewModel
    {
        private ViewType searchType;
        public ViewType SearchType
        {
            get => searchType;
            set
            {
                searchType = value;
                OnPropertyChanged(nameof(searchType));
            }
        }

        private Element ele;
        public Element Ele
        {
            get => ele;
            set
            {
                ele = value;
                OnPropertyChanged(nameof(ele));
            }
        }

        private string name;
        public string Name
        {
            get => name ?? (name = Ele.Name);
            set
            {
                name = value;
                OnPropertyChanged(nameof(name));
            }
        }
        public ClearItem(ViewType type, Element eleInput)
        {
            this.searchType = type;
            this.ele = eleInput;
        }
    }
}
