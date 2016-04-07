using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Aspectize.Core;
using TraceEvents;
using System.Web;
using System.Security.Permissions;

namespace TraceMyApps
{
    public interface ITrace
    {
        void Events(string eventName, string eventValue, string userId, string info);

        DataSet LoadTraces();
        //DataSet GetTrace(Date dateStart, Date dateEnd);
        DataSet GetEventsHistory(Guid eventsId, Date dateStart, Date dateEnd, EnumFrequency frequency);
        //void FillSerie();

    }

    [Service(Name = "TraceEventsConfigurableService", ConfigurationRequired = true)]
    public class TraceService : ITrace, IInitializable
    {
        const string ExtractFormatDateYear = "yyyy";
        const string ExtractFormatDateQuarter = "yyyyQ";
        const string ExtractFormatDateMonth = "yyyyQMM";
        const string ExtractFormatDateWeek = "yyyyQMMWW";
        const string ExtractFormatDateDay = "yyyyQMMWWdd";
        const string ExtractFormatDateHour = "yyyyQMMWWddHH";
        const string ExtractFormatDateMinute = "yyyyQMMWWddHHmm";
        const string ExtractFormatDateFull = "yyyyQMMWWddHHmmss";

        [Parameter(Optional = false)]
        string DataServiceName = "";

        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        DataSet ITrace.LoadTraces()
        {
            IDataManager dm = EntityManager.FromDataBaseService(DataServiceName);

            IEntityManager em = dm as IEntityManager;

            var root = GetRoot(dm, true);

            List<IRoleRelationQuery> roleRelationQuerys = new List<IRoleRelationQuery>();

            roleRelationQuerys.Add(new RoleRelationQuery<Root, RootTrace>(new PaginationContext(100, "LoadTraces")));
            roleRelationQuerys.Add(new RoleRelationQuery<Trace, EventsTrace>());

            dm.LoadEntitiesGraph<Root>(roleRelationQuerys, root.Id);

            var now = DateTime.UtcNow;
            foreach (Events events in em.GetAllInstances<Events>())
            {
                BuildStat(dm, root.Id, events, now);
            }

            return dm.Data;
        }

        internal static void BuildStat(IDataManager dm, Guid rootId, Events events, DateTime dateTime)
        {
            IEntityManager em = dm as IEntityManager;

            Stat stat = em.CreateInstance<Stat>();

            stat.Id = events.Id;

            em.AssociateInstance<EventsStat>(events, stat);

            BuildStat<Year, EventsYear>(dm, rootId, events, stat, dateTime, TraceService.ExtractFormatDateYear);
            BuildStat<Quarter, EventsQuarter>(dm, rootId, events, stat, dateTime, TraceService.ExtractFormatDateQuarter);
            BuildStat<Month, EventsMonth>(dm, rootId, events, stat, dateTime, TraceService.ExtractFormatDateMonth);
            BuildStat<Week, EventsWeek>(dm, rootId, events, stat, dateTime, TraceService.ExtractFormatDateWeek);
            BuildStat<Day, EventsDay>(dm, rootId, events, stat, dateTime, TraceService.ExtractFormatDateDay);
            BuildStat<Hour, EventsHour>(dm, rootId, events, stat, dateTime, TraceService.ExtractFormatDateHour);
            //BuildTagStat<Minute, TagMinute>(dm, rootId, events, stat, dateTime, TraceService.ExtractFormatDateMinute);
        }

        internal static void BuildStat<T, R>(IDataManager dm, Guid rootId, Events events, Stat stat, DateTime dateTime, string datePeriodFormat)
            where T : Entity, IEntity, IDataWrapper, new()
            where R : DataWrapper, IDataWrapper, IRelation, new()
        {
            IEntityManager em = dm as IEntityManager;

            string periodId = TraceService.GetTimeIdFromDate(rootId, events.Id, dateTime, datePeriodFormat);

            T t = dm.GetEntity<T>(periodId);

            if (t != null)
            {
                em.AssociateInstance<R>(t, events);

                string periodName = typeof(T).Name;

                string columnName = string.Format("Last{0}Count", periodName);
                string periodColumnName = string.Format("Count{0}", periodName);
                stat.data[columnName] = t.data[periodColumnName];

                columnName = string.Format("Last{0}Sum", periodName);
                periodColumnName = string.Format("Sum{0}", periodName);
                stat.data[columnName] = t.data[periodColumnName];

            }
        }


