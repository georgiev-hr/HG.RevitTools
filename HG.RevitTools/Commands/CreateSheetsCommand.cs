using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;
using static Autodesk.Revit.DB.SpecTypeId;
using HG.RevitTools.Views;
using HG.RevitTools.Services;

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
            Document doc = commandData.Application.ActiveUIDocument.Document;

            var collectorService = new RevitElementCollectorService();

            var viewsWithScopebox = collectorService.GetUnplacedViewsWithScopeBoxes(doc);

            var viewsSelector = new ElementSelector(
                viewsWithScopebox,
                v => ((Element)v).Name,
                "Select Views"
            );

            IList<ViewPlan> selectedViews = null;

            if (viewsSelector.ShowDialog() == true)
            {
                selectedViews = viewsSelector.GetSelectedItems<ViewPlan>();
            }

            if (selectedViews == null || !selectedViews.Any())
                return Result.Cancelled;

            // Collect title block types
            var titleBlockTypes = collectorService.GetTitleBlocks(doc);

            if (!titleBlockTypes.Any())
            {
                TaskDialog.Show("Create Sheets", "No title block types found in the project.");
                return Result.Failed;
            }

            // Let user select one title block
            var titleBlockSelector = new ElementSelector(
                titleBlockTypes,
                tb => ((FamilySymbol)tb).FamilyName + " : " + ((Element)tb).Name,
                "Select Title Block"
            );

            FamilySymbol selectedTitleBlock = null;

            if (titleBlockSelector.ShowDialog() == true)
            {
                selectedTitleBlock = titleBlockSelector
                    .GetSelectedItems<FamilySymbol>()
                    .FirstOrDefault();
            }

            if (selectedTitleBlock == null)
                return Result.Cancelled;

            using (Transaction t = new Transaction(doc, "Create Sheets"))
            {
                t.Start();

                if (!selectedTitleBlock.IsActive)
                {
                    selectedTitleBlock.Activate();
                    doc.Regenerate();
                }

                int serialCounter = 101;

                foreach (ViewPlan view in selectedViews)
                {
                    ViewSheet newSheet = ViewSheet.Create(doc, selectedTitleBlock.Id);
                    if (newSheet == null)
                        continue;

                    SheetNamingData namingData = BuildNamingDataFromView(doc, view, serialCounter);

                    string finalNumber = BuildSheetNumber(namingData);
                    while (SheetNumberExists(doc, finalNumber))
                    {
                        serialCounter++;
                        namingData.SerialNo = serialCounter.ToString("000");
                        finalNumber = BuildSheetNumber(namingData);
                    }

                    try
                    {
                        newSheet.Name = BuildSheetNumber(namingData);
                    }
                    catch
                    {
                    }

                    newSheet.SheetNumber = finalNumber;
                    serialCounter++;

                    FamilyInstance titleBlockInstance = new FilteredElementCollector(doc, newSheet.Id)
                        .OfCategory(BuiltInCategory.OST_TitleBlocks)
                        .WhereElementIsNotElementType()
                        .Cast<FamilyInstance>()
                        .FirstOrDefault();

                    if (titleBlockInstance == null)
                        continue;

                    LocationPoint lp = titleBlockInstance.Location as LocationPoint;
                    if (lp == null)
                        continue;

                    XYZ bottomLeft = lp.Point;

                    Parameter scopeParam = view.get_Parameter(BuiltInParameter.VIEWER_VOLUME_OF_INTEREST_CROP);
                    if (scopeParam == null)
                        continue;

                    Element scopeBox = doc.GetElement(scopeParam.AsElementId());
                    if (scopeBox == null)
                        continue;

                    BoundingBoxXYZ sbBox = scopeBox.get_BoundingBox(null);
                    if (sbBox == null)
                        continue;

                    double scopeWidth = sbBox.Max.X - sbBox.Min.X;
                    double scopeHeight = sbBox.Max.Y - sbBox.Min.Y;

                    XYZ location = DefineLocation(
                        bottomLeft,
                        scopeWidth,
                        scopeHeight,
                        view.Scale
                    );

                    if (Viewport.CanAddViewToSheet(doc, newSheet.Id, view.Id))
                    {
                        Viewport.Create(doc, newSheet.Id, view.Id, location);
                    }
                }

                t.Commit();
            }

            return Result.Succeeded;
        }

        private XYZ DefineLocation(
            XYZ bottomLeft,
            double scopeWidth,
            double scopeHeight,
            int viewScale)
        {
            // Convert model size to approximate sheet size
            double paperWidth = scopeWidth / viewScale;
            double paperHeight = scopeHeight / viewScale;

            // Margins from title block bottom-left
            double marginX = 0.30;
            double marginY = 0.30;

            // Viewport.Create expects the CENTER point
            return new XYZ(
                bottomLeft.X + marginX + paperWidth / 2.0,
                bottomLeft.Y + marginY + paperHeight / 2.0,
                0);
        }

        private bool SheetNumberExists(Document doc, string sheetNumber)
        {
            return new FilteredElementCollector(doc)
                .OfClass(typeof(ViewSheet))
                .Cast<ViewSheet>()
                .Any(s => s.SheetNumber == sheetNumber);
        }

        private class SheetNamingData
        {
            public string ProjectNumber { get; set; }
            public string Originator { get; set; }
            public string SystemFacility { get; set; }
            public string Phase { get; set; }
            public string Location { get; set; }
            public string Type { get; set; }
            public string RoleDiscipline { get; set; }
            public string Level { get; set; }
            public string Package { get; set; }
            public string Scale { get; set; }
            public string SerialNo { get; set; }
            public string RevisionNo { get; set; }
            public string DrawingTitle { get; set; }
        }

        private SheetNamingData BuildNamingDataFromView(Document doc, ViewPlan view, int serialNo)
        {
            return new SheetNamingData
            {
                // this
                ProjectNumber = "BIA1",
                Originator = "SPK",
                // this
                SystemFacility = "B01",
                Phase = "TD",
                // this
                Location = GetLocationCodeFromScopeBox(view),
                Type = "DW",
                RoleDiscipline = "EL",
                // this
                Level = "ZZ",
                Package = "1",
                Scale = GetScaleCode(view.Scale),
                SerialNo = serialNo.ToString("000"),
                RevisionNo = "00",
                DrawingTitle = view.Name
            };
        }

        private string BuildSheetNumber(SheetNamingData d)
        {
            return $"{d.ProjectNumber}-{d.Originator}-" +
                $"{d.SystemFacility}-{d.Phase}-{d.Location}-" +
                $"{d.Type}-{d.RoleDiscipline}-{d.Level}-" +
                $"{d.Package}{d.Scale}{d.SerialNo}-{d.RevisionNo}";
        }

        private string BuildSheetName(SheetNamingData d)
        {
            return d.DrawingTitle;
        }

        private string GetProjectNumber(Document doc)
        {
            string value = doc.ProjectInformation.Number;
            return string.IsNullOrWhiteSpace(value) ? "BIA1" : value.Trim();
        }

        private string GetSystemFacility(Document doc)
        {
            Parameter p = doc.ProjectInformation.LookupParameter("System / Facility");
            if (p != null && p.HasValue)
            {
                string value = p.AsString();
                if (!string.IsNullOrWhiteSpace(value))
                    return value.Trim();
            }

            return "B01";
        }

        private string GetPhaseCode(View view)
        {
            Parameter p = view.get_Parameter(BuiltInParameter.VIEW_PHASE);
            if (p != null && p.HasValue)
            {
                ElementId phaseId = p.AsElementId();
                if (phaseId != ElementId.InvalidElementId)
                {
                    Phase phase = view.Document.GetElement(phaseId) as Phase;
                    if (phase != null)
                    {
                        string phaseName = phase.Name.Trim().ToUpper();

                        switch (phaseName)
                        {
                            case "DD":
                            case "DETAILED DESIGN":
                                return "DD";
                            case "CD":
                            case "CONCEPT DESIGN":
                                return "CD";
                            default:
                                return phase.Name.Trim().ToUpper();
                        }
                    }
                }
            }

            return "DD";
        }

        private string GetLocationCodeFromScopeBox(ViewPlan view)
        {
            Parameter scopeParam = view.get_Parameter(BuiltInParameter.VIEWER_VOLUME_OF_INTEREST_CROP);
            if (scopeParam == null)
                return "XX";

            ElementId scopeBoxId = scopeParam.AsElementId();
            if (scopeBoxId == ElementId.InvalidElementId)
                return "XX";

            Element scopeBox = view.Document.GetElement(scopeBoxId);
            if (scopeBox == null)
                return "XX";

            string scopeBoxName = scopeBox.Name;
            string result = scopeBoxName.Split('.')[1];

            return result;
        }

        private string GetRoleDiscipline(ViewPlan view)
        {
            string upperName = view.Name.ToUpper();

            if (upperName.Contains("LIGHTING"))
                return "EL";

            if (upperName.Contains("POWER"))
                return "EP";

            if (upperName.Contains("EQUIPMENT"))
                return "ER";

            return "AG";
        }

        private string GetLevelCode(ViewPlan view)
        {
            Level level = view.GenLevel;
            if (level == null)
                return "00";

            string name = level.Name.ToUpper();

            if (name.Contains("B1 MEZZANINE")) return "B1M";
            if (name.Contains("LEVEL B1")) return "B1";
            if (name.Contains("LEVEL 00")) return "00";
            if (name.Contains("LEVEL 01")) return "01";
            if (name.Contains("LEVEL 02")) return "02";
            if (name.Contains("LEVEL 03")) return "03";
            if (name.Contains("LEVEL 04")) return "04";
            if (name.Contains("LEVEL 05")) return "05";

            return level.Name.Trim().ToUpper();
        }

        private string GetScaleCode(int viewScale)
        {
            switch (viewScale)
            {
                case 50: return "1";
                case 100: return "2";
                case 750: return "3";
                case 200: return "4";
                default: return "0";
            }
        }
    }
}