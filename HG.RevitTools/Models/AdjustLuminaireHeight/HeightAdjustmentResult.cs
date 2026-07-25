using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HG.RevitTools.Models.AdjustLuminaireHeight
{
    public class HeightAdjustmentResult
    {
        public int AdjustedCount { get; set; }

        public int FailedCount { get; set; }

        public List<string> Messages { get; }

        public HeightAdjustmentResult()
        {
            Messages = new List<string>();
        }
    }
}
