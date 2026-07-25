using Autodesk.Revit.DB;
using HG.RevitTools.Models.SheetCreator;
using System.Linq;

namespace HG.RevitTools.Services.SheetCreator
{
    public class SheetNumberService
    {
        public bool SheetNumberExists(
            Document document,
            string sheetNumber)
        {
            return new FilteredElementCollector(document)
                .OfClass(typeof(ViewSheet))
                .Cast<ViewSheet>()
                .Any(sheet => sheet.SheetNumber == sheetNumber);
        }

        public string CreateUniqueSheetNumber(
            Document document,
            SheetNamingService sheetNamingService,
            SheetNamingData namingData,
            ref int serialCounter)
        {
            string sheetNumber =
                sheetNamingService.BuildSheetNumber(
                    namingData);

            while (SheetNumberExists(document, sheetNumber))
            {
                serialCounter++;

                namingData.SerialNo =
                    serialCounter.ToString("000");

                sheetNumber =
                    sheetNamingService.BuildSheetNumber(
                        namingData);
            }

            return sheetNumber;
        }
    }
}