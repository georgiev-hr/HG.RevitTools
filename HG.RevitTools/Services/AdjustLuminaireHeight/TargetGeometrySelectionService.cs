using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using HG.RevitTools.Models.AdjustLuminaireHeight;
using System;
using System.Collections.Generic;

namespace HG.RevitTools.Services.AdjustLuminaireHeight
{
    public class TargetGeometrySelectionService
    {
        public IList<TargetGeometrySelectionResult> PickTargetGeometry(
            UIDocument uiDocument)
        {
            Document document =
                uiDocument.Document;

            IList<Reference> references =
                uiDocument.Selection.PickObjects(
                    ObjectType.PointOnElement,
                    "Select target geometry from host or linked models");

            List<TargetGeometrySelectionResult> results =
                new List<TargetGeometrySelectionResult>();

            foreach (Reference reference in references)
            {
                TargetGeometrySelectionResult result =
                    CreateTargetGeometrySelectionResult(
                        document,
                        reference);

                results.Add(result);
            }

            return results;
        }

        private TargetGeometrySelectionResult CreateTargetGeometrySelectionResult(
            Document document,
            Reference reference)
        {
            if (IsLinkedReference(document, reference))
            {
                return CreateLinkedTargetGeometrySelectionResult(
                    document,
                    reference);
            }

            return CreateHostTargetGeometrySelectionResult(
                document,
                reference);
        }

        private bool IsLinkedReference(
            Document document,
            Reference reference)
        {
            Element element =
                document.GetElement(reference.ElementId);

            return element is RevitLinkInstance;
        }

        private TargetGeometrySelectionResult CreateHostTargetGeometrySelectionResult(
            Document document,
            Reference reference)
        {
            Element element =
                document.GetElement(reference.ElementId);

            return new TargetGeometrySelectionResult
            {
                PickedReference = reference,
                SourceElement = element,

                HostElementId = reference.ElementId,
                LinkedElementId = ElementId.InvalidElementId,

                LinkInstance = null,
                PickedFace = null,
                TransformToHost = Transform.Identity,
                PickedPoint = reference.GlobalPoint,
                IsFromLink = false
            };
        }

        private TargetGeometrySelectionResult CreateLinkedTargetGeometrySelectionResult(
            Document document,
            Reference reference)
        {
            RevitLinkInstance linkInstance =
                GetLinkInstance(document, reference);

            Element linkedElement =
                GetLinkedElement(linkInstance, reference);

            return new TargetGeometrySelectionResult
            {
                PickedReference = reference,
                SourceElement = linkedElement,

                HostElementId = reference.ElementId,
                LinkedElementId = reference.LinkedElementId,

                LinkInstance = linkInstance,
                PickedFace = null,
                TransformToHost = linkInstance.GetTotalTransform(),
                PickedPoint = reference.GlobalPoint,
                IsFromLink = true
            };
        }

        private RevitLinkInstance GetLinkInstance(
            Document document,
            Reference reference)
        {
            Element element =
                document.GetElement(reference.ElementId);

            RevitLinkInstance linkInstance =
                element as RevitLinkInstance;

            if (linkInstance == null)
            {
                throw new InvalidOperationException(
                    "Selected reference does not belong to a Revit link.");
            }

            return linkInstance;
        }

        private Element GetLinkedElement(
            RevitLinkInstance linkInstance,
            Reference reference)
        {
            Document linkedDocument =
                linkInstance.GetLinkDocument();

            if (linkedDocument == null)
            {
                throw new InvalidOperationException(
                    "Could not access the linked document.");
            }

            Element linkedElement =
                linkedDocument.GetElement(reference.LinkedElementId);

            if (linkedElement == null)
            {
                throw new InvalidOperationException(
                    "Could not find the selected element inside the linked document.");
            }

            return linkedElement;
        }
    }
}