using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace email_analyzer
{
    class Graph
    {
        static int Precision { get; set; }
        static int StartDay { get; set; }
        static int EndDay { get; set; }
        static int Width { get; set; }
        public static void Initialize(int startDayOfYear, int endDayOfYear, int precisionInMinutes)
        {
            StartDay = startDayOfYear;
            EndDay = endDayOfYear;
            Precision = precisionInMinutes;
            Width = (EndDay - StartDay) * 1440 / Precision;
        }
        public int[] Raw { get; private set; }
        public const string SaveLocation = "C:\\tmp\\output\\";

        public Graph()
        {
            Raw = new int[Width];
        }
        public Bitmap GetImage(int gapDistance, Color graphColor)
        {
            int height = 0;
            for (int i = 0; i < this.Raw.Length; i++)
                if (height < this.Raw[i])
                    height = this.Raw[i];
            var bmp = new Bitmap(Width, height);

            //creating background
            DateTime date = new DateTime().AddDays(StartDay);
            for (int i = 0; i < bmp.Width; i++)
            {
                if (date.Minute != 0)
                    for (int j = 0; j < bmp.Height; j++)
                        bmp.SetPixel(i, j, Color.LightBlue);
                else if (date.Hour != 0)
                    for (int j = 0; j < bmp.Height; j++)
                        bmp.SetPixel(i, j, Color.Blue);
                else if (date.Day != 1)
                    for (int j = 0; j < bmp.Height; j++)
                        bmp.SetPixel(i, j, Color.DarkBlue);
                else
                    for (int j = 0; j < bmp.Height; j++)
                        bmp.SetPixel(i, j, Color.Black);
                date = date.AddMinutes(gapDistance);
            }
            for (int i = 0; i < Raw.Length; i++)
                for (int j = 0; j < Raw[i] && j < bmp.Height; j++)
                    bmp.SetPixel(i, j, graphColor);
            return bmp;
        }
        /// <summary>
        /// this function is for graphing one graph over another and using custom backgrounds
        /// </summary>
        /// <param name="background"></param>
        /// <returns></returns>
        public Bitmap GetImage(Bitmap background, Color graphColor)
        {
            Bitmap bmp = new Bitmap(background);
            for (int i = 0; i < Raw.Length; i++)
                for (int j = 0; j < Raw[i] && j < bmp.Height; j++)
                    bmp.SetPixel(i, j, graphColor);
            return bmp;
        }
        public void Save() => Save("Untitled.bmp");
        public void Save(string title) => Save(title, 60);
        public void Save(string title, int gapDistance) => Save(title, gapDistance, Color.Red);
        public void Save(string title, int gapDistance, Color graphColor) => Save(title, GetImage(gapDistance, graphColor));
        public static void Save(string title, Bitmap bmp)
        {
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            bmp.Save(SaveLocation + title + ".bmp");
        }
        public static explicit operator Graph(string n)
        {
            ParsedPackage[] data;
            Graph graph = new Graph();
            for (int i = StartDay; i <= EndDay; i++)
                if (File.Exists(SaveLocation + "parsedData-" + i + ".json"))
                {
                    data = Newtonsoft.Json.JsonConvert.DeserializeObject<ParsedPackage[]>(File.ReadAllText(SaveLocation + "parsedData-" + i + ".json"));
                    for (int j = 0; j < data.Length; j++)
                        if (data[j].From.Contains(n))
                            graph.Raw[(((data[j].TimeDelivered.DayOfYear - StartDay) * 24 + data[j].TimeDelivered.Hour) * 60 + data[j].TimeDelivered.Minute) / Precision]++;
                }
            return graph;
        }
        public static Graph operator +(Graph a, string b)
        {
            Graph result = (Graph)b;
            for (int i = 0; i < Width; i++)
                result.Raw[i] += a.Raw[i];
            return result;
        }
        public static Graph operator -(Graph a, string b)
        {
            Graph result = (Graph)b;
            for (int i = 0; i < Width; i++)
                result.Raw[i] = a.Raw[i] - result.Raw[i];
            return result;
        }
    }
}
