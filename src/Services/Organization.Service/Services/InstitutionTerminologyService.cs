using System.Text.Json;
using Organization.Service.DTOs;

namespace Organization.Service.Services
{
    public interface IInstitutionTerminologyService
    {
        /// <summary>Full map: normalized institution-type key -> keyword swaps.</summary>
        IReadOnlyDictionary<string, List<TerminologyMappingDto>> GetAll();
    }

    /// <summary>
    /// Loads the static institution terminology map from Config/institution-terminology.json once at
    /// startup and caches it. Tolerant of a missing/invalid file (falls back to an empty map).
    /// </summary>
    public sealed class InstitutionTerminologyService : IInstitutionTerminologyService
    {
        private readonly Dictionary<string, List<TerminologyMappingDto>> _map;

        public InstitutionTerminologyService(IWebHostEnvironment env, ILogger<InstitutionTerminologyService> logger)
        {
            _map = new Dictionary<string, List<TerminologyMappingDto>>(StringComparer.OrdinalIgnoreCase);

            try
            {
                // Prefer the copied-to-output file; fall back to the project content root (local dev / dotnet run).
                var candidates = new[]
                {
                    Path.Combine(AppContext.BaseDirectory, "Config", "institution-terminology.json"),
                    Path.Combine(env.ContentRootPath, "Config", "institution-terminology.json")
                };

                var path = candidates.FirstOrDefault(File.Exists);
                if (path is null)
                {
                    logger.LogWarning("institution-terminology.json not found in {Paths}; terminology map is empty.",
                        string.Join(", ", candidates));
                    return;
                }

                var json = File.ReadAllText(path);
                var parsed = JsonSerializer.Deserialize<Dictionary<string, List<TerminologyMappingDto>>>(
                    json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (parsed is not null)
                {
                    foreach (var kv in parsed)
                    {
                        _map[NormalizeKey(kv.Key)] = kv.Value ?? new List<TerminologyMappingDto>();
                    }
                }

                logger.LogInformation("Loaded institution terminology for {Count} type(s) from {Path}.", _map.Count, path);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to load institution-terminology.json; terminology map is empty.");
            }
        }

        public IReadOnlyDictionary<string, List<TerminologyMappingDto>> GetAll() => _map;

        /// <summary>Lowercase and strip non-letters, matching the frontend's normalizeInstitutionType().</summary>
        internal static string NormalizeKey(string? value) =>
            new string((value ?? string.Empty).ToLowerInvariant().Where(char.IsLetter).ToArray());
    }
}
