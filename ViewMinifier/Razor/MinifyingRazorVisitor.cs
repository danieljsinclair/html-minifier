using HtmlMinifier;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Razor.Parser;
using System.Web.Razor.Parser.SyntaxTree;
using System.Web.Razor.Text;

namespace Razor
{
    using WebMarkupMin.Core.Minifiers;

    public class MinifyingRazorVisitor : ParserVisitor
    {
        HtmlMinifier minifier = new HtmlMinifier(new WebMarkupMin.Core.Settings.HtmlMinificationSettings { RemoveOptionalEndTags = false });
        Dictionary<string, string> codeParts = new Dictionary<string, string>();
        StringBuilder sb = new StringBuilder();
        public MarkupMinificationResult minifiedResult { get; private set; }
        string rebuiltStr = null;
        int numCodeParts = 0;

        public bool _showInput;
        public bool _generateStatistics;
        public MinifyingRazorVisitor(bool showInput = false, bool generateStatistics = true)
        {
            _showInput = showInput;
            _generateStatistics = generateStatistics;
        }

        const string regexPrefix = "---code-rep-";
        const string regexSuffix = "-code-rep---";

        public override void ThrowIfCanceled()
        {
            base.ThrowIfCanceled();
        }
        public override void VisitSpan(Span span)
        {
            switch (span.Kind)
            {
                case SpanKind.Comment:
                case SpanKind.Transition:
                case SpanKind.MetaCode:
                case SpanKind.Code:
                    // strip all code blocks and convert to pure HTML prior to minification
                    var codeInsertionTag = String.Format("{0}{1}{2}", regexPrefix, numCodeParts, regexSuffix);
                    sb.Append(codeInsertionTag);
                    codeParts.Add(codeInsertionTag, span.Content);
                    if (_showInput)
                        using (new ConsoleColour(ConsoleColor.Yellow))
                            Console.Write(span.Content);
                    numCodeParts++;
                    break;

                case SpanKind.Markup:
                    if (_showInput)
                        using (new ConsoleColour(ConsoleColor.Blue))
                            Console.Write(span.Content);
                    sb.Append(span.Content);
                    break;
            }
            base.VisitSpan(span);
        }

        public override string ToString()
        {
            return rebuiltStr;
        }

        public override void OnComplete()
        {
            if (_showInput)
                Console.WriteLine();
            var strSanitized = sb.ToString();
            minifiedResult = minifier.Minify(strSanitized, _generateStatistics);
            var minified = minifiedResult.MinifiedContent;
            if (codeParts.Count > 0)
                rebuiltStr = minified.MultiReplace(codeParts);
            else
                rebuiltStr = minified;
            base.OnComplete();
        }
        public override void VisitError(RazorError err)
        {
            base.VisitError(err);
        }

    }//class MinifiyingRazorVisitor

}
