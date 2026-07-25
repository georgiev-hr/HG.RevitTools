using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HG.RevitTools.Models.AdjustLuminaireHeight
{
    public class LuminaireHeightData
    {
        public FamilyInstance Luminaire { get; set; }

        public XYZ InsertionPoint { get; set; }

        public double CurrentHeightMm { get; set; }

        public double TargetHeightMm { get; set; }

        public double DifferenceMm { get; set; }
    }
}
