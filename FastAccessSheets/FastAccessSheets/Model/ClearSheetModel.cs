using Autodesk.Revit.DB;
using HTAddin;
using SingleData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using static FastAccessSheets.Model.Enums;
using RevitView = Autodesk.Revit.DB.View;

namespace FastAccessSheets.Model
{
    public class ClearSheetModel : BaseViewModel
    {
        private ObservableCollection<EnumModel> searchTypes;
        public ObservableCollection<EnumModel> SearchTypes
        {
            get => searchTypes ?? (searchTypes = new ObservableCollection<EnumModel>()
            {
                new EnumModel("View",(int)ItemSearchType.View),
                new EnumModel("Schedule",(int)ItemSearchType.Schedule),
                new EnumModel("Legend",(int)ItemSearchType.Legend),
                new EnumModel("All",(int)ItemSearchType.All),
            });
            set
            {
                searchTypes = value;
                OnPropertyChanged(nameof(searchTypes));
            }
        }

        private EnumModel selectedSearchType;
        public EnumModel SelectedSearchType
        {
            get => selectedSearchType ?? (selectedSearchType = SearchTypes.FirstOrDefault());
            set
            {
                selectedSearchType = value;
                OnPropertyChanged(nameof(selectedSearchType));
                ResetClearItems();
            }
        }

        private List<ClearItem> clearItemDict;
        public List<ClearItem> ClearItemDict
        {
            get => clearItemDict;
            set
            {
                clearItemDict = value;
                OnPropertyChanged(nameof(clearItemDict));
            }
        }

        private ObservableCollection<ClearItem> currentClearItems;
        public ObservableCollection<ClearItem> CurrentClearItems
        {
            get => currentClearItems;
            set
            {
                currentClearItems = value;
                OnPropertyChanged(nameof(currentClearItems));
            }
        }

        public void ResetClearItems(ItemSearchType currentType = ItemSearchType.View)
        {
            var items = new List<ClearItem>();

            switch (currentType)
            {
                case ItemSearchType.View:
                    if (ClearItemDict.Find(x => x.SearchType == ItemSearchType.View) != null) return;

                    List<RevitView> viewsNotOnSheet = new List<RevitView>(new FilteredElementCollector(Data.Instance.doc)
                                .OfClass(typeof(RevitView))
                                .ToElements()
                                .Cast<RevitView>()
                                .Where(x => !x.IsTemplate && String.IsNullOrEmpty(x.get_Parameter(BuiltInParameter.VIEWPORT_SHEET_NUMBER).AsString())));

                    ClearItemDict.AddRange(viewsNotOnSheet.Select(x => new ClearItem(ItemSearchType.View, x)));
                    break;
                case ItemSearchType.Schedule:
                    break;
                case ItemSearchType.Legend:
                    break;
                case ItemSearchType.All:
                    break;
                default:
                    break;
            }

        }

    }
}
