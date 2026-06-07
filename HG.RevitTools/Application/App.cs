using Autodesk.Revit.UI;
using System;
using System.Linq;
using System.Reflection;

namespace HG.RevitTools
{
    public class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            string tabName = "HG Solutions";
            string panelName = "Scope Tools";

            // Create tab (safe way)
            try
            {
                application.CreateRibbonTab(tabName);
            }
            catch { }

            // Safer: reuse panel if it already exists
            RibbonPanel panel = application
                .GetRibbonPanels(tabName)
                .FirstOrDefault(p => p.Name == panelName)
                ?? application.CreateRibbonPanel(tabName, panelName);

            string assemblyPath = Assembly.GetExecutingAssembly().Location;

            // -------- First button --------
            PushButtonData buttonData = new PushButtonData(
                "ScopeBoxButton",
                "Scope Boxes",
                assemblyPath,
                "HG.RevitTools.Commands.CreateViewsFromScopeBoxesCommand"   // <-- IMPORTANT
            );

            PushButton button = panel.AddItem(buttonData) as PushButton;

            // Assign images
            button.LargeImage = LoadImage("HG.RevitTools.Resources.Images.BigButton.png");
            button.Image = LoadImage("HG.RevitTools.Resources.Images.SmallButton.png");

            button.ToolTip = "Collect and select Scope Boxes.";

            // -------- Second button --------
            PushButtonData buttonData2 = new PushButtonData(
                "CreateSheetsButton",
                "Create Sheets",
                assemblyPath,
                // to do: Create second IExternalCommand to launch
                "HG.RevitTools.Commands.CreateSheetsCommand"
            );

            PushButton button2 = panel.AddItem(buttonData2) as PushButton;
            button2.LargeImage = LoadImage("HG.RevitTools.Resources.Images.BigButton.png");
            button2.Image = LoadImage("HG.RevitTools.Resources.Images.SmallButton.png");
            button2.ToolTip = "Create Sheets from selected Views.";



            return Result.Succeeded;
        }

        private System.Windows.Media.Imaging.BitmapImage LoadImage(string resourcePath)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(resourcePath);

            var image = new System.Windows.Media.Imaging.BitmapImage();
            image.BeginInit();
            image.StreamSource = stream;
            image.EndInit();

            return image;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}