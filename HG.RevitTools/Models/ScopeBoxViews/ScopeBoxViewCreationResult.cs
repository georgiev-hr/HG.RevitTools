using System.Collections.Generic;

namespace HG.RevitTools.Models.ScopeBoxViews
{
    public class ScopeBoxViewCreationResult
    {
        public int CreatedCount { get; set; }

        public int FailedCount { get; set; }

        public IList<string> Messages { get; }

        public ScopeBoxViewCreationResult()
        {
            Messages = new List<string>();
        }
    }
}