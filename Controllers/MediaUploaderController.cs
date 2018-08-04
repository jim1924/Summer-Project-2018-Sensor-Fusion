using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SensorFusion.Controllers
{
    public class MediaUploaderController : Controller
    {


		//[HttpPost]
		//[DisableRequestSizeLimit]
		//public async Task UploadVideos(IList<IFormFile> files)
		//{
		//	long size = files.Sum(f => f.Length);
		//	Console.WriteLine("The size of all the selected files is:"+size);

		//	Console.WriteLine("the file name is" + files[0].FileName);


		//	string type = files[0].ContentType;
		//	if (type.Equals("video/mp4"))
		//	{
		//		Console.WriteLine("Indeed a mp4 format");
		//	}
			
		//	// full path to file in temp location
		//	var filePath = Path.GetTempFileName();

		//	//Console.WriteLine("the length of the file is :"+ files[0].Length +"bytes");

		//	BlobsController storage = new BlobsController();

			


		//	foreach (var singleFile in files)
		//	{
		//		if (singleFile.Length > 0)
		//		{
		//			await storage.UploadBlob("customcontainer", singleFile.FileName, singleFile);
		//		}
		//	}



		//}
	}
}