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
    //TODO: Create stores other than filesystem
    public class HyraxFilesystemSignatureRepositoryService : IHyraxSignatureRepositoryService
    {
        public HyraxFilesystemSignatureRepositoryService(string folderPath) => FolderPath = folderPath;

        public string FolderPath { get; set; }
        public string PublicKeyPath => Path.Combine(FolderPath, "{0}_public.pem");
        public string PrivateKeyPath => Path.Combine(FolderPath, "{0}_private.pem");

        public async Task StoreKeyForAuthor(IAuthor author, string publicKey, string privateKey)
        {
            var publicKeyPath = string.Format(PublicKeyPath, author.Username);
            var privateKeyPath = string.Format(PrivateKeyPath, author.Username);

            Directory.CreateDirectory(FolderPath);
            File.Create(publicKeyPath).Close();
            File.Create(privateKeyPath).Close();

            await File.WriteAllTextAsync(publicKeyPath, publicKey);
            await File.WriteAllTextAsync(privateKeyPath, privateKey);
        }

        public async Task<string> GetPublicKeyForAuthor(IAuthor author)
        {
            var crt = RSA.Create();

            var publicKeyPath = string.Format(PublicKeyPath, author.Username);

            if (!File.Exists(publicKeyPath))
            {
                var publicKey = crt.ExportSubjectPublicKeyInfoPem();
                var privateKey = crt.ExportPkcs8PrivateKeyPem();
                await StoreKeyForAuthor(author, publicKey, privateKey);
                return publicKey;
            }

            var pem = await File.ReadAllTextAsync(publicKeyPath);
            crt.ImportFromPem(pem);
            return crt.ExportSubjectPublicKeyInfoPem();
        }
    }
}