        //[PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        //DataSet ITrace.GetTrace(Date dateStart, Date dateEnd)
        //{
        //    IDataManager dm = EntityManager.FromDataBaseService(DataServiceName);

        //    var root = GetRoot(dm, true);

        //    IEntityManager em = dm as IEntityManager;

        //    TimeSpan timeSpan = dateEnd - dateStart;

        //    foreach (Events events in em.GetAllInstances<Events>())
        //    {
        //        string dayStart = TraceService.GetTimeTagKey(root.Id, events.Id, dateStart);//.Substring(0, 11).PadRight(6, '0');
        //        string dayEnd = TraceService.GetTimeTagKey(root.Id, events.Id, dateEnd.DateTime.AddDays(1)); //.Substring(0, 11).PadRight(6, '0');
        //        QueryCriteria qcDay = new QueryCriteria(Day.Fields.Id, ComparisonOperator.GreaterOrEquals, dayStart);
        //        qcDay = qcDay.AND(new QueryCriteria(Day.Fields.Id, ComparisonOperator.LessOrEquals, dayEnd));

        //        List<Day> tagDays = dm.GetEntities<Day>(qcDay);

        //        foreach (Day d in tagDays)
        //        {
        //            em.AssociateInstance<EventsDay>(events, d);
        //        }
        //    }

        //    dm.Data.AcceptChanges();

        //    return dm.Data;
        //}

        DataSet ITrace.GetEventsHistory(Guid eventsId, Date dateStart, Date dateEnd, EnumFrequency frequency)
        {
            IDataManager dm = EntityManager.FromDataBaseService(DataServiceName);

            var root = GetRoot(dm, false);

            IEntityManager em = dm as IEntityManager;

            DateTime dateEndTomorrow = dateEnd.DateTime.AddDays(1);

            Events events = dm.GetEntity<Events>(eventsId);

            string dayStart = TraceService.GetTimeIdFromDate(root.Id, events.Id, dateStart, TraceService.ExtractFormatDateDay);
            string dayEnd = TraceService.GetTimeIdFromDate(root.Id, events.Id, dateEndTomorrow, TraceService.ExtractFormatDateDay);

            string weekStart = TraceService.GetTimeIdFromDate(root.Id, events.Id, dateStart, TraceService.ExtractFormatDateWeek);
            string weekEnd = TraceService.GetTimeIdFromDate(root.Id, events.Id, dateEndTomorrow, TraceService.ExtractFormatDateWeek);

            string monthStart = TraceService.GetTimeIdFromDate(root.Id, events.Id, dateStart, TraceService.ExtractFormatDateMonth);
            string monthEnd = TraceService.GetTimeIdFromDate(root.Id, events.Id, dateEndTomorrow, TraceService.ExtractFormatDateMonth);

            QueryCriteria qcMonth = new QueryCriteria(Month.Fields.Id, ComparisonOperator.GreaterOrEquals, monthStart);
            qcMonth = qcMonth.AND(new QueryCriteria(Month.Fields.Id, ComparisonOperator.LessOrEquals, monthEnd));

            List<Month> tagMonths = dm.GetEntities<Month>(qcMonth);

            foreach (Month m in tagMonths)
            {
                em.AssociateInstance<EventsMonth>(events, m);
            }

            QueryCriteria qcWeek = new QueryCriteria(Month.Fields.Id, ComparisonOperator.GreaterOrEquals, weekStart);
            qcWeek = qcWeek.AND(new QueryCriteria(Month.Fields.Id, ComparisonOperator.LessOrEquals, weekEnd));

            List<Week> tagWeeks = dm.GetEntities<Week>(qcWeek);

            foreach (Week w in tagWeeks)
            {
                em.AssociateInstance<EventsWeek>(events, w);
            }

            QueryCriteria qcDay = new QueryCriteria(Day.Fields.Id, ComparisonOperator.GreaterOrEquals, dayStart);
            qcDay = qcDay.AND(new QueryCriteria(Day.Fields.Id, ComparisonOperator.LessOrEquals, dayEnd));

            List<Day> days = dm.GetEntities<Day>(qcDay);

            foreach (Day d in days)
            {
                em.AssociateInstance<EventsDay>(events, d);
            }

            DateTime temp = dateStart.DateTime;

            TimeSpan ts = new Date(dateEndTomorrow) - dateStart;

            for (var i = 0; i < ts.Days; i++)
            {
                string tempDayId = TraceService.GetTimeIdFromDate(root.Id, events.Id, temp, TraceService.ExtractFormatDateDay);

                if (!events.Day.Exists(item => item.Id == tempDayId))
                {
                    Day tempTagDay = em.CreateInstance<Day>();
                    tempTagDay.Id = tempDayId;
                    tempTagDay.TimeKey = tempDayId.Split('|')[2]; //TraceService.GetTimeKey(temp);
                    tempTagDay.DatePeriod = temp.AddHours(12);
                    em.AssociateInstance<EventsDay>(events, tempTagDay);
                }

                string tempWeekId = TraceService.GetTimeIdFromDate(root.Id, events.Id, temp, TraceService.ExtractFormatDateWeek);

                if (!events.Week.Exists(item => item.Id == tempWeekId))
                {
                    Week tempTagWeek = em.CreateInstance<Week>();
                    tempTagWeek.Id = tempWeekId;
                    tempTagWeek.TimeKey = tempWeekId.Split('|')[2]; //TraceService.GetTimeKey(temp);
                    tempTagWeek.DatePeriod = temp.AddHours(12);
                    em.AssociateInstance<EventsWeek>(events, tempTagWeek);
                }

                string tempMonthId = TraceService.GetTimeIdFromDate(root.Id, events.Id, temp, TraceService.ExtractFormatDateMonth);

                if (!events.Month.Exists(item => item.Id == tempMonthId))
                {
                    Month tempTagMonth = em.CreateInstance<Month>();
                    tempTagMonth.Id = tempMonthId;
                    tempTagMonth.TimeKey = tempMonthId.Split('|')[2]; //TraceService.GetTimeKey(temp);
                    tempTagMonth.DatePeriod = temp.AddHours(12);
                    em.AssociateInstance<EventsMonth>(events, tempTagMonth);
                }

                temp = temp.AddDays(1);
            }

            dm.Data.AcceptChanges();

            return dm.Data;
        }


