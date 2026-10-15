using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Data;

Console.WriteLine("DB 실습 시작");

string connectionString =
    "Server=localhost;" +
    "Database=ClientLab;" +
    "Integrated Security=True;" +
    "Encrypt=True;" +
    "TrustServerCertificate=True;";

// 연결은 프로그램이 끝날 때 자동으로 해제됩니다.
using var connection = new SqlConnection(connectionString);
await ConnectAsync(connection);

Console.WriteLine("\n트랜잭션 전 목록");
await SelectDevicesAsync(connection);

//int deletedCount = await DeleteDeviceAsync(connection,4 );
//Console.WriteLine($"\n삭제된 행 수: {deletedCount}");

//int insertedCount = await InsertDeviceAsync(connection, "습도 센서2", "HUM-002");
//int updatedCount = await UpdateDeviceAsync(connection,1,"온도 센서 수정");

//Console.WriteLine("\n\n");
//await SearchDevicesAsync(connection, "정");

//Console.WriteLine("\n트랜잭션 후 목록");
//await SelectDevicesAsync(connection);

Console.Write("\n\n장비 이름: ");
string name = Console.ReadLine() ?? "";

Console.Write("시리얼 번호: ");
string serialNumber = Console.ReadLine() ?? "";

int count = await InsertDeviceAsync(connection, name, serialNumber);
Console.WriteLine($"등록된 행 수: {count}");

Console.WriteLine("\n추가 후 목록");
await SelectDevicesAsync(connection);



// 1. DB 연결
static async Task ConnectAsync(SqlConnection connection)
{
    await connection.OpenAsync();
    Console.WriteLine("DB 연결 성공");
}

// 2. 장비 목록 조회
static async Task SelectDevicesAsync(SqlConnection connection)
{
    string sql =
        "SELECT DeviceId, Name, SerialNumber FROM dbo.Device;";

    using var command = new SqlCommand(sql, connection);
    using var reader = await command.ExecuteReaderAsync();

    while (await reader.ReadAsync())
    {
        int deviceId = reader.GetInt32(0);
        string name = reader.GetString(1);
        string serialNumber = reader.GetString(2);

        Console.WriteLine($"{deviceId}|{name}|{serialNumber}");
    }

    // using으로 선언했으므로 reader.Close()는 필요 없습니다.
}

// 3. 지정한 장비 삭제
static async Task<int> DeleteDeviceAsync(SqlConnection connection,int deviceId)
{
    string sql =
        "DELETE FROM dbo.Device WHERE DeviceId = @DeviceId;";

    using var command = new SqlCommand(sql, connection);
    command.Parameters.Add("@DeviceId", SqlDbType.Int).Value = deviceId;

    return await command.ExecuteNonQueryAsync();
}
// 4. 장비 등록
static async Task<int> InsertDeviceAsync(SqlConnection connection,string name,string serialNumber)
{
    string sql = """
        INSERT INTO dbo.Device(Name,SerialNumber)
        VALUES(@Name,@SerialNumber);
        """;
    using var command = new SqlCommand(sql, connection);
    command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = name;
    command.Parameters.Add("@SerialNumber", SqlDbType.NVarChar).Value = serialNumber;

    return await command.ExecuteNonQueryAsync();

}

static async Task<int> UpdateDeviceAsync(SqlConnection connection,int deviceId,string name)
{
    string sql = """
        UPDATE dbo.Device
        SET Name=@Name
        WHERE DeviceId=@DeviceId;
        """;

    using var command=new SqlCommand(sql, connection);
    command.Parameters.Add("@DeviceId",SqlDbType.Int).Value = deviceId;
    command.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;

    return await command.ExecuteNonQueryAsync();

}


static async Task SearchDevicesAsync(SqlConnection connection,string keyword)
{
    string sql = """
        SELECT DeviceId,Name,SerialNumber
        FROM dbo.Device
        WHERE Name LIKE @keyword;
        """;
    using var command= new SqlCommand(sql, connection);

    command.Parameters.Add("@Keyword", SqlDbType.NVarChar).Value = $"%{keyword}%";
    using var reader = await command.ExecuteReaderAsync();

    bool found = false;

    while(await reader.ReadAsync())
    {
        found = true;

        int deviceId = reader.GetInt32(0);
        string name = reader.GetString(1);
        string serialNumber = reader.GetString(2);

        Console.WriteLine($"{deviceId}|{name}|{serialNumber}");

    }

    if (!found)
    {
        Console.WriteLine("검색 결과가 없습니다.");
    }
}