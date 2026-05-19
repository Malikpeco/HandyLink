using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services.Hashing
{
    public interface IHashingService
    {
        string GenerateSalt();
        string HashText(string text, string salt);
        bool Verify(string hash, string salt, string providedText);

    }
}
