using Autodesk.Revit.DB;
using SingleData;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
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
            get => name ?? (name = Ele.IsValidObject ? Ele.Name : string.Empty);
            set
            {
                name = value;
                OnPropertyChanged(nameof(name));
            }
        }

        public bool isChecked;
        public bool Ischecked
        {
            get => isChecked;
            set
            {
                isChecked = value;
                OnPropertyChanged(nameof(isChecked));
            }
        }

        #region Command
        public ICommand CheckItemCmd { get; set; }
        #endregion

        public void CheckItemCommand(object parameter)
        {
            var a = 1;

        }

        public ClearItem(ViewType type, Element eleInput)
        {
            this.searchType = type;
            this.ele = eleInput;
            CheckItemCmd = new RelayCommand(CheckItemCommand);
        }
    }
}
