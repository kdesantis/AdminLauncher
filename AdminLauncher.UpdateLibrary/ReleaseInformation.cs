using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminLauncher.UpdateLibrary
{
    public class ReleaseInformation
    {
        public required string Version { get; set; }
        public required string Url { get; set; }
        public required string ReleaseNotes { get; set; }
    }
}
