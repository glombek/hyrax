using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using hyrax.Core.Models.Implement;
using hyrax.Core.Services.Implement;
using Xunit;

namespace hyrax.Tests
{
    public class HyraxFilesystemSignatureRepositoryServiceTests
    {
        [Fact]
        public async Task GetPublicKeyForAuthor_GeneratesAndStoresKeys_InSpkiAndPkcs8Formats()
        {
            var temp = Path.Combine(Path.GetTempPath(), "hyrax_tests", Guid.NewGuid().ToString());
            try
            {
                var svc = new HyraxFilesystemSignatureRepositoryService(temp);
                var author = new Author("testuser", "Test User");

                var pub = await svc.GetPublicKeyForAuthor(author);

                Assert.False(string.IsNullOrWhiteSpace(pub));
                Assert.StartsWith("-----BEGIN PUBLIC KEY-----", pub.Trim());

                var pubPath = Path.Combine(temp, "testuser_public.pem");
                var privPath = Path.Combine(temp, "testuser_private.pem");

                Assert.True(File.Exists(pubPath));
                Assert.True(File.Exists(privPath));

                var priv = await File.ReadAllTextAsync(privPath);
                Assert.False(string.IsNullOrWhiteSpace(priv));
                Assert.StartsWith("-----BEGIN PRIVATE KEY-----", priv.Trim());

                // Ensure the stored public key can be imported and exported again
                using var rsa = RSA.Create();
                rsa.ImportFromPem(pub);
                var reExport = rsa.ExportSubjectPublicKeyInfoPem();
                Assert.Equal(NormalizePem(pub), NormalizePem(reExport));
            }
            finally
            {
                if (Directory.Exists(temp)) Directory.Delete(temp, true);
            }
        }

        private static string NormalizePem(string pem)
        {
            return pem.Replace("\r", "").Replace("\n", "").Replace(" ", "").Trim();
        }
    }
}
