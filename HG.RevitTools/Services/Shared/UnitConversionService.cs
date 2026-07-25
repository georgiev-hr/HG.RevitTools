using Autodesk.Revit.DB;

namespace HG.RevitTools.Services.Shared
{
    public class UnitConversionService
    {
        public double ConvertFeetToMillimetres(
            double valueFeet)
        {
            return UnitUtils.ConvertFromInternalUnits(
                valueFeet,
                UnitTypeId.Millimeters);
        }

        public double ConvertMillimetresToFeet(
            double valueMillimetres)
        {
            return UnitUtils.ConvertToInternalUnits(
                valueMillimetres,
                UnitTypeId.Millimeters);
        }
    }
}