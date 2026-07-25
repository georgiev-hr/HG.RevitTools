using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace HG.RevitTools.Services.AdjustLuminaireHeight
{
    public class LuminaireSelectionService
    {
        public IList<FamilyInstance> PickLightingFixtures(
            UIDocument uiDoc)
        {
            IList<Reference> references = uiDoc.Selection.PickObjects(
                ObjectType.Element,
                new LightingFixtureSelectionFilter(),
                "Select lighting fixtures to adjust");

            Document doc = uiDoc.Document;

            return ConvertReferencesToFamilyInstances(
                doc,
                references);
        }

        private IList<FamilyInstance> ConvertReferencesToFamilyInstances(
            Document document,
            IList<Reference> references)
        {
            return references
                .Select(r => document.GetElement(r.ElementId))                
                .OfType<FamilyInstance>()
                .ToList();
        }        
    }
}
