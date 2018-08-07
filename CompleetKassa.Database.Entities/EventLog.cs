using System;
using CompleetKassa.Database.Core.Entities;

namespace CompleetKassa.Database.Entities
{
	public class EventLog : IEntity
	{
		public EventLog()
		{
		}

		public int ID { get; set; }

		public int EventType { get; set; }

		public string Key { get; set; }

		public string Message { get; set; }

		public DateTime? EntryDate { get; set; }
	}
}
