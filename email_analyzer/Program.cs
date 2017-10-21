using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;
using MimeKit;
using System.Drawing;
using System.Data;
using Microsoft.TeamFoundation.WorkItemTracking.Controls;
using Newtonsoft.Json;

namespace email_analyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            Graph.Initialize(269, 286, 15);
            Graph graph = new Graph();
            Bitmap bmp = graph.GetImage(60,Color.Red);
            graph -= "github";
            graph -= "blogtrottr";
            bmp = graph.GetImage(bmp, Color.Purple);
            Graph.Save("test", bmp);
        }
        public static void ProcessData()
        {
            FileStream stream = new FileStream("C:\\tmp\\jacob@eemailme.com_2017-10-12.mbox", FileMode.Open);
            var parser = new MimeParser(stream, MimeFormat.Mbox);
            MimeMessage message;
            List<ParsedPackage> data = new List<ParsedPackage>();
            while (!parser.IsEndOfStream)
            {
                message = parser.ParseMessage();
                string text;
                if (message.TextBody == null)
                    text = HtmlFilter.Strip(message.HtmlBody);
                else text = message.TextBody;
                data.Add(new ParsedPackage(message.From.ToString(), message.Subject, message.TextBody, message.Date));
                if (data.Count % 10000 == 0)
                    Console.WriteLine(data.Count / 1000 + "k complete");
            }
            List<ParsedPackage> subset;
            int searching;
            while (data.Count > 0)
            {
                searching = data[0].TimeDelivered.DayOfYear;
                subset = new List<ParsedPackage>();
                for (int i = 0; i < data.Count; i++)
                    while (i < data.Count && data[i].TimeDelivered.DayOfYear == searching)
                    {
                        subset.Add(data[i]);
                        data.RemoveAt(i);
                    }
                File.WriteAllText(Graph.SaveLocation+"parsedData-" + searching + ".json", JsonConvert.SerializeObject(subset));
            }
        }
        public static void LoadProcessedData(int day) =>
            Newtonsoft.Json.JsonConvert.DeserializeObject<List<ParsedPackage>>(Graph.SaveLocation + "parsedData-" + day + ".json");
    }
}
