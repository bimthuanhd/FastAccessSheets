using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using HTAddin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SingleData
{
    public class Data
    {
        private static Data instance;
        public static Data Instance
        {
            get=> instance ?? (instance = new Data());
            set => instance = value;    
        }

        public Document doc { get; set; }
        public UIApplication uiApp { get; set; }
        public UIDocument uiDoc { get; set; }
        public Autodesk.Revit.ApplicationServices.Application app { get; set; }
        public Selection sel { get; set; }

        //data


    }
}
