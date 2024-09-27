using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminLauncher.BusinessLibrary
{
    public class RoutineItem
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public List<ProgramItem> Programs { get; set; }

        public void Launch()
        {
            foreach (var program in Programs)
                program.Launch();
        }
    }
}
