using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastAccessSheets.Model
{
    public class EnumModel
    {
        public string Name { get; set; }
        public int Index { get; set; }

        public EnumModel(string name, int itemIndex)
        {
            Name = name;
            Index = itemIndex;
        }
    }
}
