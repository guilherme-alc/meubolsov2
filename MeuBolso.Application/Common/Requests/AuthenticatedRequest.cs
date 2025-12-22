using System.Text.Json.Serialization;

namespace MeuBolso.Application.Common.Requests;

public class AuthenticatedRequest
{
    [JsonIgnore]
    public string UserId { get; set; } = string.Empty;
}