using Axis.Luna.Common;
using Axis.Sigma.Policy;
using System;

namespace Axis.Sigma.Utils
{
    public static class Extensions
    {
        public static Effect Flip(this Effect effect)
        {
            switch(effect)
            {
                case Effect.Deny: return Effect.Grant;
                case Effect.Grant: return Effect.Deny;
                default: throw new Exception($"Unknown effect: {effect}");
            }
        }

        public static bool IsStringAttributeType(this CommonDataType dataType)
        {
            switch(dataType)
            {
                case CommonDataType.Email: 
                case CommonDataType.Guid:
                case CommonDataType.IPV4:
                case CommonDataType.IPV6:
                case CommonDataType.JsonObject:
                case CommonDataType.Location:
                case CommonDataType.Phone:
                case CommonDataType.String:
                case CommonDataType.NVP:
                case CommonDataType.UnknownType:
                case CommonDataType.Url: return true;
                default: return false;
            }
        }
    }
}
