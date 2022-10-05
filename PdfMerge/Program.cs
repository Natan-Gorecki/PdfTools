using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using SharedTools;

System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

string usageString = @"Usage: PdfMerge -o OutputFile.pdf File1.pdf File2.pdf ...
Options: 
    --output, -o: Specify output file, this flag is mandatory";

if (args == null || args.Length == 0)
{
    Console.WriteLine(usageString);
    return;
}

try
{
    CommandLineParser commandLine = new CommandLineParser();
    commandLine.Parse(args);

    string outputOption = commandLine.GetOption("--output", "-o");
    string outputFile = GetPath(outputOption);

    if (Path.GetExtension(outputFile) != ".pdf")
    {
        throw new Exception($"Output file {outputFile} must end with .pdf extension");
    }

    PdfDocument outputDocument = new PdfDocument();
    IEnumerable<string> inputFiles = commandLine.OtherArgs;

    foreach(string inputFile in inputFiles)
    {
        string path = GetPath(inputFile);
        
        if (!File.Exists(path))
        {
            throw new Exception($"Invalid file {path}");
        }
        
        if (Path.GetExtension(path) != ".pdf")
        {
            throw new Exception($"Input file {path} doesn't end with .pdf extension");
        }

        PdfDocument inputDocument = PdfReader.Open(path, PdfDocumentOpenMode.Import);
        for (int i = 0; i < inputDocument.PageCount; i++)
        {
            PdfPage page = inputDocument.Pages[i];
            outputDocument.AddPage(page);
        }
    }

    outputDocument.Save(outputFile);
    Console.WriteLine($"Merged {inputFiles.Count()} files as {outputFile}.");
    return;
}
catch(Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    return;
}

static string GetPath(string path)
{
    if (Path.IsPathRooted(path) == false)
    {
        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
    }
    return path;
}