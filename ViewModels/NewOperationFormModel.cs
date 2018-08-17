using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using SensorFusion.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SensorFusion.ViewModels
{
    public class NewOperationFormModel
    {
		//for displaying
		public IEnumerable<SelectListItem> hospitals { get; set; }
		public IEnumerable<SelectListItem> rooms { get; set; }
		public IEnumerable<SelectListItem> typesOfOperation { get; set; }
		public IEnumerable<SelectListItem> patients { get; set; }
		public IEnumerable<SelectListItem> staff { get; set; }



		//from the user
		public IList<IFormFile> videoFiles { get; set; }
		public IList<IFormFile> audioFiles { get; set; }
		public string roomNo { get; set; }
		public long patientID { get; set; }
		public int hospitalID { get; set; }
		public int[] staffIDs { get; set; }
		public int operationTypeID { get; set; }
		public DateTime UploadedDate  { get; set; }

		//for manipulating
		public List<Video> videos { get; set; }
		public List<Audio> audios { get; set; }
		public double maxDuration { get; set; }
		public DateTime date { get; set; }



		//for searching
		public DateTime fromDate { get; set; }
		public DateTime toDate { get; set; }



		public string JavascriptToRun { get; set; }


	}
}
