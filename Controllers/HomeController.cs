using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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


		[HttpGet]
		public IActionResult Index()
		{
			var searchOperationModel = new SearchOperationViewModel();
			var newOperationModel = new NewOperationFormViewModel();

			newOperationModel.typesOfOperation = new SelectList(_context.GetAllTypes().Select(x => new SelectListItem { Value = x.operationTypeID.ToString(), Text = x.name }), "Value", "Text");
			newOperationModel.staff = new SelectList(_context.GetAllStaff().Select(x => new SelectListItem { Value = x.staffID.ToString(), Text = "ID: " + x.staffID + " " + x.firstName + " " + x.lastName }), "Value", "Text");
			newOperationModel.hospitals = new SelectList(_context.GetAllHospitals().Select(x => new SelectListItem { Value = x.hospitalID.ToString(), Text = x.name }), "Value", "Text");
			newOperationModel.patients = new SelectList(_context.GetAllPatients().Select(x => new SelectListItem { Value = x.patientID.ToString(), Text = "ID: " + x.patientID + " " + x.firstName + " " + x.lastName }), "Value", "Text");
			SelectListItem defau = new SelectListItem { Text = "Please select a room...", Value = "error", Selected = true };
			List<SelectListItem> defaultSelection = new List<SelectListItem>();
			defaultSelection.Add(defau);
			newOperationModel.rooms = defaultSelection;
			searchOperationModel.ViewOperations = _context.Get20MostRecentOperations();
			searchOperationModel.searchFields = newOperationModel;
			return View(searchOperationModel);
		}



		[HttpPost]
		[AutoValidateAntiforgeryToken]
		public IActionResult Index(SearchOperationViewModel model)
		{
			bool hospitalSelected = model.searchFields.hospitalID != 0;
			bool roomSelected = !model.searchFields.roomNo.Equals("error");
			bool fromDateSelected = !(model.searchFields.fromDate == new DateTime());
			bool toDateSelected = !(model.searchFields.toDate == new DateTime());
			bool staffSelected = model.searchFields.staffIDs != null;
			bool patientSelected = model.searchFields.patientID != 0;


			var searchOperationModel = new SearchOperationViewModel();



			if (hospitalSelected || roomSelected || fromDateSelected || toDateSelected || staffSelected || patientSelected)
			{
				Operation filters = new Operation();
				filters.fromDate = model.searchFields.fromDate;
				filters.toDate = model.searchFields.toDate;
				if (hospitalSelected)
				{
					filters.hospitalID = model.searchFields.hospitalID;
				}
				if (roomSelected)
				{
					filters.roomNO = model.searchFields.roomNo;
				}
				if (staffSelected)
				{
					filters.staffIDs = model.searchFields.staffIDs;
				}
				if (patientSelected)
				{
					filters.patientID = model.searchFields.patientID;
				}

				var newOperationModel = new NewOperationFormViewModel();

				newOperationModel.typesOfOperation = new SelectList(_context.GetAllTypes().Select(x => new SelectListItem { Value = x.operationTypeID.ToString(), Text = x.name }), "Value", "Text");
				newOperationModel.staff = new SelectList(_context.GetAllStaff().Select(x => new SelectListItem { Value = x.staffID.ToString(), Text = "ID: " + x.staffID + " " + x.firstName + " " + x.lastName }), "Value", "Text");
				newOperationModel.hospitals = new SelectList(_context.GetAllHospitals().Select(x => new SelectListItem { Value = x.hospitalID.ToString(), Text = x.name }), "Value", "Text");
				newOperationModel.patients = new SelectList(_context.GetAllPatients().Select(x => new SelectListItem { Value = x.patientID.ToString(), Text = "ID: " + x.patientID + " " + x.firstName + " " + x.lastName }), "Value", "Text");
				SelectListItem defau = new SelectListItem { Text = "Please select a room...", Value = "error", Selected = true };
				List<SelectListItem> defaultSelection = new List<SelectListItem>();
				defaultSelection.Add(defau);
				newOperationModel.rooms = defaultSelection;
				searchOperationModel.searchFields = newOperationModel;

				searchOperationModel.ViewOperations = _context.Get20MostRecentOperations();

				searchOperationModel.ViewOperations = _context.GetFilteredOperations(filters);

				return View(searchOperationModel);
			}




			return RedirectToAction("Index");
		}

		[HttpGet]
		public IActionResult Details(long id)
		{
			SingleOperationViewModel model = _context.GetFullDetailsOfOperation(id);

			if (model == null)
			{
				return RedirectToAction("Index");
			}
			return View(model);
		}



		[HttpGet]
		public IActionResult NewOperation()
        {
			var model = new NewOperationFormViewModel();

			model.typesOfOperation = new SelectList(_context.GetAllTypes().Select(x => new SelectListItem { Value = x.operationTypeID.ToString(), Text = x.name }), "Value", "Text");
			model.staff = new SelectList(_context.GetAllStaff().Select(x => new SelectListItem { Value = x.staffID.ToString(), Text ="ID: "+ x.staffID+ " "+x.firstName +" "+ x.lastName }), "Value", "Text");
			model.hospitals = new SelectList(_context.GetAllHospitals().Select(x => new SelectListItem { Value = x.hospitalID.ToString(), Text = x.name }), "Value", "Text");
			model.patients = new SelectList(_context.GetAllPatients().Select(x => new SelectListItem { Value = x.patientID.ToString(), Text ="ID: "+x.patientID+" "+  x.firstName+" "+ x.lastName }), "Value", "Text");
			SelectListItem defau = new SelectListItem { Text = "Please select a room...", Value = "error", Selected = true };
			List<SelectListItem> defaultSelection=new List<SelectListItem>();
			defaultSelection.Add(defau);
			model.rooms = defaultSelection;


			return View(model);	
        }


		[HttpPost]
		[AutoValidateAntiforgeryToken]
		public async Task<IActionResult> NewOperation(NewOperationFormViewModel model )
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
						string fullVideoPath = storage.GetBlobFullPath(containerName, videoName);

						model.videos.Add(new Video() {
							OperationID = nextID,
							size_bytes = mediaUtil.GetVideoSize(),
							timeStamp = mediaUtil.GetVideoEncodedDate(),
							type = type[1],
							duration = mediaUtil.GetVideoDuration().TotalMilliseconds,
							fileName = videoName,
							fullPath = fullVideoPath

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


						string fullAudioPath = storage.GetBlobFullPath(containerName, audioName);

						model.audios.Add(new Audio()
						{
							OperationID = nextID,
							size_bytes = audioFile.Length,
							timeStamp = mediaUtil.GetAudioEarliestDate(audioFile.FileName),
							type = type[1],
							duration = mediaUtil.GetAudioDuration().TotalMilliseconds,
							fileName = audioName,
							fullPath=fullAudioPath
						});
						mediaUtil.PrintAudioAvailableProperties();
						Console.WriteLine("The size of the audio file in bytes is:" + mediaUtil.GetAudioSize());
						Console.WriteLine("The earliert date of the specific file is: " +mediaUtil.GetAudioEarliestDate(audioFile.FileName));
						Console.WriteLine("the type0 of the audio file is " + type[0]);
						Console.WriteLine("the type1 of the audio file is " + type[1]);
						Console.WriteLine("the duration of the audio file in seconds is " + mediaUtil.GetAudioDuration().TotalSeconds);
						Console.WriteLine("the duration of the audio file in minutes is " + mediaUtil.GetAudioDuration().TotalMinutes);
						Console.WriteLine("the name of the audio file is " + audioName);




						if (mediaUtil.GetAudioDuration().TotalMilliseconds > model.maxDuration)
						{
							model.maxDuration = mediaUtil.GetAudioDuration().TotalMilliseconds;
						}
						if (model.date.CompareTo(mediaUtil.GetAudioEarliestDate(audioFile.FileName)) > 0)
						{
							model.date = mediaUtil.GetAudioEarliestDate(audioFile.FileName);
						}
						i++;
					}
				}
			}

			if (model.monitorFile!=null)
			{
				if (model.monitorFile.Length > 0)
				{
					model.patientsMonitoringFile = new PatientsMonitoringFile();
					string[] name = model.monitorFile.FileName.Split('.');
					Console.WriteLine("The fucking name is ");
					name.ToList().ForEach(Console.WriteLine);
					string suffix = name[name.Length - 1];
					string type = model.monitorFile.ContentType.ToString();

					string fileMonitorName = "patients-monitoring-file"+"."+suffix;
					await storage.UploadBlob(containerName, fileMonitorName, model.monitorFile);
					var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "TempFiles");
					var filePath = Path.Combine(uploads, fileMonitorName);
					MediaUtilities mediaUtil = new MediaUtilities(_hostingEnvironment, fileMonitorName);
					using (var fileStream = new FileStream(filePath, FileMode.Create))
					{
						await model.monitorFile.CopyToAsync(fileStream);
					}


					string fullFilePath = storage.GetBlobFullPath(containerName, fileMonitorName);


					model.patientsMonitoringFile.OperationID = nextID;
					model.patientsMonitoringFile.size_bytes = model.monitorFile.Length;
					model.patientsMonitoringFile.timeStamp = mediaUtil.GetFileEarliestDate(model.monitorFile.FileName);
					model.patientsMonitoringFile.type = type;
					model.patientsMonitoringFile.fileName = fileMonitorName;
					model.patientsMonitoringFile.fullPath = fullFilePath;

				}

			}
			Console.WriteLine("the earliest recorded date is:" + model.date);

			MediaUtilities.CleanTempFolder(_hostingEnvironment);


			_context.InsertOperation(model);

			TempData["msg"] = "<script>alert('Operation uploaded successfully');</script>";

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
