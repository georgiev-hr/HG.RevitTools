using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;
using static Autodesk.Revit.DB.SpecTypeId;
using HG.RevitTools.Views;
using HG.RevitTools.Services;
using HG.RevitTools.Models.SheetCreator;
using HG.RevitTools.Services.SheetCreator;

namespace HG.RevitTools.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class CreateSheetsCommand : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            try
            {
                Document doc =
                    commandData.Application.ActiveUIDocument.Document;

                RevitElementCollectorService collectorService =
                    new RevitElementCollectorService();

                IList<ViewPlan> viewsWithScopebox =
                    collectorService.GetUnplacedViewsWithScopeBoxes(doc);

                ElementSelector viewsSelector =
                    new ElementSelector(
                        viewsWithScopebox,
                        view => ((Element)view).Name,
                        "Select Views");

                IList<ViewPlan> selectedViews = null;

                if (viewsSelector.ShowDialog() == true)
                {
                    selectedViews =
                        viewsSelector.GetSelectedItems<ViewPlan>();
                }

                if (selectedViews == null || !selectedViews.Any())
                {
                    return Result.Cancelled;
                }

                IList<FamilySymbol> titleBlockTypes =
                    collectorService.GetTitleBlocks(doc);

                if (!titleBlockTypes.Any())
                {
                    TaskDialog.Show(
                        "Create Sheets",
                        "No title block types found in the project.");

                    return Result.Failed;
                }

                ElementSelector titleBlockSelector =
                    new ElementSelector(
                        titleBlockTypes,
                        titleBlock => ((FamilySymbol)titleBlock).FamilyName +
                                      " : " +
                                      ((Element)titleBlock).Name,
                        "Select Title Block");

                FamilySymbol selectedTitleBlock = null;

                if (titleBlockSelector.ShowDialog() == true)
                {
                    selectedTitleBlock =
                        titleBlockSelector
                            .GetSelectedItems<FamilySymbol>()
                            .FirstOrDefault();
                }

                if (selectedTitleBlock == null)
                {
                    return Result.Cancelled;
                }

                SheetNamingFormatView namingFormatView =
                    new SheetNamingFormatView();

                if (namingFormatView.ShowDialog() != true)
                {
                    return Result.Cancelled;
                }

                string sheetNameTemplate =
                    namingFormatView.SheetNameTemplate;

                SheetCreationService sheetCreationService =
                    new SheetCreationService();

                SheetCreationResult result;

                using (Transaction transaction =
                    new Transaction(doc, "Create Sheets"))
                {
                    transaction.Start();

                    result =
                        sheetCreationService.CreateSheets(
                            doc,
                            selectedViews,
                            selectedTitleBlock,
                            sheetNameTemplate);

                    transaction.Commit();
                }

                ShowResult(result);

                return Result.Succeeded;
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Cancelled;
            }
            catch (System.Exception ex)
            {
                message = ex.Message;

                TaskDialog.Show(
                    "Create Sheets - Error",
                    ex.ToString());

                return Result.Failed;
            }
        }

        private void ShowResult(
    SheetCreationResult result)
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
                "Create Sheets",
                resultMessage);
        }


    }
}