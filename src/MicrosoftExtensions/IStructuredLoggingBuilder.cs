using System;
using System.Collections.Generic;
using System.Text;

namespace Arbee.StructuredLogging.MicrosoftExtensions
{
    public interface IStructuredLoggingBuilder
    {
        IStructuredLoggingBuilder WithJsonConverter(Action<IJsonConverterBuilder> configure);
    }
}
