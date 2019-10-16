using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using SQLServerless.Extensions.Triggers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.WebJobs
{
    public static class WebJobBuilderExtensions
    {
        public static IWebJobsBuilder UseSQLTrigger(this IWebJobsBuilder builder)
        {
            if (builder == null)
                throw new NullReferenceException(nameof(builder));

            builder.AddExtension<SQLTriggerConfigProvider>();

            return builder;
        }
    }
}
