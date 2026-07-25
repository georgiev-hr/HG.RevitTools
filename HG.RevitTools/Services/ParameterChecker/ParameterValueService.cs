using Autodesk.Revit.DB;
using HG.RevitTools.Models.ParameterChecker;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HG.RevitTools.Services.ParameterChecker
{
    public class ParameterValueService
    {
        public List<ParameterValueOption> GetUniqueValues(
            Document document,
            IReadOnlyCollection<Element> elements,
            ParameterOption parameterOption)
        {
            if (document == null ||
                elements == null ||
                elements.Count == 0 ||
                parameterOption == null)
            {
                return new List<ParameterValueOption>();
            }

            List<ParameterValueData> collectedValues =
                elements
                    .Select(element =>
                        GetParameterValue(
                            document,
                            element,
                            parameterOption.Name))
                    .ToList();

            return collectedValues
                .GroupBy(
                    value => value,
                    new ParameterValueDataComparer())
                .Select(group =>
                {
                    ParameterValueData firstValue =
                        group.First();

                    return new ParameterValueOption(
                        firstValue.RawValue,
                        firstValue.DisplayValue,
                        firstValue.StorageType,
                        firstValue.HasValue,
                        group.Count());
                })
                .OrderByDescending(option =>
                    option.InstanceCount)
                .ThenBy(option =>
                    option.DisplayValue)
                .ToList();
        }

        private ParameterValueData GetParameterValue(
            Document document,
            Element element,
            string parameterName)
        {
            Parameter parameter =
                element?.LookupParameter(parameterName);

            if (parameter == null || !parameter.HasValue)
            {
                return new ParameterValueData(
                    rawValue: null,
                    displayValue: "<No value>",
                    storageType: parameter?.StorageType
                        ?? StorageType.None,
                    hasValue: false);
            }

            object rawValue =
                GetRawValue(parameter);

            string displayValue =
                GetDisplayValue(
                    document,
                    parameter,
                    rawValue);

            return new ParameterValueData(
                rawValue,
                displayValue,
                parameter.StorageType,
                hasValue: true);
        }

        private object GetRawValue(
            Parameter parameter)
        {
            switch (parameter.StorageType)
            {
                case StorageType.String:
                    return parameter.AsString();

                case StorageType.Integer:
                    return parameter.AsInteger();

                case StorageType.Double:
                    return parameter.AsDouble();

                case StorageType.ElementId:
                    return parameter.AsElementId();

                default:
                    return null;
            }
        }

        private string GetDisplayValue(
            Document document,
            Parameter parameter,
            object rawValue)
        {
            if (parameter.StorageType == StorageType.String)
            {
                string stringValue =
                    rawValue as string;

                return string.IsNullOrEmpty(stringValue)
                    ? "<Empty string>"
                    : stringValue;
            }

            if (parameter.StorageType == StorageType.ElementId)
            {
                return GetElementIdDisplayValue(
                    document,
                    rawValue as ElementId);
            }

            string valueString =
                parameter.AsValueString();

            if (!string.IsNullOrWhiteSpace(valueString))
            {
                return valueString;
            }

            return rawValue?.ToString()
                ?? "<No value>";
        }

        private string GetElementIdDisplayValue(
            Document document,
            ElementId elementId)
        {
            if (elementId == null ||
                elementId == ElementId.InvalidElementId)
            {
                return "<None>";
            }

            Element referencedElement =
                document.GetElement(elementId);

            if (referencedElement != null)
            {
                return referencedElement.Name;
            }

            return elementId.ToString();
        }

        private class ParameterValueDataComparer
            : IEqualityComparer<ParameterValueData>
        {
            public bool Equals(
                ParameterValueData first,
                ParameterValueData second)
            {
                if (ReferenceEquals(first, second))
                {
                    return true;
                }

                if (first == null || second == null)
                {
                    return false;
                }

                if (first.HasValue != second.HasValue)
                {
                    return false;
                }

                if (!first.HasValue && !second.HasValue)
                {
                    return true;
                }

                if (first.StorageType != second.StorageType)
                {
                    return false;
                }

                switch (first.StorageType)
                {
                    case StorageType.String:
                        return string.Equals(
                            first.RawValue as string,
                            second.RawValue as string,
                            StringComparison.Ordinal);

                    case StorageType.Integer:
                        return (int)first.RawValue ==
                               (int)second.RawValue;

                    case StorageType.Double:
                        return (double)first.RawValue ==
                               (double)second.RawValue;

                    case StorageType.ElementId:
                        return Equals(
                            first.RawValue as ElementId,
                            second.RawValue as ElementId);

                    default:
                        return Equals(
                            first.RawValue,
                            second.RawValue);
                }
            }

            public int GetHashCode(
                ParameterValueData value)
            {
                if (value == null || !value.HasValue)
                {
                    return 0;
                }

                unchecked
                {
                    int hash = 17;

                    hash = hash * 23 +
                           value.StorageType.GetHashCode();

                    hash = hash * 23 +
                           (value.RawValue?.GetHashCode() ?? 0);

                    return hash;
                }
            }
        }
    }
}