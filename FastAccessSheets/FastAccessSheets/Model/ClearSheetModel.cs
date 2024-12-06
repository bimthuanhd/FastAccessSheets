using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI;
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
using System.Windows.Input;
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
                new EnumModel("Undefined", (int)ViewType.Undefined),
                new EnumModel("FloorPlan", (int)ViewType.FloorPlan),
                new EnumModel("EngineeringPlan", (int)ViewType.EngineeringPlan),
                new EnumModel("AreaPlan", (int)ViewType.AreaPlan),
                new EnumModel("CeilingPlan", (int)ViewType.CeilingPlan),
                new EnumModel("Elevation", (int)ViewType.Elevation),
                new EnumModel("Section", (int)ViewType.Section),
                new EnumModel("Detail", (int)ViewType.Detail),
                new EnumModel("ThreeD", (int)ViewType.ThreeD),
                new EnumModel("Schedule", (int)ViewType.Schedule),
                new EnumModel("DraftingView", (int)ViewType.DraftingView),
                new EnumModel("DrawingSheet", (int)ViewType.DrawingSheet),
                new EnumModel("Legend", (int)ViewType.Legend),
                new EnumModel("Report", (int)ViewType.Report),
                new EnumModel("ProjectBrowser", (int)ViewType.ProjectBrowser),
                new EnumModel("SystemBrowser", (int)ViewType.SystemBrowser),
                new EnumModel("CostReport", (int)ViewType.CostReport),
                new EnumModel("LoadsReport", (int)ViewType.LoadsReport),
                new EnumModel("PresureLossReport", (int)ViewType.PresureLossReport),
                new EnumModel("PanelSchedule", (int)ViewType.PanelSchedule),
                new EnumModel("ColumnSchedule", (int)ViewType.ColumnSchedule),
                new EnumModel("Walkthrough", (int)ViewType.Walkthrough),
                new EnumModel("Rendering", (int)ViewType.Rendering),
                new EnumModel("SystemsAnalysisReport", (int)ViewType.SystemsAnalysisReport),
                new EnumModel("Internal", (int)ViewType.Internal),
                new EnumModel("All", -1)
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
                ResetClearItems(value);
            }
        }

        private List<ClearItem> clearItemDict;
        public List<ClearItem> ClearItemDict
        {
            get => clearItemDict ?? (clearItemDict = GetClearItemDict());
            set
            {
                clearItemDict = value;
                OnPropertyChanged(nameof(clearItemDict));
            }
        }

        private List<ClearItem> clearItemsToDelete;
        public List<ClearItem> ClearItemsToDelete
        {
            get => clearItemsToDelete ?? (clearItemsToDelete = new List<ClearItem>());
            set
            {
                clearItemsToDelete = value;
                OnPropertyChanged(nameof(clearItemsToDelete));
            }
        }

        private ObservableCollection<ClearItem> currentClearItems;
        public ObservableCollection<ClearItem> CurrentClearItems
        {
            get => currentClearItems ?? (currentClearItems = new ObservableCollection<ClearItem>());
            set
            {
                currentClearItems = value;
                OnPropertyChanged(nameof(currentClearItems));
            }
        }

        private ClearItem selectedCurrentClearItem;
        public ClearItem SelectedCurrentClearItem
        {
            get => selectedCurrentClearItem ?? (selectedCurrentClearItem = CurrentClearItems.FirstOrDefault());
            set
            {
                selectedCurrentClearItem = value;
                OnPropertyChanged(nameof(selectedCurrentClearItem));
            }
        }

        #region Command
        public ICommand CheckAllCmd { get; set; }
        public ICommand UnCheckALlCmd { get; set; }
        public ICommand DeleteCmd { get; set; }
        #endregion

        public void CheckAllCommand(object parameter)
        {
            foreach (var item in CurrentClearItems)
            {
                if (item.Ischecked == true) continue;
                item.Ischecked = true;
            }
        }

        public void UnCheckAllCommand(object parameter)
        {
            foreach (var item in CurrentClearItems)
            {
                if (item.Ischecked == false) continue;
                item.Ischecked = false;
            }
        }

        public void DeleteCommand(object parameter)
        {
            ClearItemsToDelete.AddRange(CurrentClearItems.Where(item => item.Ischecked == true));
            ClearItemDict.RemoveAll(x => ClearItemsToDelete.Contains(x));
            ResetClearItems(SelectedSearchType);
        }

        public List<ClearItem> GetClearItemDict()
        {
            var list = new List<ClearItem>();

            List<RevitView> viewsNotOnSheet = new List<RevitView>(new FilteredElementCollector(Data.Instance.doc)
                        .OfClass(typeof(RevitView))
                        .ToElements()
                        .Cast<RevitView>()
                        .Where(x => !x.IsTemplate && String.IsNullOrEmpty(x.get_Parameter(BuiltInParameter.VIEWPORT_SHEET_NUMBER).AsString())));

            list.AddRange(viewsNotOnSheet.Select(x => new ClearItem(x.ViewType, x)));

            return list;
        }

        public void ResetClearItems(EnumModel searchType)
        {
            var items = new List<ClearItem>();

            if (searchType.Name == "All")
            {
                CurrentClearItems = ClearItemDict.ToObservableCollection();
            }
            else
            {
                CurrentClearItems = ClearItemDict
                    .Where(x => x.SearchType.ToString() == searchType.Name)
                    .ToObservableCollection();
            }
        }

        public void DeleteDataViews()
        {
            using (Transaction trans = new Transaction(Data.Instance.doc, "Delete Views"))
            {
                trans.Start();
                string dialogText = "";
                foreach (ClearItem item in ClearItemsToDelete)
                {
                    try
                    {
                        if (!item.Ele.IsValidObject) continue;
                        Data.Instance.doc.Delete(item.Ele.Id);
                        if (item.Name == string.Empty) continue;
                        dialogText += $"\n{item.Name}";
                    }
                    catch (Autodesk.Revit.Exceptions.ArgumentException ex)
                    {
                        continue;
                    }
                }
                var dialogRes = TaskDialog.Show("Revit", $"The selected element has been removed: {dialogText}", TaskDialogCommonButtons.Ok);
                if (dialogRes != TaskDialogResult.Ok) return;

                trans.Commit();
            }
        }


        public ClearSheetModel()
        {
            CheckAllCmd = new RelayCommand(CheckAllCommand);
            UnCheckALlCmd = new RelayCommand(UnCheckAllCommand);
            DeleteCmd = new RelayCommand(DeleteCommand);
        }

    }
}
