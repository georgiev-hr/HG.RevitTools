using Autodesk.Revit.DB;
using System.Linq;

namespace HG.RevitTools.Services.ScopeBoxViews
{
    public class ScopeBoxViewNamingService
    {
        public string CreateViewName(
            View sourceView,
            Element scopeBox)
        {
            string scopeBoxSuffix =
        ExtractScopeBoxSuffix(scopeBox.Name);

            return $"{sourceView.Name} - {scopeBoxSuffix}";
        }

        public string CreateUniqueViewName(
            Document document,
            string desiredName)
        {
            if (!ViewNameExists(document, desiredName))
            {
                return desiredName;
            }

            int counter = 1;

            string uniqueName =
                $"{desiredName} ({counter})";

            while (ViewNameExists(document, uniqueName))
            {
                counter++;

                uniqueName =
                    $"{desiredName} ({counter})";
            }

            return uniqueName;
        }

        private bool ViewNameExists(
            Document document,
            string viewName)
        {
            return new FilteredElementCollector(document)
                .OfClass(typeof(View))
                .Cast<View>()
                .Any(view => view.Name == viewName);
        }

        private string ExtractScopeBoxSuffix(
            string scopeBoxName)
        {
            string[] nameParts = scopeBoxName.Split('.');

            if (nameParts.Length < 3)
            {
                return scopeBoxName;
            }

            string part1 = nameParts[1];
            string part2 = nameParts[2];

            return $"{part1}.{part2}";
        }
    }
}