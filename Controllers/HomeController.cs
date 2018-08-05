using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SensorFusion.Models;
using SensorFusion.ViewModels;

namespace SensorFusion.Controllers
{
    public class HomeController : Controller
    {
		DBContext _context;
		private IHostingEnvironment _hostingEnvironment;


		public HomeController(DBContext context, IHostingEnvironment environment)
		{
			_context = context;
			_hostingEnvironment = environment;
		}
        public IActionResult Index()
        {
            return View();
        }


		[HttpGet]
		public IActionResult NewOperation()
        {
            ViewData["Message"] = "Your application description page.";
			var model = new NewOperationFormModel();

			model.typesOfOperation = new SelectList(_context.GetAllTypes().Select(x => new SelectListItem { Value = x.operationTypeID.ToString(), Text = x.name }), "Value", "Text");



			model.staff = new SelectList(_context.GetAllStaff().Select(x => new SelectListItem { Value = x.staffID.ToString(), Text ="ID: "+ x.staffID+ " "+x.firstName +" "+ x.lastName }), "Value", "Text");

			model.hospitals = new SelectList(_context.GetAllHospitals().Select(x => new SelectListItem { Value = x.hospitalID.ToString(), Text = x.name }), "Value", "Text");

			model.patients = new SelectList(_context.GetAllPatients().Select(x => new SelectListItem { Value = x.patientID.ToString(), Text = x.firstName+" "+ x.lastName }), "Value", "Text");


			SelectListItem defau = new SelectListItem { Text = "Please select a room...", Value = "error", Selected = true };
			List<SelectListItem> defaultSelection=new List<SelectListItem>();
			defaultSelection.Add(defau);
			model.rooms = defaultSelection;


			return View(model);	
        }


		[HttpPost]
		public async Task<IActionResult> NewOperation(NewOperationFormModel model )
		{
			BlobsController storage = new BlobsController(_hostingEnvironment);
			
			var path = _hostingEnvironment.WebRootPath;
			long nextID =_context.GetNextOperationID();
			string containerName = "operation" + nextID;
			model.maxDuration = 0;
			model.date = new DateTime(9000, 1, 1);
			if (model.videoFiles!=null)
			{
				int i = 1;
				model.videos = new List<Video>();
				foreach (var VideoFile in model.videoFiles)
				{
					if (VideoFile.Length > 0)
					{
						string[] type = VideoFile.ContentType.ToString().Split('/');
						if (!type[0].Equals("video"))
						{
							continue;
						}
						string videoName = "video" + i+"."+type[1];
						await storage.UploadBlob(containerName, videoName, VideoFile);
						var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "TempFiles");
						var filePath = Path.Combine(uploads, videoName);
						MediaUtilities mediaUtil = new MediaUtilities(_hostingEnvironment,videoName);
						using (var fileStream = new FileStream(filePath, FileMode.Create))
						{
							await VideoFile.CopyToAsync(fileStream);
						}


						model.videos.Add(new Video() {
							OperationID = nextID,
							size_bytes = mediaUtil.GetVideoSize(),
							timeStamp = mediaUtil.GetVideoEncodedDate(),
							type = type[1],
							duration = mediaUtil.GetVideoDuration().TotalMilliseconds,
							fileName = videoName
						});
						mediaUtil.PrintVideoAvailableProperties();
						Console.WriteLine("The size of the video in bytes is: "+ mediaUtil.GetVideoSize());
						Console.WriteLine("The encoded date is: "+mediaUtil.GetVideoEncodedDate());
						Console.WriteLine("The first type of the video is: " + type[0]);
						Console.WriteLine("The second type of the video is: "+type[1]);
						Console.WriteLine("The duration of the video in seconds is: "+ mediaUtil.GetVideoDuration().TotalSeconds);



						if (mediaUtil.GetVideoDuration().TotalMilliseconds>model.maxDuration)
						{
							model.maxDuration = mediaUtil.GetVideoDuration().TotalMilliseconds;
						}
						if (model.date.CompareTo(mediaUtil.GetVideoEncodedDate()) > 0)
						{
							model.date = mediaUtil.GetVideoEncodedDate();
						}
					i++;
					}
				}
			}




			if (model.audioFiles != null)
			{
				int i = 1;
				model.audios = new List<Audio>();
				foreach (var audioFile in model.audioFiles)
				{
					if (audioFile.Length > 0)
					{

						string[] type = audioFile.ContentType.ToString().Split('/');
						if (!type[0].Equals("audio"))
						{
							continue;
						}
						string audioName = "audio" + i + "." + type[1];
						await storage.UploadBlob(containerName, audioName, audioFile);
						var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "TempFiles");
						var filePath = Path.Combine(uploads, audioName);
						MediaUtilities mediaUtil = new MediaUtilities(_hostingEnvironment, audioName);
						using (var fileStream = new FileStream(filePath, FileMode.Create))
						{
							await audioFile.CopyToAsync(fileStream);
						}



						model.audios.Add(new Audio()
						{
							OperationID = nextID,
							size_bytes = audioFile.Length,
							timeStamp = mediaUtil.GetAudioEarlierDate(audioFile.FileName),
							type = type[1],
							duration = mediaUtil.GetAudioDuration().TotalMilliseconds,
							fileName = audioName
						});
						mediaUtil.PrintAudioAvailableProperties();
						Console.WriteLine("The size of the audio file in bytes is:" + mediaUtil.GetAudioSize());
						Console.WriteLine("The earliert date of the specific file is: " +mediaUtil.GetAudioEarlierDate(audioFile.FileName));
						Console.WriteLine("the type0 of the audio file is " + type[0]);
						Console.WriteLine("the type1 of the audio file is " + type[1]);
						Console.WriteLine("the duration of the audio file in seconds is " + mediaUtil.GetAudioDuration().TotalSeconds);
						Console.WriteLine("the duration of the audio file in minutes is " + mediaUtil.GetAudioDuration().TotalMinutes);
						Console.WriteLine("the name of the audio file is " + audioName);




						if (mediaUtil.GetAudioDuration().TotalMilliseconds > model.maxDuration)
						{
							model.maxDuration = mediaUtil.GetAudioDuration().TotalMilliseconds;
						}
						if (model.date.CompareTo(mediaUtil.GetAudioEarlierDate(audioFile.FileName)) > 0)
						{
							model.date = mediaUtil.GetAudioEarlierDate(audioFile.FileName);
						}
						i++;
					}
				}
			}
			Console.WriteLine("the earliest recorded date is:" + model.date);

			MediaUtilities.CleanTempFolder(_hostingEnvironment);


			_context.InsertOperation(model);



			ViewData["Message"] = "Your application description page.";
			return RedirectToAction(nameof(Index));

		}





		public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
