using Autodesk.Revit.DB;

namespace HG.RevitTools.Services.SheetCreator
{
    public class ScopeBoxService
    {
        public Element GetScopeBoxFromView(
            ViewPlan view)
        {
            Parameter scopeBoxParameter =
                view.get_Parameter(
                    BuiltInParameter.VIEWER_VOLUME_OF_INTEREST_CROP);

            if (scopeBoxParameter == null)
            {
                return null;
            }

            ElementId scopeBoxId =
                scopeBoxParameter.AsElementId();

            if (scopeBoxId == ElementId.InvalidElementId)
            {
                return null;
            }

            return view.Document.GetElement(scopeBoxId);
        }

        public BoundingBoxXYZ GetScopeBoxBoundingBox(
            Element scopeBox)
        {
            if (scopeBox == null)
            {
                return null;
            }

            return scopeBox.get_BoundingBox(null);
        }

        public double GetScopeBoxWidth(
            BoundingBoxXYZ boundingBox)
        {
            return boundingBox.Max.X - boundingBox.Min.X;
        }

        public double GetScopeBoxHeight(
            BoundingBoxXYZ boundingBox)
        {
            return boundingBox.Max.Y - boundingBox.Min.Y;
        }
    }
}