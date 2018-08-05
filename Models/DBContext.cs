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
				Console.WriteLine("connection opened");
				MySqlCommand cmd = new MySqlCommand("SELECT * FROM patient", conn);
				Console.WriteLine("SQL query submited");
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
				MySqlCommand cmd = new MySqlCommand("INSERT INTO operation (patientID,hospitalID,roomNO,dateStamp,duration,operationTypeID) " +
				"VALUES (" + model.patientID + "," + model.hospitalID + ",'" + model.roomNo + "'," + model + ");", conn);

			}


		}
	}
}