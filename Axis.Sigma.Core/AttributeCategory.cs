namespace Axis.Sigma.Core
{
    public enum AttributeCategory
    {
        /// <summary>
        /// Represents attributes about the principal on who's behalf access is being granted
        /// </summary>
        Subject,

        /// <summary>
        /// Represents the intended ACTION to be performed on a specified RESOURCE
        /// </summary>
        Intent,

        /// <summary>
        /// Represents attributes of the CONTEXT/ENVIRONMENT within which the PRINCIPAL is trying to PERFORM an ACTION on a given RESOURCE
        /// </summary>
        Environment
    }
}
