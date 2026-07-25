using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using HG.RevitTools.Models.ParameterChecker;
using HG.RevitTools.Services.ParameterChecker;
using HG.RevitTools.ViewModels.ParameterChecker;
using HG.RevitTools.Views.ParameterChecker;
using System.Collections.Generic;

namespace HG.RevitTools.Commands.ParameterChecker
{
    [Transaction(TransactionMode.Manual)]
    public class CheckLightingFixtureParametersCommand
        : IExternalCommand
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

            LightingFixtureCollectorService collectorService =
                new LightingFixtureCollectorService();

            CommonParameterService commonParameterService =
                new CommonParameterService();

            ParameterValueService parameterValueService =
                new ParameterValueService();

            ParameterComparisonService comparisonService =
                new ParameterComparisonService();

            FailedElementSelectionService selectionService =
                new FailedElementSelectionService();

            List<Element> lightingFixtures =
                collectorService
                    .GetLightingFixtureInstances(document);

            if (lightingFixtures.Count == 0)
            {
                TaskDialog.Show(
                    "Parameter Checker",
                    "No lighting fixture instances were found.");

                return Result.Cancelled;
            }

            List<ParameterOption> commonParameters =
                commonParameterService
                    .GetCommonParameters(lightingFixtures);

            if (commonParameters.Count == 0)
            {
                TaskDialog.Show(
                    "Parameter Checker",
                    "No common instance parameters were found.");

                return Result.Cancelled;
            }

            ParameterCheckerViewModel viewModel =
                new ParameterCheckerViewModel(
                    document,
                    lightingFixtures,
                    commonParameters,
                    parameterValueService,
                    "Check lighting fixture parameter values");

            ParameterCheckerWindow window =
                new ParameterCheckerWindow(viewModel);

            bool? dialogResult =
                window.ShowDialog();

            if (dialogResult != true)
            {
                return Result.Cancelled;
            }

            ParameterCheckResult checkResult =
                comparisonService.CheckElements(
                    lightingFixtures,
                    viewModel.SelectedParameter,
                    viewModel.SelectedValue);

            int selectedCount =
                selectionService.SelectElements(
                    uiDocument,
                    checkResult.MismatchingElementIds);

            TaskDialog.Show(
                "Parameter Checker",
                $"Parameter: " +
                $"{viewModel.SelectedParameter.Name}\n" +

                $"Expected value: " +
                $"{viewModel.SelectedValue.DisplayValue}\n\n" +

                $"Checked: {checkResult.CheckedCount}\n" +
                $"Matching: {checkResult.MatchingCount}\n" +
                $"Mismatching: {checkResult.MismatchingCount}\n" +
                $"Selected: {selectedCount}");

            return Result.Succeeded;
        }
    }
}