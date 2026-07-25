using Autodesk.Revit.DB;

namespace HG.RevitTools.Models.ParameterChecker
{
    public class ParameterOption
    {
        public string Name { get; }

        public StorageType StorageType { get; }

        public ForgeTypeId DataTypeId { get; }

        public ParameterOption(
            string name,
            StorageType storageType,
            ForgeTypeId dataTypeId)
        {
            Name = name;
            StorageType = storageType;
            DataTypeId = dataTypeId;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}