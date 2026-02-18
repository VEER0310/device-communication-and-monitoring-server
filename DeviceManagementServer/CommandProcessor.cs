
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceManagementServer
{
    public class CommandProcessor
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(5);
        private readonly DeviceManager _deviceManager;
        private int _activeCount = 0;

        internal CommandProcessor(DeviceManager manager)
        {
            _deviceManager = manager;
        }

        public async Task ProcessCommandAsync(string command)
        {
            Console.WriteLine($"[WAITING] {command}");

            await _semaphore.WaitAsync();

            int current = Interlocked.Increment(ref _activeCount);
            Console.WriteLine($"[EXECUTING] ({current}) {command}");

            try
            {
                ////delaying to show concurrent working
                //Thread.Sleep(3000);
                await Task.Delay(3000);


                ExecuteCommand(command);
            }
            finally
            {
                current = Interlocked.Decrement(ref _activeCount);
                Console.WriteLine($"[DONE] ({current}) {command}");
                _semaphore.Release();
            }
        }

        private void ExecuteCommand(string command)
        {
            var parts = command.Split('|');
            if (parts.Length < 2) return;

            switch (parts[0])
            {
                case "REGISTER":
                    _deviceManager.Register(parts[1]);
                    break;

                case "STATUS":
                    _deviceManager.UpdateStatus(parts[1], parts[2]);
                    break;

                case "HEARTBEAT":
                    _deviceManager.Heartbeat(parts[1]);
                    break;

                case "DISCONNECT":
                    _deviceManager.Disconnect(parts[1]);
                    break;
            }
        }
    }
}
