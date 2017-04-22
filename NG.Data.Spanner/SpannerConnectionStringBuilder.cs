using System;
using System.Data.Common;

namespace NG.Data.Spanner
{
    public class SpannerConnectionStringBuilder: DbConnectionStringBuilder
    {
        public SpannerConnectionStringBuilder(string connectionString)
        {
            BuildData(connectionString);
        }

        public void BuildData(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException();
            }
            var connectionParts = connectionString.Split(';');
            if (connectionParts.Length != 3)
            {
                throw new ArgumentException("Missing db connection parameters: ProjectId, InstanceId, DBName");
            }

            ProjectId = connectionParts[0];
            InstanceId = connectionParts[1];
            DBName = connectionParts[2];
        }

        public string ProjectId { get; set; }
        public string InstanceId { get; set; }
        public string DBName { get; set; }


    }
}
