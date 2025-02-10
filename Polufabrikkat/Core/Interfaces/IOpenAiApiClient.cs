using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polufabrikkat.Core.Interfaces
{
    public interface IOpenAiApiClient
    {
        Task<string> GenerateTextAsync(string prompt);
    }
}
