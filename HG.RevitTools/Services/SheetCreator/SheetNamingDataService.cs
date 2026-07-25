using Autodesk.Revit.DB;
using HG.RevitTools.Models.SheetCreator;
using System.Collections.Generic;
using System.Linq;

namespace HG.RevitTools.Services.SheetCreator
{
    public class SheetNamingDataService
    {
        private readonly ScopeBoxService scopeBoxService;

        public SheetNamingDataService()
        {
            scopeBoxService =
                new ScopeBoxService();
        }

        public SheetNamingData BuildNamingDataFromView(
            Document document,
            ViewPlan view,
            int serialNumber)
        {
            return new SheetNamingData
            {
                ProjectNumber = GetProjectNumber(document),
                Originator = "SPK",
                SystemFacility = "B01",
                Phase = "TD",
                Location = GetLocationCodeFromScopeBox(view),
                Type = "DW",
                RoleDiscipline = "EL",
                Level = "ZZ",
                Package = "1",
                Scale = GetScaleCode(view.Scale),
                SerialNo = serialNumber.ToString("000"),
                RevisionNo = GetLatestProjectRevision(document),
                DrawingTitle = view.Name
            };
        }

        private string GetProjectNumber(
            Document document)
        {
            string value =
                document.ProjectInformation.Number;

            return string.IsNullOrWhiteSpace(value)
                ? "BIA1"
                : value.Trim();
        }

        private string GetLocationCodeFromScopeBox(
            ViewPlan view)
        {
            Element scopeBox =
                scopeBoxService.GetScopeBoxFromView(
                    view);

            if (scopeBox == null)
            {
                return "XX";
            }

            string[] nameParts =
                scopeBox.Name.Split('.');

            if (nameParts.Length < 2)
            {
                return "XX";
            }

            return nameParts[1];
        }

        private string GetScaleCode(
            int viewScale)
        {
            switch (viewScale)
            {
                case 50:
                    return "1";

                case 100:
                    return "2";

                case 750:
                    return "3";

                case 200:
                    return "4";

                default:
                    return "0";
            }
        }

        private string GetLatestProjectRevision(
            Document document)
        {
            IList<ElementId> revisionIds =
                Revision.GetAllRevisionIds(document);

            if (revisionIds == null || revisionIds.Count == 0)
            {
                return "00";
            }

            Revision latestRevision =
                revisionIds
                    .Select(id => document.GetElement(id))
                    .OfType<Revision>()
                    .OrderByDescending(revision => revision.SequenceNumber)
                    .FirstOrDefault();

            if (latestRevision == null)
            {
                return "00";
            }

            return latestRevision.SequenceNumber.ToString("00");
        }
    }
}