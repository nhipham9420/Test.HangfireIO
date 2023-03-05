using Hangfire;
using Hangfire.Storage;

namespace Test.Hangfire.Services
{
    public class ServiceManagement : IServiceManagement
    {
        public void GenerateMerchandise()
        {
            Console.WriteLine($"Generate Merchadise: Long running task {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
        }

        public void SendMail()
        {
            Console.WriteLine($"Send mail: Long running task {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
        }

        public void SyncData()
        {
            Console.WriteLine($"Sync data: Long running task {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
        }

        public void UpdateDatabase()
        {
            var jobId = BackgroundJob.Schedule(
                                        () => GenerateMerchandise(), TimeSpan.FromSeconds(30));
            
            var jobId2 = BackgroundJob.ContinueJobWith(
                                        jobId,
                                        () => Console.WriteLine($"Update database: Long running task {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}"));
            
            var jobId3 = BackgroundJob.ContinueJobWith(
                                        jobId2,
                                        () => SendMail());

            // Get state of a job
            IStorageConnection connection = JobStorage.Current.GetConnection();
            JobData jobData = connection.GetJobData(jobId3);
            string stateName = jobData.State;
            Console.WriteLine(stateName);
        }
    }
}
