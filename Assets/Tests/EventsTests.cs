using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using Assert = UnityEngine.Assertions.Assert;

namespace Tests
{
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public class EventsTests
	{
		private const double tolerance = 0.00001;

		[Test]
		public void LimitingEvent()
		{
			PlayerStats stats = new();
			stats.Init();
			stats.SetStat("TEST_STAT", 0);
			GameManager.OverridePlayerStats(stats);
			
			EventStorage storage = new();
			GameManager.OverrideEventStorage(storage);
			
			List<GameEvent> events = new()
			{
				new()
				{
					Limits = new()
					{
						new() {Type = "TEST_EXISTS"},
						new() {Type = "TEST_STAT", CompareTo = 5},
						new() {Type = "TEST_STAT", CompareTo = 9, Comparison = Utils.Comparison.LtE}
					}
				}
			};
			storage.Load(events, new(), new(), new());
			
			storage.Init();
			
			// Limited event is not added
			Assert.AreEqual(0, storage.GetQueue().Count);
			
			// Not all limits are passed
			GameManager.PlayerStats.SetFlag("TEST_EXISTS", 1);
			Assert.AreEqual(0, storage.GetQueue().Count);
			GameManager.PlayerStats.SetStat("TEST_STAT", 15);
			Assert.AreEqual(0, storage.GetQueue().Count);
			
			// Every limit is passed
			GameManager.PlayerStats.SetStat("TEST_STAT", 9);
			Assert.AreEqual(1, storage.GetQueue().Count);
		}
	}
}