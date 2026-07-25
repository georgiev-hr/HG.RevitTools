using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using HG.RevitTools.Models.LightingFixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HG.RevitTools.Services.LightingFixtures
{
    internal class LightingFixtureCollectorService
    {
        public List<LightingFixtureInfo> GetLightingFixturesIncludingLinks(Document hostDoc)
        {
            var result = new List<LightingFixtureInfo>();

            // Host Model
            var hostFixtures = CollectFixturesFromDocument(
                hostDoc,
                "Host Model",
                false,
                Transform.Identity);

            result.AddRange(hostFixtures);

            // Linked Models

            var links = new FilteredElementCollector(hostDoc)
                .OfClass(typeof(RevitLinkInstance))
                .Cast<RevitLinkInstance>()
                .ToList();

            foreach ( var link in links )
            {
                Document linkDoc = link.GetLinkDocument();

                if ( linkDoc == null )
                {
                    continue;
                }

                Transform linkTransform = link.GetTotalTransform();

                var linkedFixtures = CollectFixturesFromDocument(
                    linkDoc,
                    link.Name,
                    true,
                    linkTransform);

                result.AddRange (linkedFixtures);

            }
            return result;

        }

        private List<LightingFixtureInfo> CollectFixturesFromDocument(Document sourceDoc,
            string sourceModelName,
            bool isFromLink,
            Transform Transform)
        {
            var fixtures = new FilteredElementCollector(sourceDoc)
                .OfCategory(BuiltInCategory.OST_LightingFixtures)
                .WhereElementIsNotElementType()
                .ToElements();

            var result = new List<LightingFixtureInfo>();

            foreach (Element fixture in fixtures)
            {
                var familyInstance = fixture as FamilyInstance;
                var symbol = familyInstance?.Symbol;


            }

        }
    }
}
