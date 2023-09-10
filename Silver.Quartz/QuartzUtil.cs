using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Quartz.Impl.Triggers;
using Silver.Basic;
using Silver.Cache;
using Silver.Quartz.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Silver.Quartz
{
    /// <summary>
    /// 定时任务
    /// </summary>
    public class QuartzUtil<T> where T : IJob
    { 
        private bool IsRedisQuartz = true;

        public QuartzUtil(bool isRedisQuartz)
        {
            IsRedisQuartz = isRedisQuartz;
        }

        /// <summary>
        /// 初始化作业
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public async Task<bool> UseQuartzAsync()
        {
            ISchedulerFactory _schedulerFactory = new StdSchedulerFactory();
            var listTaskOptions = ListTaskOptions();
            foreach (var item in listTaskOptions)
            {
                await AddJob(item);
            }
            return true;
        }

        /// <summary>
        /// 添加作业
        /// </summary>
        /// <param name="taskOptions"></param>
        /// <returns></returns>
        public async Task AddJob(TaskOptions taskOptions)
        {
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            (bool, string) validExpression = IsValidExpression(taskOptions.BasicsCron);
            if (validExpression.Item1 == false)
            {
                Writelogs($"Quartz Add Valid Error:{validExpression.Item2}");
                return;
            }
            if (Exists(taskOptions))
            {
                RemoveJob(taskOptions);
            }
            IScheduler sched = await schedulerFactory.GetScheduler();
            IJobDetail job = JobBuilder.Create<T>().WithIdentity(taskOptions.TaskID, taskOptions.GroupName).Build();
            ITrigger trigger = TriggerBuilder.Create().StartNow().WithDescription(taskOptions.Describe).WithCronSchedule(taskOptions.BasicsCron).Build();
            await sched.ScheduleJob(job, trigger);
            if (taskOptions.Status == (int)TriggerState.Normal)
            {
                WriteTaskOptions(taskOptions);
                await sched.Start();
            }
            else
            {
                await PauseJobTask(taskOptions);
            }
        }

        /// <summary>
        /// 获取所有作业
        /// </summary>
        /// <returns></returns>
        public List<TaskOptions> GetJobs()
        {
            return ListTaskOptions();
        }

        /// <summary>
        /// 移除作业
        /// </summary>
        /// <param name="taskOptions"></param>
        /// <returns></returns>
        public object RemoveJob(TaskOptions taskOptions)
        {
            DeleteTaskOptions(taskOptions.TaskID);
            return TriggerTaskAction(JobAction.删除, taskOptions);
        }

        /// <summary>
        /// 更新作业
        /// </summary>
        /// <param name="taskOptions"></param>
        /// <returns></returns>
        public Task<object> UpdateJob(TaskOptions taskOptions)
        {
            WriteTaskOptions(taskOptions);
            return TriggerTaskAction(JobAction.修改, taskOptions);
        }

        /// <summary>
        /// 暂停作业
        /// </summary>
        /// <param name="taskOptions"></param>
        /// <returns></returns>
        public Task<object> PauseJobTask(TaskOptions taskOptions)
        {
            WriteTaskOptions(taskOptions);
            return TriggerTaskAction(JobAction.暂停, taskOptions);
        }

        /// <summary>
        /// 启动作业
        /// </summary>
        /// <param name="taskOptions"></param>
        /// <returns></returns>
        public Task<object> StartJob(TaskOptions taskOptions)
        {
            WriteTaskOptions(taskOptions);
            return TriggerTaskAction(JobAction.开启, taskOptions);
        }

        /// <summary>
        /// 立即运行任务
        /// </summary>
        /// <param name="taskOptions"></param>
        /// <returns></returns>
        public Task<object> RunJob(TaskOptions taskOptions)
        {
            WriteTaskOptions(taskOptions);
            return TriggerTaskAction(JobAction.立即执行, taskOptions);
        }

        /// <summary>
        /// 触发新增、删除、修改、暂停、启用、立即执行事件
        /// </summary>
        /// <param name="action"></param>
        /// <param name="taskOptions"></param>
        /// <returns></returns>
        public async Task<object> TriggerTaskAction(JobAction action, TaskOptions taskOptions)
        {
            string errorMsg = "";
            var TaskList = GetJobs();
            try
            {
                ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
                IScheduler scheduler = await schedulerFactory.GetScheduler();
                List<JobKey> jobKeys = scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(taskOptions.GroupName)).Result.ToList();
                if (jobKeys == null || jobKeys.Count() == 0)
                {
                    errorMsg = $"未找到分组[{taskOptions.GroupName}]";
                    return new { status = false, msg = errorMsg };
                }
                //JobKey jobKey = jobKeys.Where(s => scheduler.GetTriggersOfJob(s).Result.Any(x => (x as CronTriggerImpl).Name == taskOptions.TaskID)).FirstOrDefault();
                JobKey jobKey = jobKeys.Where(s => s.Name == taskOptions.TaskID).FirstOrDefault();
                if (jobKey == null)
                {
                    errorMsg = $"未找到触发器[{taskOptions.TaskID}]";
                    return new { status = false, msg = errorMsg };
                }
                var triggers = await scheduler.GetTriggersOfJob(jobKey);
                if (triggers == null)
                {
                    errorMsg = $"未获取到触发器[{taskOptions.TaskID}]";
                    return new { status = false, msg = errorMsg };
                }
                //ITrigger trigger = triggers?.Where(x => (x as CronTriggerImpl).Name == taskOptions.TaskID).FirstOrDefault();
                ITrigger trigger = triggers.Where(x => x.JobKey.Name == taskOptions.TaskID).FirstOrDefault();
                if (trigger == null)
                {
                    errorMsg = $"未找到触发器[{taskOptions.TaskID}]";
                    return new { status = false, msg = errorMsg };
                }
                object result = null;
                switch (action)
                {
                    case JobAction.删除:
                        await scheduler.PauseTrigger(trigger.Key);
                        await scheduler.UnscheduleJob(trigger.Key);// 移除触发器
                        await scheduler.DeleteJob(trigger.JobKey);
                        for (int i = 0; i < TaskList.Count; i++)
                        {
                            if (TaskList[i].TaskID.ToLower() == taskOptions.TaskID.ToLower())
                            {
                                TaskList.RemoveAt(i);
                            }
                        }
                        break;
                    case JobAction.修改:
                        await scheduler.PauseTrigger(trigger.Key);
                        await scheduler.UnscheduleJob(trigger.Key);// 移除触发器
                        await scheduler.DeleteJob(trigger.JobKey);
                        for (int i = 0; i < TaskList.Count; i++)
                        {
                            if (TaskList[i].TaskID.ToLower() == taskOptions.TaskID.ToLower())
                            {
                                TaskList.RemoveAt(i);
                            }
                        }
                        IScheduler sched = await schedulerFactory.GetScheduler();
                        IJobDetail job = JobBuilder.Create<T>().WithIdentity(taskOptions.TaskID, taskOptions.GroupName).Build();
                        trigger = TriggerBuilder.Create().StartNow().WithDescription(taskOptions.Describe).WithCronSchedule(taskOptions.BasicsCron).Build();
                        await sched.ScheduleJob(job, trigger);
                        if (taskOptions.Status == (int)TriggerState.Normal)
                        {
                            await sched.Start();
                        }
                        break;
                    case JobAction.暂停:
                    case JobAction.停止:
                    case JobAction.开启:
                        var options = TaskList.Where(x => x.TaskName == taskOptions.TaskName && x.GroupName == taskOptions.GroupName).FirstOrDefault();
                        if (action == JobAction.暂停)
                        {
                            options.Status = (int)TriggerState.Paused;
                            await scheduler.PauseTrigger(trigger.Key);
                            await scheduler.PauseJob(trigger.JobKey);
                        }
                        else if (action == JobAction.停止)
                        {
                            options.Status = (int)action;
                            await scheduler.Shutdown();
                        }
                        else
                        {
                            options.Status = (int)TriggerState.Normal;
                            await scheduler.ResumeTrigger(trigger.Key);
                        }
                        break;
                    case JobAction.立即执行:
                        await scheduler.TriggerJob(jobKey);
                        break;
                }
                return result ?? new { status = true, msg = $"作业{action.ToString()}成功" };
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return new { status = false, msg = ex.Message };
            }
            finally
            {
                if (TaskList.Count > 0)
                {
                    foreach (var item in TaskList)
                    {
                        WriteTaskOptions(item);
                    }
                }
            }
        }

        /// <summary>
        /// 记录任务
        /// </summary>
        /// <param name="options"></param>
        private void WriteTaskOptions(TaskOptions options)
        {
            if (IsRedisQuartz)
            {
                var redis = new RedisUtil().GetClient();
                {
                    redis.HSet("Quartz:Task:" + typeof(T).FullName, options.TaskID, options);
                }
            }
            else
            {
                string taskPath = $"{Directory.GetCurrentDirectory()}/Quartz/{typeof(T).FullName}/";
                if (!Directory.Exists(taskPath))
                {
                    Directory.CreateDirectory(taskPath);
                }
                File.WriteAllText($"{taskPath}{options.TaskID}.txt", options.ToJson());
            }
        }

        /// <summary>
        /// 读取任务列表
        /// </summary>
        /// <returns></returns>
        public List<TaskOptions> ListTaskOptions()
        {
            List<TaskOptions> listOption = new List<TaskOptions>();
            if (IsRedisQuartz)
            {
                var redis = new RedisUtil().GetClient();
                {
                    if (redis.Exists("Quartz:Task:" + typeof(T).FullName))
                    {
                        listOption = redis.HGetAll<TaskOptions>("Quartz:Task:" + typeof(T).FullName).Values.ToList();
                    }
                }
            }
            else
            {
                string taskPath = $"{Directory.GetCurrentDirectory()}/Quartz/{typeof(T).FullName}/";
                if (!Directory.Exists(taskPath))
                {
                    Directory.CreateDirectory(taskPath);
                }
                string[] files = Directory.GetFiles(taskPath);
                foreach (var item in files)
                {
                    listOption.Add(File.ReadAllText(item).JsonToObject<TaskOptions>());
                }
            }
            return listOption;
        }

        /// <summary>
        /// 记录删除
        /// </summary>
        /// <param name="TaskID"></param>
        public void DeleteTaskOptions(string TaskID)
        {
            if (IsRedisQuartz)
            {
                var redis = new RedisUtil().GetClient();
                {
                    if (redis.HExists("Quartz:Task:" + typeof(T).FullName, TaskID))
                    {
                        redis.HDel("Quartz:Task:" + typeof(T).FullName, TaskID);
                    }
                }
            }
            else
            {
                string taskPath = $"{Directory.GetCurrentDirectory()}/Quartz/{typeof(T).FullName}/";
                if (!Directory.Exists(taskPath))
                {
                    Directory.CreateDirectory(taskPath);
                }
                if (File.Exists($"{taskPath}{TaskID}.txt"))
                {
                    File.Delete($"{taskPath}{TaskID}.txt");
                }
            }
        }


        /// <summary>
        /// Cron格式是否正确
        /// </summary>
        /// <param name="cronExpression"></param>
        /// <returns></returns>
        public (bool, string) IsValidExpression(string cronExpression)
        {
            try
            {
                CronTriggerImpl trigger = new CronTriggerImpl();
                trigger.CronExpressionString = cronExpression;
                DateTimeOffset? date = trigger.ComputeFirstFireTimeUtc(null);
                return (date != null, date == null ? $"请确认表达式{cronExpression}是否正确!" : "");
            }
            catch (Exception e)
            {
                return (false, $"请确认表达式{cronExpression}是否正确!{e.Message}");
            }
        }

        /// <summary>
        /// 作业是否存在
        /// </summary>
        /// <param name="taskOptions"></param> 
        /// <returns></returns>
        public bool Exists(TaskOptions taskOptions)
        {
            if (GetJobs().Where(x => x.TaskID == taskOptions.TaskID && x.GroupName == taskOptions.GroupName).Count() > 0)
            {
                return false;
            }
            return true;
        }

        public void Writelogs(string message, string method = "QuartzExtension")
        {
            string path = Directory.GetCurrentDirectory() + "//logs//";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path += DateTime.Now.ToString("yyyy-MM-dd") + ".log";
            string content = $"时间：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}  \r\n  方法：{method}  \r\n  内容：{message} \r\n\r\n ";
            File.AppendAllText(path, content);
        }


    }  
}
