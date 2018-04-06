using CoreFtp;
using CsvHelper;
using SmarterBalanced.SampleItems.Dal.Configurations.Models;
using SmarterBalanced.SampleItems.Dal.Providers.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SmarterBalanced.SampleItems.Dal.Translations
{
    public class BrailleManifestReader
    {
        /// <summary>
        /// Reads the braille file manifest
        /// </summary>
        /// <param name="brailleManifest">Path to the braille manifest</param>
        /// <returns></returns>
        public static async Task<ImmutableArray<BrailleFileInfo>> GetBrailleFileInfo(AppSettings settings)
        {
            string brailleManifest = $"{settings.SbBraille.FtpBraillePath}/{settings.SbBraille.BrailleFileManifest}";
            using (var ftpClient = new FtpClient(new FtpClientConfiguration
            {
                Host = settings.SbBraille.SmarterBalancedFtpHost,
                Username = settings.SbBraille.SmarterBalancedFtpUsername,
                Password = settings.SbBraille.SmarterBalancedFtpPassword
            }))
            {
                await ftpClient.LoginAsync();
                List<BrailleFileInfo> fileInfo = new List<BrailleFileInfo>();
                using (var inputStream = await ftpClient.OpenFileReadStreamAsync(brailleManifest))
                using (var reader = new StreamReader(inputStream))
                using (var csv = new CsvReader(reader))
                {
                    csv.ReadHeader();
                    while (csv.Read())
                    {
                        var subject = csv.GetField<string>("Subject");
                        var grade = csv.GetField<int>("Grade");
                        var itemId = csv.GetField<int>("ItemOrStimId");
                        var brailleType = csv.GetField<string>("UebCode");
                        fileInfo.Add(
                            new BrailleFileInfo(
                                itemKey: itemId,
                                subject: subject,
                                grade: GradeLevelsUtils.FromString(grade.ToString()),
                                brailleType: $"TDS_BT_{brailleType}"
                            )
                        );
                    }
                }
                return fileInfo.ToImmutableArray();
            }

        }
    }
}
