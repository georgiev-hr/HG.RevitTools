using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using HG.RevitTools.Models.ScopeBoxViews;
using HG.RevitTools.Services;
using HG.RevitTools.Services.ScopeBoxViews;
using HG.RevitTools.Views;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HG.RevitTools.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class CreateViewsFromScopeBoxesCommand : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIDocument uiDocument =
                commandData.Application.ActiveUIDocument;

            Document document =
                uiDocument.Document;

            try
            {
                RevitElementCollectorService collectorService =
                    new RevitElementCollectorService();

                ScopeBoxViewCreationService creationService =
                    new ScopeBoxViewCreationService();

                IList<ViewPlan> sourceViews =
                    collectorService.GetFloorPlans(document);

                ElementSelector viewSelector =
                    new ElementSelector(
                        sourceViews.Cast<object>().ToList(),
                        item => ((View)item).Name,
                        "Select Source View");

                bool? viewDialogResult =
                    viewSelector.ShowDialog();

                if (viewDialogResult != true)
                {
                    return Result.Cancelled;
                }

                View selectedView =
                    viewSelector
                        .GetSelectedItems<ViewPlan>()
                        .FirstOrDefault();

                if (selectedView == null)
                {
                    TaskDialog.Show(
                        "Scope Box Views",
                        "No source view was selected.");

                    return Result.Cancelled;
                }

                IList<Element> scopeBoxes =
                    collectorService.GetScopeBoxes(document);

                ElementSelector scopeBoxSelector =
                    new ElementSelector(
                        scopeBoxes.Cast<object>().ToList(),
                        item => ((Element)item).Name,
                        "Select Scope Boxes");

                bool? scopeBoxDialogResult =
                    scopeBoxSelector.ShowDialog();

                if (scopeBoxDialogResult != true)
                {
                    return Result.Cancelled;
                }

                IList<Element> selectedScopeBoxes =
                    scopeBoxSelector.GetSelectedItems<Element>();

                if (selectedScopeBoxes.Count == 0)
                {
                    TaskDialog.Show(
                        "Scope Box Views",
                        "No scope boxes were selected.");

                    return Result.Cancelled;
                }

                ScopeBoxViewCreationResult result;

                using (Transaction transaction =
                    new Transaction(
                        document,
                        "Create Dependent Views From Scope Boxes"))
                {
                    transaction.Start();

                    result =
                        creationService.CreateDependentViews(
                            document,
                            selectedView,
                            selectedScopeBoxes);

                    transaction.Commit();
                }

                ShowResult(result);

                return Result.Succeeded;
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Cancelled;
            }
            catch (Exception ex)
            {
                message = ex.Message;

                return Result.Failed;
            }
        }

        private void ShowResult(
            ScopeBoxViewCreationResult result)
        {
            string resultMessage =
                $"Created: {result.CreatedCount}\n" +
                $"Failed: {result.FailedCount}";

            if (result.Messages.Count > 0)
            {
                resultMessage +=
                    "\n\nMessages:\n" +
                    string.Join("\n", result.Messages);
            }

            TaskDialog.Show(
                "Scope Box Views",
                resultMessage);
        }
    }
}