using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PassportInfoExtractor.Data.Models
{
    public class Passport
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Guid { get; set; }

        // Document Number (Passport Number)
        public string DocumentNumber { get; set; }
        public double DocumentNumberConfidence { get; set; }

        // First Name (Given Name)
        public string FirstName { get; set; }
        public double FirstNameConfidence { get; set; }

        // Middle Name
        public string MiddleName { get; set; }
        public double MiddleNameConfidence { get; set; }
        public bool IsMiddleNameDerived { get; set; }

        // Last Name (Surname)
        public string LastName { get; set; }
        public double LastNameConfidence { get; set; }

        // Aliases (Also known as)
        public List<string> Aliases { get; set; }
        public double AliasesConfidence { get; set; }

        // Date of Birth
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime DateOfBirth { get; set; }
        public double DateOfBirthConfidence { get; set; }

        // Date of Expiration
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime DateOfExpiration { get; set; }
        public double DateOfExpirationConfidence { get; set; }

        // Date of Issue
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime DateOfIssue { get; set; }
        public double DateOfIssueConfidence { get; set; }

        // Sex
        public string Sex { get; set; }
        public double SexConfidence { get; set; }

        // Country/Region (Issuing country or organization)
        public string CountryRegion { get; set; }
        public double CountryRegionConfidence { get; set; }

        // Document Type
        public string DocumentType { get; set; }
        public double DocumentTypeConfidence { get; set; }

        // Nationality
        public string Nationality { get; set; }
        public double NationalityConfidence { get; set; }

        // Place of Birth
        public string PlaceOfBirth { get; set; }
        public double PlaceOfBirthConfidence { get; set; }

        // Place of Issue
        public string PlaceOfIssue { get; set; }
        public double PlaceOfIssueConfidence { get; set; }

        // Issuing Authority
        public string IssuingAuthority { get; set; }
        public double IssuingAuthorityConfidence { get; set; }

        // Personal Number
        public string PersonalNumber { get; set; }
        public double PersonalNumberConfidence { get; set; }

        // Machine Readable Zone (MRZ)
        public string MachineReadableZone { get; set; }
        public double MachineReadableZoneConfidence { get; set; }

        // Additional fields from the original model
        public string Address { get; set; }
        public double AddressConfidence { get; set; }

        public string Notes { get; set; }
        public string GroupId { get; set; }

        // Overall accuracy of the extracted information
        public Dictionary<string, double> Accuracy { get; set; } = new Dictionary<string, double>();

        // Name of the uploaded file
        public string FileName { get; set; }
    }
}