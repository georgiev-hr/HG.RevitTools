using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace HG.RevitTools.Services
{
    public class RevitElementCollectorService
    {
        public List<ViewPlan> GetFloorPlans(Document document)
        {
            return new FilteredElementCollector(document)
                .OfClass(typeof(ViewPlan))
                .Cast<ViewPlan>()
                .Where(v => !v.IsTemplate && v.ViewType == ViewType.FloorPlan)
                .ToList();
        }

        public List<Element> GetScopeBoxes(Document document)
        {
            return new FilteredElementCollector(document)
                .OfCategory(BuiltInCategory.OST_VolumeOfInterest)
                .WhereElementIsNotElementType()
                .ToList();
        }

        public List<ViewPlan> GetUnplacedViewsWithScopeBoxes(Document document)
        {
            var placedViewIds = new FilteredElementCollector(document)
                .OfClass(typeof(Viewport))
                .Cast<Viewport>()
                .Select(viewPort => viewPort.ViewId)
                .ToHashSet();
            return new FilteredElementCollector(document)
                .OfClass(typeof(ViewPlan))
                .Cast<ViewPlan>()
                .Where(viewPlan =>
                    !viewPlan.IsTemplate &&
                    viewPlan.ViewType == ViewType.FloorPlan &&
                    !placedViewIds.Contains(viewPlan.Id) &&
                    viewPlan.get_Parameter(BuiltInParameter.VIEWER_VOLUME_OF_INTEREST_CROP) != null &&
                    viewPlan.get_Parameter(BuiltInParameter.VIEWER_VOLUME_OF_INTEREST_CROP)
                    .AsElementId() != ElementId.InvalidElementId)
                .ToList();
        }

        public List<FamilySymbol> GetTitleBlocks(Document document)
        {
            return new FilteredElementCollector(document)
                .OfClass(typeof(FamilySymbol))
                .OfCategory(BuiltInCategory.OST_TitleBlocks)
                .Cast<FamilySymbol>()
                .OrderBy(tb => tb.FamilyName)
                .ThenBy(tb => tb.Name)
                .ToList();
        }
    }
}
