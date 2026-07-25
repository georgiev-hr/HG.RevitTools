using HG.RevitTools.Models.SheetCreator;

namespace HG.RevitTools.Services.SheetCreator
{
    public class SheetNamingService
    {
        public string BuildSheetNumber(
            SheetNamingData data)
        {
            return $"{data.ProjectNumber}-{data.Originator}-" +
                   $"{data.SystemFacility}-{data.Phase}-{data.Location}-" +
                   $"{data.Type}-{data.RoleDiscipline}-{data.Level}-" +
                   $"{data.Package}{data.Scale}{data.SerialNo}-{data.RevisionNo}";
        }

        public string GenerateSheetName(
            SheetNamingData data,
            string template)
        {
            return template
                .Replace("{number}", BuildSheetNumber(data))
                .Replace("{name}", data.DrawingTitle)
                .Replace("{project}", data.ProjectNumber)
                .Replace("{originator}", data.Originator)
                .Replace("{system}", data.SystemFacility)
                .Replace("{phase}", data.Phase)
                .Replace("{location}", data.Location)
                .Replace("{type}", data.Type)
                .Replace("{role}", data.RoleDiscipline)
                .Replace("{level}", data.Level)
                .Replace("{package}", data.Package)
                .Replace("{scale}", data.Scale)
                .Replace("{serial}", data.SerialNo)
                .Replace("{revision}", data.RevisionNo);
        }
    }
}