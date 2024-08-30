using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using FastAccessSheets.Model;
using SingleData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace FastAccessSheets.ViewModel
{
    public class FastAccessVM : BaseViewModel
    {
        #region Props
        public IList<Element> Alltitleblocks = new List<Element>();
        public List<Tuple<ViewSheet, FamilyInstance>> AllSheetTitleList = new List<Tuple<ViewSheet, FamilyInstance>>();

        private ObservableCollection<SheetEnt> sheetEnts;
        public ObservableCollection<SheetEnt> SheetEnts
        {
            get => sheetEnts ?? (sheetEnts = new ObservableCollection<SheetEnt>());
            set => OnPropertyChanged(nameof(sheetEnts));
        }

        private SheetEnt selectedSheetEnt;
        public SheetEnt SelectedSheetEnt
        {
            get => selectedSheetEnt ?? (selectedSheetEnt = SheetEnts.FirstOrDefault());
            set => OnPropertyChanged(nameof(selectedSheetEnt));
        }
        #endregion

        #region Command
        ICommand ViewListChangedCommand = new RelayCommand<object>((p) => true, p =>
        {
            TaskDialog.Show("Revit", "");
        });
        #endregion

        #region Method
        public FastAccessVM()
        {
            Initialize();
        }

        public void Initialize()
        {
            GetAllSheets();

            //var firstSheet = sheetEnts.First();
            //Data.Instance.uiApp.ActiveUIDocument.ActiveView = firstSheet.ViewSheet;
            //var allViewPorts = firstSheet.ViewSheet.GetAllViewports();

            //List<Viewport> viewports = new List<Viewport>();
            //FilteredElementCollector collector = new FilteredElementCollector(Data.Instance.doc)
            //                           .OfClass(typeof(Viewport));
            //foreach (Viewport viewport in collector)
            //{
            //    if (viewport.SheetId == firstSheet.ViewSheet.Id)
            //    {
            //        viewports.Add(viewport);
            //    }
            //}

            //var firstViewport = viewports.First();
            //var view = Data.Instance.doc.GetElement(firstViewport.ViewId);
        }


        public void GetAllTitleBlocks(Document doc)
        {
            //get all titleblocks
            FilteredElementCollector fi = new FilteredElementCollector(doc);
            fi.OfClass(typeof(FamilyInstance));
            fi.OfCategory(BuiltInCategory.OST_TitleBlocks);
            Alltitleblocks = fi.ToElements();
        }

        public void GetAllSheets()
        {
            IEnumerable<ViewSheet> AllSheets = new FilteredElementCollector(Data.Instance.doc).OfClass(typeof(ViewSheet)).OfCategory(BuiltInCategory.OST_Sheets).Cast<ViewSheet>();
            GetAllTitleBlocks(Data.Instance.doc);

            foreach (FamilyInstance tb in Alltitleblocks)
            {
                foreach (ViewSheet vs in AllSheets)
                {
                    if (tb.OwnerViewId.IntegerValue == vs.Id.IntegerValue)
                    {
                        AllSheetTitleList.Add(Tuple.Create(vs, tb));

                        var tempSheetEnt = new SheetEnt
                        {
                            SheetName = vs.Title,
                            ViewSheet = vs,
                        };
                        if (!SheetEnts.Contains(tempSheetEnt))
                        {
                            GetAllViewsInSheet(ref tempSheetEnt);
                            SheetEnts.Add(tempSheetEnt);
                        }
                    }
                }
            }
        }

        public void GetAllViewsInSheet(ref SheetEnt sheetEnt)
        {
            FilteredElementCollector collector = new FilteredElementCollector(Data.Instance.doc)
                                       .OfClass(typeof(Viewport));
            foreach (Viewport viewport in collector)
            {
                if (viewport.SheetId == sheetEnt.ViewSheet.Id)
                {
                    var view = Data.Instance.doc.GetElement(viewport.ViewId);

                    ViewEnt tempViewEnt = new ViewEnt
                    {
                        View = view,
                        ViewName = view.Name,
                    };
                    sheetEnt.ViewEnts.Add(tempViewEnt);
                }
            }
        }
        #endregion
    }
}


