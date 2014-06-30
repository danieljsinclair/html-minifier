using Razor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Razor.Parser;
using System.Web.Razor.Parser.SyntaxTree;
using System.Web.Razor.Text;
using WebMarkupMin.Core.Minifiers;

namespace Razor
{
    class RazorMinifier
    {
        /// <summary>
        /// Returns a fully minified razor file as a string and optionally returns the MarkupMinificationResult for diagnostics and errors. Note
        /// that the minifiedContent property minifierResult output will not be fully translated.
        /// </summary>
        /// <param name="razorFile"></param>
        /// <param name="showOutput"></param>
        /// <param name="showInput"></param>
        /// <param name="minifiedResult">Fully minified razor file</param>
        /// <returns></returns>
        public static string Minify(string razorFile, out MarkupMinificationResult minifiedResult, bool generateStatistics = true, bool showOutput = false, bool showInput = false)
        {
            var textReader = new StringReader(razorFile);
            var textDocument = new BufferingTextReader(textReader);
            var parser = new RazorParser(new CSharpCodeParser(), new HtmlMarkupParser());
            var visitor = new MinifyingRazorVisitor(showInput, generateStatistics: generateStatistics);

            // Process the file
            parser.Parse(textDocument, visitor); // NOTE: for some reason this just hangs waiting for something if we pass it the textReader directly

            // return minifier data struct and final output as string
            var output = visitor.ToString();
            if (showOutput)
            {
                using (new ConsoleColour(ConsoleColor.Gray))
                    Console.WriteLine(output);
            }
            minifiedResult = visitor.minifiedResult;
            return output;
        }
    }
}
