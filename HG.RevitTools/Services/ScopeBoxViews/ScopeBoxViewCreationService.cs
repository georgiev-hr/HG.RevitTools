using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using HG.RevitTools.Models.ScopeBoxViews;

namespace HG.RevitTools.Services.ScopeBoxViews
{
    public class ScopeBoxViewCreationService
    {
        private readonly ScopeBoxViewNamingService namingService;

        public ScopeBoxViewCreationService()
        {
            namingService = new ScopeBoxViewNamingService();
        }

        public ScopeBoxViewCreationResult CreateDependentViews(
            Document document,
            View sourceView,
            IList<Element> scopeBoxes)
        {
            ScopeBoxViewCreationResult result = new ScopeBoxViewCreationResult();


            foreach (Element scopeBox in scopeBoxes)
            {
                bool success = TryCreateDependentView(
                    document,
                    sourceView,
                    scopeBox,
                    out string message);

                if (success)
                {
                    result.CreatedCount++;
                }
                else
                {
                    result.FailedCount++;
                    result.Messages.Add(message);
                }
            }
            return result;
        }

        private bool TryCreateDependentView(
            Document document,
            View sourceView,
            Element scopeBox,
            out string message)
        {
            message = string.Empty;

            if (!CanDuplicateAsDependent(sourceView))
            {
                message = $"View '{sourceView.Name}' cannot be duplicated as dependent.";
                return false;
            }

            View dependentView =
                DuplicateAsDependentView(
                    document,
                    sourceView);

            if (dependentView == null)
            {
                message = $"Could not duplicate view '{sourceView.Name}'.";
                return false;
            }

            string desiredName =
                namingService.CreateViewName(
                    sourceView,
                    scopeBox);

            string uniqueName =
                namingService.CreateUniqueViewName(
                    document,
                    desiredName);

            dependentView.Name = uniqueName;

            bool scopeBoxAssigned =
                TryAssignScopeBox(
                    dependentView,
                    scopeBox,
                    out message);

            if (!scopeBoxAssigned)
            {
                return false;
            }

            message = $"Created '{dependentView.Name}'.";

            return true;
        }

        private View DuplicateAsDependentView(
            Document document,
            View sourceView)
        {
            ElementId newViewId =
                sourceView.Duplicate(
                    ViewDuplicateOption.AsDependent);

            View dependentView =
                document.GetElement(newViewId) as View;

            return dependentView;
        }

        private bool TryAssignScopeBox(
            View dependentView,
            Element scopeBox,
            out string message)
        {
            message = string.Empty;

            Parameter scopeBoxParameter =
                dependentView.get_Parameter(
                    BuiltInParameter.VIEWER_VOLUME_OF_INTEREST_CROP);

            if (scopeBoxParameter == null)
            {
                message = $"View '{dependentView.Name}' does not have a scope box parameter.";
                return false;
            }

            if (scopeBoxParameter.IsReadOnly)
            {
                message = $"Scope box parameter on view '{dependentView.Name}' is read-only.";
                return false;
            }

            scopeBoxParameter.Set(scopeBox.Id);

            return true;
        }

        private bool CanDuplicateAsDependent(
            View sourceView)
        {
            return sourceView.CanViewBeDuplicated(
                ViewDuplicateOption.AsDependent);
        }
    }
}