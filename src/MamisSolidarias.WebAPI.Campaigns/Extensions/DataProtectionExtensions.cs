using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;

namespace MamisSolidarias.WebAPI.Campaigns.Extensions;

internal static class DataProtectionExtensions
{
    public static void AddDataProtection(this IServiceCollection services, IConfiguration configuration,
        ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("DataProtection");
        var dataProtectionKeysPath = configuration.GetValue<string>("DataProtectionKeysPath");
        if (string.IsNullOrWhiteSpace(dataProtectionKeysPath))
        {
            logger.LogWarning("DataProtectionKeysPath is not set. Data protection keys will not be persisted to storage.");
            return;
        }
        services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(dataProtectionKeysPath))
            .UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration
            {
                EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
            });
    }
}