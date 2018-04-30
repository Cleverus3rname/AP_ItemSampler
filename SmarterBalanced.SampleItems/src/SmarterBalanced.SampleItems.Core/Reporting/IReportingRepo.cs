using Microsoft.AspNetCore.Mvc;

namespace SmarterBalanced.SampleItems.Core.Reporting
{
    public interface IReportingRepo
    {
        FileStreamResult AccessibilityWalk(string baseUrl);
    }
}