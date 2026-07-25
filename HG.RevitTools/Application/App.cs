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
            button.LargeImage = LoadLargeImage("HG.RevitTools.Resources.Images.BigButton.png");
            button.Image = LoadSmallImage("HG.RevitTools.Resources.Images.SmallButton.png");

            button.ToolTip = "Collect and select Scope Boxes.";

            // -------- Second button --------
            PushButtonData buttonData2 = new PushButtonData(
                "CreateSheetsButton",
                "Create Sheets",
                assemblyPath,                
                "HG.RevitTools.Commands.CreateSheetsCommand"
            );

            PushButton button2 = panel.AddItem(buttonData2) as PushButton;
            button2.LargeImage = LoadLargeImage("HG.RevitTools.Resources.Images.BigButton.png");
            button2.Image = LoadSmallImage("HG.RevitTools.Resources.Images.SmallButton.png");
            button2.ToolTip = "Create Sheets from selected Views.";


            // -------- Third button --------
            PushButtonData buttonData3 = new PushButtonData(
                "AdjustLuminaireHeight",
                "Adjust Height",
                assemblyPath,                
                "HG.RevitTools.Commands.AdjustLuminaireHeight.AdjustLuminaireHeightCommand"
            );

            PushButton button3 = panel.AddItem(buttonData3) as PushButton;
            button3.LargeImage = LoadLargeImage("HG.RevitTools.Resources.Images.BigButton.png");
            button3.Image = LoadSmallImage("HG.RevitTools.Resources.Images.SmallButton.png");
            button3.ToolTip = "Adjust selected luminaires to a picked host or linked face.";

            // -------- Fourth button --------
            PushButtonData buttonData4 = new PushButtonData(
                "CheckParameters",
                "Compare Parameters",
                assemblyPath,
                "HG.RevitTools.Commands.ParameterChecker.CheckLightingFixtureParametersCommand"
            );

            PushButton button4 = panel.AddItem(buttonData4) as PushButton;
            button4.LargeImage = LoadLargeImage("HG.RevitTools.Resources.Images.BigButton.png");
            button4.Image = LoadSmallImage("HG.RevitTools.Resources.Images.SmallButton.png");
            button4.ToolTip = "Adjust selected luminaires to a picked host or linked face.";


            return Result.Succeeded;
        }

        private System.Windows.Media.Imaging.BitmapImage LoadLargeImage(
    string resourcePath)
        {
            return LoadImage(resourcePath, 32);
        }

        private System.Windows.Media.Imaging.BitmapImage LoadSmallImage(
            string resourcePath)
        {
            return LoadImage(resourcePath, 16);
        }

        private System.Windows.Media.Imaging.BitmapImage LoadImage(
            string resourcePath,
            int pixelSize)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream(resourcePath))
            {
                if (stream == null)
                {
                    throw new InvalidOperationException(
                        $"Could not find embedded resource: {resourcePath}");
                }

                var image = new System.Windows.Media.Imaging.BitmapImage();

                image.BeginInit();
                image.StreamSource = stream;
                image.DecodePixelWidth = pixelSize;
                image.DecodePixelHeight = pixelSize;
                image.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                image.EndInit();
                image.Freeze();

                return image;
            }
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}