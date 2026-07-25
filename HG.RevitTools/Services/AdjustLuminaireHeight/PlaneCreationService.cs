using Autodesk.Revit.DB;
using HG.RevitTools.Models.AdjustLuminaireHeight;
using System;

namespace HG.RevitTools.Services.AdjustLuminaireHeight
{
    public class PlaneCreationService
    {
        public TargetPlaneInfo CreatePlaneFromFace(
            TargetGeometrySelectionResult faceSelection)
        {
            PlanarFace planarFace =
                GetPlanarFace(faceSelection.PickedFace);

            Plane hostPlane =
                CreateHostPlane(
                    planarFace,
                    faceSelection.TransformToHost,
                    faceSelection.PickedPoint);

            return new TargetPlaneInfo
            {
                Plane = hostPlane,
                Origin = hostPlane.Origin,
                Normal = hostPlane.Normal,
                IsFromLinkedFace = faceSelection.IsFromLink,
                SourceElement = faceSelection.SourceElement
            };
        }

        private PlanarFace GetPlanarFace(
            Face face)
        {
            PlanarFace planarFace = face as PlanarFace;

            if (planarFace == null)
            {
                throw new InvalidOperationException(
                    "Selected face must be planar.");
            }

            return planarFace;
        }

        private Plane CreateHostPlane(
            PlanarFace planarFace,
            Transform transformToHost,
            XYZ pickedPoint)
        {
            XYZ hostOrigin =
                pickedPoint;

            XYZ hostNormal =
                transformToHost
                    .OfVector(planarFace.FaceNormal)
                    .Normalize();

            return Plane.CreateByNormalAndOrigin(
                hostNormal,
                hostOrigin);
        }
    }
}