using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace HG.RevitTools.Services.ParameterChecker
{
    public class LightingFixtureCollectorService
    {
        public List<Element> GetLightingFixtureInstances(
            Document document)
        {
            if (document == null)
            {
                return new List<Element>();
            }

            return new FilteredElementCollector(document)
                .OfCategory(BuiltInCategory.OST_LightingFixtures)
                .WhereElementIsNotElementType()
                .ToElements()
                .Where(element => element != null)
                .ToList();
        }
    }
}