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
			//SelectedHospitals selected = new SelectedHospitals();


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

		internal void InsertOperation(NewOperationFormModel model)
		{
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				//insert the operation to the database
				MySqlCommand cmd = conn.CreateCommand();
				cmd.CommandText = "INSERT INTO operation (patientID,hospitalID,roomNO,dateStamp,duration,operationTypeID) VALUES (?patientID,?hospitalID,?roomNo,?date,?maxDuration,?operationTypeID)";

				cmd.Parameters.AddWithValue("?patientID", model.patientID);
				cmd.Parameters.AddWithValue("?hospitalID", model.hospitalID);
				cmd.Parameters.AddWithValue("?roomNO", model.roomNo);
				cmd.Parameters.AddWithValue("?date", model.date.ToString("yyyy-MM-dd HH:mm:ss"));
				cmd.Parameters.AddWithValue("?maxDuration", model.maxDuration);
				cmd.Parameters.AddWithValue("?operationTypeID", model.operationTypeID);
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
						cmd.CommandText = "INSERT INTO video (operationID,size_bytes,timeStamp,type,duration,fileName) VALUES (?operationID,?size_bytes,?timeStamp,?type,?duration,?fileName)";
						cmd.Parameters.AddWithValue("?operationID", operationID);
						cmd.Parameters.AddWithValue("?size_bytes", video.size_bytes);
						cmd.Parameters.AddWithValue("?timeStamp", video.timeStamp);
						cmd.Parameters.AddWithValue("?type", video.type);
						cmd.Parameters.AddWithValue("?duration", video.duration);
						cmd.Parameters.AddWithValue("?fileName", video.fileName);
						cmd.ExecuteNonQuery();
						cmd.Parameters.Clear();

					}

				}
				if (model.audios != null)
				{
					foreach (var audio in model.audios)
					{
						Console.WriteLine(audio.fileName);
						cmd.CommandText = "INSERT INTO audio (operationID,size_bytes,timeStamp,type,duration,fileName) VALUES (?operationID,?size_bytes,?timeStamp,?type,?duration,?fileName)";
						cmd.Parameters.AddWithValue("?operationID", operationID);
						cmd.Parameters.AddWithValue("?size_bytes", audio.size_bytes);
						cmd.Parameters.AddWithValue("?timeStamp", audio.timeStamp);
						cmd.Parameters.AddWithValue("?type", audio.type);
						cmd.Parameters.AddWithValue("?duration", audio.duration);
						cmd.Parameters.AddWithValue("?fileName", audio.fileName);
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