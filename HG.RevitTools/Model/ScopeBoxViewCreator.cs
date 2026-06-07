using Autodesk.Revit.DB;

using System.Collections.Generic;

namespace HG.RevitTools.Models
{
    public class ScopeBoxViewCreator
    {
        public void CreateDependentViews(
            Document document,
            View sourceView,
            IList<Element> scopeBoxes)
        {
            using (Transaction transaction = new Transaction(
                document,
                "Create Dependent Views From Scope Boxes"))
            {
                transaction.Start();

                foreach (Element scopeBox in scopeBoxes)
                {
                    CreateDependentView(document, sourceView, scopeBox);
                }

                transaction.Commit();
            }
        }

        private void CreateDependentView(
            Document document,
            View sourceView,
            Element scopeBox)
        {
            ElementId newViewId = sourceView.Duplicate(
                ViewDuplicateOption.AsDependent);

            View dependentView = document.GetElement(newViewId) as View;

            if (dependentView == null)
            {
                return;
            }

            dependentView.Name = CreateViewName(
                sourceView.Name,
                scopeBox.Name);

            Parameter scopeBoxParameter = dependentView.get_Parameter(
                BuiltInParameter.VIEWER_VOLUME_OF_INTEREST_CROP);

            if (scopeBoxParameter != null && !scopeBoxParameter.IsReadOnly)
            {
                scopeBoxParameter.Set(scopeBox.Id);
            }
        }

        private string CreateViewName(
            string sourceViewName,
            string scopeBoxName)
        {
            string[] nameParts = scopeBoxName.Split('.');

            if (nameParts.Length < 3)
            {
                return $"{sourceViewName} - {scopeBoxName}";
            }

            string part1 = nameParts[1];
            string part2 = nameParts[2];

            return $"{sourceViewName} - {part1}.{part2}";
        }
    }
}