using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HG.RevitTools.Models.AdjustLuminaireHeight
{
    public class TargetPlaneInfo
    {
        public Plane Plane { get; set; }

        public XYZ Origin { get; set; }

        public XYZ Normal { get; set; }

        public bool IsFromLinkedFace { get; set; }

        public Element SourceElement { get; set; }
    }
}

