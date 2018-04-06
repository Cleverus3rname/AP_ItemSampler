using System;
using System.Collections.Generic;
using System.Text;

namespace SmarterBalanced.SampleItems.Dal.Configurations.Models
{
    public class SbBrailleSettings
    {
        /// <summary>
        /// CSV File name for Braille Files to Items
        /// ex: braille-manifest.csv
        /// </summary>
        public string BrailleFileManifest { get; set; }

        /// <summary>
        /// Host url for the ftp site
        /// ex: ftps.smarterbalanced.org
        /// </summary>
        public string SmarterBalancedFtpHost { get; set; }

        /// <summary>
        /// Username for ftp site
        /// ex: anonymous
        /// </summary>
        public string SmarterBalancedFtpUsername { get; set; }

        /// <summary>
        /// Password for ftp site
        /// ex: guest
        /// </summary>
        public string SmarterBalancedFtpPassword { get; set; }

        /// <summary>
        /// Ftp path to Braille files 
        /// ex: /~sbacpublic/Public/PracticeAndTrainingTests/2016-2017_PracticeAndTrainingBrailleFiles
        /// </summary>
        public string FtpBraillePath { get; set; }

        /// <summary>
        /// Full path to Braille files
        /// </summary>
        public string FullPath
        {
            get
            {
                return $"{SmarterBalancedFtpHost}/{FtpBraillePath}/";
            }
        }
     
    }
}
