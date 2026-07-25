using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace HG.RevitTools.Models.ParameterChecker
{
    public class ParameterCheckResult
    {
        public int CheckedCount { get; }

        public IReadOnlyCollection<ElementId> MismatchingElementIds { get; }

        public int MismatchingCount =>
            MismatchingElementIds.Count;

        public int MatchingCount =>
            CheckedCount - MismatchingCount;

        public bool HasMismatches =>
            MismatchingCount > 0;

        public ParameterCheckResult(
            int checkedCount,
            IReadOnlyCollection<ElementId> mismatchingElementIds)
        {
            CheckedCount = checkedCount;

            MismatchingElementIds =
                mismatchingElementIds ??
                new List<ElementId>();
        }
    }
}