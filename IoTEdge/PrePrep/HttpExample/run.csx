using System.Net;

public static string Run(HttpRequestMessage req, TraceWriter log)
{
  log.Info("C# HTTP trigger function processed a request.");

  return "Hello World!";
}