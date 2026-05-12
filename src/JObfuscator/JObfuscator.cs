/******************************************************************************
 * JObfuscator WebApi interface
 *
 * Version        : v1.1.0
 * Language       : C#
 * Author         : Bartosz Wójcik
 * Web page       : https://www.pelock.com
 *
 *****************************************************************************/

using System.IO.Compression;
using System.Text;
using System.Text.Json;

namespace PELock;

/// <summary>JObfuscator WebApi client.</summary>
public sealed class JObfuscator
{
    private const string DefaultUserAgent = "PELock JObfuscator";

    private readonly HttpClient _httpClient;
    private readonly string? _apiKey;

    /// <summary>Default JObfuscator WebApi endpoint.</summary>
    public static Uri ApiUri { get; } = new("https://www.pelock.com/api/jobfuscator/v1");

    /// <summary>Default API endpoint as a string (same resource as <see cref="ApiUri"/>).</summary>
    public static readonly string ApiUrl = ApiUri.ToString();

    /// <summary>Success.</summary>
    public const int ErrorSuccess = 0;

    /// <summary>Invalid source size (e.g. 1500 bytes max for demo).</summary>
    public const int ErrorInputSize = 1;

    /// <summary>Input source is empty.</summary>
    public const int ErrorInput = 2;

    /// <summary>Java source parsing error.</summary>
    public const int ErrorParsing = 3;

    /// <summary>Obfuscation error.</summary>
    public const int ErrorObfuscation = 4;

    /// <summary>Output generation error.</summary>
    public const int ErrorOutput = 5;

    /// <summary>Whether request/response zlib compression is enabled (matches Node deflate/inflate).</summary>
    public bool EnableCompression { get; set; } = true;

    public bool MixCodeFlow { get; set; } = true;
    public bool RenameVariables { get; set; } = true;
    public bool RenameMethods { get; set; } = true;
    public bool ShuffleMethods { get; set; } = true;
    public bool IntsMathCrypt { get; set; } = true;
    public bool CryptStrings { get; set; } = true;
    public bool IntsToArrays { get; set; } = true;
    public bool DblsToArrays { get; set; } = true;

    public bool RemoveComments { get; set; } = true;
    public bool DblsMathCrypt { get; set; } = true;
    public bool StringCharVault { get; set; } = true;
    public bool IntsFromDoubleMath { get; set; } = true;
    public bool OpaqueMixerChain { get; set; } = true;
    public bool ComplexifyBooleans { get; set; } = true;
    public bool TryFinallyNoise { get; set; } = true;
    public bool ArrayIntCrypt { get; set; } = true;
    public bool ArrayCharCrypt { get; set; } = true;
    public bool ArrayDoubleCrypt { get; set; } = true;
    public bool ArrayStringCrypt { get; set; } = true;

    public JObfuscator(string? apiKey = null)
        : this(SharedClient.Value, apiKey)
    {
    }

    /// <summary>Uses supplied <see cref="HttpClient"/> (caller owns lifetime).</summary>
    public JObfuscator(HttpClient httpClient, string? apiKey = null)
    {
        _httpClient = httpClient;
        _apiKey = apiKey;
    }

    private static readonly Lazy<HttpClient> SharedClient = new(() =>
    {
        var client = new HttpClient { Timeout = TimeSpan.FromMinutes(5) };
        return client;
    });

    /// <summary>Login and read license/quota limits.</summary>
    public Task<JObfuscatorResult?> LoginAsync(CancellationToken cancellationToken = default) =>
        PostRequestAsync(CreateLoginParams(), cancellationToken);

