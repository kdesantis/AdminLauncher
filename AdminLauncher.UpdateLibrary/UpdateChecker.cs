using System;
using System.Net.Http;
using System.Diagnostics;
using System.Text.Json;
namespace AdminLauncher.UpdateLibrary
{
    public class UpdateChecker
    {
        private const string pastebinUrl = "https://pastebin.com/raw/yhxzjRXj";
        public ReleaseInformation? UpdateInformation { get; set; }

        public async Task<bool> CheckForUpdatesAsync(Version currentVersion)
        {
            bool result = false;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Recupera il contenuto di Pastebin
                    var response = await client.GetStringAsync(pastebinUrl);
                    UpdateInformation = JsonSerializer.Deserialize<ReleaseInformation>(response);

                    result = new Version(UpdateInformation.Version) > currentVersion;
                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }

    }
}
