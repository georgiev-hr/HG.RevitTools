using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using HG.RevitTools.Models.AdjustLuminaireHeight;
using HG.RevitTools.Services.AdjustLuminaireHeight;
using HG.RevitTools.Services.AdjustLuminaireHeight.HeightTargets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HG.RevitTools.Commands.AdjustLuminaireHeight
{
    [Transaction(TransactionMode.Manual)]
    public class AdjustLuminaireHeightCommand : IExternalCommand
    {
        private const string HeightParameterName = "Offset_Mounting_mm";

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;

            Document doc = uiDoc.Document;


            try
            {
                LuminaireSelectionService luminaireSelectionService = new LuminaireSelectionService();

                FaceSelectionService faceSelectionService = new FaceSelectionService();

                PlaneCreationService planeCreationService = new PlaneCreationService();

                HeightCalculationService heightCalculationService = new HeightCalculationService();

                LuminaireHeightParameterService parameterService = new LuminaireHeightParameterService();

                IList<FamilyInstance> luminaires = luminaireSelectionService.PickLightingFixtures(uiDoc);

                //TargetGeometrySelectionResult faceSelection = faceSelectionService.PickFace(uiDoc);

                //TargetPlaneInfo targetPlane = planeCreationService.CreatePlaneFromFace(faceSelection);

                //IHeightTargetProvider targetProvider = new PlanarFaceHeightTargetProvider(targetPlane.Plane);
                               
                TargetGeometrySelectionService targetGeometrySelectionService = new TargetGeometrySelectionService();

                View3DService view3DService = new View3DService();

                TargetGeometryFilterService targetGeometryFilterService = new TargetGeometryFilterService();


                IList<TargetGeometrySelectionResult> selectedTargets =
                    targetGeometrySelectionService.PickTargetGeometry(
                        uiDoc);

                View3D view3D =
                    view3DService.GetNonTemplate3DView(
                        doc);

                ElementFilter targetFilter =
                    targetGeometryFilterService.CreateFloorTopoCeilingFilter();

                IHeightTargetProvider targetProvider =
                    new CurvedGeometryHeightTargetProvider(
                        targetFilter,
                        view3D,
                        XYZ.BasisZ,
                        selectedTargets);

                IList<LuminaireHeightData> heightData = heightCalculationService.CalculateAll(
                    luminaires,
                    targetProvider,
                    HeightParameterName);


                HeightAdjustmentResult adjustmentResult;

                using (Transaction transaction =
                    new Transaction(doc, "Adjust Luminaire Height"))
                {
                    transaction.Start();

                    adjustmentResult =
                        parameterService.ApplyHeightAdjustments(
                            heightData,
                            HeightParameterName);

                    transaction.Commit();
                }

                TaskDialog.Show(
                    "Adjust Luminaire Height",
                    $"Adjusted: {adjustmentResult.AdjustedCount}\n" +
                    $"Failed: {adjustmentResult.FailedCount}");

                return Result.Succeeded;
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Cancelled;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }

        }
    }
}
