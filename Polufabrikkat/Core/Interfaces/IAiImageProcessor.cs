namespace Polufabrikkat.Core.Interfaces
{
    public interface IAiImageProcessor
    {
        Task<byte[]> AddTextToImageAsync(string imageUrl, string text);
    }
}