using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using HG.RevitTools.Models.AdjustLuminaireHeight;
using System;

namespace HG.RevitTools.Services.AdjustLuminaireHeight
{
    public class FaceSelectionService
    {
        public TargetGeometrySelectionResult PickFace(
            UIDocument uiDoc)
        {
            Document document = uiDoc.Document;

            Reference reference = uiDoc.Selection.PickObject(
                ObjectType.PointOnElement,
                "Pick Target Face");

            if (IsLinkedReference(document, reference))
            {
                return CreateLinkedFaceSelectionResult(document, reference);
            }

            return CreateHostFaceSelectionResult(document, reference);
        }

        private bool IsLinkedReference(
            Document document,
            Reference reference)
        {
            Element element = document.GetElement(reference.ElementId);

            return element is RevitLinkInstance;
        }

        private TargetGeometrySelectionResult CreateHostFaceSelectionResult(
            Document document,
            Reference reference)
        {
            Element element =
        document.GetElement(reference.ElementId);

            Face face =
                GetFaceFromReference(
                    element,
                    reference);

            TargetGeometrySelectionResult result =
                new TargetGeometrySelectionResult();

            result.PickedReference = reference;
            result.SourceElement = element;

            result.HostElementId = reference.ElementId;
            result.LinkedElementId = ElementId.InvalidElementId;

            result.LinkInstance = null;
            result.PickedFace = face;
            result.TransformToHost = Transform.Identity;
            result.PickedPoint = reference.GlobalPoint;
            result.IsFromLink = false;

            return result;
        }

        private TargetGeometrySelectionResult CreateLinkedFaceSelectionResult(
            Document document,
            Reference reference)
        {
            RevitLinkInstance linkInstance = GetLinkInstance(document, reference);

            Element linkedElement = GetLinkedElement(linkInstance, reference);

            Face face = GetFaceFromLinkedReference(linkInstance, reference);

            Transform transformToHost = GetTransformToHost(linkInstance);

            TargetGeometrySelectionResult result = new TargetGeometrySelectionResult();

            result.PickedReference = reference;
            result.SourceElement = linkedElement;

            result.HostElementId = reference.ElementId;
            result.LinkedElementId = reference.LinkedElementId;

            result.LinkInstance = linkInstance;
            result.PickedFace = face;
            result.TransformToHost = transformToHost;
            result.PickedPoint = reference.GlobalPoint;
            result.IsFromLink = true;

            return result;

        }

        private Face GetFaceFromReference(
            Element element,
            Reference reference)
        {
            GeometryObject geometryObject = element.GetGeometryObjectFromReference(reference);

            Face face = geometryObject as Face;

            if (face == null)
            {
                throw new InvalidOperationException(
                    "The selected reference is not a face!");
            }

            return face;
        }

        private Face GetFaceFromLinkedReference(
            RevitLinkInstance linkInstance,
            Reference reference)
        {
            
            Element linkedElement = GetLinkedElement(
                linkInstance,
                reference);

            Reference linkedReference =
                reference.CreateReferenceInLink();

            GeometryObject geometryObject =
                linkedElement.GetGeometryObjectFromReference(
                    linkedReference);

            Face face = geometryObject as Face;

            if (face == null)
            {
                throw new InvalidOperationException(
                    "The selected linked reference is not a face.");
            }

            return face;
        }

        private RevitLinkInstance GetLinkInstance(
            Document document,
            Reference reference)
        {
            Element element = document.GetElement(
                reference.ElementId);

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
            Document linkedDocument = linkInstance.GetLinkDocument();

            if (linkedDocument == null)
            {
                throw new InvalidOperationException(
                    "Could not access the linked document.");
            }

            Element linkedElement = linkedDocument.GetElement(
                reference.LinkedElementId);

            if (linkedElement == null)
            {
                throw new InvalidOperationException(
                    "Could not find the selected element inside the linked document.");
            }

            return linkedElement;
        }

        private Transform GetTransformToHost(
            RevitLinkInstance linkInstance)
        {
            return linkInstance.GetTotalTransform();
        }
    }
}