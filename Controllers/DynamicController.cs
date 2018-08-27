using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SensorFusion.Models;

namespace SensorFusion.Controllers
{
    public class DynamicController : Controller
    {
		DBContext _context;
		public DynamicController(DBContext context)
		{
			_context = context;
		}
		public JsonResult UpdateRooms(int hospitalID)
		{

			var result=_context.GetAllRoomsForHospitalID(hospitalID);
			return Json(result);


		}
    }
}