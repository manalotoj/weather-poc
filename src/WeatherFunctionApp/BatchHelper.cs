using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace WeatherFunctionApp
{
    public class BatchHelper
    {
        public static async Task<List<List<string>>> GetCoordinates(int batchSize = 1000)
        {
            List<List<string>> groupsOfLines = new List<List<string>>();
            using (Stream stream = typeof(BatchHelper).Assembly.GetManifestResourceStream("WeatherFunctionApp.ListOfLatLongs.csv"))
            {
                int maxRecords = int.Parse(Config.MaxRecordCount);
                using (StreamReader reader = new StreamReader(stream))
                {
                    int outerCount = 0;
                    int counter = 0;
                    List<string> lines = new List<string>();
                    while (!reader.EndOfStream && outerCount < maxRecords)
                    {
                        string line = await reader.ReadLineAsync();
                        lines.Add(line);
                        counter += 1;
                        outerCount += 1;
                        if (counter >= batchSize)
                        {
                            counter = 0;
                            groupsOfLines.Add(lines);
                            lines = new List<string>();
                        }
                    }
                    if (counter < batchSize)
                    {
                        groupsOfLines.Add(lines);
                    }
                }
            }
            return groupsOfLines;
        }
    }
}
