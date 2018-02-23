using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SmarterBalanced.SampleItems.Core.Reporting.Models
{
    public class CsvReport
    {
        public IList<string> HeaderColumns { get; set; }
        public IList<Dictionary<string, string>> TableRows { get; set; }

        public void WriteHeader(ref CsvWriter csvWriter)
        {
            foreach (var field in HeaderColumns)
            {
                csvWriter.WriteField(field);
            }
            csvWriter.NextRecord();
        }

        public void WriteBody(ref CsvWriter csvWriter)
        {
            foreach(var item in TableRows)
            {
                WriteRecord(ref csvWriter, item);
            }
        }

        public void WriteRecord(ref CsvWriter csvWriter, Dictionary<string, string> currentRow)
        {
            foreach(var key in HeaderColumns)
            {
                csvWriter.WriteField(currentRow[key]);
            }
            csvWriter.NextRecord();
        }


        public FileStreamResult CsvFileStream()
        {
            var csvStream = new MemoryStream();
            var writer = new StreamWriter(csvStream, System.Text.Encoding.UTF8);
            var csv = new CsvWriter(writer);
            WriteHeader(ref csv);
            WriteBody(ref csv);

            writer.Flush();
            csvStream.Seek(0, SeekOrigin.Begin);

            return new FileStreamResult(csvStream, "text/csv");
        }

    }
}
