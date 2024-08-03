namespace BetaBank.Utils.Enums
{
    public enum UserActionType
    {
        // General actions
        Get = 1,         // Example: Viewed an item
        Viewed,          // Example: Viewed an item
        Searched,        // Example: Searched for something

        // Authentication actions
        Logined,         // Example: User logged in
        Logouted,        // Example: User logged out

        // Content modification actions
        Created,         // Example: Created a new item
        Edited,          // Example: Edited an existing item
        Deleted,         // Example: Deleted an item
        Submitted,       // Example: Submitted a form or data
        Passed,          // Example: Passed an item (e.g., review passed)
        Send,            // Example: Sent a message or notification
        Answered,
        AttemptedEdit,

        // Subscription actions
        MakeSubscribed,      // Example: Subscribed to a service or newsletter
        MakeUnsubscribed,    // Example: Unsubscribed from a service or newsletter

        // Moderation actions
        Banned,          // Example: Banned a user
        Unbanned,        // Example: Unbanned a user
        Disabled,        // Example: Disabled a feature or user
        Suspended        // Example: Suspended a user or service
    }
}
