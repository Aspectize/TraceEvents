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
        object Events(string events, string value, Guid? whoId, string longitude, string latitude, string info);

        DataSet LoadTraces();
        DataSet GetTrace(Date dateStart, Date dateEnd);
        DataSet GetTagHistory(Guid tagId, Date dateStart, Date dateEnd, EnumFrequency frequency, DateTime lastDateTraceAccount);
 
    }

    [Service(Name = "TraceEventsConfigurableService", ConfigurationRequired = true)]
    public class TraceService : ITrace, IInitializable//, ISingleton
    {
        internal static string ExtractFormatDateYear ="yyyy";
        internal static string ExtractFormatDateQuarter = "yyyyQ";
        internal static string ExtractFormatDateMonth = "yyyyQMM";
        internal static string ExtractFormatDateWeek = "yyyyQMMWW";
        internal static string ExtractFormatDateDay = "yyyyQMMWWdd";
        internal static string ExtractFormatDateHour = "yyyyQMMWWddHH";
        internal static string ExtractFormatDateMinute = "yyyyQMMWWddHHmm";
        internal static string ExtractFormatDateFull = "yyyyQMMWWddHHmmss";

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
            roleRelationQuerys.Add(new RoleRelationQuery<Trace, TagTrace>());

            dm.LoadEntitiesGraph<Root>(roleRelationQuerys, root.Id);

            var now = DateTime.UtcNow;
            foreach (Tag tag in em.GetAllInstances<Tag>())
            {
                BuildTagStat(dm, root.Id, tag, now);
            }

            return dm.Data;
        }

        internal static void BuildTagStat(IDataManager dm, Guid rootId, Tag tag, DateTime dateTime)
        {
            IEntityManager em = dm as IEntityManager;

            TagStat stat = em.CreateInstance<TagStat>();

            stat.Id = tag.Id;

            em.AssociateInstance<TagStatTag>(tag, stat);

            string timeTagKey = TraceService.GetTimeTagKey(rootId, tag.Id, dateTime);

            BuildTagStat<Year, TagYear>(dm, stat, timeTagKey, TraceService.ExtractFormatDateYear);
            BuildTagStat<Quarter, TagQuarter>(dm, stat, timeTagKey, TraceService.ExtractFormatDateQuarter);
            BuildTagStat<Month, TagMonth>(dm, stat, timeTagKey, TraceService.ExtractFormatDateMonth);
            BuildTagStat<Week, TagWeek>(dm, stat, timeTagKey, TraceService.ExtractFormatDateWeek);
            BuildTagStat<Day, TagDay>(dm, stat, timeTagKey, TraceService.ExtractFormatDateDay);
            BuildTagStat<Hour, TagHour>(dm, stat, timeTagKey, TraceService.ExtractFormatDateHour);
            //BuildTagStat<Minute, TagMinute>(dm, stat, timeTagKey, TraceService.ExtractFormatDateMinute);
        }

        internal static void BuildTagStat<T, R>(IDataManager dm, TagStat tag, string timeTagKey, string datePeriodFormat)
            where T : Entity, IEntity, IDataWrapper, new()
            where R : DataWrapper, IDataWrapper, IRelation, new()
        {
            IEntityManager em = dm as IEntityManager;

            string periodId = TraceService.GetTimeIdFromTimeTagKey(timeTagKey, datePeriodFormat);

            T t = dm.GetEntity<T>(periodId);

            if (t != null)
            {
                //em.AssociateInstance<R>(t, tag);

                string periodName = typeof(T).Name;

                string tagColumnName = string.Format("Last{0}Count", periodName);
                string periodColumnName = string.Format("Count{0}", periodName);
                tag.data[tagColumnName] = t.data[periodColumnName];

                tagColumnName = string.Format("Last{0}Sum", periodName);
                periodColumnName = string.Format("Sum{0}", periodName);
                tag.data[tagColumnName] = t.data[periodColumnName];

            }
        }


        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        DataSet ITrace.GetTrace(Date dateStart, Date dateEnd)
        {
            IDataManager dm = EntityManager.FromDataBaseService(DataServiceName);

            var root = GetRoot(dm, true);

            IEntityManager em = dm as IEntityManager;

            TimeSpan timeSpan = dateEnd - dateStart;

            foreach (Tag tag in em.GetAllInstances<Tag>())
            {
                string dayStart = TraceService.GetTimeTagKey(root.Id, tag.Id, dateStart);//.Substring(0, 11).PadRight(6, '0');
                string dayEnd = TraceService.GetTimeTagKey(root.Id, tag.Id, dateEnd.DateTime.AddDays(1)); //.Substring(0, 11).PadRight(6, '0');
                QueryCriteria qcDay = new QueryCriteria(Day.Fields.Id, ComparisonOperator.GreaterOrEquals, dayStart);
                qcDay = qcDay.AND(new QueryCriteria(Day.Fields.Id, ComparisonOperator.LessOrEquals, dayEnd));

                List<Day> tagDays = dm.GetEntities<Day>(qcDay);

                foreach (Day d in tagDays)
                {
                    em.AssociateInstance<TagDay>(tag, d);
                }
            }

            dm.Data.AcceptChanges();

            return dm.Data;
        }

        DataSet ITrace.GetTagHistory(Guid tagId, Date dateStart, Date dateEnd, EnumFrequency frequency, DateTime lastDateTraceAccount)
        {
            IDataManager dm = EntityManager.FromDataBaseService(DataServiceName);

            var root = GetRoot(dm, false);

            IEntityManager em = dm as IEntityManager;

            DateTime dateEndTomorrow = dateEnd.DateTime.AddDays(1);

            Tag tag = dm.GetEntity<Tag>(tagId);

            string dayStart = TraceService.GetTimeTagKey(root.Id, tag.Id, dateStart);
            string dayEnd = TraceService.GetTimeTagKey(root.Id, tag.Id, dateEndTomorrow);

            string weekStart = TraceService.GetTimeIdFromTimeTagKey(dayStart, TraceService.ExtractFormatDateWeek);
            string weekEnd = TraceService.GetTimeIdFromTimeTagKey(dayEnd, TraceService.ExtractFormatDateWeek);

            string monthStart = TraceService.GetTimeIdFromTimeTagKey(dayStart, TraceService.ExtractFormatDateMonth);
            string monthEnd = TraceService.GetTimeIdFromTimeTagKey(dayEnd, TraceService.ExtractFormatDateMonth);

            QueryCriteria qcMonth = new QueryCriteria(Month.Fields.Id, ComparisonOperator.GreaterOrEquals, monthStart);
            qcMonth = qcMonth.AND(new QueryCriteria(Month.Fields.Id, ComparisonOperator.LessOrEquals, monthEnd));

            List<Month> tagMonths = dm.GetEntities<Month>(qcMonth);

            foreach (Month m in tagMonths)
            {
                em.AssociateInstance<TagMonth>(tag, m);
            }

            QueryCriteria qcWeek = new QueryCriteria(Month.Fields.Id, ComparisonOperator.GreaterOrEquals, weekStart);
            qcWeek = qcWeek.AND(new QueryCriteria(Month.Fields.Id, ComparisonOperator.LessOrEquals, weekEnd));

            List<Week> tagWeeks = dm.GetEntities<Week>(qcWeek);

            foreach (Week w in tagWeeks)
            {
                em.AssociateInstance<TagWeek>(tag, w);
            }

            QueryCriteria qcDay = new QueryCriteria(Day.Fields.Id, ComparisonOperator.GreaterOrEquals, dayStart);
            qcDay = qcDay.AND(new QueryCriteria(Day.Fields.Id, ComparisonOperator.LessOrEquals, dayEnd));

            List<Day> tagDays = dm.GetEntities<Day>(qcDay);

            foreach (Day d in tagDays)
            {
                em.AssociateInstance<TagDay>(tag, d);
            }

            DateTime temp = dateStart.DateTime;

            TimeSpan ts = new Date(dateEndTomorrow) - dateStart;

            for (var i = 0; i < ts.Days; i++)
            {
                string tempDayId = TraceService.GetTimeTagKey(root.Id, tagId, temp);

                if (!tag.Day.Exists(item => item.Id == tempDayId))
                {
                    Day tempTagDay = em.CreateInstance<Day>();
                    tempTagDay.Id = tempDayId;
                    tempTagDay.TimeKey = TraceService.GetTimeKey(temp);
                    tempTagDay.DatePeriod = temp.AddHours(12);
                    em.AssociateInstance<TagDay>(tag, tempTagDay);
                }

                string tempWeekId = TraceService.GetTimeIdFromTimeTagKey(tempDayId, TraceService.ExtractFormatDateWeek);

                if (!tag.Week.Exists(item => item.Id == tempWeekId))
                {
                    Week tempTagWeek = em.CreateInstance<Week>();
                    tempTagWeek.Id = tempWeekId;
                    tempTagWeek.TimeKey = TraceService.GetTimeKey(temp);
                    tempTagWeek.DatePeriod = temp.AddHours(12);
                    em.AssociateInstance<TagWeek>(tag, tempTagWeek);
                }

                string tempMonthId = TraceService.GetTimeIdFromTimeTagKey(tempDayId, TraceService.ExtractFormatDateMonth);

                if (!tag.Month.Exists(item => item.Id == tempMonthId))
                {
                    Month tempTagMonth = em.CreateInstance<Month>();
                    tempTagMonth.Id = tempMonthId;
                    tempTagMonth.TimeKey = TraceService.GetTimeKey(temp);
                    tempTagMonth.DatePeriod = temp.AddHours(12);
                    em.AssociateInstance<TagMonth>(tag, tempTagMonth);
                }

                temp = temp.AddDays(1);
            }

            dm.Data.AcceptChanges();

            return dm.Data;
        }


        object ITrace.Events(string events, string value, Guid ? whoId, string longitude, string latitude, string info)
        {
            IDataManager dm = EntityManager.FromDataBaseService(DataServiceName);

            IEntityManager em = dm as IEntityManager;

            DateTime traceDate = DateTime.UtcNow;

            //List<IRoleRelationQuery> roleRelationQuerys = new List<IRoleRelationQuery>();

            //roleRelationQuerys.Add(new RoleRelationQuery<Key, AccountKey>());
            //roleRelationQuerys.Add(new RoleRelationQuery<Account, AccountTag>());

            //dm.LoadEntitiesGraph<Key>(roleRelationQuerys, accountKey);

            //Key key = em.GetInstance<Key>(accountKey);

            //if (key != null)
            //{
            //    Account account = key.Account;

            //    if (account != null)
            //    {
            //        if (!account.DateFirstTrace.HasValue)
            //        {
            //            account.DateFirstTrace = traceDate;
            //        }

            //        account.DateLastTrace = traceDate;

            //        if (!key.Disable)
            //        {
                        decimal? decimalValue = null;

                        if (!string.IsNullOrEmpty(value))
                        {
                            decimal decimalValueNonNull;
                            if (decimal.TryParse(value, out decimalValueNonNull))
                            {
                                decimalValue = decimalValueNonNull;
                            }
                            //else
                            //{
                            //    error = BuildAccountError(dm, account, string.Format("Value {0} is not in correct format", values));
                            //}
                        }

                        //if (error == null)
                        //{

                        Root root = GetRoot(dm, true);

                        Who who = null;

                        if (!whoId.HasValue)
                        {
                            whoId = Guid.Empty;
                        }
                            who = dm.GetEntity<Who>(whoId.Value);

                            if (who == null)
                            {
                                who = em.CreateInstance<Who>();

                                who.Id = whoId.Value;

                                em.AssociateInstance<RootWho>(who, root);
                            }

                            who.DateLastTrace = DateTime.UtcNow;
                        
                            string[] eventslist = events.Split('|');

                            string dateKey = GetTimeKey(traceDate);

                            foreach (string eventName in eventslist)
                            {
                                BuildTrace(root, events, decimalValue, who, null, null, info, dm, traceDate, dateKey, eventName.Trim());
                            }
                        //}
            //        }
            //        else
            //        {
            //            error = BuildAccountError(dm, account, string.Format("Key {0} is disabled", accountKey));
            //        }
            //    }
            //    else
            //    {
            //        error = BuildAccountError(dm, null, string.Format("Account {0} is unknown", accountKey));
            //    }
            //}
            //else
            //{
            //    error = BuildAccountError(dm, null, string.Format("Key {0} is unknown", accountKey));
            //}

            dm.SaveTransactional();

            Dictionary<string, object> result = new Dictionary<string, object>();

            //if (error != null)
            //{
            //    result.Add("trace", "error");
            //    result.Add("errors", error.Description);
            //}
            //else
            //{
            //    result.Add("trace", "success");
            //}

            return result;
        }

        internal static string GetTimeIdFromTimeTagKey(string tagTimeTagKey, string extractFormat)
        {
            string[] parts = tagTimeTagKey.Split('|');

            parts[2] = parts[2].Substring(0, extractFormat.Length).PadRight(ExtractFormatDateFull.Length, '0');

            // Bug pour les Week: un même numéro de Week peut se retrouver sur 2 mois...
            if (extractFormat == ExtractFormatDateWeek)
            {
                parts[2] = parts[2].Substring(0, 4) + "000" + parts[2].Substring(7, 10);
            }

            return string.Join("|", parts);
        }

        internal static string GetTimeTagKey(Guid accountId, Guid tagId, DateTime date)
        {
            return string.Format("{0:N}|{1:N}|{2}", accountId, tagId, GetTimeKey(date));
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

            int weekNum = cul.Calendar.GetWeekOfYear(date, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            
            return weekNum;
        }

        internal static void BuildTrace(Root root, string events, decimal? value, Who who, double? longitude, double? latitude, string info, IDataManager dm, DateTime traceDate, string dateKey, string tagName)
        {
            IEntityManager em = dm as IEntityManager;
            
            Tag tag = root.Tag.Find(item => item.Name == tagName);

            if (tag == null)
            {
                tag = em.CreateInstance<Tag>();

                tag.Name = tagName;

                if (value.HasValue)
                {
                    tag.ValueType = EnumTagValueType.Summarize;
                }

                em.AssociateInstance<RootTag>(root, tag);
            }

            Trace trace = em.CreateInstance<Trace>();

            trace.DateTrace = traceDate;
            trace.Tags = events;
            trace.Value = value;
            trace.Who = who.Id.ToString("N");
            trace.Longitude = longitude;
            trace.Latitude = latitude;
            trace.Info = info;
            
            if (HttpContext.Current != null && HttpContext.Current.Request != null)
            {
                trace.UserAgent = HttpContext.Current.Request.UserAgent;
            }

            trace.IndexKey = string.Format("{0:N}|{1:N}|{2}", root.Id, tag.Id, dateKey);

            if (value.HasValue)
            {
                tag.SumTag += value.Value;
            }

            int quarter = GetNumQuarter(traceDate);

            //BuildPeriod<Minute, TraceMinute>(dm, traceDate, trace, new DateTime(traceDate.Year, traceDate.Month, traceDate.Day, traceDate.Hour, traceDate.Minute, 0), ExtractFormatDateMinute);
            BuildPeriod<Hour, TraceHour>(dm, traceDate, trace, new DateTime(traceDate.Year, traceDate.Month, traceDate.Day, traceDate.Hour, 0, 0), ExtractFormatDateHour);
            BuildPeriod<Day, TraceDay>(dm, traceDate, trace, new DateTime(traceDate.Year, traceDate.Month, traceDate.Day, 12, 0, 0), ExtractFormatDateDay);

            DayOfWeek dayOfWeek = traceDate.DayOfWeek;
            int dayFromMonday = ((int)dayOfWeek -1) % 7;
            DateTime weekDate = traceDate.AddDays(-dayFromMonday);
            weekDate = weekDate.AddTicks(-(traceDate.Ticks % TimeSpan.TicksPerDay));
            weekDate = weekDate.AddHours(12);

            BuildPeriod<Week, TraceWeek>(dm, traceDate, trace, weekDate, ExtractFormatDateWeek);
            BuildPeriod<Month, TraceMonth>(dm, traceDate, trace, new DateTime(traceDate.Year, traceDate.Month, 1, 12, 0, 0), ExtractFormatDateMonth);
            BuildPeriod<Quarter, TraceQuarter>(dm, traceDate, trace, new DateTime(traceDate.Year, 3 * (quarter - 1) + 1, 1, 12, 0, 0), ExtractFormatDateQuarter);
            BuildPeriod<Year, TraceYear>(dm, traceDate, trace, new DateTime(traceDate.Year, 1, 1, 12, 0, 0), ExtractFormatDateYear);

            em.AssociateInstance<TagTrace>(trace, tag);
            em.AssociateInstance<RootTrace>(root, trace);
            if (who != null)
            {
                em.AssociateInstance<WhoTrace>(who, trace);
            }
            //em.AssociateInstance<AccountTrace>(account, trace);
        }

        internal static void BuildPeriod<T, R>(IDataManager dm, DateTime traceDate, Trace trace, DateTime datePeriod, string datePeriodFormat) 
        where T:Entity, IEntity, IDataWrapper, new()
        where R : DataWrapper, IDataWrapper, IRelation, new()
        {
            IEntityManager em = dm as IEntityManager;

            string timeId = GetTimeIdFromTimeTagKey(trace.IndexKey, datePeriodFormat);

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

        private static Dictionary<string, object> buildReturnDictionary(string key, object o)
        {
            Dictionary<string, object> d = new Dictionary<string, object>();

            d.Add(key, o);

            return d;
        }

        static Root root = null;

        public static Root GetRoot(IDataManager dm, bool withTag)
        {
            IEntityManager em = dm as IEntityManager;

            if (withTag)
            {
                var roleRelations = new List<IRoleRelationQuery>();

                roleRelations.Add(new RoleRelationQuery<Root, RootTag>());

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
    }
}
