using NLog;

namespace AdminLauncher.BusinessLibrary
{
    public class Manager: ICloneable
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
        public bool Load(out string backupPath)
        {
            logger.Info("Load Manager from file");
            bool succes = true;
            try
            {
                var savedManager = new PersistenceManager().LoadData();
                programManager = (ProgramManager)savedManager.programManager.Clone();
                settingsManager = (SettingsManager)savedManager.settingsManager.Clone();
                backupPath = null;
            }
            catch (Exception ex)
            {
                backupPath = new PersistenceManager().CreateBackupManager();
                logger.Error(ex, $"Created configurator backup at {backupPath}");
                //Ignore configuration File
                succes = false;
            }
            return succes;
        }

        public object Clone()
        {
            return new Manager
            {
                programManager = (ProgramManager)this.programManager.Clone(),
                settingsManager = (SettingsManager)this.settingsManager.Clone()
            };
        }
    }
}
