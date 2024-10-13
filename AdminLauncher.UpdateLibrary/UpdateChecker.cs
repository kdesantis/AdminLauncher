using System.Text.Json;
namespace AdminLauncher.UpdateLibrary
{
    public class UpdateChecker(string pasteBinUrl)
    {
        private readonly string PastebinUrl = pasteBinUrl;
        public ReleaseInformation? UpdateInformation { get; set; }
        public string? Error { get; set; }

        public async Task<bool> CheckForUpdatesAsync(Version currentVersion)
        {
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
                Error = ex.Message;
            }
            return result;
        }

    }
}
