namespace Polufabrikkat.Core.Interfaces
{
    public interface IAiGeneratePostService
    {
        Task<byte[]> GetGeneratedImage(string query, string prompt);
    }
}