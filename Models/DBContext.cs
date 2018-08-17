using MySql.Data.MySqlClient;
using SensorFusion.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SensorFusion.Models
{
	public class DBContext
	{
		public string ConnectionString { get; set; }

		public DBContext(string connectionString)
		{
			this.ConnectionString = connectionString;
		}

		private MySqlConnection GetConnection()
		{
			return new MySqlConnection(ConnectionString);
		}

		public IEnumerable<Hospital> GetAllHospitals()

		{
			List<Hospital> list = new List<Hospital>();


			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("SELECT * FROM hospital", conn);
				using (MySqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						list.Add(new Hospital()
						{
							hospitalID = reader.GetInt32("hospitalID"),
							name = reader.GetString("name"),
							address = reader.GetString("address"),
							postCode = reader.GetString("postCode"),
							city = reader.GetString("city")
						});
					}
				}
			}

			return list;
		}

		public IEnumerable<TypeOfOperation> GetAllTypes()
		{
			List<TypeOfOperation> list = new List<TypeOfOperation>();


			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("SELECT * FROM Type_Of_Operation", conn);
				using (MySqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						list.Add(new TypeOfOperation()
						{
							operationTypeID = reader.GetInt32("operationTypeID"),
							name = reader.GetString("description")
						});
					}
				}
			}

			return list;


		}

		public long GetNextOperationID()
		{
			long next = 0;
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("SELECT Auto_increment FROM information_schema.tables WHERE table_name='operation'", conn);
				using (MySqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						next = reader.GetInt64("Auto_increment");

					}
				}
			}
			return next;



		}

		public IEnumerable<SingleOperationViewModel> Get20MostRecentOperations()
		{

			List<SingleOperationViewModel> list = new List<SingleOperationViewModel>();


			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand cmd = new MySqlCommand(
				"select twentyoperations.operationID,hospital.name AS 'Hospital Name',hospital_operating_room.roomNO,twentyoperations.dateStamp,patient.firstName AS 'Patients first name',patient.lastName AS 'Patients last name',patient.patientID " +
				" from twentyoperations inner join hospital ON twentyoperations.hospitalID = hospital.hospitalID 		" +
				" inner join hospital_operating_room ON twentyoperations.roomNO = hospital_operating_room.roomNO" +
				" inner join patient ON twentyoperations.patientID = patient.patientID ", conn);
				using (MySqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						SingleOperationViewModel operation = new SingleOperationViewModel();
						operation.date = (DateTime) reader.GetMySqlDateTime("dateStamp");
						operation.hospitalName = reader.GetString("Hospital Name");
						operation.operationID = reader.GetInt64("operationID");
						operation.patient = new Patient();
						operation.patient.firstName = reader.GetString("Patients first name");
						operation.patient.lastName = reader.GetString("Patients last name");
						operation.patient.patientID=reader.GetInt64("patientID");
						operation.roomNO = reader.GetString("roomNO");

						operation.staff = GetStaffForOperationID(operation.operationID);
						list.Add(operation);
					}
				}
			}

			return list;
		}

		public IEnumerable<SingleOperationViewModel> GetFilteredOperations(Operation filters)
		{
			List<SingleOperationViewModel> list = new List<SingleOperationViewModel>();

			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand cmd = conn.CreateCommand();
				string staffQuery = "";
				if (filters.staffIDs != null)
				{
					string staffNumbers = filters.staffIDs[0].ToString();
					for (int i = 1; i < filters.staffIDs.Count(); i++)
					{
						staffNumbers = staffNumbers + "," + filters.staffIDs[i].ToString();
					}
					staffNumbers = "(" + staffNumbers + ")";
					staffQuery = "AND operation.operationID in ( select operations_staff.operationID FROM operations_staff WHERE operations_staff.staffID in " + staffNumbers + " group by operations_staff.operationID  having count(operation.operationID)=" + filters.staffIDs.Count().ToString()+")";
				}


				cmd.CommandText =
				"select operation.operationID,hospital.name AS 'Hospital Name',hospital_operating_room.roomNO,operation.dateStamp,patient.firstName AS 'Patients first name',patient.lastName AS 'Patients last name',patient.patientID" +
				" from operation inner join hospital ON operation.hospitalID = hospital.hospitalID" +
				" inner join hospital_operating_room ON operation.roomNO = hospital_operating_room.roomNO" +
				" inner join patient ON operation.patientID = patient.patientID" +
				" where (operation.hospitalID=?hospitalID OR ?hospitalID=0) AND (operation.roomNO=?roomNO OR ?roomNO IS NULL) AND (operation.dateStamp>?fromDate OR ?fromDate IS NULL)" +
				" AND (operation.dateStamp<?toDate OR ?toDate IS NULL) AND (operation.patientID=?patientID OR ?patientID=0) "+ staffQuery;

				cmd.Parameters.AddWithValue("?hospitalID", (filters.hospitalID != 0) ? filters.hospitalID : 0);
				cmd.Parameters.AddWithValue("?roomNO", (filters.roomNO != null) ? filters.roomNO : null);
				cmd.Parameters.AddWithValue("?fromDate", (filters.fromDate != new DateTime()) ? filters.fromDate.ToString("yyyy-MM-dd HH:mm:ss") : null);
				cmd.Parameters.AddWithValue("?toDate", (filters.toDate != new DateTime()) ? filters.toDate.ToString("yyyy-MM-dd HH:mm:ss") : null);
				cmd.Parameters.AddWithValue("?patientID", (filters.patientID != 0) ? filters.patientID : 0);





				using (MySqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						SingleOperationViewModel operation = new SingleOperationViewModel();
						operation.date = (DateTime)reader.GetMySqlDateTime("dateStamp");
						operation.hospitalName = reader.GetString("Hospital Name");
						operation.operationID = reader.GetInt64("operationID");
						operation.patient = new Patient();
						operation.patient.firstName = reader.GetString("Patients first name");
						operation.patient.lastName = reader.GetString("Patients last name");
						operation.patient.patientID = reader.GetInt64("patientID");
						operation.roomNO = reader.GetString("roomNO");
						operation.staff = GetStaffForOperationID(operation.operationID);
						list.Add(operation);


					}
				}
			}

			return list;

		}

		public SingleOperationViewModel GetFullDetailsOfOperation(long id)
		{
			SingleOperationViewModel operation = new SingleOperationViewModel();
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand cmd = new MySqlCommand(
				"select operation.operationID, operation.duration_ms, hospital.name AS 'Hospital Name', hospital_operating_room.roomNO, operation.dateStamp, patient.firstName AS 'Patients first name', patient.lastName AS 'Patients last name', patient.patientID, type_of_operation.description" +
				" from operation inner join hospital ON operation.hospitalID = hospital.hospitalID" +
				" inner join hospital_operating_room ON operation.roomNO = hospital_operating_room.roomNO" +
				" inner join patient ON operation.patientID = patient.patientID" +
				" inner join type_of_operation ON operation.operationTypeID = type_of_operation.operationTypeID" +
				" where operation.operationID='" +id+"'", conn);
				using (MySqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						operation.audioFiles = GetAudiosForOperationID(id);
						operation.staff = GetStaffForOperationID(id);
						operation.videoFiles = GetVideosForOperationID(id);
						operation.date = (DateTime)reader.GetMySqlDateTime("dateStamp");
						operation.hospitalName = reader.GetString("Hospital Name");
						operation.operationID = reader.GetInt64("operationID");
						operation.patient = new Patient();
						operation.patient.firstName = reader.GetString("Patients first name");
						operation.patient.lastName = reader.GetString("Patients last name");
						operation.patient.patientID = reader.GetInt64("patientID");
						operation.roomNO = reader.GetString("roomNO");
						operation.type = reader.GetString("description");
						operation.duration = (double)reader.GetInt64("duration_ms") / 1000 / 60;

					}
				}
			}

			return operation;





		}

		public string GetStaffForOperationID(long id)
		{
			List<Staff> list = new List<Staff>();


			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand cmd = new MySqlCommand(
				"select * from operations_staff inner join staff on operations_staff.staffID=staff.staffID WHERE operations_staff.operationID='" + id +"'", conn);
				using (MySqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						Staff objectstaff = new Staff();
						objectstaff.staffID = reader.GetInt32("staffID");
						objectstaff.firstName = reader.GetString("firstName");
						objectstaff.lastName = reader.GetString("lastName");
						list.Add(objectstaff);
					}
				}
			}
			string staff= list[0].firstName + " " + list[0].lastName;
			for (int i = 1; i < list.Count; i++)
			{
				staff = staff + ", " + list[i].firstName+" "+ list[i].lastName;
			}
			return staff;

		}
		public List<Video> GetVideosForOperationID(long id)
		{
			List<Video> list = new List<Video>();


			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand cmd = new MySqlCommand(
				"select * from video WHERE video.operationID='" + id + "'", conn);
				using (MySqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						Video video = new Video();
						video.fullPath = reader.GetString("fullPath");
						video.size_bytes = reader.GetInt64("size_bytes");
						video.fileName = reader.GetString("fileName");
						video.duration = reader.GetInt64("duration_ms");
						video.timeStamp = (DateTime)reader.GetMySqlDateTime("timeStamp");
						video.type = reader.GetString("type");
						list.Add(video);
					}
				}
			}

			return list;

		}
		public List<Audio> GetAudiosForOperationID(long id)
		{
			List<Audio> list = new List<Audio>();


			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand cmd = new MySqlCommand(
				"select * from audio WHERE audio.operationID='" + id + "'", conn);
				using (MySqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						Audio audio = new Audio();
						audio.fullPath = reader.GetString("fullPath");
						audio.size_bytes = reader.GetInt64("size_bytes");
						audio.fileName = reader.GetString("fileName");
						audio.duration = reader.GetInt64("duration_ms");
						audio.timeStamp = (DateTime)reader.GetMySqlDateTime("timeStamp");
						audio.type = reader.GetString("type");
						list.Add(audio);
					}
				}
			}

			return list;

		}

		public IEnumerable<Staff> GetAllStaff()
		{
			List<Staff> list = new List<Staff>();


			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("SELECT * FROM staff", conn);
				using (MySqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						Staff staff = new Staff();
						staff.staffID = reader.GetInt32("staffID");
						staff.firstName = reader.GetString("firstName");
						staff.lastName = reader.GetString("lastName");
						staff.address = reader.GetString("address");
						staff.hiringDate = reader.GetDateTime("hiringDate");
						staff.phoneNo = reader.GetString("phoneNO");
						if (reader.IsDBNull(5))
						{
							staff.speciality = reader.GetString("speciality");
						}

						list.Add(staff);

					}
				}
			}

			return list;

		}



		public IEnumerable<Patient> GetAllPatients()
		{

			List<Patient> list = new List<Patient>();


			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("SELECT * FROM patient", conn);
				using (MySqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						list.Add(new Patient()
						{
							patientID = reader.GetInt64("patientID"),
							firstName = reader.GetString("firstName"),
							lastName = reader.GetString("lastName"),
							address = reader.GetString("address"),
							postCode = reader.GetString("postCode"),
							phoneNO = reader.GetString("phoneNO")
						});
					}
				}
			}

			return list;
		}

		public List<OperatingRoom> UpdateRooms(int hospitalID)
		{

			List<OperatingRoom> list = new List<OperatingRoom>();


			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("SELECT * FROM hospital_operating_room WHERE hospitalID='" + hospitalID + "'", conn);
				using (MySqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						list.Add(new OperatingRoom()
						{
							hospitalID = reader.GetInt32("hospitalID"),
							roomNO = reader.GetString("roomNO"),
							size = reader.GetInt32("size_m2")

						});
					}
				}
			}
			return list;

		}


		public List<OperatingRoom> NewOperation(NewOperationFormModel model)
		{

			List<OperatingRoom> list = new List<OperatingRoom>();


			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("SELECT * FROM hospital_operating_room WHERE hospitalID='" + model.hospitalID + "'", conn);
				using (MySqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						list.Add(new OperatingRoom()
						{
							hospitalID = reader.GetInt32("hospitalID"),
							roomNO = reader.GetString("roomNO"),
							size = reader.GetInt32("size_m2")

						});
					}
				}
			}
			return list;

		}

		public void InsertOperation(NewOperationFormModel model)
		{
			model.UploadedDate = new DateTime();
			model.UploadedDate = DateTime.Now;
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				//insert the operation to the database
				MySqlCommand cmd = conn.CreateCommand();
				cmd.CommandText = "INSERT INTO operation (patientID,hospitalID,roomNO,dateStamp,duration_ms,operationTypeID,uploadedDate) VALUES (?patientID,?hospitalID,?roomNo,?date,?maxDuration,?operationTypeID,?uploadedDate)";

				cmd.Parameters.AddWithValue("?patientID", model.patientID);
				cmd.Parameters.AddWithValue("?hospitalID", model.hospitalID);
				cmd.Parameters.AddWithValue("?roomNO", model.roomNo);
				cmd.Parameters.AddWithValue("?date", model.date.ToString("yyyy-MM-dd HH:mm:ss"));
				cmd.Parameters.AddWithValue("?maxDuration", model.maxDuration);
				cmd.Parameters.AddWithValue("?operationTypeID", model.operationTypeID);
				cmd.Parameters.AddWithValue("?uploadedDate", model.UploadedDate.ToString("yyyy-MM-dd HH:mm:ss"));
				cmd.ExecuteNonQuery();
				cmd.Parameters.Clear();

				long operationID=0;
				cmd.CommandText = "SELECT `AUTO_INCREMENT`from  INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'sensor_fusionv1' AND   TABLE_NAME   = 'operation'";
				using (MySqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						operationID = reader.GetInt64("AUTO_INCREMENT") - 1;
					}
				}

				if (model.videos!=null)
				{
					foreach (var video in model.videos)
					{
						cmd.CommandText = "INSERT INTO video (operationID,size_bytes,timeStamp,type,duration_ms,fileName,fullPath) VALUES (?operationID,?size_bytes,?timeStamp,?type,?duration,?fileName,?fullPath)";
						cmd.Parameters.AddWithValue("?operationID", operationID);
						cmd.Parameters.AddWithValue("?size_bytes", video.size_bytes);
						cmd.Parameters.AddWithValue("?timeStamp", video.timeStamp);
						cmd.Parameters.AddWithValue("?type", video.type);
						cmd.Parameters.AddWithValue("?duration", video.duration);
						cmd.Parameters.AddWithValue("?fileName", video.fileName);
						cmd.Parameters.AddWithValue("?fullPath", video.fullPath);

						cmd.ExecuteNonQuery();
						cmd.Parameters.Clear();
					}

				}
				if (model.audios != null)
				{
					foreach (var audio in model.audios)
					{
						Console.WriteLine(audio.fileName);
						cmd.CommandText = "INSERT INTO audio (operationID,size_bytes,timeStamp,type,duration_ms,fileName,fullPath) VALUES (?operationID,?size_bytes,?timeStamp,?type,?duration,?fileName,?fullPath)";
						cmd.Parameters.AddWithValue("?operationID", operationID);
						cmd.Parameters.AddWithValue("?size_bytes", audio.size_bytes);
						cmd.Parameters.AddWithValue("?timeStamp", audio.timeStamp);
						cmd.Parameters.AddWithValue("?type", audio.type);
						cmd.Parameters.AddWithValue("?duration", audio.duration);
						cmd.Parameters.AddWithValue("?fileName", audio.fileName);
						cmd.Parameters.AddWithValue("?fullPath", audio.fullPath);




						cmd.ExecuteNonQuery();
						cmd.Parameters.Clear();

					}
				}

				foreach (var id in model.staffIDs)
				{
					cmd.CommandText = "INSERT INTO operations_staff (operationID,staffID) VALUES (?operationID,?staffID)";
					cmd.Parameters.AddWithValue("?staffID", id);
					cmd.Parameters.AddWithValue("?operationID", operationID);
					cmd.ExecuteNonQuery();
					cmd.Parameters.Clear();
				}
			}


		}
	}
}