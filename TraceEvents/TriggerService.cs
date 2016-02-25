using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Aspectize.Core;

namespace TraceMyApps
{
    public interface ITraceMyAppsTriggerService
    {
        void ToLower(string fieldName);
        void CalcSum(string fieldAggreg);
    }

    [Service(Name = "TraceMyAppsTriggerService")]
    public class TraceMyAppsTriggerService : ITraceMyAppsTriggerService //, IInitializable, ISingleton
    {
        void ITraceMyAppsTriggerService.ToLower(string fieldName)
        {
            DataRow dr = ExecutingContext.DataContext;

            if (dr.Table.Columns.Contains(fieldName))
            {
                dr[fieldName] = dr[fieldName].ToString().ToLower();
            }
        }

        void ITraceMyAppsTriggerService.CalcSum(string fieldAggreg)
        {
            DataRow dr = ExecutingContext.DataContext;

            if (dr.Table.Columns.Contains(fieldAggreg))
            {
                dr[fieldAggreg] = (decimal) dr[fieldAggreg] + (decimal) dr["TempSum"];
            }
        }

    }

}
