using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Cloud.Spanner.Admin.Database.V1;
using Google.Cloud.Spanner.Admin.Instance.V1;
using Google.LongRunning;
using InstanceName = Google.Cloud.Spanner.Admin.Instance.V1.InstanceName;

namespace NG.Data.Spanner
{
    public class SpannerInstanceModifier
    {

        public static async Task CreateInstance(string projectId, string instanceName, string config, int nodesCount)
        {
            // Create client
            InstanceAdminClient instanceAdminClient = await InstanceAdminClient.CreateAsync();
            // Initialize request argument(s)
            CreateInstanceRequest request = new CreateInstanceRequest
            {
                ParentAsProjectName = new ProjectName(projectId),
                InstanceIdAsInstanceName = new InstanceName(projectId, instanceName),
                InstanceId = instanceName,
                Parent = projectId,
                Instance = new Instance
                {
                    NodeCount = nodesCount,
                    Config = config,
                    Name = instanceName,
                    DisplayName = instanceName
                },
            };
            // Make the request
            Operation<Instance> response = await instanceAdminClient.CreateInstanceAsync(request);

            // Poll until the returned long-running operation is complete
            Operation<Instance> completedResponse = await response.PollUntilCompletedAsync();
            // Retrieve the operation result
            Instance result = completedResponse.Result;

            // Or get the name of the operation
            string operationName = response.Name;
            // This name can be stored, then the long-running operation retrieved later by name
            Operation<Instance> retrievedResponse = await instanceAdminClient.PollOnceCreateInstanceAsync(operationName);
            // Check if the retrieved long-running operation has completed
            if (retrievedResponse.IsCompleted)
            {
                // If it has completed, then access the result
                Instance retrievedResult = retrievedResponse.Result;
            }
            // End snippet
        }

        public static async Task CreateDatabaseAsync(string projectId, string instance, string dbName, List<string> schemas)
        {
            // Snippet: CreateDatabaseAsync(CreateDatabaseRequest,CallSettings)
            // Create client
            DatabaseAdminClient databaseAdminClient = await DatabaseAdminClient.CreateAsync();
            // Initialize request argument(s)
            CreateDatabaseRequest request = new CreateDatabaseRequest
            {
                ParentAsInstanceName = new Google.Cloud.Spanner.Admin.Database.V1.InstanceName(projectId, instance),
                CreateStatement = $"CREATE DATABASE {dbName}",
                ExtraStatements = { schemas }
            };
            // Make the request
            Operation<Database> response =
                await databaseAdminClient.CreateDatabaseAsync(request);

            // Poll until the returned long-running operation is complete
            Operation<Database> completedResponse =
                await response.PollUntilCompletedAsync();
            // Retrieve the operation result
            Database result = completedResponse.Result;

            // Or get the name of the operation
            string operationName = response.Name;
            // This name can be stored, then the long-running operation retrieved later by name
            Operation<Database> retrievedResponse =
                await databaseAdminClient.PollOnceCreateDatabaseAsync(operationName);
            // Check if the retrieved long-running operation has completed
            if (retrievedResponse.IsCompleted)
            {
                // If it has completed, then access the result
                Database retrievedResult = retrievedResponse.Result;
            }
            // End snippet
        }

        public static async Task DropDatabaseAsync(string projectId, string instance, string dbName)
        {
            var databaseAdminClient = await DatabaseAdminClient.CreateAsync();

            var req = new DropDatabaseRequest
            {
                DatabaseAsDatabaseName = new DatabaseName(projectId, instance, dbName)
            };
            
            await databaseAdminClient.DropDatabaseAsync(req);
        }


    }
}
