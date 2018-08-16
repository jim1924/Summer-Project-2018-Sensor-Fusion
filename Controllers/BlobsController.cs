using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.AspNetCore.Hosting;

namespace SensorFusion.Controllers
{
	public class BlobsController : Controller
    {
		CloudBlobClient blobClient;
		private IHostingEnvironment _env;

		public BlobsController(IHostingEnvironment env)
		{
			_env = env;
			var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
			IConfigurationRoot Configuration = builder.Build();
			CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Configuration["ConnectionStrings:sensorfusionstorage_AzureStorageConnectionString"]);
			blobClient = storageAccount.CreateCloudBlobClient();
		}

		//this method return a CloudBlobContainer
		private CloudBlobContainer GetCloudBlobContainer(string blobName)
		{

			//"test blob container" doesn't exist yet but it creates reference to it,
			//so that the container can be created with the CreateIfNotExists method below
			CloudBlobContainer container = blobClient.GetContainerReference(blobName);

			return container;
		}


		//THIS METHOD IS CALLING THE GetCloudBlobContainer. So this method is the entry point to create the container.
		public async Task<ActionResult> CreateBlobContainer(string containerName)
		{
			//Here we're calling the GetCloudBlobContainer method (which takes the container name and return the equivalent container
			CloudBlobContainer container = GetCloudBlobContainer(containerName);

			//check if the container exists and create a new if it doesn't
			ViewBag.Success = container.CreateIfNotExistsAsync().Result;

			//Update the view bag with the name of the blob container
			ViewBag.BlobContainerName = container.Name;
			BlobContainerPermissions permissions = await container.GetPermissionsAsync();
			permissions.PublicAccess = BlobContainerPublicAccessType.Container;
			await container.SetPermissionsAsync(permissions);


			return View();
		}


		public async Task UploadBlob(string containerName,string blobName,IFormFile file)
		{

			//try to get the container with the specified name
			CloudBlobContainer container = GetCloudBlobContainer(containerName);
			//if there is no container, create a new one
			if (! await container.ExistsAsync())
			{
				await CreateBlobContainer(containerName);
			//try again to read the container
			container = GetCloudBlobContainer(containerName);

			}

			//we just create the reference to the object
			CloudBlockBlob blob = container.GetBlockBlobReference(blobName);
			//The blob name is part of the URL used to retrieve a blob, and can be any string, including the name of the file.

			//after there is the blob reference, i can upload any data stream
				//this creates the blob is it doesn't exist or overwrites it if it does exist
			blob.UploadFromStreamAsync(file.OpenReadStream()).Wait();

		}


		public ActionResult ListBlobs(string containerName)
		{

			CloudBlobContainer container = GetCloudBlobContainer(containerName);

			List<string> blobs = new List<string>();
			BlobResultSegment resultSegment = container.ListBlobsSegmentedAsync(null).Result;

			foreach (IListBlobItem item in resultSegment.Results)
			{
				if (item.GetType() == typeof(CloudBlockBlob))
				{
					CloudBlockBlob blob = (CloudBlockBlob)item;
					blobs.Add(blob.Name);
				}
				else if (item.GetType() == typeof(CloudPageBlob))
				{
					CloudPageBlob blob = (CloudPageBlob)item;
					blobs.Add(blob.Name);
				}
				else if (item.GetType() == typeof(CloudBlobDirectory))
				{
					CloudBlobDirectory dir = (CloudBlobDirectory)item;
					blobs.Add(dir.Uri.ToString());
				}
			}
			//automatically goes to the Blobs views and searches for the "ListBlobs" view, passing the blobs object
			return View(blobs);

		}

		public string GetBlobFullPath(string containerName, string blobName)
		{
			CloudBlobContainer container = GetCloudBlobContainer(containerName);
			CloudBlockBlob blob = container.GetBlockBlobReference(blobName);
			return blob.StorageUri.PrimaryUri.ToString();

		}


		public string DownloadBlob(string containerName,string blobName,string downloadPath)
		{

			CloudBlobContainer container = GetCloudBlobContainer(containerName);

			CloudBlockBlob blob = container.GetBlockBlobReference(blobName);

			using (var fileStream = System.IO.File.OpenWrite(downloadPath))
			{
				blob.DownloadToStreamAsync(fileStream).Wait();
			}

			return "success!";
		}

		public string DeleteBlob(string containerName,string blobName)
		{
			CloudBlobContainer container = GetCloudBlobContainer(containerName);
			CloudBlockBlob blob = container.GetBlockBlobReference(blobName);
			blob.DeleteAsync().Wait();

			return "success!";
		}
		public string DeleteContainer(string containerName)
		{
			CloudBlobContainer container = GetCloudBlobContainer(containerName);
			container.DeleteIfExistsAsync();

			return "success!";

		}






	}
}