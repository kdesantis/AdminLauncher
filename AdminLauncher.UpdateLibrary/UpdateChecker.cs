using System;
using System.Net.Http;
using System.Diagnostics;
using System.Text.Json;
namespace AdminLauncher.UpdateLibrary
{
    public class UpdateChecker
    {
        private string PastebinUrl;
        public ReleaseInformation? UpdateInformation { get; set; }
        public string? Error { get; set; }

        public UpdateChecker(string pasteBinUrl)
        {
            PastebinUrl = pasteBinUrl;
        }

        public async Task<bool> CheckForUpdatesAsync(Version currentVersion)
        {
            bool result = false;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetStringAsync(PastebinUrl);
                    UpdateInformation = JsonSerializer.Deserialize<ReleaseInformation>(response);

                    result = new Version(UpdateInformation.Version) > currentVersion;
                }
            }
            catch (Exception ex)
            {
                Error = ex.Message;
            }
            return result;
        }

    }
}
