using Hl7.Fhir.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hl7.Fhir.ElementModel
{
    public class PocoPositionInfo : IPositionInfo
    {
        public PocoPositionInfo(IPositionInfo pos)
        {
            LineNumber = pos.LineNumber;
            LinePosition = pos.LinePosition;
        }
        public int LineNumber { get; set; }

        public int LinePosition { get; set; }
    }
}
