using NLog;

namespace AdminLauncher.BusinessLibrary
{
    public class Manager
    {
        public ProgramManager programManager { get; set; } = new ProgramManager();
        public SettingsManager settingsManager { get; set; } = new SettingsManager();
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public void Save()
        {
            logger.Info("Save Manager to file");
            PersistenceManager persistenceManager = new();
            persistenceManager.SaveData(this);
        }
        public bool Load()
        {
            logger.Info("Load Manager from file");
            bool succes = true;
            try
            {
                var savedManager = new PersistenceManager().LoadData();
                programManager = (ProgramManager)savedManager.programManager.Clone();
                settingsManager = (SettingsManager)savedManager.settingsManager.Clone();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                //Ignore configuration File
                succes = false;
            }
            return succes;
        }
    }
}
