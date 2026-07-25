using Autodesk.Revit.DB;

namespace HG.RevitTools.Models.ParameterChecker
{
    public class ParameterValueData
    {
        public object RawValue { get; }

        public string DisplayValue { get; }

        public StorageType StorageType { get; }

        public bool HasValue { get; }

        public ParameterValueData(
            object rawValue,
            string displayValue,
            StorageType storageType,
            bool hasValue)
        {
            RawValue = rawValue;
            DisplayValue = displayValue;
            StorageType = storageType;
            HasValue = hasValue;
        }
    }
}