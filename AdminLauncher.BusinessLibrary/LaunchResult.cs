using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminLauncher.BusinessLibrary
{
    public class LaunchResult
    {
        public GenericItem GenericItem { get; set; }
        public LaunchStateEnum LaunchState { get; set; }
        public string Message { get; set; }
    }

    public enum LaunchStateEnum
    {
        Success,
        Partial,
        Error
    }
}
