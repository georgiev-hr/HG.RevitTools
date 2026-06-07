using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;


namespace HG.RevitTools.Model
{
    internal class SheetNameGenerator
    {
        public string GenerateSheetName(
            ViewPlan viewPlan,
            string template,
            int index)
        {
            return template
                .Replace("{index", index.ToString())
                .Replace("{number}", index.ToString("000"))
                .Replace("{name}", viewPlan.Name);
        }
    }
}
