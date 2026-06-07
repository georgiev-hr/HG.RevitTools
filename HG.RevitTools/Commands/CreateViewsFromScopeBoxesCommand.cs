using Autodesk;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HG.RevitTools.Models;
using HG.RevitTools.Views;
using HG.RevitTools.Services;

namespace HG.RevitTools.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class CreateViewsFromScopeBoxesCommand : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            Document document = commandData.Application.ActiveUIDocument.Document;

            var collectorService = new RevitElementCollectorService();

            var views = collectorService.GetFloorPlans(document);
                        
            // Open WPF select window with current elements
            var viewSelector = new ElementSelector(
                views,
                v => ((Element)v).Name,
                "Select view to copy"
            );

            View selectedView = null;

            if (viewSelector.ShowDialog() == true)
            {
                selectedView = viewSelector.GetSelectedItems<View>().FirstOrDefault();
            }
            if (selectedView == null)
                return Result.Cancelled;


            //Get Selected Scope Boxes
            var scopeBoxes = collectorService.GetScopeBoxes(document);

            // Open WPF select window with current elements
            var scopeSelector = new ElementSelector(
                scopeBoxes,
                o => ((Element)o).Name,
                "Select Scope Boxes to create Views"
            );

            IList<Element> selectedScopeBoxes = null;

            if (scopeSelector.ShowDialog() == true)
            {
                selectedScopeBoxes = scopeSelector.GetSelectedItems<Element>();
            }
            if (selectedScopeBoxes == null || !selectedScopeBoxes.Any())
            {
                return Result.Cancelled;
            }



            var scopeBoxViewCreator = new ScopeBoxViewCreator();

            scopeBoxViewCreator.CreateDependentViews(
                document,
                selectedView,
                selectedScopeBoxes);



            return Result.Succeeded;
        }
    }
}
