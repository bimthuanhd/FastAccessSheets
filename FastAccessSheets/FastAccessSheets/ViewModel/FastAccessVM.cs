using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using FastAccessSheets.Model;
using GalaSoft.MvvmLight.Command;
using HTAddin;
using SingleData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static FastAccessSheets.Model.Enums;

namespace FastAccessSheets.ViewModel
{
    public class FastAccessVM : BaseViewModel
    {
        #region Props
        public IList<Element> Alltitleblocks = new List<Element>();
        public List<Tuple<ViewSheet, FamilyInstance>> AllSheetTitleList = new List<Tuple<ViewSheet, FamilyInstance>>();
        public List<SheetEnt> SheetEntsDict = new List<SheetEnt>();

        private SheetTab sheetTab = SheetTab.Access;
        public SheetTab SheetTab
        {
            get => sheetTab;
            set
            {
                sheetTab = value;
                OnPropertyChanged(nameof(sheetTab));
            }
        }

        private ClearSheetModel clearSheetModel;
        public ClearSheetModel ClearSheetModel
        {
            get => clearSheetModel ?? (clearSheetModel = new ClearSheetModel());
            set => clearSheetModel = value;
        }

        private ObservableCollection<SheetEnt> sheetEnts;
        public ObservableCollection<SheetEnt> SheetEnts
        {
            get => sheetEnts ?? (sheetEnts = new ObservableCollection<SheetEnt>());
            set
            {
                sheetEnts = value;
                OnPropertyChanged(nameof(sheetEnts));
            }
        }

        private SheetEnt selectedSheetEnt;
        public SheetEnt SelectedSheetEnt
        {
            get => selectedSheetEnt ?? (selectedSheetEnt = SheetEnts.FirstOrDefault());
            set
            {
                selectedSheetEnt = value;
                OnPropertyChanged(nameof(selectedSheetEnt));
            }
        }

        private string textTagSearch;
        public string TextTagSearch
        {
            get => textTagSearch;
            set
            {
                textTagSearch = value;
                OnPropertyChanged(nameof(textTagSearch));
                HandleSearch(value);
            }
        }
        #endregion

        #region Command
        public ICommand OkCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        #endregion

        #region Method
        public FastAccessVM()
        {
            Initialize();
            OkCommand = new RelayCommand(BtnOkeCommand);
            CancelCommand = new RelayCommand(BtnCancelCommand);
        }
        public void BtnOkeCommand(object parameter)
        {
            FormData.Instance.HandleOkClick();
        }
        public void BtnCancelCommand(object parameter)
        {
            FormData.Instance.HandleCancleClick();
        }

        public void Initialize()
        {
            GetAllSheets();
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
                        if (!SheetEnts.Select(x => x.SheetName).ToList().Contains(tempSheetEnt.SheetName))
                        {
                            GetAllViewsInSheet(ref tempSheetEnt);
                            SheetEnts.Add(tempSheetEnt);
                        }
                    }
                }
            }
            SheetEntsDict = SheetEnts.ToList();
        }

        public void GetAllViewsInSheet(ref SheetEnt sheetEnt)
        {
            FilteredElementCollector viewPortCollector = new FilteredElementCollector(Data.Instance.doc)
                .OfClass(typeof(Viewport));

            // add tags, textnotes into SheetEnt
            FilteredElementCollector textNoteCollector = new FilteredElementCollector(Data.Instance.doc, sheetEnt.ViewSheet.Id)
                .OfClass(typeof(TextNote));
            FilteredElementCollector tagCollector = new FilteredElementCollector(Data.Instance.doc, sheetEnt.ViewSheet.Id)
                .OfClass(typeof(IndependentTag));

            foreach (TextNote textNote in textNoteCollector)
            {
                sheetEnt.TextNotes.Add(textNote);
            }
            foreach (IndependentTag tag in tagCollector)
            {
                sheetEnt.Tags.Add(tag);
            }

            // add tags, textnotes into ViewEnt
            foreach (Viewport viewport in viewPortCollector)
            {
                if (viewport.SheetId == sheetEnt.ViewSheet.Id)
                {
                    var view = Data.Instance.doc.GetElement(viewport.ViewId);

                    // get AllTextNotes
                    List<TextNote> textNotes = new List<TextNote>();
                    textNoteCollector = new FilteredElementCollector(Data.Instance.doc, view.Id).OfClass(typeof(TextNote));
                    foreach (TextNote textNote in textNoteCollector)
                    {
                        textNotes.Add(textNote);
                    }

                    // get AllTags
                    List<IndependentTag> tags = new List<IndependentTag>();
                    tagCollector = new FilteredElementCollector(Data.Instance.doc, view.Id).OfClass(typeof(IndependentTag));
                    foreach (IndependentTag tag in tagCollector)
                    {
                        tags.Add(tag);
                    }

                    ViewEnt tempViewEnt = new ViewEnt
                    {
                        View = view,
                        ViewName = view.Name,
                        TextNotes = textNotes,
                        Tags = tags,
                    };
                    sheetEnt.ViewEnts.Add(tempViewEnt);
                }
            }
        }
        public void ReLoadSheets()
        {
            SheetEnts = SheetEntsDict.ToObservableCollection();
        }

        public void HandleSearch(string stringSearch)
        {
            if (stringSearch == string.Empty)
            {
                ReLoadSheets();
                return;
            }

            var listDataSearch = new List<SheetEnt>();
            foreach (var sE in SheetEntsDict)
            {
                var tempSheetSearch = new SheetEnt();
                tempSheetSearch.SheetName = sE.SheetName;
                tempSheetSearch.ViewSheet = sE.ViewSheet;
                tempSheetSearch.ViewEnts = sE.ViewEnts;
                tempSheetSearch.Tags = sE.Tags;
                tempSheetSearch.TextNotes = sE.TextNotes;

                var listViewSearch = new List<ViewEnt>();

                // if sheet or views in sheet have tag, text contains "stringSearch"
                if (stringSearch != null)
                {
                    // check Sheet
                    if (sE.TextNotes.FirstOrDefault(x => x.Text.ToUpper().Contains(stringSearch.ToUpper())) != null
                        || sE.Tags.FirstOrDefault(x => x.TagText.ToUpper().Contains(stringSearch.ToUpper())) != null)
                    {
                        if (!listDataSearch.Select(x => x.SheetName).Contains(sE.SheetName))
                        {
                            listDataSearch.Add(tempSheetSearch);
                        }
                    }

                    // check View
                    foreach (var vE in tempSheetSearch.ViewEnts)
                    {
                        if (vE.TextNotes.FirstOrDefault(x => x.Text.ToUpper().Contains(stringSearch.ToUpper())) != null)
                        {
                            listViewSearch.Add(vE);
                            continue;
                        }

                        if (vE.Tags.FirstOrDefault(x => x.TagText.ToUpper().Contains(stringSearch.ToUpper())) != null)
                        {
                            listViewSearch.Add(vE);
                            continue;
                        }
                    }

                    if (!listDataSearch.Select(x => x.SheetName).Contains(tempSheetSearch.SheetName))
                    {
                        if (listViewSearch.Count != 0)
                        {
                            tempSheetSearch.ViewEnts = listViewSearch.ToObservableCollection();
                            listDataSearch.Add(tempSheetSearch);
                        }
                    }
                    else
                    {
                        tempSheetSearch.ViewEnts = listViewSearch.ToObservableCollection();
                    }
                }

                SheetEnts = new ObservableCollection<SheetEnt>(listDataSearch);
            }

            #endregion
        }
    }
}


