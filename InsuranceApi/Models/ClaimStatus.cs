namespace InsuranceApi.Models
{
    public enum ClaimStatus
    {
        Pending = 1,
        RecommendedForApproval,
        RecommendedForRejection,
        Approved,
        Rejected
    }
}
