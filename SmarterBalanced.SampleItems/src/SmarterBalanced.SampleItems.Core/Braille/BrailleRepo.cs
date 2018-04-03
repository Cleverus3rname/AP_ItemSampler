using System;
using System.Collections.Generic;
using System.Text;
using SmarterBalanced.SampleItems.Dal.Providers.Models;
using System.Collections.Immutable;
using System.Threading.Tasks;
using System.IO;
using CoreFtp;
using System.IO.Compression;
using SmarterBalanced.SampleItems.Dal.Providers;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text.RegularExpressions;

namespace SmarterBalanced.SampleItems.Core.Braille
{
    public class BrailleRepo : IBrailleRepo
    {
        private readonly SampleItemsContext context;
        private readonly ILogger logger;

        public BrailleRepo(SampleItemsContext context, ILoggerFactory loggerFactory)
        {
            this.context = context;
            logger = loggerFactory.CreateLogger<BrailleRepo>();
        }

        private string GetItemRootDirectory(SampleItem item)
        {
            return $"{context.AppSettings.SbBraille.FtpBraillePath}/{item.Subject.Code}/{item.Grade.IndividualGradeToNumString()}";
        }
        private string GetItemFtpDirectory(SampleItem item)
        {
            return $"{GetItemRootDirectory(item)}/item-{item.ItemKey}";
        }

        private string GetPassageFtpDirectory(SampleItem item)
        {
            return $"{GetItemRootDirectory(item)}/stim-{item.AssociatedStimulus.Value}";
        }

        
        private SampleItem GetSampleItem(int bankKey, int itemKey)
        {
            return context.SampleItems.SingleOrDefault(item => item.BankKey == bankKey && item.ItemKey == itemKey);

        }

        public SampleItem GetAssoicatedBrailleItem(SampleItem item)
        {
            var matchingBraille = context.SampleItems
                .FirstOrDefault(s => s.BrailleOnlyItem &&
                    s.CopiedFromItem == item.ItemKey);

            return matchingBraille;
        }

        private ImmutableArray<SampleItem> GetAllAssociatedItems(SampleItem item)
        {
            if (item.BrailleOnlyItem && item.CopiedFromItem.HasValue)
            {
                item = GetSampleItem(item.BankKey, item.CopiedFromItem.Value);
            }

            var brailleItems = GetAssociatedBrailleItems(item);
            var associatedItems = context.GetAssociatedPerformanceItems(item);
            return brailleItems.Union(associatedItems).ToImmutableArray();
        }

        public IList<SampleItem> GetAssociatedBrailleItems(SampleItem item)
        {
            var items = Enumerable.Repeat(item, 1);

            if (item.IsPerformanceItem)
            {
                items = context.GetAssociatedPerformanceItems(item);
            }

            var order = items.Select(i => i.ItemKey).ToList();

            var matchingBraille = context.SampleItems.Where(s => s.BrailleOnlyItem &&
                items.Any(i => i.ItemKey == s.CopiedFromItem));

            var itemNames = items.Where(pt => !matchingBraille.Any(bi => bi.CopiedFromItem == pt.ItemKey))
                .Concat(matchingBraille)
                .OrderBy(i => order.IndexOf(i.CopiedFromItem ?? i.ItemKey))
                .ToList();

            return itemNames;
        }
        public string GetBrailleItemNames(SampleItem item)
        {
            if (item == null)
            {
                return string.Empty;
            }

            var items = GetAssociatedBrailleItems(item);
            var names = items.Select(i => i.ToString());
            string res = string.Join(",", names);

            return res;
        }

        private string GetBrailleTypeFromCode(string code)
        {
            //Codes look like TDS_BT_TYPE except for the no braille code which looks like TDS_BT0
            var bt = code.Split('_');
            if (bt.Length != 3)
            {
                return string.Empty;
            }
            return bt[2];
        }

        public async Task<Dictionary<string, string>> GetBrailleFileNames(
          FtpClient ftpClient,
          IEnumerable<string> baseDirectories,
          string brailleCode)
        {
            string brailleType = GetBrailleTypeFromCode(brailleCode).ToLower();
            Dictionary<string, string> brailleFiles = new Dictionary<string, string>();
            foreach (string directory in baseDirectories)
            {

                try
                {
                    await ftpClient.ChangeWorkingDirectoryAsync(directory);
                }
                catch (CoreFtp.Infrastructure.FtpException)
                {
                    logger.LogError($"Failed to load braille from ftp server for {directory}");
                    continue;
                }

                var files = await ftpClient.ListFilesAsync();
                var fileNames = files.Select(f => f.Name).Where(f => Regex.IsMatch(f, $"(?i){brailleType}"));
                foreach (string file in fileNames)
                {
                    if (!brailleFiles.ContainsKey(file))
                    {
                        brailleFiles.Add(file, $"{directory}/{file}");
                    }
                }
            }
            return brailleFiles;
        }

        public string GenerateBrailleZipName(int itemId, string brailleCode)
        {
            return $"{itemId}-{GetBrailleTypeFromCode(brailleCode)}.zip";
        }

        public async Task<Stream> GetItemBrailleZip(int itemBank, int itemKey, string brailleCode)
        {
            SampleItem item = GetSampleItem(itemBank, itemKey);
            string brailleType = GetBrailleTypeFromCode(brailleCode);
            if (brailleType == string.Empty || item == null)
            {
                throw new ArgumentException("Invalid arguments for item or braille");
            }

            ImmutableArray<string> itemDirectories = GetItemBrailleDirectories(item);

            using (var ftpClient = new FtpClient(new FtpClientConfiguration
            {
                Host = context.AppSettings.SbBraille.SmarterBalancedFtpHost,
                Username = context.AppSettings.SbBraille.SmarterBalancedFtpUsername,
                Password = context.AppSettings.SbBraille.SmarterBalancedFtpPassword
            }))
            {
                await ftpClient.LoginAsync();
                var brailleFiles = await GetBrailleFileNames(ftpClient, itemDirectories, brailleCode);

                var memoryStream = new MemoryStream();
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (KeyValuePair<string, string> file in brailleFiles)
                    {
                        var entry = archive.CreateEntry(file.Key);
                        using (var ftpStream = await ftpClient.OpenFileReadStreamAsync(file.Value))
                        using (var entryStream = entry.Open())
                        {
                            ftpStream.CopyTo(entryStream);
                        }
                    }
                }

                memoryStream.Seek(0, SeekOrigin.Begin);
                return memoryStream;
            }

        }

        private ImmutableArray<string> GetItemBrailleDirectories(SampleItem item)
        {
            var associatedItems = GetAllAssociatedItems(item);
            associatedItems.Add(item);

            List<string> itemDirectories = associatedItems.Select(a => GetItemFtpDirectory(a)).ToList();

            if (item.AssociatedStimulus.HasValue)
            {
                itemDirectories.Add(GetPassageFtpDirectory(item));
            }

            return itemDirectories.Distinct().ToImmutableArray();
        }
    }
}
