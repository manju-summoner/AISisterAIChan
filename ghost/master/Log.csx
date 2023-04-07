using System.IO;

static class Log
{
    public static string Prompt = "prompt.txt";
    public static string Response = "response.txt";
    static string path = "./log/";
    
    static Log()
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }
    public static void WriteAllText(string fileName, string value)
    {
        File.WriteAllText(path + fileName, value);
    }
}