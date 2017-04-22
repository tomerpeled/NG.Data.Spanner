using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;

namespace NG.Data.Spanner.EF.Storage.Internal
{
    public class SpannerCommandBuilderFactory: RelationalCommandBuilderFactory
    {
        private readonly ISensitiveDataLogger _logger;
        private readonly DiagnosticSource _diagnosticSource;
        private readonly IRelationalTypeMapper _typeMapper;

        public SpannerCommandBuilderFactory(ISensitiveDataLogger<IRelationalCommandBuilderFactory> logger, DiagnosticSource diagnosticSource, IRelationalTypeMapper typeMapper) : base(logger, diagnosticSource, typeMapper)
        {
            _logger = logger;
            _diagnosticSource = diagnosticSource;
            _typeMapper = typeMapper;
        }

        public override IRelationalCommandBuilder Create()
        {
            return new SpannerCommandBuilder(
                _logger,
                _diagnosticSource,
                _typeMapper);
        }
    }
}
