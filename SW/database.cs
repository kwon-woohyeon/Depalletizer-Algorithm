using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project
{
    public class database
    {
        private MySqlConnection connection;
        private string connectionString;


        // 생성자: 데이터베이스 연결 문자열 초기화
        public database(string connectionString)
        {
            this.connectionString = connectionString;
            connection = new MySqlConnection(connectionString);
        }

            // 특정 날짜 데이터를 가져오는 메서드
            public DataTable LoadDataFromDatabase(string formattedDate)
        {
            string query = "SELECT id, name, address, `Date`, total FROM project.test WHERE DATE_FORMAT(`Date`, '%Y%m%d') = @formattedDate";

            DataTable dataTable = new DataTable();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // SQL 파라미터 설정
                        cmd.Parameters.AddWithValue("@formattedDate", formattedDate);

                        // 데이터 가져오기
                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        adapter.Fill(dataTable);
                    }
                }
            }
            catch (Exception ex)
            {
                // 예외 처리
                Console.WriteLine("데이터를 로드하는 중 오류 발생: " + ex.Message);
            }

            return dataTable;
        }
        public void Updateendtimeforid(string currentDate, string end)
        {

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // 해당 id에 대해 state를 업데이트하는 쿼리
                    string updateQuery = "UPDATE log SET EndTime = @EndTime WHERE StartTime = @StartTime;";
                    MySqlCommand command = new MySqlCommand(updateQuery, connection);

                    // 파라미터 추가
                    command.Parameters.AddWithValue("EndTime", end);
                    command.Parameters.AddWithValue("@StartTime", currentDate);

                    // 쿼리 실행
                    int rowsAffected = command.ExecuteNonQuery();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"오류 발생: {ex.Message}");
            }
        }
        public void Updatelog(string userid, string currentDate) // 로그 업데이트
        {
            string insertQuery = "INSERT INTO  project.log (id, StartTime) VALUES (@id, @StartTime)";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn))
                {
                    // 쿼리 파라미터 설정


                    insertCmd.Parameters.AddWithValue("@id", userid); // 로그 업데이트

                    insertCmd.Parameters.AddWithValue("@StartTime", currentDate);

                    // SQL 실행
                    int insertRowsAffected = insertCmd.ExecuteNonQuery();

                }
            }
        }

        public DataTable LoadlogData() //db에서 유저 데이터 로드
        {
            string connectionString = "Server=localhost;Database=project;User Id=root;Password=1234;";
            string query = "SELECT idlog, id,  StartTime,  endTime FROM project.log";

            DataTable dataTable = new DataTable(); // DataTable 선언

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            adapter.Fill(dataTable); // 데이터 로드
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("데이터 로드 중 오류: " + ex.Message);
            }

            return dataTable; // DataTable 반환
        }
        public MySqlConnection Connection
        {
            get
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                {
                    connection.Open();
                }
                return connection;
            }
        }
        //날짜와 회사 이름
        public DataTable LoadDcompanyanddate(string formattedDate, string companyName)
        {
            string query = "SELECT id, name, address, `Date`, total " +
                           "FROM project.test " +
                           "WHERE DATE_FORMAT(`Date`, '%Y%m%d') = @formattedDate AND name = @companyName";

            DataTable dataTable = new DataTable();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // 날짜 및 회사명 파라미터 추가
                        cmd.Parameters.AddWithValue("@formattedDate", formattedDate);
                        cmd.Parameters.AddWithValue("@companyName", companyName);

                        // 데이터 가져오기
                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        adapter.Fill(dataTable);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("데이터를 로드하는 중 오류 발생: " + ex.Message);
            }

            return dataTable;
        }

       
        //날짜 수량 회사 이름
        public DataTable Loadcdt()
        {
            string query = "SELECT name, `Date`, total, address " +
                           "FROM project.test ";
                          

            DataTable dataTable = new DataTable();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // 데이터 가져오기
                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        adapter.Fill(dataTable);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("데이터를 로드하는 중 오류 발생: " + ex.Message);
            }

            return dataTable;
        }
        // database 클래스에 route 가져오는 메서드 추가

        public int GetFirstIdForToday()
        {
            int firstId = -1;  // 기본값 -1로 설정, 만약 데이터가 없다면 -1이 반환됩니다.

            // 오늘 날짜를 구하는 SQL 쿼리
            string query = @"
    SELECT id
    FROM project.test
    WHERE Date = CAST(CURDATE() AS UNSIGNED) AND OK != total
    ORDER BY id ASC
    LIMIT 1";  // 첫 번째 레코드만 선택

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    // 쿼리 실행
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(query, conn);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    // 데이터가 있으면 'id' 값을 가져옴
                    if (dataTable.Rows.Count > 0)
                    {
                        firstId = Convert.ToInt32(dataTable.Rows[0]["id"]);
                    }
                    else
                    {
                        MessageBox.Show("오늘 날짜의 데이터가 없습니다.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            return firstId;
        }

        public void UpdateStateForFirstIdAndGetTotal(int firstId, string newOK)
        {

            if (firstId == -1)
            {
                // 오늘 날짜의 데이터가 없으면 종료
                MessageBox.Show("오늘 날짜에 해당하는 레코드가 없습니다.");
                return;
            }

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // 해당 id에 대해 state를 업데이트하는 쿼리
                    string updateQuery = "UPDATE test SET OK = @OK WHERE id = @id;";
                    MySqlCommand command = new MySqlCommand(updateQuery, connection);

                    // 파라미터 추가
                    command.Parameters.AddWithValue("@OK", newOK);
                    command.Parameters.AddWithValue("@id", firstId);

                    // 쿼리 실행
                    int rowsAffected = command.ExecuteNonQuery();

                    /*if (rowsAffected > 0)
                    {
                        MessageBox.Show($"id {firstId}에 대해 상태가 {newOK}로 업데이트되었습니다.");
                    }
                    else
                    {
                        MessageBox.Show($"id {firstId}에 대해 상태를 업데이트할 수 없습니다.");
                    }*/

                    // 해당 id에 대한 total 값 가져오기
                    string totalValue = GetTotalForId(firstId);
                    /* MessageBox.Show($"id {firstId}에 해당하는 total 값: {totalValue}");*/
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"오류 발생: {ex.Message}");
            }
        }
        public void UpdateNG(int firstId, string NG)
        {

            if (firstId == -1)
            {
                // 오늘 날짜의 데이터가 없으면 종료
                MessageBox.Show("오늘 날짜에 해당하는 레코드가 없습니다.");
                return;
            }

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // 해당 id에 대해 state를 업데이트하는 쿼리
                    string updateQuery = "UPDATE test SET NG = @NG WHERE id = @id;";
                    MySqlCommand command = new MySqlCommand(updateQuery, connection);

                    // 파라미터 추가
                    command.Parameters.AddWithValue("@NG", NG);
                    command.Parameters.AddWithValue("@id", firstId);

                    // 쿼리 실행
                    int rowsAffected = command.ExecuteNonQuery();

                    /*if (rowsAffected > 0)
                    {
                        MessageBox.Show($"id {firstId}에 대해 상태가 {NG}로 업데이트되었습니다.");
                    }
                    else
                    {
                        MessageBox.Show($"id {firstId}에 대해 상태를 업데이트할 수 없습니다.");
                    }*/

                    // 해당 id에 대한 total 값 가져오기
                    string totalValue = GetTotalForId(firstId);
                    /*  MessageBox.Show($"id {firstId}에 해당하는 total 값: {totalValue}");*/
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"오류 발생: {ex.Message}");
            }
        }
        public string GetTotalForId(int id)
        {
            string total = null;

            // id에 해당하는 total 값을 구하는 SQL 쿼리
            string query = @"
    SELECT total
    FROM project.test
    WHERE id = @id";  // 해당 id에 해당하는 total 값 조회

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    // 쿼리 실행
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(query, conn);
                    dataAdapter.SelectCommand.Parameters.AddWithValue("@id", id); // id 값 파라미터 전달
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    // 데이터가 있으면 'total' 값을 가져옴
                    if (dataTable.Rows.Count > 0)
                    {
                        total = dataTable.Rows[0]["total"].ToString(); // 첫 번째 row에서 total 값 추출
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return total;
        }
        public string GetnameForId(int id)
        {
            string name = null;

            // id에 해당하는 total 값을 구하는 SQL 쿼리
            string query = @"
    SELECT name
    FROM project.test
    WHERE id = @id";  // 해당 id에 해당하는 total 값 조회

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    // 쿼리 실행
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(query, conn);
                    dataAdapter.SelectCommand.Parameters.AddWithValue("@id", id); // id 값 파라미터 전달
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    // 데이터가 있으면 'total' 값을 가져옴
                    if (dataTable.Rows.Count > 0)
                    {
                        name = dataTable.Rows[0]["name"].ToString(); // 첫 번째 row에서 total 값 추출
                    }
                    else
                    {
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return name;
        }
        public int GetNGForId(int id)
        {
            int ng = 0;

            // id에 해당하는 total 값을 구하는 SQL 쿼리
            string query = @"
    SELECT NG
    FROM project.test
    WHERE id = @id";  // 해당 id에 해당하는 total 값 조회

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    // 쿼리 실행
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(query, conn);
                    dataAdapter.SelectCommand.Parameters.AddWithValue("@id", id); // id 값 파라미터 전달
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    // 데이터가 있으면 'total' 값을 가져옴
                    if (dataTable.Rows.Count > 0)
                    {
                        ng = Convert.ToInt32(dataTable.Rows[0]["NG"]); // 첫 번째 row에서 total 값 추출
                    }
                    else
                    {
                        MessageBox.Show($"id {id}에 해당하는 total 값을 찾을 수 없습니다.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"오류 발생: {ex.Message}");
            }

            return ng;
        }
        public int GetstateForId(int id)
        {
            int total = 0;

            // id에 해당하는 total 값을 구하는 SQL 쿼리
            string query = @"
    SELECT OK
    FROM project.test
    WHERE id = @id";  // 해당 id에 해당하는 total 값 조회

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    // 쿼리 실행
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(query, conn);
                    dataAdapter.SelectCommand.Parameters.AddWithValue("@id", id); // id 값 파라미터 전달
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    // 데이터가 있으면 'total' 값을 가져옴
                    if (dataTable.Rows.Count > 0)
                    {
                        total = Convert.ToInt32(dataTable.Rows[0]["OK"]); // 첫 번째 row에서 total 값 추출
                    }

                }
            }
            catch (Exception ex)
            {
            }

            return total;
        }
        public int GetdateForId(int id)
        {
            int date = 0;

            // id에 해당하는 total 값을 구하는 SQL 쿼리
            string query = @"
    SELECT Date
    FROM project.test
    WHERE id = @id";  // 해당 id에 해당하는 total 값 조회

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    // 쿼리 실행
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(query, conn);
                    dataAdapter.SelectCommand.Parameters.AddWithValue("@id", id); // id 값 파라미터 전달
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    // 데이터가 있으면 'total' 값을 가져옴
                    if (dataTable.Rows.Count > 0)
                    {
                        date = Convert.ToInt32(dataTable.Rows[0]["Date"]); // 첫 번째 row에서 total 값 추출
                    }
                    else
                    {
                        MessageBox.Show($"id {id}에 해당하는 total 값을 찾을 수 없습니다.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"오류 발생: {ex.Message}");
            }

            return date;
        }
        public string Getcompanyforid(int id)
        {
            string company = null;

            // id에 해당하는 total 값을 구하는 SQL 쿼리
            string query = @"
    SELECT name
    FROM project.test
    WHERE id = @id";  // 해당 id에 해당하는 total 값 조회

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    // 쿼리 실행
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(query, conn);
                    dataAdapter.SelectCommand.Parameters.AddWithValue("@id", id); // id 값 파라미터 전달
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    // 데이터가 있으면 'total' 값을 가져옴
                    if (dataTable.Rows.Count > 0)
                    {
                        company = dataTable.Rows[0]["name"].ToString(); // 첫 번째 row에서 total 값 추출
                    }
                    else
                    {
                        MessageBox.Show($"id {id}에 해당하는 total 값을 찾을 수 없습니다.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"오류 발생: {ex.Message}");
            }

            return company;
        }
        // 데이터베이스 클래스에 id를 증가시키는 메서드 추가
        public int GetNextIdForToday(int currentId)
        {
            int nextId = -1;

            // SQL 쿼리 수정: 현재 id보다 큰 첫 번째 id를 찾음
            string query = @"
    SELECT id
    FROM project.test
    WHERE Date =  CAST(CURDATE() AS UNSIGNED) AND id > @currentId AND total !=OK
    ORDER BY id ASC
    LIMIT 1";  // 첫 번째로 큰 id만 선택

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    // 쿼리 실행
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(query, conn);
                    dataAdapter.SelectCommand.Parameters.AddWithValue("@currentId", currentId);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    // 데이터가 있으면 'id' 값을 가져옴
                    if (dataTable.Rows.Count > 0)
                    {
                        nextId = Convert.ToInt32(dataTable.Rows[0]["id"]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            return nextId;
        }
        public int GetidToday(int currentId)
        {
            int nextId = -1;

            // SQL 쿼리 수정: 현재 id보다 큰 첫 번째 id를 찾음
            string query = @"
    SELECT id
    FROM project.test
    WHERE Date =  CAST(CURDATE() AS UNSIGNED) 
    ORDER BY id ASC
    LIMIT 1";  // 첫 번째로 큰 id만 선택

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    // 쿼리 실행
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(query, conn);
                    dataAdapter.SelectCommand.Parameters.AddWithValue("@currentId", currentId);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    // 데이터가 있으면 'id' 값을 가져옴
                    if (dataTable.Rows.Count > 0)
                    {
                        nextId = Convert.ToInt32(dataTable.Rows[0]["id"]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            return nextId;
        }
        public int Get_Total_Today_NOW()
        {
            int Id = -1;
            int total = 0;
            int OK = 0;

            // SQL 쿼리 수정: 현재 id보다 큰 첫 번째 id를 찾음
            string query = @"
    SELECT id, total,OK
    FROM project.test
    WHERE Date =  CAST(DATE_FORMAT(CURDATE(), '%Y%m%d') AS UNSIGNED) AND total != OK
    ORDER BY id ASC
    LIMIT 1";  // 첫 번째로 큰 id만 선택

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    // 쿼리 실행
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(query, conn);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    // 데이터가 있으면 'id' 값을 가져옴
                    if (dataTable.Rows.Count > 0)
                    {
                        OK = Convert.ToInt32(dataTable.Rows[0]["OK"]);
                        total =Convert.ToInt32(dataTable.Rows[0]["total"]);
                       
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            
            return total-OK;

            
        }
        public (bool IsValid, string MemberType) ValidateUser(string userId, string password)
        {
            string query = "SELECT password, member FROM project.login WHERE id = @userId"; // 멤버 타입 가져오기
            string dbPassword = null;
            string dbMemberType = null;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId); // 사용자 ID 매개변수 추가

                        using (MySqlDataReader reader = cmd.ExecuteReader()) // 여러 칼럼을 읽기 위해 DataReader 사용
                        {
                            if (reader.Read()) // 결과가 있을 경우
                            {
                                dbPassword = reader["password"]?.ToString(); // 비밀번호 가져오기
                                dbMemberType = reader["member"]?.ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"오류 발생: {ex.Message}");
                return (false, null); // 오류 발생 시 실패로 간주
            }

            // 입력된 비밀번호와 데이터베이스 비밀번호 비교
            bool isValid = dbPassword == password;
            return (isValid, dbMemberType); // 인증 결과와 멤버 타입 반환
        }
        public DataTable LoadUserData() //db에서 유저 데이터 로드
        {
            string connectionString = "Server=localhost;Database=project;User Id=root;Password=1234;";
            string query = "SELECT i, id,  password,  member FROM project.login";

            DataTable dataTable = new DataTable(); // DataTable 선언

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            adapter.Fill(dataTable); // 데이터 로드
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("데이터 로드 중 오류: " + ex.Message);
            }

            return dataTable; // DataTable 반환
        }

        public DataTable LoadCompanyData()
        {
            string connectionString = "Server=localhost;Database=project;User Id=root;Password=1234;";
            string query = "SELECT id, name, address, Date, total FROM project.test";

            DataTable dataTable = new DataTable(); // DataTable 선언

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            adapter.Fill(dataTable); // 데이터 로드
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("데이터 로드 중 오류: " + ex.Message);
            }

            return dataTable; // DataTable 반환
        }

        public int GetOKForId()
        {
            int ok = 0;

            // id에 해당하는 total 값을 구하는 SQL 쿼리
            string query = @"
    SELECT SUM(OK) AS TotalOK
    FROM project.test
    WHERE Date = CAST(CURDATE() AS UNSIGNED)";  // 해당 id에 해당하는 total 값 조회

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    // 쿼리 실행
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(query, conn);

                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    // 데이터가 있으면 'total' 값을 가져옴
                    if (dataTable.Rows.Count > 0 && dataTable.Rows[0]["TotalOK"] != DBNull.Value)
                    {
                        ok = Convert.ToInt32(dataTable.Rows[0]["TotalOK"]); // 합산된 OK 값 추출
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"오류 발생: {ex.Message}");
            }

            return ok;
        }

        public int GetNGsum()
        {
            int ng = 0;

            // id에 해당하는 total 값을 구하는 SQL 쿼리
            string query = @"
    SELECT SUM(NG) AS TotalNG
    FROM project.test
    WHERE Date = CAST(CURDATE() AS UNSIGNED)";  // 해당 id에 해당하는 total 값 조회

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    // 쿼리 실행
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(query, conn);

                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    // 데이터가 있으면 'total' 값을 가져옴
                    if (dataTable.Rows.Count > 0 && dataTable.Rows[0]["TotalNG"] != DBNull.Value)
                    {
                        ng = Convert.ToInt32(dataTable.Rows[0]["TotalNG"]); // 합산된 OK 값 추출
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"오류 발생: {ex.Message}");
            }

            return ng;
        }

        public int GetTotalsum()
        {
            int ttotal = 0;

            // id에 해당하는 total 값을 구하는 SQL 쿼리
            string query = @"
            SELECT SUM(total) AS TotalNG
            FROM project.test
            WHERE Date = DATE_FORMAT(CURDATE(), '%Y%m%d')";  // 오늘 날짜로 필터링

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    // 쿼리 실행
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(query, conn);

                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    // 데이터가 있으면 'total' 값을 가져옴
                    if (dataTable.Rows.Count > 0 && dataTable.Rows[0]["TotalNG"] != DBNull.Value)
                    {
                        ttotal = Convert.ToInt32(dataTable.Rows[0]["TotalNG"]); // 합산된 OK 값 추출
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"오류 발생: {ex.Message}");
            }

            return ttotal;
        }

        // 회사, total, 정상 객체
        public (string name, int total, int ok, int ng) GetCompanyAllData(int id)
        {
            string name = null;
            int total = 0;
            int ok = 0;
            int ng = 0;

            string query = @"
            SELECT name, total, ok, ng
            FROM project.test
            WHERE Date = DATE_FORMAT(CURDATE(), '%Y%m%d') AND id = @id";  // 오늘 날짜로 필터링

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(query, conn);
                    dataAdapter.SelectCommand.Parameters.AddWithValue("@id", id); // id 값을 파라미터로 전달
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    // 데이터가 있으면 name, total, ok 값을 가져옴
                    if (dataTable.Rows.Count > 0)
                    {
                        name = dataTable.Rows[0]["name"].ToString();
                        total = Convert.ToInt32(dataTable.Rows[0]["total"]);
                        ok = Convert.ToInt32(dataTable.Rows[0]["ok"]);
                        ng = Convert.ToInt32(dataTable.Rows[0]["ng"]);
                    }
                    else
                    {
                        Console.WriteLine($"id {id}에 해당하는 데이터를 찾을 수 없습니다.");
                    }
                }
            }
            catch (Exception ex)
            {
                // 예외 처리
                Console.WriteLine($"오류 발생: {ex.Message}");
            }

            return (name, total, ok, ng); // 튜플로 반환
        }
    }
}
