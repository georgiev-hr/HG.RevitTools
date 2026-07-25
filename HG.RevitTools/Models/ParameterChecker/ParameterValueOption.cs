using Autodesk.Revit.DB;

namespace HG.RevitTools.Models.ParameterChecker
{
    public class ParameterValueOption
    {
        public object RawValue { get; }

        public string DisplayValue { get; }

        public StorageType StorageType { get; }

        public bool HasValue { get; }

        public int InstanceCount { get; }

        public ParameterValueOption(
            object rawValue,
            string displayValue,
            StorageType storageType,
            bool hasValue,
            int instanceCount)
        {
            RawValue = rawValue;
            DisplayValue = displayValue;
            StorageType = storageType;
            HasValue = hasValue;
            InstanceCount = instanceCount;
        }

        public override string ToString()
        {
            return $"{DisplayValue} ({InstanceCount})";
        }
    }
}