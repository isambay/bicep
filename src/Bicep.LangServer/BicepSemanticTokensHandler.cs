using System;
using System.Threading;
using System.Threading.Tasks;
using Bicep.LanguageServer.CompilationManager;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document.Proposals;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals;

namespace Bicep.LanguageServer
{
    [Obsolete] // proposed LSP feature must be marked 'obsolete' to access
    public class BicepSemanticTokensHandler : SemanticTokensHandlerBase
    {
        private readonly ILogger<BicepSemanticTokensHandler> logger;
        private readonly ICompilationManager compilationManager;

        public BicepSemanticTokensHandler(ILogger<BicepSemanticTokensHandler> logger, ICompilationManager compilationManager)
            : base(GetSemanticTokensRegistrationOptions())
        {
            this.logger = logger;
            this.compilationManager = compilationManager;
        }

        protected override Task<SemanticTokensDocument> GetSemanticTokensDocument(ITextDocumentIdentifierParams @params, CancellationToken cancellationToken)
        {
            return Task.FromResult(new SemanticTokensDocument(GetRegistrationOptions().Legend));
        }

        protected override Task Tokenize(SemanticTokensBuilder builder, ITextDocumentIdentifierParams identifier, CancellationToken cancellationToken)
        {
            var compilationContext = this.compilationManager.GetCompilation(identifier.TextDocument.Uri);

            if (compilationContext != null)
            {
                SemanticTokenVisitor.BuildSemanticTokens(builder, compilationContext);
            }

            return Task.CompletedTask;
        }

        private static SemanticTokensRegistrationOptions GetSemanticTokensRegistrationOptions()
        {
            return new SemanticTokensRegistrationOptions
            {
                DocumentSelector = DocumentSelector.ForLanguage(LanguageServerConstants.LanguageId),
                Legend = new SemanticTokensLegend(),
                Full = new SemanticTokensCapabilityRequestFull
                {
                    Delta = true
                },
                Range = true
            };
        }
    }
}