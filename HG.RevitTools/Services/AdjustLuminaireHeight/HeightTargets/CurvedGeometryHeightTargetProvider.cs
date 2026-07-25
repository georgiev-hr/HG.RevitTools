using Autodesk.Revit.DB;
using HG.RevitTools.Models.AdjustLuminaireHeight;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HG.RevitTools.Services.AdjustLuminaireHeight.HeightTargets
{
    public class CurvedGeometryHeightTargetProvider : IHeightTargetProvider
    {
        private readonly ReferenceIntersector referenceIntersector;
        private readonly XYZ searchDirection;
        private readonly IList<TargetGeometrySelectionResult> selectedTargets;

        public CurvedGeometryHeightTargetProvider(
            ElementFilter targetFilter,
            View3D view3D,
            XYZ searchDirection,
            IList<TargetGeometrySelectionResult> selectedTargets)
        {
            this.searchDirection =
                searchDirection.Normalize();

            this.selectedTargets =
                selectedTargets;

            referenceIntersector =
                new ReferenceIntersector(
                    targetFilter,
                    FindReferenceTarget.Face,
                    view3D);

            referenceIntersector.FindReferencesInRevitLinks = true;
        }

        public double GetVerticalDifferenceFeet(
    XYZ insertionPoint)
        {
            IList<ReferenceWithContext> allHits =
                referenceIntersector
                    .Find(
                        insertionPoint,
                        searchDirection)
                    .ToList();

            IList<ReferenceWithContext> selectedHits =
                allHits
                    .Where(hit => IsHitFromSelectedTarget(
                        hit.GetReference()))
                    .ToList();

            if (selectedHits.Count == 0)
            {
                throw new InvalidOperationException(
                    "No vertical intersection was found with the selected target geometry.");
            }

            ReferenceWithContext bestHit =
                ChooseNearestHit(selectedHits);

            XYZ intersectionPoint =
                bestHit.GetReference().GlobalPoint;

            return intersectionPoint.Z - insertionPoint.Z;
        }

        private bool IsHitFromSelectedTarget(
    Reference hitReference)
        {
            foreach (TargetGeometrySelectionResult selectedTarget in selectedTargets)
            {
                if (selectedTarget.IsFromLink)
                {
                    bool sameLinkInstance =
                        hitReference.ElementId == selectedTarget.HostElementId;

                    bool sameLinkedElement =
                        hitReference.LinkedElementId == selectedTarget.LinkedElementId;

                    if (sameLinkInstance && sameLinkedElement)
                    {
                        return true;
                    }
                }
                else
                {
                    bool sameHostElement =
                        hitReference.ElementId == selectedTarget.HostElementId;

                    bool hitIsNotLinked =
                        hitReference.LinkedElementId == ElementId.InvalidElementId;

                    if (sameHostElement && hitIsNotLinked)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private ReferenceWithContext ChooseNearestHit(
            IList<ReferenceWithContext> hits)
        {
            return hits
                .OrderBy(hit => hit.Proximity)
                .FirstOrDefault();
        }
    }
}