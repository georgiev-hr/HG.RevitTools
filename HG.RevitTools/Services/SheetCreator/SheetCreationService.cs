using Autodesk.Revit.DB;
using HG.RevitTools.Models.SheetCreator;
using System.Collections.Generic;
using System.Linq;

namespace HG.RevitTools.Services.SheetCreator
{
    public class SheetCreationService
    {
        public SheetCreationResult CreateSheets(
            Document document,
            IList<ViewPlan> selectedViews,
            FamilySymbol selectedTitleBlock,
            string sheetNameTemplate)
        {

            SheetCreationResult result =
                new SheetCreationResult();            
                
                
                int serialCounter = 101;

                var sheetNamingService =
                    new SheetNamingService();

                var sheetNumberService =
                    new SheetNumberService();

                var titleBlockService =
                    new TitleBlockService();

                var scopeBoxService =
                    new ScopeBoxService();

                var sheetNamingDataService =
                    new SheetNamingDataService();

                var viewportPlacementService =
                    new ViewportPlacementService();

                titleBlockService.ActivateTitleBlock(
                    document,
                    selectedTitleBlock);


                foreach (ViewPlan view in selectedViews)
                {
                    ViewSheet newSheet = ViewSheet.Create(
                        document,
                        selectedTitleBlock.Id);

                    if (newSheet == null)
                    {
                        result.FailedCount++;
                        result.Messages.Add(
                            $"Could not create sheet for view '{view.Name}'.");

                        continue;
                    }

                    SheetNamingData namingData = sheetNamingDataService.BuildNamingDataFromView(
                        document,
                        view,
                        serialCounter);

                    string finalNumber =
                        sheetNumberService.CreateUniqueSheetNumber(
                            document,
                            sheetNamingService,
                            namingData,
                            ref serialCounter);

                    newSheet.Name =
                        sheetNamingService.GenerateSheetName(
                            namingData,
                            sheetNameTemplate);

                    newSheet.SheetNumber = finalNumber;

                    serialCounter++;

                    FamilyInstance titleBlockInstance =
                        titleBlockService.GetTitleBlockInstance(
                            document,
                            newSheet);

                    if (titleBlockInstance == null)
                    {
                        result.FailedCount++;
                        result.Messages.Add(
                            $"Could not find title block instance on sheet '{newSheet.SheetNumber}'.");

                        continue;
                    }

                    XYZ titleBlockLocation =
                        titleBlockService.GetTitleBlockLocation(
                            titleBlockInstance);

                    if (titleBlockLocation == null)
                    {
                        result.FailedCount++;
                        result.Messages.Add(
                            $"Title block on sheet '{newSheet.SheetNumber}' has no location point.");

                        continue;
                    }

                    Element scopeBox =
                        scopeBoxService.GetScopeBoxFromView(
                            view);

                    if (scopeBox == null)
                    {
                        result.FailedCount++;
                        result.Messages.Add(
                            $"View '{view.Name}' has no assigned scope box.");

                        continue;
                    }

                    BoundingBoxXYZ scopeBoxBoundingBox =
                        scopeBoxService.GetScopeBoxBoundingBox(
                            scopeBox);

                    if (scopeBoxBoundingBox == null)
                    {
                        result.FailedCount++;
                        result.Messages.Add(
                            $"Scope box for view '{view.Name}' has no bounding box.");

                        continue;
                    }

                    double scopeWidth =
                        scopeBoxService.GetScopeBoxWidth(
                            scopeBoxBoundingBox);

                    double scopeHeight =
                        scopeBoxService.GetScopeBoxHeight(
                            scopeBoxBoundingBox);

                    bool viewPlaced = viewportPlacementService.TryPlaceViewOnSheet(
                        document,
                        newSheet,
                        view,
                        titleBlockLocation,
                        scopeWidth,
                        scopeHeight,
                        out string viewportMessage);

                    if (!viewPlaced)
                    {
                        result.FailedCount++;
                        result.Messages.Add(viewportMessage);

                        continue;
                    }

                    result.CreatedCount++;
                    result.Messages.Add(
                        $"Created sheet '{newSheet.SheetNumber}' for view '{view.Name}'.");
                }
            return result;
        }
    }
}