using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace HG.RevitTools.Services.AdjustLuminaireHeight
{
    public class LightingFixtureSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(
            Element element)
        {
            return element.Category != null
                && element.Category.Id.Value == (int)BuiltInCategory.OST_LightingFixtures;
        }

        public bool AllowReference(
            Reference reference,
            XYZ position)
        {
            return false;
        }
    }
}