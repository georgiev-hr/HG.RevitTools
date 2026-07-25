using System.Collections.Generic;

namespace HG.RevitTools.Models.SheetCreator
{
    public class SheetCreationResult
    {
        public int CreatedCount { get; set; }

        public int FailedCount { get; set; }

        public IList<string> Messages { get; }

        public SheetCreationResult()
        {
            Messages = new List<string>();
        }
    }
}