        void ITrace.Events(string eventName, string eventValue, string userId, string info)
        {
            IDataManager dm = EntityManager.FromDataBaseService(DataServiceName);

            IEntityManager em = dm as IEntityManager;

            DateTime traceDate = DateTime.UtcNow;

            decimal? decimalValue = null;

            if (!string.IsNullOrEmpty(eventValue))
            {
                decimal decimalValueNonNull;
                if (decimal.TryParse(eventValue, out decimalValueNonNull))
                {
                    decimalValue = decimalValueNonNull;
                }
            }

            Root root = GetRoot(dm, true);

            Who who = null;

            if (string.IsNullOrEmpty(userId))
            {
                userId = Guid.Empty.ToString("N");
            }

            who = dm.GetEntity<Who>(userId);

            if (who == null)
            {
                who = em.CreateInstance<Who>();

                who.Id = userId;

                em.AssociateInstance<RootWho>(who, root);
            }

            //who.DateLastTrace = DateTime.UtcNow;

            string[] eventslist = eventName.Split('|');

            foreach (string name in eventslist)
            {
                BuildTrace(root, decimalValue, who, null, null, info, dm, traceDate, name.Trim());
            }

            dm.SaveTransactional();
        }

        internal static string GetTimeIdFromDate(Guid rootId, Guid eventsId, DateTime dateTime, string extractFormat)
        {
            var timeKey = GetTimeKey(dateTime).Substring(0, extractFormat.Length).PadRight(ExtractFormatDateFull.Length, '0');

            // Bug pour les Week: un même numéro de Week peut se retrouver sur 2 mois...
            if (extractFormat == ExtractFormatDateWeek)
            {
                timeKey = timeKey.Substring(0, 4) + "000" + timeKey.Substring(7, 10);
            }

            return string.Format("{0:N}|{1:N}|{2}", rootId, eventsId, timeKey);
        }

        internal static string GetTimeKey(DateTime date)
        {
            int quarter = GetNumQuarter(date);

            int weekNum = GetNumWeek(date);

            return string.Format("{0}{1}{2}{3}{4}{5}{6}{7}", date.ToString("yyyy"), quarter, date.ToString("MM"), weekNum.ToString().PadLeft(2, '0'), date.ToString("dd"), date.ToString("HH"), date.ToString("mm"), date.ToString("ss"));
        }

