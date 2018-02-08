using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmarterBalanced.SampleItems.Dal.Providers.Models
{
    public class Target
    {
        /// <summary>
        /// Short name of Target, parsed out of description
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Description of Target without short name
        /// </summary>
        public string Descripton { get; }

        /// <summary>
        /// Example: 10-5
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Example: 10
        /// </summary>
        public string IdLabel { get; }
        public string Subject { get; }
        public string ClaimId { get; }

        /// <summary>
        /// Stores hash of name once it has been computed
        /// </summary>
        public int NameHash { get; }

        /// <param name="description">Description with short name</param>
        public Target(
            string description,
            string id, 
            string idLabel,
            string subjectCode,
            string claimId)
        {
            string name = NameFromDesc(description);
            name = String.IsNullOrEmpty(name) || subjectCode == "MATH"
                ? idLabel ?? String.Empty
                : $"{idLabel}. {name}";
            Name = name;
            Descripton = RemoveNameFromDescription(description);
            Id = id;
            IdLabel = idLabel;
            Subject = subjectCode;
            ClaimId = claimId;
            NameHash = GetHashCode();
        }

        /// <summary>
        /// Creates a new target with the same Ids but given Description (and therefore Name).
        /// </summary>
        /// <param name="desc"></param>
        /// <returns></returns>
        public Target WithDescription(string desc)
        {
            return new Target(
                description: desc,
                id: Id,
                idLabel: IdLabel,
                subjectCode: Subject,
                claimId: ClaimId);
        }

        /// <summary>
        /// Used to compare targets with identical Names.
        /// </summary>
        /// <returns>The target name encoded in a hash.</returns>
        public override int GetHashCode()
        {
            var hash = 0;
            for (int i = 0; i < Name.Length; i++)
            {
                hash = (hash + Name[i] * (i + 1)) % 999999; 
            }
            return hash;
        }

        /// <summary>
        /// Shorthand method for creating a Target. Used in testing. 
        /// </summary>
        /// <returns>Target with unspecified parameters as empty strings.</returns>
        public static Target Create(
            string desc = "",
            string id = "",
            string idLabel = "",
            string subject = "",
            string claim = "")
        {
            return new Target(
                description: desc,
                id: id,
                idLabel: idLabel,
                subjectCode: subject,
                claimId: claim);
        }

        /// <summary>
        /// Get the target's short name from the target description string.
        /// </summary>
        /// <param name="targetDesc">The target description</param>
        /// <returns>Target's short name or an empty string</returns>
        private string NameFromDesc(string targetDesc)
        {
            if (!string.IsNullOrEmpty(targetDesc) && targetDesc.Contains(':'))
            {
                int colonLocation = targetDesc.IndexOf(':');
                string shortName = targetDesc.Substring(0, colonLocation);
                shortName = String.Join(" ",
                    shortName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(word => char.ToUpper(word[0]) + word.Substring(1).ToLower()));
                return shortName;
            }
            return null;
        }

        /// <summary>
        /// Remove the short name from the given target description
        /// </summary>
        /// <param name="targetDesc"></param>
        /// <returns>Target description as a string without short name</returns>
        private string RemoveNameFromDescription(string targetDesc)
        {
            if (!string.IsNullOrEmpty(targetDesc) && targetDesc.Contains(':'))
            {
                int colonLocation = targetDesc.IndexOf(':');
                if (targetDesc.Length >= colonLocation + 2)
                {
                    return targetDesc.Substring(colonLocation + 2);
                }
            }
            return targetDesc;
        }
    }
}