    /// <summary>Obfuscate Java source read from UTF-8 file.</summary>
    public async Task<JObfuscatorResult?> ObfuscateJavaFileAsync(string javaFilePath, CancellationToken cancellationToken = default)
    {
        string source;
        try
        {
            source = await File.ReadAllTextAsync(javaFilePath, Encoding.UTF8, cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            return null;
        }

        return string.IsNullOrEmpty(source) ? null : await ObfuscateJavaSourceAsync(source, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Obfuscate Java source string.</summary>
    public Task<JObfuscatorResult?> ObfuscateJavaSourceAsync(string javaSource, CancellationToken cancellationToken = default) =>
        PostRequestAsync(CreateObfuscateParams(javaSource), cancellationToken);

    private static IEnumerable<KeyValuePair<string, string>> CreateLoginParams() =>
        new[] { new KeyValuePair<string, string>("command", "login") };

    private static IEnumerable<KeyValuePair<string, string>> CreateObfuscateParams(string javaSource)
    {
        yield return new("command", "obfuscate");
        yield return new("source", javaSource);
    }

    private async Task<JObfuscatorResult?> PostRequestAsync(
        IEnumerable<KeyValuePair<string, string>> paramsEnumerable,
        CancellationToken cancellationToken)
    {
        var dict = paramsEnumerable.ToDictionary(static e => e.Key, static e => e.Value, StringComparer.Ordinal);

        if (!string.IsNullOrEmpty(_apiKey))
            dict["key"] = _apiKey!;

        if (RemoveComments)
            dict["remove_comments"] = "1";
        if (ArrayIntCrypt)
            dict["array_int_crypt"] = "1";
        if (ArrayCharCrypt)
            dict["array_char_crypt"] = "1";
        if (ArrayDoubleCrypt)
            dict["array_double_crypt"] = "1";
        if (ArrayStringCrypt)
            dict["array_string_crypt"] = "1";
        if (MixCodeFlow)
            dict["mix_code_flow"] = "1";
        if (RenameVariables)
            dict["rename_variables"] = "1";
        if (RenameMethods)
            dict["rename_methods"] = "1";
        if (ShuffleMethods)
            dict["shuffle_methods"] = "1";
        if (IntsMathCrypt)
            dict["ints_math_crypt"] = "1";
        if (CryptStrings)
            dict["crypt_strings"] = "1";
        if (StringCharVault)
            dict["string_char_vault"] = "1";
        if (DblsMathCrypt)
            dict["dbls_math_crypt"] = "1";
        if (IntsFromDoubleMath)
            dict["ints_from_double_math"] = "1";
        if (OpaqueMixerChain)
            dict["opaque_mixer_chain"] = "1";
        if (ComplexifyBooleans)
            dict["complexify_booleans"] = "1";
        if (TryFinallyNoise)
            dict["try_finally_noise"] = "1";
        if (IntsToArrays)
            dict["ints_to_arrays"] = "1";
        if (DblsToArrays)
            dict["dbls_to_arrays"] = "1";

        if (EnableCompression && dict.TryGetValue("source", out var sourceValue) && !string.IsNullOrEmpty(sourceValue))
        {
            var compressed = ZCompressUtf8(sourceValue);
            dict["source"] = Convert.ToBase64String(compressed);
            dict["compression"] = "1";
        }

        using var form = new MultipartFormDataContent();
        foreach (var pair in dict)
            form.Add(new StringContent(pair.Value, Encoding.UTF8), pair.Key);

        using var message = new HttpRequestMessage(HttpMethod.Post, ApiUri) { Content = form };
        message.Headers.TryAddWithoutValidation("User-Agent", DefaultUserAgent);

        string responseText;
        try
        {
            using var response = await _httpClient.SendAsync(message, cancellationToken).ConfigureAwait(false);
            responseText = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            return null;
        }

        if (string.IsNullOrEmpty(responseText))
            return null;

        JObfuscatorResult? result;
        try
        {
            result = JsonSerializer.Deserialize<JObfuscatorResult>(responseText, JsonOpts);
        }
        catch
        {
            return null;
        }

        if (result is null)
            return null;

        if (!EnableCompression || result.Error != ErrorSuccess || string.IsNullOrEmpty(result.Output))
            return result;

        try
        {
            result.Output = ZDecompressUtf8FromBase64(result.Output);
        }
        catch
        {
            return null;
        }

        return result;
    }

    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = false };

    /// <summary>Zlib compress UTF-8 (compatible with Node <c>zlib.deflateSync(...)</c> with default zlib format).</summary>
    private static byte[] ZCompressUtf8(string text)
    {
        var utf8Bytes = Encoding.UTF8.GetBytes(text);
        using var ms = new MemoryStream();
        using (var zs = new ZLibStream(ms, CompressionLevel.SmallestSize))
        {
            zs.Write(utf8Bytes, 0, utf8Bytes.Length);
        }

        return ms.ToArray();
    }

    /// <summary>Zlib decompress from base64 (compatible with Node <c>zlib.inflateSync</c>).</summary>
    private static string ZDecompressUtf8FromBase64(string base64)
    {
        var buf = Convert.FromBase64String(base64);
        using var input = new MemoryStream(buf, writable: false);
        using var zs = new ZLibStream(input, CompressionMode.Decompress);
        using var outMs = new MemoryStream();
        zs.CopyTo(outMs);
        return Encoding.UTF8.GetString(outMs.ToArray());
    }
}
