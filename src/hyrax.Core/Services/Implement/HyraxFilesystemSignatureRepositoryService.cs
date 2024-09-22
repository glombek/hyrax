using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using hyrax.Core.Models;
using Microsoft.AspNetCore.Mvc.Routing;

namespace hyrax.Core.Services.Implement
{
    public class HyraxFilesystemSignatureRepositoryService : IHyraxSignatureRepositoryService
    {
        public HyraxFilesystemSignatureRepositoryService(string folderPath) => FolderPath = folderPath;

        public string FolderPath { get; set; }
        public string PublicKeyPath => Path.Combine(FolderPath, "public.pem");
        public string PrivateKeyPath => Path.Combine(FolderPath, "private.pem");

        public async Task StoreKeyForAuthor(IAuthor author, string publicKey, string privateKey)
        {
            Directory.CreateDirectory(FolderPath);
            File.Create(PublicKeyPath).Close();
            File.Create(PrivateKeyPath).Close();

            await File.WriteAllTextAsync(PublicKeyPath, publicKey);
            await File.WriteAllTextAsync(PrivateKeyPath, privateKey);
        }

        public async Task<string> GetPublicKeyForAuthor(IAuthor author)
        {
            var crt = RSA.Create();

            if (!File.Exists(PublicKeyPath))
            {
                var publicKey = crt.ExportRSAPublicKeyPem();
                var privateKey = crt.ExportRSAPrivateKeyPem();
                await StoreKeyForAuthor(author, publicKey, privateKey);
                return publicKey;
            }
            var pem = await File.ReadAllTextAsync(PublicKeyPath);
            crt.ImportFromPem(pem);
            return crt.ExportRSAPublicKeyPem();
        }
    }
}
