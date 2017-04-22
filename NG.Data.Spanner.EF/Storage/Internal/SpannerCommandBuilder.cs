using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;

namespace NG.Data.Spanner.EF.Storage.Internal
{
    public class SpannerCommandBuilder : RelationalCommandBuilder, IInfrastructure<IndentedStringBuilder>
    {
        private readonly ISensitiveDataLogger _logger;
        private readonly DiagnosticSource _diagnosticSource;

        private readonly IndentedStringBuilder _commandTextBuilder = new IndentedStringBuilder();

        public SpannerCommandBuilder(ISensitiveDataLogger logger, DiagnosticSource diagnosticSource, IRelationalTypeMapper typeMapper) : base(logger, diagnosticSource, typeMapper)
        {
            _logger = logger;
            _diagnosticSource = diagnosticSource;
            ParameterBuilder = new RelationalParameterBuilder(typeMapper);
        }

        IndentedStringBuilder IInfrastructure<IndentedStringBuilder>.Instance => _commandTextBuilder;

        public override IRelationalParameterBuilder ParameterBuilder { get; }

        public override IRelationalCommand Build()
           => new SpannerRelationalCommand(
               _logger,
               _diagnosticSource,
               _commandTextBuilder.ToString(),
               ParameterBuilder.Parameters);

        public override string ToString() => _commandTextBuilder.ToString();

    }
}
