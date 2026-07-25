using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace HG.RevitTools.Services.AdjustLuminaireHeight
{
    public class TargetGeometryFilterService
    {
        public ElementFilter CreateFloorTopoCeilingFilter()
        {
            List<ElementFilter> filters =
                new List<ElementFilter>
                {
                    new ElementCategoryFilter(
                        BuiltInCategory.OST_Floors),

                    new ElementCategoryFilter(
                        BuiltInCategory.OST_Topography),

                    new ElementCategoryFilter(
                        BuiltInCategory.OST_Ceilings),

                    new ElementCategoryFilter(
                        BuiltInCategory.OST_GenericModel)
                };

            return new LogicalOrFilter(filters);
        }
    }
}