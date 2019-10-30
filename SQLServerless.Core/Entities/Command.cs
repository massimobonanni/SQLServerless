using System;
using System.Collections.Generic;

namespace SQLServerless.Core.Entities
{
    public class Command
    {
        public string StoredName { get; set; }

        public Dictionary<string, object> Parameters { get; set; }

        #region Constructors

        private Command()
        {
            this.Parameters = new Dictionary<string, object>();
        }

        public Command(string storedName) : this()
        {
            if (string.IsNullOrWhiteSpace(storedName))
                throw new ArgumentNullException(nameof(storedName));

            this.StoredName = storedName;
        }

        public Command(string storedName, Dictionary<string, object> parameters) : this()
        {
            this.StoredName = storedName;
            if (parameters != null)
            {
                this.Parameters = parameters;
            }
        }

        #endregion

    }
}