        internal static int GetNumQuarter(DateTime date)
        {
            int quarter = (int)((date.Month - 1) / 3) + 1;

            return quarter;
        }

        internal static int GetNumWeek(DateTime date)
        {
            System.Globalization.CultureInfo cul = System.Globalization.CultureInfo.InvariantCulture;

            int weekNum = cul.Calendar.GetWeekOfYear(date, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday);

            return weekNum;
        }

        internal static void BuildTrace(Root root, decimal? value, Who who, double? longitude, double? latitude, string info, IDataManager dm, DateTime traceDate, string tagName)
        {
            IEntityManager em = dm as IEntityManager;

            Events existingEvents = root.Events.Find(item => item.Name == tagName);

            if (existingEvents == null)
            {
                existingEvents = em.CreateInstance<Events>();

                existingEvents.Name = tagName;

                if (value.HasValue)
                {
                    existingEvents.ValueType = EnumTagValueType.Summarize;
                }

                em.AssociateInstance<RootEvents>(root, existingEvents);
            }

            Trace trace = em.CreateInstance<Trace>();

            trace.DateTrace = traceDate;
            trace.Value = value;
            trace.Who = (who != null) ? who.Id : "";
            trace.Longitude = longitude;
            trace.Latitude = latitude;
            trace.Info = info;

            if (HttpContext.Current != null && HttpContext.Current.Request != null)
            {
                trace.UserAgent = HttpContext.Current.Request.UserAgent;
            }

            trace.IndexKey = string.Format("{0:N}|{1:N}|{2}", root.Id, existingEvents.Id, GetTimeKey(traceDate)); //GetTimeTagKey(root.Id, existingEvents.Id, traceDate);

            if (value.HasValue)
            {
                existingEvents.SumEvents += value.Value;
            }

            int quarter = GetNumQuarter(traceDate);

            //BuildPeriod<Minute, TraceMinute>(dm, trace, new DateTime(traceDate.Year, traceDate.Month, traceDate.Day, traceDate.Hour, traceDate.Minute, 0), ExtractFormatDateMinute);
            BuildPeriod<Hour, TraceHour>(dm, root.Id, existingEvents.Id, trace, new DateTime(traceDate.Year, traceDate.Month, traceDate.Day, traceDate.Hour, 0, 0), ExtractFormatDateHour);
            BuildPeriod<Day, TraceDay>(dm, root.Id, existingEvents.Id, trace, new DateTime(traceDate.Year, traceDate.Month, traceDate.Day, 12, 0, 0), ExtractFormatDateDay);

            DayOfWeek dayOfWeek = traceDate.DayOfWeek;
            int dayFromMonday = ((int)dayOfWeek - 1) % 7;
            if (dayFromMonday < 0) dayFromMonday += 7;
            DateTime weekDate = traceDate.AddDays(-dayFromMonday);
            weekDate = weekDate.AddTicks(-(traceDate.Ticks % TimeSpan.TicksPerDay));
            weekDate = weekDate.AddHours(12);

            BuildPeriod<Week, TraceWeek>(dm, root.Id, existingEvents.Id, trace, weekDate, ExtractFormatDateWeek);
            BuildPeriod<Month, TraceMonth>(dm, root.Id, existingEvents.Id, trace, new DateTime(traceDate.Year, traceDate.Month, 1, 12, 0, 0), ExtractFormatDateMonth);
            BuildPeriod<Quarter, TraceQuarter>(dm, root.Id, existingEvents.Id, trace, new DateTime(traceDate.Year, 3 * (quarter - 1) + 1, 1, 12, 0, 0), ExtractFormatDateQuarter);
            BuildPeriod<Year, TraceYear>(dm, root.Id, existingEvents.Id, trace, new DateTime(traceDate.Year, 1, 1, 12, 0, 0), ExtractFormatDateYear);

            em.AssociateInstance<EventsTrace>(trace, existingEvents);
            em.AssociateInstance<RootTrace>(root, trace);
            if (who != null)
            {
                em.AssociateInstance<WhoTrace>(who, trace);
            }
        }

