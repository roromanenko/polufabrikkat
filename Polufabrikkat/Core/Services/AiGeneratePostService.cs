using MongoDB.Driver;
using Polufabrikkat.Core.Interfaces;
using Polufabrikkat.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polufabrikkat.Core.Services
{
    internal class AiGeneratePostService : IAiGeneratePostService
    {
        private readonly IUnsplashApiClient _unsplashApiClient;
        private readonly IOpenAiApiClient _openAiApiClient;
        private readonly IAiImageProcessor _aiImageProcessor;

        public AiGeneratePostService(IUnsplashApiClient unsplashApiClient, IOpenAiApiClient openAiApiClient, IAiImageProcessor aiImageProcessor)
        {
            _unsplashApiClient = unsplashApiClient;
            _openAiApiClient = openAiApiClient;
            _aiImageProcessor = aiImageProcessor;
        }

        public async Task<byte[]> GetGeneratedImage(string query, string prompt)
        {
            string imageUrl = await _unsplashApiClient.GetRandomImageUrlAsync(query);
            string generatedText = await _openAiApiClient.GenerateTextAsync(prompt);

            byte[] resultImage = await _aiImageProcessor.AddTextToImageAsync(imageUrl, generatedText);

            return resultImage;
        }
    }
}
