using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

//Try update file
try
{

    Ping ping = new Ping();
    var reply = ping.Send("www.google.com", 3000);
    if (reply.Status != IPStatus.Success)
        throw new Exception("Connection Error");

    int crrversion = 0, version = 0, maxversion = 0;
    //Check version
    if (File.Exists("gof.info"))
    {
        StreamReader reader = new StreamReader("gof.info");
        crrversion = int.Parse(reader.ReadLine() ?? "0");
        reader.Close();
    }

    WebClient client = new WebClient();

    //Discover Last Version
    var gitpage = client.DownloadString("https://github.com/trevisharp/GameOfLife");
    Regex regex = new Regex("(<a href=\"https://github.com/trevisharp/GameOfLife/releases/tag/)(.*)(</a>)");

    //Search last release
    foreach (Match match in regex.Matches(gitpage))
    {
        version = int.Parse(match.Value.Substring(match.Value.Length - 7, 3).Replace(".", ""));
        if (version > maxversion)
            maxversion = version;
    }

    //update files
    if (maxversion != crrversion)
    {
        string release = "https://github.com/trevisharp/GameOfLife/releases/download/";
        release += ((int)maxversion / 10).ToString() + "%2C" + (maxversion % 10).ToString() + "/";
        string file;

        //get new gof.info
        File.Delete("gof.info");
        client.DownloadFile(release + "gof.info", "gof.info");

        //get other files
        StreamReader reader = new StreamReader("gof.info");
        reader.ReadLine();
        while (!reader.EndOfStream)
        {
            file = reader.ReadLine();
            if (File.Exists(file))
                File.Delete(file);
            client.DownloadFile(release + file, file);
        }
        reader.Close();
    }
}
catch (Exception ex)
{
    StreamWriter filerror = new StreamWriter("update.log");
    filerror.WriteLine(ex.Message + "\n" + ex.StackTrace);
    filerror.Close();
}

try
{
    if (File.Exists("gof.info"))
        Process.Start(new ProcessStartInfo("GameOfLife.exe"));
}
catch (Exception ex)
{
    StreamWriter filerror = new StreamWriter("open.log");
    filerror.WriteLine(ex.Message + "\n" + ex.StackTrace);
    filerror.Close();
}