using ADO.Models;
using System.Data;
//using System.Data.SqlClient;
using Microsoft.Data.SqlClient;

namespace ADO.Data
{
    public class AppDBContext
    {
        private string _databaseName;
        public AppDBContext(IConfiguration config)
        {
            _databaseName = config.GetConnectionString("DefaultConnection");
        }

        public List<Employee> GetEmployees()
        {
            SqlConnection sc = new SqlConnection(_databaseName);
            SqlCommand cmd = new SqlCommand("spGetEmployees", sc);
            cmd.CommandType = CommandType.StoredProcedure;
            sc.Open();
            using (SqlDataReader dr = cmd.ExecuteReader())
            {
                List<Employee> empList = new List<Employee>();
                while (dr.Read())
                {
                    Employee emp = new Employee();
                    emp.Name = dr.GetString(1);
                    emp.DOJ = DateOnly.FromDateTime(dr.GetDateTime(2));
                    emp.EmpId = dr.GetInt32(0);
                    emp.DeptID = dr.GetInt32(3);
                    emp.DesgID = (int)dr[4];
                    if (!dr.IsDBNull(5))
                        emp.ProfileImg = (byte[])dr[5];
                    else
                        emp.ProfileImg = null;


                    empList.Add(emp);
                }
                sc.Close();
                return empList;
            }

        }

        public bool CreateEmployee(Employee emp)
        {
            SqlConnection sc = new SqlConnection(_databaseName);
            SqlCommand cmd = new SqlCommand("spCreateEmp", sc);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@name", emp.Name));
            cmd.Parameters.Add(new SqlParameter("@doj", emp.DOJ));
            cmd.Parameters.Add(new SqlParameter("@deptid", emp.DeptID));
            cmd.Parameters.Add(new SqlParameter("@desgid", emp.DesgID));

            cmd.Parameters.Add("@img", SqlDbType.VarBinary).Value =
            emp.ProfileImg;
            sc.Open();
            int i = cmd.ExecuteNonQuery();

            if (i > 0)
                return true;
            else
                return false;
        }

        public Employee GetEmployee(int id)
        {
            SqlConnection sc = new SqlConnection(_databaseName);
            SqlCommand cmd = new SqlCommand("spEmpDetails", sc);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@id", id));
            sc.Open();
            Employee emp = new Employee();

            using (SqlDataReader rd = cmd.ExecuteReader())
            {

                while (rd.Read())
                {
                    emp.EmpId = rd.GetInt32(0);
                    emp.Name = rd.GetString(1);
                    emp.DOJ = DateOnly.FromDateTime(rd.GetDateTime(2));
                    emp.DeptID = rd.GetInt32(3);
                    emp.DesgID = rd.GetInt32(4);
                    if (!rd.IsDBNull(5))
                    {
                        emp.ProfileImg = (byte[])rd[5];
                    }
                    else
                    {
                        emp.ProfileImg = null;
                    }
                }
            }
            return emp;
        }

        public Department GetDepartmentbyId(int id)
        {
            SqlConnection sc = new SqlConnection(_databaseName);
            SqlCommand cmd = new SqlCommand("Select * from dept where DeptId = @id", sc);
            cmd.Parameters.Add(new SqlParameter("@id", id));
            sc.Open();
            Department department = new Department();
            using (SqlDataReader dr = cmd.ExecuteReader())
            {
                if (dr.Read())
                {
                    department.DeptID = dr.GetInt32(0);
                    department.DeptName = dr.GetString(1);
                }
            }
            return department;
        }

        public Desgination GetDesignationbyId(int id)
        {
            SqlConnection sc = new SqlConnection(_databaseName);
            SqlCommand cmd = new SqlCommand("Select * from desg where DesgId = @id", sc);
            cmd.Parameters.Add(new SqlParameter("@id", id));
            sc.Open();
            Desgination department = new Desgination();
            using (SqlDataReader dr = cmd.ExecuteReader())
            {
                if (dr.Read())
                {
                    department.DesgID = dr.GetInt32(0);
                    department.DesgName = dr.GetString(1);
                }
            }
            return department;
        }

        public List<Department> GetDepartments()
        {
            List<Department> list = new List<Department>();
            SqlConnection sc = new SqlConnection(_databaseName);
            SqlCommand cmd = new SqlCommand("select * from dept", sc);
            sc.Open();

            using (SqlDataReader dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    list.Add(new Department
                    {
                        DeptID = dr.GetInt32(0),
                        DeptName = dr.GetString(1)
                    });
                }
            }
            return list;
        }

        public List<Desgination> GetDesignationsByDept(int deptId)
        {
            List<Desgination> list = new List<Desgination>();
            SqlConnection sc = new SqlConnection(_databaseName);
            SqlCommand cmd = new SqlCommand("select * from desg where DeptID=@id", sc);
            cmd.Parameters.AddWithValue("@id", deptId);
            sc.Open();

            using (SqlDataReader dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    list.Add(new Desgination
                    {
                        DesgID = dr.GetInt32(0),
                        DesgName = dr.GetString(1),
                        DeptID = dr.GetInt32(2)
                    });
                }
            }
            return list;
        }

        public bool UpdateEmployee(Employee emp)
        {
            SqlConnection sc = new SqlConnection(_databaseName);
            SqlCommand cmd = new SqlCommand(
                @"update emp 
          set empname=@name,
              DOJ=@doj,
              DeptID=@deptid,
              DesgID=@desgid,
              ProfileImg=@img
          where EmpId=@id", sc);

            cmd.Parameters.AddWithValue("@name", emp.Name);
            cmd.Parameters.AddWithValue("@doj", emp.DOJ);
            cmd.Parameters.AddWithValue("@deptid", emp.DeptID);
            cmd.Parameters.AddWithValue("@desgid", emp.DesgID);
            cmd.Parameters.AddWithValue("@img", (object?)emp.ProfileImg ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@id", emp.EmpId);

            sc.Open();
            int i = cmd.ExecuteNonQuery();
            return i > 0;
        }

        public bool DeleteEmployee(int id)
        {
            SqlConnection sc = new SqlConnection(_databaseName);
            SqlCommand cmd = new SqlCommand("delete from emp where EmpId=@id", sc);
            cmd.Parameters.AddWithValue("@id", id);
            sc.Open();

            int i = cmd.ExecuteNonQuery();
            return i > 0;
        }

        public List<Department> GetAllDepartments()
        {
            List<Department> deptList = new List<Department>();

            using (SqlConnection sc = new SqlConnection(_databaseName))
            {
                SqlCommand cmd = new SqlCommand("SELECT DeptID, DeptName FROM dept", sc);
                sc.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        Department dept = new Department
                        {
                            DeptID = dr.GetInt32(0),
                            DeptName = dr.GetString(1)
                        };

                        deptList.Add(dept);
                    }
                }
            }

            return deptList;
        }

        public List<Desgination> GetAllDesignations()
        {
            List<Desgination> desgList = new List<Desgination>();

            using (SqlConnection sc = new SqlConnection(_databaseName))
            {
                SqlCommand cmd = new SqlCommand("SELECT DesgID, DesgName, DeptID FROM desg", sc);
                sc.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        Desgination desg = new Desgination
                        {
                            DesgID = dr.GetInt32(0),
                            DesgName = dr.GetString(1),
                            DeptID = dr.GetInt32(2)
                        };

                        desgList.Add(desg);
                    }
                }
            }

            return desgList;
        }


    }
}
