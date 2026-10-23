namespace NutriSubs.Data.Entities
{
    // Confidence in the substitution recommendation. Only "Verified" results are
    // returned by default lookups (see SubstitutionLookupService).
    public enum SubstitutionVerificationStatus
    {
        Unverified = 0,
        Verified = 1
    }
}
