using System;
using System.Collections.Generic;
using System.Text;

namespace System.Data.SqlClient
{
    public static class SqlDataReaderExtensions
    {

        public static List<string> GetColumnNames(this SqlDataReader reader)
        {
            if (reader == null)
                throw new NullReferenceException(nameof(reader));

            var columnNames = new List<string>();

            var readerMetadataInfo = reader.GetType().GetProperty("MetaData", Reflection.BindingFlags.Instance |
                Reflection.BindingFlags.NonPublic | Reflection.BindingFlags.Public);
            var readerMetadataObj = readerMetadataInfo.GetValue(reader);

            var readerMetadataArrayInfo = readerMetadataObj.GetType().GetField("_metaDataArray", Reflection.BindingFlags.Instance |
                Reflection.BindingFlags.NonPublic | Reflection.BindingFlags.Public);

            var readerMetadataArrayObj = readerMetadataArrayInfo.GetValue(readerMetadataObj);

            var readerMetadataArray = readerMetadataArrayObj as object[];
            for (int i = 0; i < readerMetadataArray.Length; i++)
            {
                var metadataObj = readerMetadataArray[i];

                var columnInfo = metadataObj.GetType().GetField("column", Reflection.BindingFlags.Instance |
                        Reflection.BindingFlags.NonPublic | Reflection.BindingFlags.Public);
                var columnObj = columnInfo.GetValue(metadataObj);
                var columnName = columnObj.ToString();
                columnNames.Add(columnName);
            }

            return columnNames;
        }
    }
}
