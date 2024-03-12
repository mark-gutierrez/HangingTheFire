using Hangfire;
using HangingTheFire.Jobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HangingTheFire.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        [HttpPost]
        [Route("CreateBackgroundJob")]
        public ActionResult CreateBackgroundJob()
        {
            //BackgroundJob.Enqueue(() => Console.WriteLine("Background Job Triggered"));
            BackgroundJob.Enqueue<TestJob>(x => x.WriteLog("Background Job Triggered"));
            return Ok();
        }

        [HttpPost]
        [Route("CreateScheduledJob")]
        public ActionResult CreateScheduledJob()
        {
            var schedulerDateTime = DateTime.UtcNow.AddSeconds(5);
            var dateTimeOffset = new DateTimeOffset(schedulerDateTime);
            //BackgroundJob.Schedule(() => Console.WriteLine("Scheduled Job Triggered"), dateTimeOffset);
            BackgroundJob.Schedule<TestJob>(x => x.WriteLog("Scheduled Job Triggered"), dateTimeOffset);
            return Ok();
        }
        
        [HttpPost]
        [Route("CreateContinuationJob")]
        public ActionResult CreateContinuationJob()
        {
            var schedulerDateTime = DateTime.UtcNow.AddSeconds(5);
            var dateTimeOffset = new DateTimeOffset(schedulerDateTime);
            var jobId = BackgroundJob.Schedule<TestJob>(x => x.WriteLog("Scheduled Job 2 Triggered"), dateTimeOffset);

            var job2Id = BackgroundJob.ContinueJobWith<TestJob>(jobId, x => x.WriteLog("Continuation Job 1 Triggered"));
            var job3Id = BackgroundJob.ContinueJobWith<TestJob>(job2Id, x => x.WriteLog("Continuation Job 2 Triggered"));
            var job4Id = BackgroundJob.ContinueJobWith<TestJob>(job3Id, x => x.WriteLog("Continuation Job 3 Triggered"));
            return Ok();
        }
        
        [HttpPost]
        [Route("CreateRcurringJob")]
        public ActionResult CreateRcurringJob()
        {
            RecurringJob.AddOrUpdate<TestJob>("RecurringJob1", x => x.WriteLog("Recurring Job Triggered"), "* * * * *");
            return Ok();
        }
    }
}
