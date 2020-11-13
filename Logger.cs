using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace LoggerNamespace
{
	public class Logger
	{
		public Logger(string file)
		{
			_file = file;
			_queue = new ConcurrentQueue<string>();

			_runner = new Thread(Run);
			_runner.Start();
		}

		public void Log(string log)
		{
			_queue.Enqueue(log);
		}

		public void Close()
		{
			_closing = true;
			_runner.Join();
		}

		private void Run()
		{
			using (StreamWriter writer = new StreamWriter(_file))
			{
				string log;
				while (!_closing || _queue.Count > 0)
				{
					if (_queue.TryDequeue(out log))
					{
						writer.WriteLine(log);
					}
				}
			}
		}

		private bool _closing = false;
		private readonly string _file;
		private ConcurrentQueue<string> _queue;
		private Thread _runner;
	}
}
