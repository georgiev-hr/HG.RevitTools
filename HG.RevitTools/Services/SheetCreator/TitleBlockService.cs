using Autodesk.Revit.DB;
using System;
using System.Linq;

namespace HG.RevitTools.Services.SheetCreator
{
    public class TitleBlockService
    {
        public void ActivateTitleBlock(
            Document document,
            FamilySymbol titleBlockSymbol)
        {
            if (titleBlockSymbol == null)
            {
                throw new InvalidOperationException(
                    "No title block type was selected.");
            }

            if (!titleBlockSymbol.IsActive)
            {
                titleBlockSymbol.Activate();
                document.Regenerate();
            }
        }

        public FamilyInstance GetTitleBlockInstance(
            Document document,
            ViewSheet sheet)
        {
            return new FilteredElementCollector(document, sheet.Id)
                .OfCategory(BuiltInCategory.OST_TitleBlocks)
                .WhereElementIsNotElementType()
                .Cast<FamilyInstance>()
                .FirstOrDefault();
        }

        public XYZ GetTitleBlockLocation(
            FamilyInstance titleBlockInstance)
        {
            LocationPoint locationPoint =
                titleBlockInstance.Location as LocationPoint;

            if (locationPoint == null)
            {
                return null;
            }

            return locationPoint.Point;
        }
    }
}