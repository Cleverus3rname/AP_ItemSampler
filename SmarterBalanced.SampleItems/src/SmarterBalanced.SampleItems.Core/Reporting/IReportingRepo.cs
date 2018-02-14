using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmarterBalanced.SampleItems.Core.Reporting
{
    public interface IReportingRepo
    {
        FileStreamResult AccessibilityWalk(string baseUrl);
    }
}
