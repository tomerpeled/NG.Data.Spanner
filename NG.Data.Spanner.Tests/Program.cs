using System;

namespace NG.Data.Spanner.Tests
{
    public class Program
    {
        public const string SpannerConnectionString = "projectid;instance;db";

        public static void Main(string[] args)
        {
            Console.WriteLine("Start Spanner Test...");

            //Set your service account json file location - when running outside of the Google cloud environment
           // Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "json file");

            var dbInitializer = new DBInitializer(SpannerConnectionString);
            dbInitializer.Seed().Wait();

            Console.WriteLine("End Spanner Test.");
            Console.ReadLine();
        }

    }
}
