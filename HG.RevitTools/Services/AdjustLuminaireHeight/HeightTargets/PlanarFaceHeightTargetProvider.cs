using Autodesk.Revit.DB;
using System;

namespace HG.RevitTools.Services.AdjustLuminaireHeight.HeightTargets
{
    public class PlanarFaceHeightTargetProvider : IHeightTargetProvider
    {
        private readonly Plane targetPlane;

        public PlanarFaceHeightTargetProvider(
            Plane targetPlane)
        {
            this.targetPlane = targetPlane;
        }

        public double GetVerticalDifferenceFeet(
            XYZ insertionPoint)
        {
            XYZ direction =
                XYZ.BasisZ;

            double denominator =
                targetPlane.Normal.DotProduct(direction);

            if (Math.Abs(denominator) < 1e-9)
            {
                throw new InvalidOperationException(
                    "Vertical line is parallel to the selected plane.");
            }

            double numerator =
                targetPlane.Normal.DotProduct(
                    targetPlane.Origin - insertionPoint);

            double t =
                numerator / denominator;

            XYZ intersectionPoint =
                insertionPoint + t * direction;

            return intersectionPoint.Z - insertionPoint.Z;
        }
    }
}