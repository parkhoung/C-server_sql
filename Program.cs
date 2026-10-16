using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;

var listener = new TcpListener(IPAddress.Loopback, 5000);
listener.Start();


Console.WriteLine("서버 시작: 127.0.0.1:5000");
Console.WriteLine("장비 연결 대기 중...");

using var client=await listener.AcceptTcpClientAsync();
Console.WriteLine("장비 연결됨");

using var stream=client.GetStream();
using var reader=new StreamReader(stream, Encoding.UTF8);

using var writer = new StreamWriter(
    stream,
    new UTF8Encoding(false))
{
    AutoFlush = true
};

while (true)
{
    string? message=await reader.ReadLineAsync();
    string writemessage = $"{message} OK";
    
    if (message == null)
    {
        Console.WriteLine("장비 연결 종료");
        break; 
    }
    string[] parts = message.Split('|');

    if(parts.Length !=2)
    {
        Console.WriteLine($"잘못된 데이터 형식: {message}");
        await writer.WriteLineAsync("ERROR|잘못된 데이터 형식");
        continue;
    }
    string serialnumber=parts[0].Trim();
    if(string.IsNullOrWhiteSpace(serialnumber))
    {
        Console.WriteLine("시리얼 번호가 비어 있습니다.");
        await writer.WriteLineAsync("ERROR|시리얼 번호 누락");
        continue;
    }

    bool success = double.TryParse(
        parts[1],
        NumberStyles.Float,
        CultureInfo.InvariantCulture,
        out double temperature);

    if(!success||!double.IsFinite(temperature))
    {
        Console.WriteLine($"잘못된 온도 값:{parts[1]}");
        await writer.WriteLineAsync("ERROR|잘못된 온도 값");
        continue;
    }


    Console.WriteLine(
      $"장비: {serialnumber}, 온도: {temperature:F1}°C");
    await writer.WriteLineAsync("OK");

}

listener.Stop();