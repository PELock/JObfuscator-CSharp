/******************************************************************************
 * JObfuscator WebApi interface
 *
 * Version        : v1.0.0
 * Language       : C#
 * Author         : Bartosz Wójcik
 * Web page       : https://www.pelock.com
 *
 *****************************************************************************/

using System.Text.Json.Serialization;

namespace PELock;

/// <summary>Server JSON response payload (see ERROR_* on <see cref="JObfuscator"/>).</summary>
public sealed class JObfuscatorResult
{
    /// <summary>Error code (<see cref="JObfuscator.ErrorSuccess"/> on success).</summary>
    [JsonPropertyName("error")]
    public int Error { get; init; }

    /// <summary>Obfuscated source when successful.</summary>
    [JsonPropertyName("output")]
    public string? Output { get; set; }

    /// <summary>Whether demo mode was used (invalid or empty key).</summary>
    [JsonPropertyName("demo")]
    public bool? Demo { get; init; }

    /// <summary>Credits remaining after this operation.</summary>
    [JsonPropertyName("credits_left")]
    public long? CreditsLeft { get; init; }

    /// <summary>Total credits for this activation key.</summary>
    [JsonPropertyName("credits_total")]
    public long? CreditsTotal { get; init; }

    /// <summary>True when the last credit was used.</summary>
    [JsonPropertyName("expired")]
    public bool? Expired { get; init; }

    /// <summary>Max. source code size allowed (bytes), e.g. demo limit.</summary>
    [JsonPropertyName("string_limit")]
    public long? StringLimit { get; init; }
}
