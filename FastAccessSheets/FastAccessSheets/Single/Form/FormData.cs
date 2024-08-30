using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Autodesk.Revit.UI.Selection;
using System.Windows.Markup;
using HTAddin;
using Autodesk.Revit.DB;
using FastAccessSheets.View;

namespace SingleData
{
    public class FormData
    {
        private static FormData instance;
        public static FormData Instance
        {
            get => instance ?? (instance = new FormData());
            set => instance = value;
        }

        //model
        public Data ModelData => Data.Instance;

        private FastAccessView form;
        public FastAccessView Form => form ?? (form = new FastAccessView());

        public void HandleOkClick()
        {
            FormData formData = FormData.Instance;
            Data Revitdata = Data.Instance;

            var uiApp = Instance.ModelData.uiApp;
            var app = Instance.ModelData.app;
            var uiDoc = Instance.ModelData.uiDoc;
            var doc = Instance.ModelData.doc;
            Selection sel = Instance.ModelData.sel;

            //var form = data.Form;

            //Revitdata.doc = doc;
            //Revitdata.sel = sel;

            Form.Close();
        }

        public void HandleCancleClick()
        {
            Form.Close();
        }
    }
}
