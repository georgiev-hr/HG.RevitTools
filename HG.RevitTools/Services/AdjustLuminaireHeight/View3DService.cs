using Autodesk.Revit.DB;
using System;
using System.Linq;

namespace HG.RevitTools.Services.AdjustLuminaireHeight
{
    public class View3DService
    {
        public View3D GetNonTemplate3DView(
            Document document)
        {
            View3D view3D =
                new FilteredElementCollector(document)
                    .OfClass(typeof(View3D))
                    .Cast<View3D>()
                    .FirstOrDefault(view => !view.IsTemplate);

            if (view3D == null)
            {
                throw new InvalidOperationException(
                    "No non-template 3D view was found.");
            }

            return view3D;
        }
    }
}