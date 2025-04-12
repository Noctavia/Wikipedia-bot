using System;
using System.Net.Http;
using System.Speech.Synthesis;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        SpeechSynthesizer synth = new SpeechSynthesizer();
        synth.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult);

        Console.Write("Entre ta question : ");
        string question = Console.ReadLine();

        Console.WriteLine("Je vais chercher la réponse sur Wikipedia...");

        string result = await GetWikipediaSummary(question);

        if (!string.IsNullOrEmpty(result))
        {
            Console.WriteLine(result);
            synth.Speak(result);
        }
        else
        {
            Console.WriteLine("Aucune réponse trouvée.");
            synth.Speak("Aucune réponse trouvée.");
        }
    }

    static async Task<string> GetWikipediaSummary(string query)
    {
        try
        {
            string apiUrl = $"https://fr.wikipedia.org/api/rest_v1/page/summary/{Uri.EscapeDataString(query)}";

            using HttpClient client = new HttpClient();
            string json = await client.GetStringAsync(apiUrl);

            using JsonDocument doc = JsonDocument.Parse(json);
            JsonElement root = doc.RootElement;

            if (root.TryGetProperty("extract", out JsonElement extract))
            {
                return extract.GetString();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erreur : " + ex.Message);
        }

        return null;
    }
}
