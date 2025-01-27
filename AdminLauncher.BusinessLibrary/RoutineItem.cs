using NLog;

namespace AdminLauncher.BusinessLibrary
{
    public class RoutineItem : GenericItem, ICloneable
    {
        private string name;
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public override string Name
        {
            get { return name.EndsWith("(routine)") ? name : name + "(routine)"; }
            set { name = string.IsNullOrEmpty(value) ? $"GenericRoutine" : value; }
        }

        public required List<ProgramItem> Programs { get; set; }

        public void AddProgram(ProgramItem program)
        {
            if (!Programs.Contains(program))
                Programs.Add(program);
        }
        public void RemoveProgram(ProgramItem program)
        {
            if (Programs.Contains(program))
                Programs.Remove(program);
        }
        /// <summary>
        /// Launches programs in the routine in succession
        /// </summary>
        /// <returns></returns>
        public override LaunchResult Launch()
        {
            var result = new List<LaunchResult>();
            foreach (var program in Programs)
                result.Add(program.Launch());

            var countSuccess = result.Select(e => e.LaunchState).Count(e => e == LaunchStateEnum.Success);
            var countError = result.Select(e => e.LaunchState).Count(e => e == LaunchStateEnum.Error);
            LaunchStateEnum launchState;

            if (countSuccess == result.Count)
                launchState = LaunchStateEnum.Success;
            else if (countError == result.Count)
                launchState = LaunchStateEnum.Error;
            else
                launchState = LaunchStateEnum.Partial;

            string message = "Launch success";
            if (launchState != LaunchStateEnum.Success)
                message = $"The launch of the following items failed:{string.Join(", ", result.Where(e => e.LaunchState == LaunchStateEnum.Error).Select(e => e.GenericItem.Name))}";

            return new LaunchResult() { GenericItem = this, LaunchState = launchState, Message = message };
        }
        /// <summary>
        /// Returns the path icon to be shown in the button
        /// </summary>
        /// <returns></returns>
        public override string GetIconPath()
        {
            if (!string.IsNullOrEmpty(CustomIconPath))
            {
                if (File.Exists(CustomIconPath))
                    return CustomIconPath;
                else
                    logger.Error($"Routine {Name}: file {CustomIconPath} not exist");
            }
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "list.png");
        }

        public object Clone()
        {
            var clonedItem = (RoutineItem)this.MemberwiseClone();
            clonedItem.Programs = new List<ProgramItem>(this.Programs.Select(p => (ProgramItem)p.Clone()));
            return clonedItem;
        }
    }
}
