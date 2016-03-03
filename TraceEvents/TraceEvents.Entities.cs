
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.ComponentModel;

using Aspectize.Core;

[assembly:AspectizeDALAssemblyAttribute]

namespace TraceEvents
{
	public static partial class SchemaNames
	{
		public static partial class Entities
		{
			public const string Trace = "Trace";
			public const string Events = "Events";
			public const string Year = "Year";
			public const string Month = "Month";
			public const string Day = "Day";
			public const string Hour = "Hour";
			public const string Minute = "Minute";
			public const string DateFilterTemp = "DateFilterTemp";
			public const string Week = "Week";
			public const string Quarter = "Quarter";
			public const string Stat = "Stat";
			public const string TraceRequest = "TraceRequest";
			public const string Who = "Who";
			public const string Root = "Root";
		}

		public static partial class Relations
		{
			public const string EventsTrace = "EventsTrace";
			public const string TraceYear = "TraceYear";
			public const string TraceMonth = "TraceMonth";
			public const string TraceDay = "TraceDay";
			public const string TraceHour = "TraceHour";
			public const string TraceMinute = "TraceMinute";
			public const string EventsMinute = "EventsMinute";
			public const string EventsDay = "EventsDay";
			public const string EventsMonth = "EventsMonth";
			public const string EventsHour = "EventsHour";
			public const string EventsYear = "EventsYear";
			public const string TraceWeek = "TraceWeek";
			public const string EventsWeek = "EventsWeek";
			public const string EventsQuarter = "EventsQuarter";
			public const string TraceQuarter = "TraceQuarter";
			public const string EventsStat = "EventsStat";
			public const string WhoTrace = "WhoTrace";
			public const string RootWho = "RootWho";
			public const string RootTrace = "RootTrace";
			public const string RootEvents = "RootEvents";
		}
	}

	[SchemaNamespace]
	public class DomainProvider : INamespace
	{
		public string Name { get { return GetType().Namespace; } }
		public static string DomainName { get { return new DomainProvider().Name; } }
	}


	[DataDefinition(MustPersist = false)]
	public enum EnumTimeUnity
	{
		[Description("Minutes")]
		Minutes,
		[Description("Hour")]
		Hour,
		[Description("Day")]
		Day,
		[Description("Month")]
		Month
	}

	[DataDefinition(MustPersist = false)]
	[Flags]
	public enum EnumFrequency
	{
		[Description("Minute")]
		Minutely		 = 		1,
		[Description("Hour")]
		Hourly		 = 		2,
		[Description("Day")]
		Daily		 = 		4,
		[Description("Week")]
		Weekly		 = 		8,
		[Description("Month")]
		Monthly		 = 		16,
		[Description("Quarter")]
		Quarterly		 = 		32,
		[Description("Year")]
		Yearly		 = 		64
	}

	[DataDefinition(MustPersist = false)]
	public enum EnumTagValueType
	{
		[Description("NoValue")]
		NoValue,
		[Description("Summarize")]
		Summarize,
		[Description("Discrete")]
		Discrete
	}

	[DataDefinition(PhysicalName = "TraceEventsTrace")]
	public class Trace : Entity, IDataWrapper
	{
		public static partial class Fields
		{
			public const string Id = "Id";
			public const string DateTrace = "DateTrace";
			public const string Who = "Who";
			public const string UserAgent = "UserAgent";
			public const string Longitude = "Longitude";
			public const string Latitude = "Latitude";
			public const string Info = "Info";
			public const string IndexKey = "IndexKey";
			public const string Value = "Value";
		}

		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[Data(IsPrimaryKey=true)]
		public Guid Id
		{
			get { return getValue<Guid>("Id"); }
			set { setValue<Guid>("Id", value); }
		}

		[Data(IsIndexed = IndexOrderType.Desc)]
		public DateTime DateTrace
		{
			get { return getValue<DateTime>("DateTrace"); }
			set { setValue<DateTime>("DateTrace", value); }
		}

		[Data(IsNullable = true)]
		public string Who
		{
			get { return getValue<string>("Who"); }
			set { setValue<string>("Who", value); }
		}

		[Data(IsNullable = true)]
		public string UserAgent
		{
			get { return getValue<string>("UserAgent"); }
			set { setValue<string>("UserAgent", value); }
		}

		[Data(IsNullable = true)]
		public double? Longitude
		{
			get { return getValue<double?>("Longitude"); }
			set { setValue<double?>("Longitude", value); }
		}

		[Data(IsNullable = true)]
		public double? Latitude
		{
			get { return getValue<double?>("Latitude"); }
			set { setValue<double?>("Latitude", value); }
		}

		[Data(IsNullable = true)]
		public string Info
		{
			get { return getValue<string>("Info"); }
			set { setValue<string>("Info", value); }
		}

