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
        private ItemSearchType searchType;
        public ItemSearchType SearchType
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
        public ClearItem(ItemSearchType type, Element eleInput )
        {
            this.searchType = type;
            this.ele = eleInput;
        }
    }
}
