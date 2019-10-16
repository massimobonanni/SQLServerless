using System.Collections.Generic;

namespace SQLServerless.Core.Entities
{
    public class Command
    {
        public string CommandText { get; set; }

        public Dictionary<string, object> Parameters { get; set; }

        public bool IsCommandText { get; set; }

        #region Constructors

        public Command()
        {
            this.Parameters = new Dictionary<string, object>();
            this.IsCommandText = false;
        }

        public Command(string commandText) : this()
        {
            this.CommandText = commandText;
        }

        public Command(string commandName, Dictionary<string, object> parameters) : this()
        {
            this.CommandText = commandName;
            if (parameters != null)
            {
                this.Parameters = parameters;
            }
        }

        #endregion
       
    }
}