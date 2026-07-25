using Autodesk.Revit.DB;
using HG.RevitTools.Models.ParameterChecker;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HG.RevitTools.Services.ParameterChecker
{
    public class CommonParameterService
    {
        public List<ParameterOption> GetCommonParameters(
            IReadOnlyCollection<Element> elements)
        {
            if (elements == null || elements.Count == 0)
            {
                return new List<ParameterOption>();
            }

            Element firstElement = elements.First();

            Dictionary<string, Parameter> commonParameters =
                GetParametersByName(firstElement);

            foreach (Element element in elements.Skip(1))
            {
                Dictionary<string, Parameter> currentParameters =
                    GetParametersByName(element);

                List<string> parameterNamesToRemove =
                    commonParameters
                        .Where(pair =>
                            !currentParameters.TryGetValue(
                                pair.Key,
                                out Parameter currentParameter) ||
                            !AreCompatible(
                                pair.Value,
                                currentParameter))
                        .Select(pair => pair.Key)
                        .ToList();

                foreach (string parameterName in parameterNamesToRemove)
                {
                    commonParameters.Remove(parameterName);
                }
            }

            return commonParameters
                .Values
                .Select(CreateParameterOption)
                .OrderBy(option => option.Name)
                .ToList();
        }

        private Dictionary<string, Parameter> GetParametersByName(
            Element element)
        {
            Dictionary<string, Parameter> parameters =
                new Dictionary<string, Parameter>(
                    StringComparer.OrdinalIgnoreCase);

            if (element == null)
            {
                return parameters;
            }

            foreach (Parameter parameter in element.Parameters)
            {
                if (parameter?.Definition == null)
                {
                    continue;
                }

                string parameterName =
                    parameter.Definition.Name;

                if (string.IsNullOrWhiteSpace(parameterName))
                {
                    continue;
                }

                if (!parameters.ContainsKey(parameterName))
                {
                    parameters.Add(
                        parameterName,
                        parameter);
                }
            }

            return parameters;
        }

        private bool AreCompatible(
            Parameter firstParameter,
            Parameter secondParameter)
        {
            if (firstParameter == null ||
                secondParameter == null)
            {
                return false;
            }

            if (firstParameter.StorageType !=
                secondParameter.StorageType)
            {
                return false;
            }

            ForgeTypeId firstDataTypeId =
                firstParameter.Definition.GetDataType();

            ForgeTypeId secondDataTypeId =
                secondParameter.Definition.GetDataType();

            return firstDataTypeId == secondDataTypeId;
        }

        private ParameterOption CreateParameterOption(
            Parameter parameter)
        {
            return new ParameterOption(
                parameter.Definition.Name,
                parameter.StorageType,
                parameter.Definition.GetDataType());
        }
    }
}