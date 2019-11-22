using SQLServerless.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public static class ObjectExtensions
    {
        public static RowOperation ToRowOperation(this object source)
        {
            if (source == null)
                throw new NullReferenceException(nameof(source));

            switch (source.ToString().ToLower())
            {
                case "u":
                    return RowOperation.Update;
                case "i":
                    return RowOperation.Insert;
                case "d":
                    return RowOperation.Delete;
                default:
                    return RowOperation.Unknown;
            }
        }
    }
}