        internal static void BuildPeriod<T, R>(IDataManager dm, Guid rootId, Guid eventsId, Trace trace, DateTime datePeriod, string datePeriodFormat)
            where T : Entity, IEntity, IDataWrapper, new()
            where R : DataWrapper, IDataWrapper, IRelation, new()
        {
            IEntityManager em = dm as IEntityManager;

            string timeId = GetTimeIdFromDate(rootId, eventsId, datePeriod, datePeriodFormat);

            T t = em.GetInstance<T>(timeId);

            if (t == null)
            {
                t = dm.GetEntity<T>(timeId);

                if (t == null)
                {
                    t = em.CreateInstance<T>();

                    t.data["Id"] = timeId;
                    t.data["TimeKey"] = timeId.Split('|')[2];
                    t.data["DatePeriod"] = datePeriod;
                }
            }

            em.AssociateInstance<R>(trace, t);

            if (trace.Value != null)
            {
                t.data["Sum" + typeof(T).Name] = (decimal)t.data["Sum" + typeof(T).Name] + trace.Value;
            }
        }

        static Root root = null;

        public static Root GetRoot(IDataManager dm, bool withTag)
        {
            IEntityManager em = dm as IEntityManager;

            if (withTag)
            {
                var roleRelations = new List<IRoleRelationQuery>();

                roleRelations.Add(new RoleRelationQuery<Root, RootEvents>());

                dm.LoadEntitiesGraph<Root>(roleRelations);
            }
            else
            {
                dm.LoadEntities<Root>();
            }

            if (root == null)
            {
                if (em.GetAllInstances<Root>().Count == 0)
                {
                    root = em.CreateInstance<Root>();
                    dm.SaveTransactional();
                }
                else root = em.GetAllInstances<Root>()[0];
            }

            return em.GetAllInstances<Root>()[0];
        }


        void IInitializable.Initialize(Dictionary<string, object> parameters)
        {
            IDataManager dm = EntityManager.FromDataBaseService(DataServiceName);

            GetRoot(dm, false);

            dm.SaveTransactional();
        }

        //void ITrace.FillSerie()
        //{
        //    IDataManager dm = EntityManager.FromDataBaseService(DataServiceName);

        //    IEntityManager em = dm as IEntityManager;

        //    var r = GetRoot(dm, true);

        //    List<Couple<string, bool>> tags = new List<Couple<string, bool>>();

        //    tags.Add(new Couple<string, bool>("Sales", true));
        //    tags.Add(new Couple<string, bool>("Subscription", false));
        //    //tags.Add(new Couple<string, bool>("GetStartedClick", false));
        //    //tags.Add(new Couple<string, bool>("About", false));
        //    //tags.Add(new Couple<string, bool>("Subscribe", false));
        //    //tags.Add(new Couple<string, bool>("Tweet", false));
        //    //tags.Add(new Couple<string, bool>("GooglePlus", false));
        //    //tags.Add(new Couple<string, bool>("AddToBasket", false));
        //    //tags.Add(new Couple<string, bool>("Downloads", false));
        //    //tags.Add(new Couple<string, bool>("Pricing", false));
        //    //tags.Add(new Couple<string, bool>("Documentation", false));
        //    //tags.Add(new Couple<string, bool>("Likes", false));
        //    //tags.Add(new Couple<string, bool>("Votes", false));
        //    //tags.Add(new Couple<string, bool>("Share", false));
        //    //tags.Add(new Couple<string, bool>("Audience", true));

        //    foreach (Couple<string, bool> couple in tags)
        //    {
        //        Random random = new Random();

        //        int nbTagTrace = random.Next(200);

        //        string eventsName = couple.First;
        //        for (var i = 0; i < nbTagTrace; i++)
        //        {
        //            random = new Random();
        //            decimal? value = null;
        //            if (couple.Second)
        //            {
        //                value = random.Next(200);
        //                value = value + ((decimal)random.Next(100) / 100);

        //            }

        //            var day = random.Next(90);
        //            var hour = random.Next(23);
        //            var minute = random.Next(60);

        //            var now = DateTime.UtcNow;
        //            DateTime traceDate = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);

        //            traceDate = traceDate.AddDays(-day);
        //            traceDate = traceDate.AddHours(hour);
        //            traceDate = traceDate.AddMinutes(minute);

        //            TraceService.BuildTrace(r, value, null, null, null, "", dm, traceDate, eventsName);
        //        }

        //        dm.SaveTransactional();
        //    }

        //}
    }
}
