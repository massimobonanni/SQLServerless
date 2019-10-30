using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLServerless.Core.Entities
{
    internal static class CommandExtensions
    {
        public static object GetSqlSafeValue(this Command command, KeyValuePair<string, object> param)
        {
            if (command == null)
                throw new NullReferenceException(nameof(command));

            if (param.Value == null)
            {
                return param.Value;
            }

            if (param.Value.GetType() == typeof(DateTime))
            {
                if (param.Value.Equals(DateTime.MinValue))
                    return DBNull.Value;
                else
                    return param.Value;
            }

            if (param.Value.GetType() == typeof(UInt16)
                || param.Value.GetType() == typeof(UInt32)
                || param.Value.GetType() == typeof(UInt64))
            {
                return param.Value.ToString();
            }
            
            return param.Value;
        }

    }
}
