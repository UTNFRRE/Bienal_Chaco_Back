using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Contexts;

namespace Servicios
{
    public class AzureBlobStorageService : IAzureStorageService
    {
        private readonly string azureStorageConnectionString;
        private readonly string containerName;

        public AzureBlobStorageService()
        {
            this.azureStorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=bienalobjectstorage;AccountKey=HNvY3gIDOrAU13taKy1SIYJ0JXntAomzUzHQYfD8BQULlxaz+n1dUkSdxQ8n9RsaI48s/f11Mjrm+ASt/6tBCA==;EndpointSuffix=core.windows.net";
            this.containerName = "imagenes";
        }

        public async Task<string> UploadAsync(IFormFile file, string blobFilename = null)
        {
            if (file.Length == 0) return null;

            var blobContainerCliente = new BlobContainerClient(this.azureStorageConnectionString, this.containerName);

            if (string.IsNullOrEmpty(blobFilename))
            {
                blobFilename = Guid.NewGuid().ToString();
            }

            var blobCliente = blobContainerCliente.GetBlobClient(blobFilename);

            var blobHttpHeader = new BlobHttpHeaders { ContentType = file.ContentType };

            await using (Stream stream = file.OpenReadStream())
            {
                await blobCliente.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = blobHttpHeader });

            }

            return blobFilename;


        }

        public async Task DeleteAsync(string blobFilename)
        {
            var blobContainerCliente = new BlobContainerClient(this.azureStorageConnectionString, this.containerName);
            var blobCliente = blobContainerCliente.GetBlobClient(blobFilename);

            await blobCliente.DeleteAsync();
        }
    }

    public interface IAzureStorageService
    {
        Task<string> UploadAsync(IFormFile file, string blobFilename = null);

        Task DeleteAsync(string blobFilename);
    }

}
