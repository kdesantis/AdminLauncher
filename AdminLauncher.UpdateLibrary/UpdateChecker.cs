using NLog;
using System.Text.Json;
namespace AdminLauncher.UpdateLibrary
{
    public class UpdateChecker(string pasteBinUrl)
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly string PastebinUrl = pasteBinUrl;
        public ReleaseInformation? UpdateInformation { get; set; }
        public string? Error { get; set; }

        public async Task<bool> CheckForUpdatesAsync(Version currentVersion)
        {
            logger.Info("start CheckForUpdatesAsync");
            bool result = false;
            try
            {
                using HttpClient client = new();
                var response = await client.GetStringAsync(PastebinUrl);
                UpdateInformation = JsonSerializer.Deserialize<ReleaseInformation>(response);

                result = new Version(UpdateInformation.Version) > currentVersion;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Error = ex.Message;
            }
            return result;
        }

    }
}
