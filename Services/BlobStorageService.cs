using Azure.Storage.Blobs;
using System.IO;
using System.Threading.Tasks;

//example code for upload/download when injecting at bottom..........SCROLL DOWN

namespace AppDev2Project.Services
{
    public class BlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;

        // Constructor to inject BlobServiceClient other services/controllers etc
        public BlobStorageService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        // Method to upload a file to Blob
        public async Task UploadFileAsync(string containerName, string fileName, Stream fileStream)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync(); // Create the container if it doesn't exist

            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileStream, overwrite: true); // Upload the file, overwriting if necessary
        }

        // Method to download a file from Blob Storage
        public async Task DownloadFileAsync(string containerName, string fileName, Stream destinationStream)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.DownloadToAsync(destinationStream); // Download the file
        }
    }
}


//ATTENTION *** please read ****
/*  //<----remove star to enable syntax-highlighting... please put back when done referencing =) thanks


// How to use the BlobStorageService in a controller or other service

// Injecting BlobStorageService into a controller
public class FileController : Controller
{
    private readonly BlobStorageService _blobStorageService;

    // Constructor to inject BlobStorageService
    public FileController(BlobStorageService blobStorageService)
    {
        _blobStorageService = blobStorageService;
    }

    // Action to handle file upload
    [HttpPost]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file != null && file.Length > 0)
        {
            using (var fileStream = file.OpenReadStream())
            {
                // Upload the file to Blob Storage
                await _blobStorageService.UploadFileAsync("your-container-name", file.FileName, fileStream);
            }
            return Ok("File uploaded successfully!");
        }
        return BadRequest("No file uploaded.");
    }

    // Action to handle file download
    [HttpGet]
    public async Task<IActionResult> DownloadFile(string fileName)
    {
        var memoryStream = new MemoryStream();
        
        // Download the file from Blob Storage
        await _blobStorageService.DownloadFileAsync("your-container-name", fileName, memoryStream);
        
        // Return the file as a response
        memoryStream.Position = 0;
        return File(memoryStream, "application/octet-stream", fileName);
    }
}

/*
In the above example:

- The BlobStorageService is injected into the controller using constructor injection.
- The `UploadFileAsync` method is used to upload files to Azure Blob Storage.
- The `DownloadFileAsync` method is used to download them.
- In the `UploadFile` action, we check if the file is not null and then upload it using the `UploadFileAsync` method.
- In the `DownloadFile` action, we download the file from Blob Storage and return it as a response to the user.
*/
