using System;
using System.Net.Http;
using System.Diagnostics;
using System.Text.Json;
namespace AdminLauncher.UpdateLibrary
{
    public class UpdateChecker
    {
        private const string pastebinUrl = ""; // Inserisci l'URL del post raw su Pastebin
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
                // Gestisci eventuali errori di rete o parsing
                Console.WriteLine($"Errore durante il controllo degli aggiornamenti: {ex.Message}");
            }
            return result;
        }

    }
}
