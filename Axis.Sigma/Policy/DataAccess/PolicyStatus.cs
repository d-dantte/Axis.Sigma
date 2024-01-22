namespace Axis.Sigma.Policy.DataAccess
{
    /// <summary>
    /// Policy status
    /// </summary>
    public enum PolicyStatus
    {
        /// <summary>
        /// Policy is active and will participate in enforcement
        /// </summary>
        Active,

        /// <summary>
        /// Policy is suspended, temporarily barred from enforcement
        /// </summary>
        Suspended,

        /// <summary>
        /// Permanently barred from enforcement
        /// </summary>
        Defunct
    }
}
