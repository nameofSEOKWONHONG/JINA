using System.Net.Mime;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using eXtensionSharp;
using Jina.Storage.Abstract;
using Jina.Storage.Data;

namespace Jina.Storage.Impl;

public class AzureStorage : IStorage
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly BlobContainerClient _blobContainerClient;
    
    public AzureStorage(string connectionString, string containerName = "")
    {
        _blobServiceClient = new BlobServiceClient(connectionString);
        _blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName.xIsEmpty() ? "root" : containerName);
    }

    /// <summary>
    /// 파일 다운로드
    /// </summary>
    /// <param name="fullUri"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public async Task<string> FileDownloadUrl(string fullUri, string fileName)
    {
        return await Task.Factory.StartNew(() =>
        {
            var blobClientUrl = fullUri.xSubstring(44);
            // var accountSasBuilder = new AccountSasBuilder(AccountSasPermissions.Read,
            //     DateTimeOffset.UtcNow.AddMinutes(360),
            //     AccountSasServices.Blobs, AccountSasResourceTypes.Object);
            var client = _blobContainerClient.GetBlobClient(blobClientUrl);

            // Create a SAS token that's also valid for 7 days.
            ContentDisposition cd = new ContentDisposition();
            cd.FileName = fileName;
            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = client.BlobContainerName,
                BlobName = client.Name,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow,
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(360),
                ContentDisposition = cd.ToString(),
            };
            // Specify read and write permissions for the SAS.
            sasBuilder.SetPermissions(BlobSasPermissions.Read /*| BlobSasPermissions.Write*/);
            return client.GenerateSasUri(sasBuilder).ToString();
        });
    }
    
    /// <summary>
    /// 파일 존재 유무
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="hashedFileName"></param>
    public async Task<bool> FileExist(string userId, string hashedFileName)
    {
        var client = _blobContainerClient.GetBlobClient($"{userId}/{hashedFileName}");
        var res = await client.ExistsAsync();
        return res.Value;
    }


    /// <summary>
    /// 파일 삭제
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="hashedFileName"></param>
    /// <returns></returns>
    public async Task<bool> FileDelete(string userId, string hashedFileName)
    {
        var client = _blobContainerClient.GetBlobClient($"{userId}/{hashedFileName}");
        var res = await client.DeleteIfExistsAsync();
        return res.Value;
    }

    public async Task<StorageFile> FileUpload(string userId, StorageFile file)
    {
        var blobUri = $"{userId}/{file.HashedFileName}";
        var client = _blobContainerClient.GetBlobClient(blobUri);
        var res = await client.UploadAsync(new BinaryData(file.FileBuffer), new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = file.ContentType },
            TransferOptions = new StorageTransferOptions
            {
                InitialTransferSize = 1024 * 1024,
                MaximumConcurrency = 10
            },
            Metadata = new Dictionary<string, string>()
            {
                {"Filename", file.FileName.xToBase64()} 
            }
        });
        file.FullUri = blobUri.ToString();
        return file;
    }
}