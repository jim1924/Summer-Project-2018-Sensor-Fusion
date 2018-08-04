using Microsoft.AspNetCore.Mvc;
using SensorFusion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SensorFusion.Controllers
{
    public class HospitalsController: Controller
    {
		public IActionResult Index(int id = 0)
		{
			DBContext context = HttpContext.RequestServices.GetService(typeof(SensorFusion.Models.DBContext)) as DBContext;//just create the object/connection

			return View(context.GetAllHospitals());
		}

		[HttpPost]
		public IActionResult Index()
		{

			return View();
		}

	}
}
