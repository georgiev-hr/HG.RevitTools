using Autodesk.Revit.DB;
using System;

namespace HG.RevitTools.Services.SheetCreator
{
    public class ViewportPlacementService
    {
        public bool TryPlaceViewOnSheet(
            Document document,
            ViewSheet sheet,
            ViewPlan view,
            XYZ titleBlockLocation,
            double scopeWidth,
            double scopeHeight,
            out string message)
        {
            message = string.Empty;

            XYZ viewportLocation =
                DefineLocation(
                    titleBlockLocation,
                    scopeWidth,
                    scopeHeight,
                    view.Scale);

            if (!Viewport.CanAddViewToSheet(
                document,
                sheet.Id,
                view.Id))
            {
                message =
                    $"View '{view.Name}' could not be placed on sheet '{sheet.SheetNumber}'.";

                return false;
            }

            Viewport.Create(
                document,
                sheet.Id,
                view.Id,
                viewportLocation);

            return true;
        }

        private XYZ DefineLocation(
            XYZ bottomLeft,
            double scopeWidth,
            double scopeHeight,
            int viewScale)
        {
            double paperWidth =
                scopeWidth / viewScale;

            double paperHeight =
                scopeHeight / viewScale;

            double marginX =
                0.30;

            double marginY =
                0.30;

            return new XYZ(
                bottomLeft.X + marginX + paperWidth / 2.0,
                bottomLeft.Y + marginY + paperHeight / 2.0,
                0);
        }
    }
}