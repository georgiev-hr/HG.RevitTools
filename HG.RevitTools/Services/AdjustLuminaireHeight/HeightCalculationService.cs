using Autodesk.Revit.DB;
using HG.RevitTools.Models.AdjustLuminaireHeight;
using HG.RevitTools.Services.AdjustLuminaireHeight.HeightTargets;
using HG.RevitTools.Services.Shared;
using System;
using System.Collections.Generic;


namespace HG.RevitTools.Services.AdjustLuminaireHeight
{
    public class HeightCalculationService
    {
        private readonly UnitConversionService unitConversionService;

        public HeightCalculationService()
        {
            unitConversionService =
                new UnitConversionService();
        }
        public LuminaireHeightData Calculate(
            FamilyInstance luminaire,
            IHeightTargetProvider targetProvider,
            string heightParameterName)
        {
            XYZ insertionPoint =
                GetInsertionPoint(luminaire);

            double currentHeightMm =
                GetCurrentHeightMm(
                    luminaire,
                    heightParameterName);

            double differenceFeet =
                targetProvider.GetVerticalDifferenceFeet(
                    insertionPoint);

            double differenceMm =
                unitConversionService.ConvertFeetToMillimetres(
                    differenceFeet);

            double targetHeightMm =
                CalculateTargetHeightMm(
                    currentHeightMm,
                    differenceMm);

            return new LuminaireHeightData
            {
                Luminaire = luminaire,
                InsertionPoint = insertionPoint,
                CurrentHeightMm = currentHeightMm,
                DifferenceMm = differenceMm,
                TargetHeightMm = targetHeightMm
            };
        }

        public IList<LuminaireHeightData> CalculateAll(
    IList<FamilyInstance> luminaires,
    IHeightTargetProvider targetProvider,
    string heightParameterName)
        {
            List<LuminaireHeightData> results =
                new List<LuminaireHeightData>();

            foreach (FamilyInstance luminaire in luminaires)
            {
                LuminaireHeightData data =
                    Calculate(
                        luminaire,
                        targetProvider,
                        heightParameterName);

                results.Add(data);
            }

            return results;
        }

        private XYZ GetInsertionPoint(
            FamilyInstance luminaire)
        {
            FamilyInstance targetLuminaire = (FamilyInstance)luminaire;

            LocationPoint locationPoint = targetLuminaire.Location as LocationPoint;

            if (locationPoint != null)
            {
                XYZ point = locationPoint.Point;
                return point;
            }
            else
            {                
                /// To Do : Collect instance with no insertion point to display in a list after proccess
                throw new Exception("Instance has no InsertionPoint");
            }
            
        }

        private double GetCurrentHeightMm(
            FamilyInstance luminaire,
            string parameterName)
        {
            
            Parameter heightParameter = luminaire.LookupParameter(parameterName);

            if (heightParameter == null)
            {
                throw new InvalidOperationException(
                    $"Parameter '{parameterName}' was not found.");
            }

            double currentHeightFeet =
                heightParameter.AsDouble();

            return unitConversionService.ConvertFeetToMillimetres(
                currentHeightFeet);
        }
                       

        private double CalculateTargetHeightMm(
            double currentHeightMm,
            double differenceMm)
        {
            return currentHeightMm + differenceMm;
        }
    }
}