		[Data(IsIndexed = IndexOrderType.Asc)]
		public string IndexKey
		{
			get { return getValue<string>("IndexKey"); }
			set { setValue<string>("IndexKey", value); }
		}

		[Data(IsNullable = true)]
		public decimal? Value
		{
			get { return getValue<decimal?>("Value"); }
			set { setValue<decimal?>("Value", value); }
		}

	}

	[DataDefinition(PhysicalName = "TraceEventsEvents")]
	public class Events : Entity, IDataWrapper
	{
		public static partial class Fields
		{
			public const string Id = "Id";
			public const string Name = "Name";
			public const string CountEvents = "CountEvents";
			public const string SumEvents = "SumEvents";
			public const string ValueType = "ValueType";
			public const string Selected = "Selected";
			public const string MinValue = "MinValue";
			public const string MaxValue = "MaxValue";
			public const string GraphMin = "GraphMin";
			public const string GraphMax = "GraphMax";
			public const string GraphStep = "GraphStep";
			public const string Unity = "Unity";
			public const string Frequency = "Frequency";
			public const string HasValue = "HasValue";
		}

		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);


		}

		[Data(IsPrimaryKey=true)]
		public Guid Id
		{
			get { return getValue<Guid>("Id"); }
			set { setValue<Guid>("Id", value); }
		}

		[Data]
		public string Name
		{
			get { return getValue<string>("Name"); }
			set { setValue<string>("Name", value); }
		}

		[Data(AggregationChild = "EventsTrace")]
		public long CountEvents
		{
			get { return getValue<long>("CountEvents"); }
			set { setValue<long>("CountEvents", value); }
		}

		[Data]
		public decimal SumEvents
		{
			get { return getValue<decimal>("SumEvents"); }
			set { setValue<decimal>("SumEvents", value); }
		}

		[Data(DefaultValue = EnumTagValueType.NoValue)]
		public EnumTagValueType ValueType
		{
			get { return getValue<EnumTagValueType>("ValueType"); }
			set { setValue<EnumTagValueType>("ValueType", value); }
		}

		[Data(DefaultValue = false)]
		public bool Selected
		{
			get { return getValue<bool>("Selected"); }
			set { setValue<bool>("Selected", value); }
		}

		[Data(MustPersist = false)]
		public decimal MinValue
		{
			get { return getValue<decimal>("MinValue"); }
			set { setValue<decimal>("MinValue", value); }
		}

		[Data(MustPersist = false)]
		public decimal MaxValue
		{
			get { return getValue<decimal>("MaxValue"); }
			set { setValue<decimal>("MaxValue", value); }
		}

		[Data(MustPersist = false)]
		public decimal GraphMin
		{
			get { return getValue<decimal>("GraphMin"); }
			set { setValue<decimal>("GraphMin", value); }
		}

		[Data(MustPersist = false)]
		public decimal GraphMax
		{
			get { return getValue<decimal>("GraphMax"); }
			set { setValue<decimal>("GraphMax", value); }
		}

		[Data(MustPersist = false)]
		public decimal GraphStep
		{
			get { return getValue<decimal>("GraphStep"); }
			set { setValue<decimal>("GraphStep", value); }
		}

		[Data(IsNullable = true)]
		public string Unity
		{
			get { return getValue<string>("Unity"); }
			set { setValue<string>("Unity", value); }
		}

		[Data(DefaultValue = 126)]
		public EnumFrequency Frequency
		{
			get { return getValue<EnumFrequency>("Frequency"); }
			set { setValue<EnumFrequency>("Frequency", value); }
		}

		[Data(MustPersist = false, Expression = "IIF(ValueType = 1, 'true', 'false')")]
		public bool HasValue
		{
			get { return getValue<bool>("HasValue"); }
		}

		public List<Day> Day
		{
			get { return this.GetAssociatedInstances<Day, EventsDay>(); }
		}

		public List<Month> Month
		{
			get { return this.GetAssociatedInstances<Month, EventsMonth>(); }
		}

		public List<Hour> Hour
		{
			get { return this.GetAssociatedInstances<Hour, EventsHour>(); }
		}

		public List<Year> Year
		{
			get { return this.GetAssociatedInstances<Year, EventsYear>(); }
		}

		public List<Week> Week
		{
			get { return this.GetAssociatedInstances<Week, EventsWeek>(); }
		}

		public List<Quarter> Quarter
		{
			get { return this.GetAssociatedInstances<Quarter, EventsQuarter>(); }
		}

	}

	[DataDefinition(PhysicalName = "TraceEventsYear")]
	public class Year : Entity, IDataWrapper
	{
		public static partial class Fields
		{
			public const string Id = "Id";
			public const string TimeKey = "TimeKey";
			public const string DatePeriod = "DatePeriod";
			public const string CountYear = "CountYear";
			public const string SumYear = "SumYear";
			public const string Display = "Display";
		}

		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[Data(IsPrimaryKey = true)]
		public string Id
		{
			get { return getValue<string>("Id"); }
			set { setValue<string>("Id", value); }
		}

		[Data]
		public string TimeKey
		{
			get { return getValue<string>("TimeKey"); }
			set { setValue<string>("TimeKey", value); }
		}

		[Data]
		public DateTime DatePeriod
		{
			get { return getValue<DateTime>("DatePeriod"); }
			set { setValue<DateTime>("DatePeriod", value); }
		}

		[Data(AggregationChild = "TraceYear")]
		public long CountYear
		{
			get { return getValue<long>("CountYear"); }
			set { setValue<long>("CountYear", value); }
		}

		[Data]
		public decimal SumYear
		{
			get { return getValue<decimal>("SumYear"); }
			set { setValue<decimal>("SumYear", value); }
		}

		[Data(DefaultValue = true, MustPersist = false)]
		public bool Display
		{
			get { return getValue<bool>("Display"); }
			set { setValue<bool>("Display", value); }
		}

	}

	[DataDefinition(PhysicalName = "TraceEventsMonth")]
	public class Month : Entity, IDataWrapper
	{
		public static partial class Fields
		{
			public const string Id = "Id";
			public const string TimeKey = "TimeKey";
			public const string DatePeriod = "DatePeriod";
			public const string CountMonth = "CountMonth";
			public const string SumMonth = "SumMonth";
			public const string Display = "Display";
		}

		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[Data(IsPrimaryKey = true)]
		public string Id
		{
			get { return getValue<string>("Id"); }
			set { setValue<string>("Id", value); }
		}

		[Data]
		public string TimeKey
		{
			get { return getValue<string>("TimeKey"); }
			set { setValue<string>("TimeKey", value); }
		}

		[Data]
		public DateTime DatePeriod
		{
			get { return getValue<DateTime>("DatePeriod"); }
			set { setValue<DateTime>("DatePeriod", value); }
		}

		[Data(AggregationChild = "TraceMonth")]
		public long CountMonth
		{
			get { return getValue<long>("CountMonth"); }
			set { setValue<long>("CountMonth", value); }
		}

		[Data]
		public decimal SumMonth
		{
			get { return getValue<decimal>("SumMonth"); }
			set { setValue<decimal>("SumMonth", value); }
		}

		[Data(DefaultValue = true, MustPersist = false)]
		public bool Display
		{
			get { return getValue<bool>("Display"); }
			set { setValue<bool>("Display", value); }
		}

	}

	[DataDefinition(PhysicalName = "TraceEventsDay")]
	public class Day : Entity, IDataWrapper
	{
		public static partial class Fields
		{
			public const string Id = "Id";
			public const string TimeKey = "TimeKey";
			public const string DatePeriod = "DatePeriod";
			public const string CountDay = "CountDay";
			public const string SumDay = "SumDay";
			public const string Display = "Display";
		}

		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[Data(IsPrimaryKey = true)]
		public string Id
		{
			get { return getValue<string>("Id"); }
			set { setValue<string>("Id", value); }
		}

		[Data]
		public string TimeKey
		{
			get { return getValue<string>("TimeKey"); }
			set { setValue<string>("TimeKey", value); }
		}

		[Data]
		public DateTime DatePeriod
		{
			get { return getValue<DateTime>("DatePeriod"); }
			set { setValue<DateTime>("DatePeriod", value); }
		}

		[Data(AggregationChild = "TraceDay")]
		public long CountDay
		{
			get { return getValue<long>("CountDay"); }
			set { setValue<long>("CountDay", value); }
		}

		[Data]
		public decimal SumDay
		{
			get { return getValue<decimal>("SumDay"); }
			set { setValue<decimal>("SumDay", value); }
		}

		[Data(DefaultValue = true, MustPersist = false)]
		public bool Display
		{
			get { return getValue<bool>("Display"); }
			set { setValue<bool>("Display", value); }
		}

	}

	[DataDefinition(PhysicalName = "TraceEventsHour")]
	public class Hour : Entity, IDataWrapper
	{
		public static partial class Fields
		{
			public const string Id = "Id";
			public const string TimeKey = "TimeKey";
			public const string DatePeriod = "DatePeriod";
			public const string CountHour = "CountHour";
			public const string SumHour = "SumHour";
			public const string Display = "Display";
		}

		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[Data(IsPrimaryKey = true)]
		public string Id
		{
			get { return getValue<string>("Id"); }
			set { setValue<string>("Id", value); }
		}

		[Data]
		public string TimeKey
		{
			get { return getValue<string>("TimeKey"); }
			set { setValue<string>("TimeKey", value); }
		}

		[Data]
		public DateTime DatePeriod
		{
			get { return getValue<DateTime>("DatePeriod"); }
			set { setValue<DateTime>("DatePeriod", value); }
		}

		[Data(AggregationChild = "TraceHour")]
		public long CountHour
		{
			get { return getValue<long>("CountHour"); }
			set { setValue<long>("CountHour", value); }
		}

		[Data]
		public decimal SumHour
		{
			get { return getValue<decimal>("SumHour"); }
			set { setValue<decimal>("SumHour", value); }
		}

		[Data(DefaultValue = true, MustPersist = false)]
		public bool Display
		{
			get { return getValue<bool>("Display"); }
			set { setValue<bool>("Display", value); }
		}

	}

	[DataDefinition(PhysicalName = "TraceEventsMinute")]
	public class Minute : Entity, IDataWrapper
	{
		public static partial class Fields
		{
			public const string Id = "Id";
			public const string TimeKey = "TimeKey";
			public const string DatePeriod = "DatePeriod";
			public const string CountMinute = "CountMinute";
			public const string SumMinute = "SumMinute";
			public const string Display = "Display";
		}

		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[Data(IsPrimaryKey = true)]
		public string Id
		{
			get { return getValue<string>("Id"); }
			set { setValue<string>("Id", value); }
		}

		[Data]
		public string TimeKey
		{
			get { return getValue<string>("TimeKey"); }
			set { setValue<string>("TimeKey", value); }
		}

		[Data]
		public DateTime DatePeriod
		{
			get { return getValue<DateTime>("DatePeriod"); }
			set { setValue<DateTime>("DatePeriod", value); }
		}

		[Data(AggregationChild = "TraceMinute")]
		public long CountMinute
		{
			get { return getValue<long>("CountMinute"); }
			set { setValue<long>("CountMinute", value); }
		}

		[Data]
		public decimal SumMinute
		{
			get { return getValue<decimal>("SumMinute"); }
			set { setValue<decimal>("SumMinute", value); }
		}

		[Data(DefaultValue = true, MustPersist = false)]
		public bool Display
		{
			get { return getValue<bool>("Display"); }
			set { setValue<bool>("Display", value); }
		}

	}

	[DataDefinition(MustPersist = false)]
	public class DateFilterTemp : Entity, IDataWrapper
	{
		public static partial class Fields
		{
			public const string Id = "Id";
			public const string DateStart = "DateStart";
			public const string DateEnd = "DateEnd";
			public const string Frequency = "Frequency";
			public const string DateNow = "DateNow";
			public const string GridGraph = "GridGraph";
		}

		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);

		}

		[Data(IsPrimaryKey=true)]
		public Guid Id
		{
			get { return getValue<Guid>("Id"); }
			set { setValue<Guid>("Id", value); }
		}

		[Data]
		public Date DateStart
		{
			get { return getValue<Date>("DateStart"); }
			set { setValue<Date>("DateStart", value); }
		}

		[Data]
		public Date DateEnd
		{
			get { return getValue<Date>("DateEnd"); }
			set { setValue<Date>("DateEnd", value); }
		}

		[Data(DefaultValue = EnumFrequency.Daily)]
		public EnumFrequency Frequency
		{
			get { return getValue<EnumFrequency>("Frequency"); }
			set { setValue<EnumFrequency>("Frequency", value); }
		}

		[Data]
		public DateTime DateNow
		{
			get { return getValue<DateTime>("DateNow"); }
			set { setValue<DateTime>("DateNow", value); }
		}

		[Data(DefaultValue = true)]
		public bool GridGraph
		{
			get { return getValue<bool>("GridGraph"); }
			set { setValue<bool>("GridGraph", value); }
		}

	}

	[DataDefinition(PhysicalName = "TraceEventsWeek")]
	public class Week : Entity, IDataWrapper
	{
		public static partial class Fields
		{
			public const string Id = "Id";
			public const string TimeKey = "TimeKey";
			public const string DatePeriod = "DatePeriod";
			public const string CountWeek = "CountWeek";
			public const string SumWeek = "SumWeek";
			public const string Display = "Display";
		}

		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[Data(IsPrimaryKey = true)]
		public string Id
		{
			get { return getValue<string>("Id"); }
			set { setValue<string>("Id", value); }
		}

		[Data]
		public string TimeKey
		{
			get { return getValue<string>("TimeKey"); }
			set { setValue<string>("TimeKey", value); }
		}

		[Data]
		public DateTime DatePeriod
		{
			get { return getValue<DateTime>("DatePeriod"); }
			set { setValue<DateTime>("DatePeriod", value); }
		}

		[Data(AggregationChild = "TraceWeek")]
		public long CountWeek
		{
			get { return getValue<long>("CountWeek"); }
			set { setValue<long>("CountWeek", value); }
		}

		[Data]
		public decimal SumWeek
		{
			get { return getValue<decimal>("SumWeek"); }
			set { setValue<decimal>("SumWeek", value); }
		}

		[Data(DefaultValue = true, MustPersist = false)]
		public bool Display
		{
			get { return getValue<bool>("Display"); }
			set { setValue<bool>("Display", value); }
		}

	}

	[DataDefinition(PhysicalName = "TraceEventsQuarter")]
	public class Quarter : Entity, IDataWrapper
	{
		public static partial class Fields
		{
			public const string Id = "Id";
			public const string TimeKey = "TimeKey";
			public const string DatePeriod = "DatePeriod";
			public const string CountQuarter = "CountQuarter";
			public const string SumQuarter = "SumQuarter";
			public const string Display = "Display";
		}

		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[Data(IsPrimaryKey = true)]
		public string Id
		{
			get { return getValue<string>("Id"); }
			set { setValue<string>("Id", value); }
		}

		[Data]
		public string TimeKey
		{
			get { return getValue<string>("TimeKey"); }
			set { setValue<string>("TimeKey", value); }
		}

		[Data]
		public DateTime DatePeriod
		{
			get { return getValue<DateTime>("DatePeriod"); }
			set { setValue<DateTime>("DatePeriod", value); }
		}

		[Data(AggregationChild = "TraceQuarter")]
		public long CountQuarter
		{
			get { return getValue<long>("CountQuarter"); }
			set { setValue<long>("CountQuarter", value); }
		}

		[Data]
		public decimal SumQuarter
		{
			get { return getValue<decimal>("SumQuarter"); }
			set { setValue<decimal>("SumQuarter", value); }
		}

		[Data(DefaultValue = true, MustPersist = false)]
		public bool Display
		{
			get { return getValue<bool>("Display"); }
			set { setValue<bool>("Display", value); }
		}

	}

	[DataDefinition(MustPersist = false)]
	public class Stat : Entity, IDataWrapper
	{
		public static partial class Fields
		{
			public const string Id = "Id";
			public const string LastMinuteCount = "LastMinuteCount";
			public const string LastMinuteSum = "LastMinuteSum";
			public const string LastHourCount = "LastHourCount";
			public const string LastHourSum = "LastHourSum";
			public const string LastDayCount = "LastDayCount";
			public const string LastDaySum = "LastDaySum";
			public const string LastWeekCount = "LastWeekCount";
			public const string LastWeekSum = "LastWeekSum";
			public const string LastMonthCount = "LastMonthCount";
			public const string LastMonthSum = "LastMonthSum";
			public const string LastQuarterCount = "LastQuarterCount";
			public const string LastQuarterSum = "LastQuarterSum";
			public const string LastYearCount = "LastYearCount";
			public const string LastYearSum = "LastYearSum";
		}

		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[Data(IsPrimaryKey=true)]
		public Guid Id
		{
			get { return getValue<Guid>("Id"); }
			set { setValue<Guid>("Id", value); }
		}

		[Data(DefaultValue = 0, MustPersist = false)]
		public long LastMinuteCount
		{
			get { return getValue<long>("LastMinuteCount"); }
			set { setValue<long>("LastMinuteCount", value); }
		}

		[Data(DefaultValue = 0, MustPersist = false)]
		public decimal LastMinuteSum
		{
			get { return getValue<decimal>("LastMinuteSum"); }
			set { setValue<decimal>("LastMinuteSum", value); }
		}

		[Data(DefaultValue = 0, MustPersist = false)]
		public long LastHourCount
		{
			get { return getValue<long>("LastHourCount"); }
			set { setValue<long>("LastHourCount", value); }
		}

		[Data(DefaultValue = 0, MustPersist = false)]
		public decimal LastHourSum
		{
			get { return getValue<decimal>("LastHourSum"); }
			set { setValue<decimal>("LastHourSum", value); }
		}

		[Data(DefaultValue = 0, MustPersist = false)]
		public long LastDayCount
		{
			get { return getValue<long>("LastDayCount"); }
			set { setValue<long>("LastDayCount", value); }
		}

		[Data(DefaultValue = 0, MustPersist = false)]
		public decimal LastDaySum
		{
			get { return getValue<decimal>("LastDaySum"); }
			set { setValue<decimal>("LastDaySum", value); }
		}

		[Data(DefaultValue = 0, MustPersist = false)]
		public long LastWeekCount
		{
			get { return getValue<long>("LastWeekCount"); }
			set { setValue<long>("LastWeekCount", value); }
		}

		[Data(DefaultValue = 0, MustPersist = false)]
		public decimal LastWeekSum
		{
			get { return getValue<decimal>("LastWeekSum"); }
			set { setValue<decimal>("LastWeekSum", value); }
		}

		[Data(DefaultValue = 0, MustPersist = false)]
		public long LastMonthCount
		{
			get { return getValue<long>("LastMonthCount"); }
			set { setValue<long>("LastMonthCount", value); }
		}

		[Data(DefaultValue = 0, MustPersist = false)]
		public decimal LastMonthSum
		{
			get { return getValue<decimal>("LastMonthSum"); }
			set { setValue<decimal>("LastMonthSum", value); }
		}

		[Data(DefaultValue = 0, MustPersist = false)]
		public long LastQuarterCount
		{
			get { return getValue<long>("LastQuarterCount"); }
			set { setValue<long>("LastQuarterCount", value); }
		}

		[Data(DefaultValue = 0, MustPersist = false)]
		public decimal LastQuarterSum
		{
			get { return getValue<decimal>("LastQuarterSum"); }
			set { setValue<decimal>("LastQuarterSum", value); }
		}

		[Data(DefaultValue = 0, MustPersist = false)]
		public long LastYearCount
		{
			get { return getValue<long>("LastYearCount"); }
			set { setValue<long>("LastYearCount", value); }
		}

		[Data(DefaultValue = 0, MustPersist = false)]
		public decimal LastYearSum
		{
			get { return getValue<decimal>("LastYearSum"); }
			set { setValue<decimal>("LastYearSum", value); }
		}

	}

	[DataDefinition(PhysicalName = "TraceEventsTraceRequest")]
	public class TraceRequest : Entity, IDataWrapper
	{
		public static partial class Fields
		{
			public const string Id = "Id";
			public const string DateCreated = "DateCreated";
			public const string Storage = "Storage";
			public const string Request = "Request";
			public const string Result = "Result";
			public const string TimeSpan = "TimeSpan";
		}

		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[Data(IsPrimaryKey=true)]
		public Guid Id
		{
			get { return getValue<Guid>("Id"); }
			set { setValue<Guid>("Id", value); }
		}

		[Data]
		public DateTime DateCreated
		{
			get { return getValue<DateTime>("DateCreated"); }
			set { setValue<DateTime>("DateCreated", value); }
		}

		[Data(DefaultValue = "")]
		public string Storage
		{
			get { return getValue<string>("Storage"); }
			set { setValue<string>("Storage", value); }
		}

		[Data(DefaultValue = "")]
		public string Request
		{
			get { return getValue<string>("Request"); }
			set { setValue<string>("Request", value); }
		}

		[Data(DefaultValue = "")]
		public string Result
		{
			get { return getValue<string>("Result"); }
			set { setValue<string>("Result", value); }
		}

		[Data]
		public long TimeSpan
		{
			get { return getValue<long>("TimeSpan"); }
			set { setValue<long>("TimeSpan", value); }
		}

	}

	[DataDefinition(PhysicalName = "TraceEventsWho")]
	public class Who : Entity, IDataWrapper
	{
		public static partial class Fields
		{
			public const string Id = "Id";
			public const string DateFirstTrace = "DateFirstTrace";
			public const string DateLastTrace = "DateLastTrace";
		}

		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[Data(IsPrimaryKey = true)]
		public string Id
		{
			get { return getValue<string>("Id"); }
			set { setValue<string>("Id", value); }
		}

		[Data]
		public DateTime DateFirstTrace
		{
			get { return getValue<DateTime>("DateFirstTrace"); }
			set { setValue<DateTime>("DateFirstTrace", value); }
		}

		[Data]
		public DateTime DateLastTrace
		{
			get { return getValue<DateTime>("DateLastTrace"); }
			set { setValue<DateTime>("DateLastTrace", value); }
		}

	}

	[DataDefinition(PhysicalName = "TraceEventsRoot")]
	public class Root : Entity, IDataWrapper
	{
		public static partial class Fields
		{
			public const string Id = "Id";
			public const string Now = "Now";
		}

		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[Data(IsPrimaryKey=true)]
		public Guid Id
		{
			get { return getValue<Guid>("Id"); }
			set { setValue<Guid>("Id", value); }
		}

		[Data(MustPersist = false)]
		public DateTime Now
		{
			get { return getValue<DateTime>("Now"); }
			set { setValue<DateTime>("Now", value); }
		}

		public List<Events> Events
		{
			get { return this.GetAssociatedInstances<Events, RootEvents>(); }
		}

	}

	[DataDefinition]
	[RelationPersistenceMode(SeparateTable = false)]
	public class EventsTrace : DataWrapper, IDataWrapper, IRelation
	{
		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[RelationEnd(Type = typeof(Events), Role = typeof(Events), Multiplicity = Multiplicity.One, FkNames = "EventsId")]
		public IEntity Events;

		[RelationEnd(Type = typeof(Trace), Role = typeof(Trace), Multiplicity = Multiplicity.ZeroOrMany)]
		public IEntity Trace;

	}

	[DataDefinition]
	[RelationPersistenceMode(SeparateTable = false)]
	public class TraceYear : DataWrapper, IDataWrapper, IRelation
	{
		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[RelationEnd(Type = typeof(Year), Role = typeof(Year), Multiplicity = Multiplicity.One, FkNames = "YearId", SortColumn = "IndexKey")]
		public IEntity Year;

		[RelationEnd(Type = typeof(Trace), Role = typeof(Trace), Multiplicity = Multiplicity.ZeroOrMany)]
		public IEntity Trace;

	}

	[DataDefinition]
	[RelationPersistenceMode(SeparateTable = false)]
	public class TraceMonth : DataWrapper, IDataWrapper, IRelation
	{
		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[RelationEnd(Type = typeof(Month), Role = typeof(Month), Multiplicity = Multiplicity.One, FkNames = "MonthId", SortColumn = "IndexKey")]
		public IEntity Month;

		[RelationEnd(Type = typeof(Trace), Role = typeof(Trace), Multiplicity = Multiplicity.ZeroOrMany)]
		public IEntity Trace;

	}

	[DataDefinition]
	[RelationPersistenceMode(SeparateTable = false)]
	public class TraceDay : DataWrapper, IDataWrapper, IRelation
	{
		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[RelationEnd(Type = typeof(Day), Role = typeof(Day), Multiplicity = Multiplicity.One, FkNames = "DayId", SortColumn = "IndexKey")]
		public IEntity Day;

		[RelationEnd(Type = typeof(Trace), Role = typeof(Trace), Multiplicity = Multiplicity.ZeroOrMany)]
		public IEntity Trace;

	}

	[DataDefinition]
	[RelationPersistenceMode(SeparateTable = false)]
	public class TraceHour : DataWrapper, IDataWrapper, IRelation
	{
		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[RelationEnd(Type = typeof(Hour), Role = typeof(Hour), Multiplicity = Multiplicity.One, FkNames = "HourId", SortColumn = "IndexKey")]
		public IEntity Hour;

		[RelationEnd(Type = typeof(Trace), Role = typeof(Trace), Multiplicity = Multiplicity.ZeroOrMany)]
		public IEntity Trace;

	}

	[DataDefinition]
	[RelationPersistenceMode(SeparateTable = false)]
	public class TraceMinute : DataWrapper, IDataWrapper, IRelation
	{
		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[RelationEnd(Type = typeof(Trace), Role = typeof(Trace), Multiplicity = Multiplicity.ZeroOrMany)]
		public IEntity Trace;

		[RelationEnd(Type = typeof(Minute), Role = typeof(Minute), Multiplicity = Multiplicity.One, FkNames = "MinuteId")]
		public IEntity Minute;

	}

	[DataDefinition(MustPersist = false)]
	[RelationPersistenceMode(SeparateTable = false)]
	public class EventsMinute : DataWrapper, IDataWrapper, IRelation
	{
		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[RelationEnd(Type = typeof(Minute), Role = typeof(Minute), Multiplicity = Multiplicity.ZeroOrMany)]
		public IEntity Minute;

		[RelationEnd(Type = typeof(Events), Role = typeof(Events), Multiplicity = Multiplicity.One, FkNames = "TagId", SortColumn = "TimeKey")]
		public IEntity Events;

	}

	[DataDefinition(MustPersist = false)]
	[RelationPersistenceMode(SeparateTable = false)]
	public class EventsDay : DataWrapper, IDataWrapper, IRelation
	{
		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[RelationEnd(Type = typeof(Day), Role = typeof(Day), Multiplicity = Multiplicity.ZeroOrMany)]
		public IEntity Day;

		[RelationEnd(Type = typeof(Events), Role = typeof(Events), Multiplicity = Multiplicity.One, FkNames = "TagId", SortColumn = "TimeKey")]
		public IEntity Events;

	}

	[DataDefinition(MustPersist = false)]
	[RelationPersistenceMode(SeparateTable = false)]
	public class EventsMonth : DataWrapper, IDataWrapper, IRelation
	{
		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[RelationEnd(Type = typeof(Month), Role = typeof(Month), Multiplicity = Multiplicity.ZeroOrMany)]
		public IEntity Month;

		[RelationEnd(Type = typeof(Events), Role = typeof(Events), Multiplicity = Multiplicity.One, FkNames = "TagId", SortColumn = "TimeKey")]
		public IEntity Events;

	}

	[DataDefinition(MustPersist = false)]
	[RelationPersistenceMode(SeparateTable = false)]
	public class EventsHour : DataWrapper, IDataWrapper, IRelation
	{
		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[RelationEnd(Type = typeof(Hour), Role = typeof(Hour), Multiplicity = Multiplicity.ZeroOrMany)]
		public IEntity Hour;

		[RelationEnd(Type = typeof(Events), Role = typeof(Events), Multiplicity = Multiplicity.One, FkNames = "TagId", SortColumn = "TimeKey")]
		public IEntity Events;

	}

	[DataDefinition(MustPersist = false)]
	[RelationPersistenceMode(SeparateTable = false)]
	public class EventsYear : DataWrapper, IDataWrapper, IRelation
	{
		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[RelationEnd(Type = typeof(Events), Role = typeof(Events), Multiplicity = Multiplicity.One, FkNames = "TagId", SortColumn = "TimeKey")]
		public IEntity Events;

		[RelationEnd(Type = typeof(Year), Role = typeof(Year), Multiplicity = Multiplicity.ZeroOrMany)]
		public IEntity Year;

	}

	[DataDefinition]
	[RelationPersistenceMode(SeparateTable = false)]
	public class TraceWeek : DataWrapper, IDataWrapper, IRelation
	{
		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[RelationEnd(Type = typeof(Week), Role = typeof(Week), Multiplicity = Multiplicity.One, FkNames = "WeekId")]
		public IEntity Week;

		[RelationEnd(Type = typeof(Trace), Role = typeof(Trace), Multiplicity = Multiplicity.ZeroOrMany)]
		public IEntity Trace;

	}

	[DataDefinition(MustPersist = false)]
	[RelationPersistenceMode(SeparateTable = false)]
	public class EventsWeek : DataWrapper, IDataWrapper, IRelation
	{
		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[RelationEnd(Type = typeof(Events), Role = typeof(Events), Multiplicity = Multiplicity.One, FkNames = "TagId", SortColumn = "TimeKey")]
		public IEntity Events;

		[RelationEnd(Type = typeof(Week), Role = typeof(Week), Multiplicity = Multiplicity.ZeroOrMany)]
		public IEntity Week;

	}

	[DataDefinition(MustPersist = false)]
	[RelationPersistenceMode(SeparateTable = false)]
	public class EventsQuarter : DataWrapper, IDataWrapper, IRelation
	{
		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[RelationEnd(Type = typeof(Quarter), Role = typeof(Quarter), Multiplicity = Multiplicity.ZeroOrMany)]
		public IEntity Quarter;

		[RelationEnd(Type = typeof(Events), Role = typeof(Events), Multiplicity = Multiplicity.One, FkNames = "TagId", SortColumn = "TimeKey")]
		public IEntity Events;

	}

	[DataDefinition]
	[RelationPersistenceMode(SeparateTable = false)]
	public class TraceQuarter : DataWrapper, IDataWrapper, IRelation
	{
		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[RelationEnd(Type = typeof(Quarter), Role = typeof(Quarter), Multiplicity = Multiplicity.One, FkNames = "QuarterId")]
		public IEntity Quarter;

		[RelationEnd(Type = typeof(Trace), Role = typeof(Trace), Multiplicity = Multiplicity.ZeroOrMany)]
		public IEntity Trace;

	}

	[DataDefinition(MustPersist = false)]
	public class EventsStat : DataWrapper, IDataWrapper, IRelation
	{
		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[RelationEnd(Type = typeof(Stat), Role = typeof(Stat), Multiplicity = Multiplicity.One)]
		public IEntity Stat;

		[RelationEnd(Type = typeof(Events), Role = typeof(Events), Multiplicity = Multiplicity.ZeroOrOne)]
		public IEntity Events;

	}

	[DataDefinition]
	[RelationPersistenceMode(SeparateTable = false)]
	public class WhoTrace : DataWrapper, IDataWrapper, IRelation
	{
		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[RelationEnd(Type = typeof(Who), Role = typeof(Who), Multiplicity = Multiplicity.ZeroOrOne, FkNames = "WhoId", SortColumn = "DateTrace")]
		public IEntity Who;

		[RelationEnd(Type = typeof(Trace), Role = typeof(Trace), Multiplicity = Multiplicity.ZeroOrMany)]
		public IEntity Trace;

	}

	[DataDefinition]
	[RelationPersistenceMode(SeparateTable = false)]
	public class RootWho : DataWrapper, IDataWrapper, IRelation
	{
		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[RelationEnd(Type = typeof(Root), Role = typeof(Root), Multiplicity = Multiplicity.One, FkNames = "RootId")]
		public IEntity Root;

		[RelationEnd(Type = typeof(Who), Role = typeof(Who), Multiplicity = Multiplicity.ZeroOrMany)]
		public IEntity Who;

	}

	[DataDefinition]
	[RelationPersistenceMode(SeparateTable = false)]
	public class RootTrace : DataWrapper, IDataWrapper, IRelation
	{
		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[RelationEnd(Type = typeof(Root), Role = typeof(Root), Multiplicity = Multiplicity.One, FkNames = "RootId", SortColumn = "DateTrace")]
		public IEntity Root;

		[RelationEnd(Type = typeof(Trace), Role = typeof(Trace), Multiplicity = Multiplicity.ZeroOrMany)]
		public IEntity Trace;

	}

	[DataDefinition]
	[RelationPersistenceMode(SeparateTable = false)]
	public class RootEvents : DataWrapper, IDataWrapper, IRelation
	{
		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[RelationEnd(Type = typeof(Events), Role = typeof(Events), Multiplicity = Multiplicity.ZeroOrMany)]
		public IEntity Events;

		[RelationEnd(Type = typeof(Root), Role = typeof(Root), Multiplicity = Multiplicity.One, FkNames = "RootId", SortColumn = "Name")]
		public IEntity Root;

	}

}


  
