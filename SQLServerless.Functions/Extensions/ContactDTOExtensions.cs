using SQLServerless.Core.Entities;
using SQLServerless.Functions.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace SQLServerless.Functions.Extensions
{
    internal static class ContactDTOExtensions
    {
        public static TableRowData ToRowData(this ContactDTO contact)
        {
            if (contact == null)
                throw new NullReferenceException(nameof(contact));

            var row = new TableRowData();
            row.Add(nameof(contact.FirstName), contact.FirstName);
            row.Add(nameof(contact.LastName), contact.LastName);
            row.Add(nameof(contact.Email), contact.Email);
            row.Add(nameof(contact.BirthDate), contact.BirthDate);
            row.Add(nameof(contact.Height), contact.Height);

            return row;
        }

        public static TableData ToTableData(this ContactDTO contact, string tableName)
        {
            if (contact == null)
                throw new NullReferenceException(nameof(contact));
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException(nameof(tableName));

            var table = new TableData()
            {
                TableName = tableName
            };

            table.Rows.Add(contact.ToRowData());

            return table;
        }
    }
}
