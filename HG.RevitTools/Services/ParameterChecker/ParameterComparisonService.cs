using Autodesk.Revit.DB;
using HG.RevitTools.Models.ParameterChecker;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HG.RevitTools.Services.ParameterChecker
{
    public class ParameterComparisonService
    {
        public ParameterCheckResult CheckElements(
            IReadOnlyCollection<Element> elements,
            ParameterOption parameterOption,
            ParameterValueOption expectedValue)
        {
            if (elements == null ||
                parameterOption == null ||
                expectedValue == null)
            {
                return new ParameterCheckResult(
                    0,
                    new List<ElementId>());
            }

            List<ElementId> mismatchingElementIds =
                elements
                    .Where(element =>
                        !MatchesExpectedValue(
                            element,
                            parameterOption,
                            expectedValue))
                    .Select(element => element.Id)
                    .ToList();

            return new ParameterCheckResult(
                elements.Count,
                mismatchingElementIds);
        }

        private bool MatchesExpectedValue(
            Element element,
            ParameterOption parameterOption,
            ParameterValueOption expectedValue)
        {
            if (element == null)
            {
                return false;
            }

            Parameter parameter =
                element.LookupParameter(parameterOption.Name);

            if (parameter == null)
            {
                return false;
            }

            if (!parameter.HasValue)
            {
                return !expectedValue.HasValue;
            }

            if (!expectedValue.HasValue)
            {
                return false;
            }

            if (parameter.StorageType != expectedValue.StorageType)
            {
                return false;
            }

            switch (parameter.StorageType)
            {
                case StorageType.String:
                    return string.Equals(
                        parameter.AsString() ?? string.Empty,
                        expectedValue.RawValue as string
                            ?? string.Empty,
                        StringComparison.Ordinal);

                case StorageType.Integer:
                    return expectedValue.RawValue is int expectedInteger &&
                           parameter.AsInteger() == expectedInteger;

                case StorageType.Double:
                    return expectedValue.RawValue is double expectedDouble &&
                           parameter.AsDouble().Equals(expectedDouble);

                case StorageType.ElementId:
                    return expectedValue.RawValue is ElementId expectedId &&
                           parameter.AsElementId() == expectedId;

                default:
                    return false;
            }
        }
    }
}