namespace EA.Weee.Web.ViewModels.Shared
{
    using System.ComponentModel.DataAnnotations;

    public enum NoteUpdatedStatusEnum
    {
        Draft = 1,
        Submitted = 2,
        Approved = 3,
        Rejected = 4,
        Void = 5,
        Returned = 6,
        ReturnedSaved = 7
    }
}