using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.Reflection.PortableExecutable;

System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

string usageString = @"Usage: FileMerge -o OutputFile.pdf File1.pdf File2.pdf ...
Options: 
    --output, -o: Specify output file, this flag is mandatory";

if (args == null || args.Length == 0)
{
    Console.WriteLine(usageString);
    return;
}

int oIndex = Array.IndexOf(args, "-o");
int outputIndex = Array.IndexOf(args, "--output");
if (oIndex.IndexFound() && outputIndex.IndexFound())
{
    Console.WriteLine("Error: Duplicated output arguments");
    return;
}

int index = oIndex.IndexFound() ? oIndex : outputIndex;
string outputFile = GetPath(args[index + 1]);

if (Path.GetExtension(outputFile) != ".pdf")
{
    Console.WriteLine($"Error: Output file {outputFile} must end with .pdf extension");
}

var inputFiles = new List<string>();
for (int i = 0; i < args.Length; i++)
{
    string arg = args[i];

    // skip flags
    if (arg.StartsWith("-"))
    {
        i++;
        continue;
    }

    string path = GetPath(arg);
    if (File.Exists(path) == false)
    {
        Console.WriteLine($"Error: Invalid file {path}");
        return;
    }
    if (Path.GetExtension(path) != ".pdf")
    {
        Console.WriteLine($"Error: Input file {path} doesn't end with .pdf extension");
    }

    inputFiles.Add(path);
}

PdfDocument outputDocument = new PdfDocument();

foreach (string file in inputFiles)
{
    PdfDocument inputDocument = PdfReader.Open(file, PdfDocumentOpenMode.Import);

    for (int i = 0; i < inputDocument.PageCount; i++)
    {
        PdfPage page = inputDocument.Pages[i];
        outputDocument.AddPage(page);
    }
}

outputDocument.Save(outputFile);
Console.WriteLine($"Merged {inputFiles.Count} files as {outputFile}.");
return;

string GetPath(string path)
{
    if (Path.IsPathRooted(path) == false)
    {
        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
    }
    return path;
}

public static class Extensions
{
    public static bool IndexFound(this int index)
    {
        return index != -1;
    }
}