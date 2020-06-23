using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HyperFabric.Tasks
{
    public abstract class ConcurrentTasks<T> : IConcurrentTasks
    {
        private readonly ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();
        private readonly IEnumerable<T> _items;
        private readonly int _taskCount;

        protected ConcurrentTasks(IEnumerable<T> items, int taskCount)
        {
            var itemList = items?.ToList() ?? new List<T>(0);
            
            if (!itemList.Any()) throw new NullReferenceException("The list of items is null or empty.");
            
            if (taskCount < 1 || taskCount > 20)
                throw new ArgumentOutOfRangeException(
                    nameof(taskCount), taskCount, "Invalid number of tasks provided.");
            
            _items = itemList;
            _taskCount = taskCount;
        }

        public async Task StartAsync()
        {
            await BeforeTaskRunAsync();
            EnqueueItems();
            var tasks = BuildTasks();
            await Task.WhenAll(tasks);
        }

        private void EnqueueItems()
        {
            _items.ToList().ForEach(i => _queue.Enqueue(i));
        }
        
        private IEnumerable<Task> BuildTasks()
        {
            var tasks = new List<Task>();
            
            for (var i = 0; i < _taskCount; i++)
            {
                var currentId = i;
                tasks.Add(Task.Run(async () => await RunnerAsync(currentId)));
            }

            return tasks;
        }

        private async Task RunnerAsync(int id)
        {
            while (_queue.Any())
            {
                if (_queue.TryDequeue(out var item))
                {
                    try
                    {
                        await HandleItemAsync(item, id);
                    }
                    catch (Exception error)
                    {
                        await HandleErrorAsync(item, error);
                    }
                }
            }
        }

        protected abstract Task HandleItemAsync(T item, int taskId);

        protected abstract Task HandleErrorAsync(T item, Exception error);

        protected virtual Task BeforeTaskRunAsync()
        {
            return Task.CompletedTask;
        }
    }
}
