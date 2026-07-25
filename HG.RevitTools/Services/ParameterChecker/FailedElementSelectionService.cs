using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;

namespace HG.RevitTools.Services.ParameterChecker
{
    public class FailedElementSelectionService
    {
        public int SelectElements(
            UIDocument uiDocument,
            IEnumerable<ElementId> elementIds)
        {
            if (uiDocument == null)
            {
                return 0;
            }

            List<ElementId> validElementIds =
                elementIds?
                    .Where(elementId =>
                        elementId != null &&
                        elementId != ElementId.InvalidElementId &&
                        uiDocument.Document.GetElement(elementId) != null)
                    .Distinct()
                    .ToList()
                ?? new List<ElementId>();

            uiDocument.Selection.SetElementIds(
                validElementIds);

            return validElementIds.Count;
        }
    }
}