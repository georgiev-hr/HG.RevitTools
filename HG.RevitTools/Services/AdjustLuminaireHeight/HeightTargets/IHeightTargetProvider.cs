using Autodesk.Revit.DB;

namespace HG.RevitTools.Services.AdjustLuminaireHeight.HeightTargets
{
    public interface IHeightTargetProvider
    {
        double GetVerticalDifferenceFeet(
            XYZ insertionPoint);
    }
}