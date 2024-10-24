using Spire.Pdf.Security;
using Spire.Pdf.Widget;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Spire.Pdf;

namespace GetSignatureCertificate
{
    class Program
    {
        static void Main(string[] args)
        {
            // Check if input file path is provided
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide the input PDF file path as an argument.");
                return;
            }

            string pdfFilePath = args[0];

            // Check if the file exists
            if (!File.Exists(pdfFilePath))
            {
                Console.WriteLine($"The specified PDF file does not exist: {pdfFilePath}");
                return;
            }

            // Create a PdfDocument object
            PdfDocument pdf = new PdfDocument();

            // Load the PDF file
            pdf.LoadFromFile(pdfFilePath);

            // Prepare output file path in the same directory as the input file
            string outputFilePath = Path.Combine(Path.GetDirectoryName(pdfFilePath) ?? "", "output.txt");

            // Create the output file and write validation results
            using (StreamWriter writer = new StreamWriter(outputFilePath))
            {
                // Get a collection of form fields in the PDF file
                PdfFormWidget pdfFormWidget = (PdfFormWidget)pdf.Form;
                PdfFormFieldWidgetCollection pdfFormFieldWidgetCollection = pdfFormWidget.FieldsWidget;

                // Iterate through all fields
                for (int i = 0; i < pdfFormFieldWidgetCollection.Count; i++)
                {
                    // Get the signature fields
                    if (pdfFormFieldWidgetCollection[i] is PdfSignatureFieldWidget)
                    {
                        PdfSignatureFieldWidget signatureFieldWidget = (PdfSignatureFieldWidget)pdfFormFieldWidgetCollection[i];

                        // Get the signatures
                        PdfSignature signature = signatureFieldWidget.Signature;

                        // Verify signatures
                        bool valid = signature.VerifySignature();
                        writer.WriteLine(valid ? "Valid signatures" : "Invalid signatures");

                        // Get the certificate attached to the signature
                        X509Certificate2 certificate = signature.Certificate;
                        if (certificate != null)
                        {
                            // Extract ValidFrom and ValidTo dates
                            DateTime validFrom = certificate.NotBefore;
                            DateTime validTo = certificate.NotAfter;

                            // Print the validity dates to file
                            writer.WriteLine($"Valid From: {validFrom}");
                            writer.WriteLine($"Valid To: {validTo}");

                            // Extract and write certificate subject and issuer
                            writer.WriteLine($"Certificate Subject: {certificate.Subject}");
                            writer.WriteLine($"Certificate Issuer: {certificate.Issuer}");
                        }
                        else
                        {
                            writer.WriteLine("No certificate attached to the signature.");
                        }
                    }
                }
            }

            Console.WriteLine($"Signature validation results written to {outputFilePath}");
        }
    }
}
