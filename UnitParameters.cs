using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter_DesktopApp_Sql_Database
{
    [Serializable]
    public class UnitParameters
    {
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public int CateId { get; set; }
        public string Value { get; set; }

    }
}
