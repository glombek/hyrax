using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using hyrax.Core.Models;

namespace hyrax.Core.Services
{
    public interface IHyraxSignatureRepositoryService
    {
        Task StoreKeyForAuthor(IAuthor author, string publicKey, string privateKey);
        Task<string> GetPublicKeyForAuthor(IAuthor author);
    }
}
