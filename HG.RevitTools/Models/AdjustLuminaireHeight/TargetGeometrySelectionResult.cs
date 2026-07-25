using Autodesk.Revit.DB;

namespace HG.RevitTools.Models.AdjustLuminaireHeight
{
    public class TargetGeometrySelectionResult
    {
        public Reference PickedReference { get; set; }

        public Element SourceElement { get; set; }

        public ElementId HostElementId { get; set; }

        public ElementId LinkedElementId { get; set; }

        public RevitLinkInstance LinkInstance { get; set; }

        public Face PickedFace { get; set; }

        public Transform TransformToHost { get; set; }

        public XYZ PickedPoint { get; set; }

        public bool IsFromLink { get; set; }
    }
}