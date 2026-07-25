using Autodesk.Revit.DB;
using HG.RevitTools.Models.AdjustLuminaireHeight;
using System;
using System.Collections.Generic;
using HG.RevitTools.Services.Shared;

namespace HG.RevitTools.Services.AdjustLuminaireHeight
{
    public class LuminaireHeightParameterService
    {
        private readonly UnitConversionService unitConversionService;
        public LuminaireHeightParameterService()
        {
            unitConversionService =
                new UnitConversionService();
        }
        public HeightAdjustmentResult ApplyHeightAdjustments(
            IList<LuminaireHeightData> heightData,
            string parameterName)
        {
            HeightAdjustmentResult result =
                new HeightAdjustmentResult();

            foreach (LuminaireHeightData data in heightData)
            {
                bool success = TrySetHeightParameter(
                    data,
                    parameterName,
                    out string message);

                if (success)
                {
                    result.AdjustedCount++;
                }
                else
                {
                    result.FailedCount++;
                    result.Messages.Add(message);
                }
            }

            return result;
        }

        private bool TrySetHeightParameter(
                LuminaireHeightData data,
                string parameterName,
                out string message)
        {
            message = string.Empty;

            bool success = TryGetWritableParameter(
                data,
                parameterName,
                out Parameter parameter,
                out message);

            if (!success)
            {
                return false;
            }

            try
            {
                double targetHeightFeet =
                    unitConversionService.ConvertMillimetresToFeet(
                        data.TargetHeightMm);

                parameter.Set(targetHeightFeet);

                return true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }
        private bool TryGetWritableParameter(
                LuminaireHeightData data,
                string parameterName,
                out Parameter parameter,
                out string message)
        {
            parameter = data.Luminaire.LookupParameter(parameterName);

            if (parameter == null)
            {
                message = $"{data.Luminaire.Name}: Parameter '{parameterName}' was not found.";

                return false;
            }

            if (parameter.IsReadOnly)
            {
                message = $"{data.Luminaire.Name}: Parameter '{parameterName}' is read-only.";

                return false;
            }

            message = string.Empty;

            return true;
        }
    }
}