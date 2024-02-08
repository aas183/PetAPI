using Azure.Storage;
using Azure.Storage.Blobs;
using PetAPI.Models;


namespace PetAPI.Services;

public class FileService
{
    private readonly string _storageAccount = "petimagestorage";
    private readonly string _key = "SZTXqgn0X2s3B42WxfI2aS8ZHL61j6BYzPTe6kqxLrcgdBczzduXWadnFMtJ8guE7B98xeSXgDpB+AStRX72QQ==";
    private readonly BlobContainerClient _fileContainer;

    public FileService()
    {
        var credential = new StorageSharedKeyCredential(_storageAccount, _key);
        var blobUri = $"https://{_storageAccount}.blob.core.windows.net";
        var blobServiceClient = new BlobServiceClient (new Uri(blobUri), credential);
        _fileContainer = blobServiceClient.GetBlobContainerClient("pet-images");
    }

    public async Task<List<BlobDto>> ListAsync()
    {
        List<BlobDto> files = new List<BlobDto>();

        await foreach(var file in _fileContainer.GetBlobsAsync())
        {
            string uri = _fileContainer.Uri.ToString();
            var name = file.Name;
            var fullUri = $"{uri}/{name}";

            files.Add(new BlobDto
            {
                Uri = fullUri,
                Name = name,
                ContentType = file.Properties.ContentType,

            }); 
            
        }
        return files;
    }

    public async Task<BlobResponseDto> UploadImageAsync(IFormFile blob)
    {
        //Add try catches
        BlobResponseDto response = new();
        BlobClient client = _fileContainer.GetBlobClient(blob.FileName);

        await using (Stream? data = blob.OpenReadStream())
        {
            await client.UploadAsync(data);
        }

        response.Status = $"File {blob.FileName} Uploaded Successfully";
        response.Error = false;
        response.Blob.Uri = client.Uri.AbsoluteUri;
        response.Blob.Name = client.Name;

        return response;
    }

    public async Task<BlobDto?>  DownloadImageAsync(string blobFilename)
    {
        BlobClient file = _fileContainer.GetBlobClient(blobFilename);

        if (await file.ExistsAsync())
        {
            var data = await file.OpenReadAsync();
            Stream blobContent = data;

            var content = await file.DownloadContentAsync();

            string name = blobFilename;
            string contentType = content.Value.Details.ContentType;

            return new BlobDto { Content = blobContent, Name = name, ContentType = contentType };
        }

        return null;
    }

    public async Task<BlobResponseDto> DeleteImageAsync(string blobFilename)
    {
        BlobClient file = _fileContainer.GetBlobClient(blobFilename);

        await file.DeleteAsync();

        return new BlobResponseDto { Error = false, Status = $"File: {blobFilename} has been successfully deleted" };
    }
}
