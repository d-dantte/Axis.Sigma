using Axis.Luna.Common;
using Axis.Luna.Extensions;
using Axis.Sigma.Authority;
using Axis.Sigma.Authority.Attribute;
using Axis.Sigma.Exceptions;
using Axis.Sigma.Policy;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

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

        public static bool IsNull(this object obj) => obj == null;
        public static bool IsNotNull(this object obj) => obj != null;

        public static bool IsNull<V>(this V? v)
        where V : struct => !v.HasValue;

        public static bool IsNotNull<V>(this V? v)
        where V : struct => v.HasValue;

        //public static bool IsStringAttributeType(this CommonDataType dataType)
        //{
        //    switch(dataType)
        //    {
        //        case CommonDataType.Email: 
        //        case CommonDataType.Guid:
        //        case CommonDataType.IPV4:
        //        case CommonDataType.IPV6:
        //        case CommonDataType.JsonObject:
        //        case CommonDataType.Location:
        //        case CommonDataType.Phone:
        //        case CommonDataType.String:
        //        case CommonDataType.NVP:
        //        case CommonDataType.UnknownType:
        //        case CommonDataType.Url: return true;
        //        default: return false;
        //    }
        //}

        internal static bool ItemsEqual<TItem>(this
            ImmutableArray<TItem> first,
            ImmutableArray<TItem> second)
        {
            if (first.Length != second.Length)
                return false;

            var equalityComparer = EqualityComparer<TItem>.Default;
            for (int cnt = 0; cnt < first.Length; cnt++)
            {
                if (!equalityComparer.Equals(first[cnt], second[cnt]))
                    return false;
            }

            return true;
        }

        internal static int ItemHash<TItem>(this
            ImmutableArray<TItem> array)
            => array.Aggregate(0, HashCode.Combine);

        internal static IEnumerable<IAttribute> ActiveAttributes(this
            IAttributeCollectionEntity entity)
            => entity.Attributes.Where(att =>
                att.ValidUntil is null
                || DateTimeOffset.Now > att.ValidUntil.Value);

        internal static Task<TOut> Map<TIn, TOut>(this
            Task<TIn> task,
            Func<TIn, TOut> mapper)
        {
            ArgumentNullException.ThrowIfNull(task);
            ArgumentNullException.ThrowIfNull(mapper);

            return task.ContinueWith(t => t.Status switch
            {
                // note that if the status is not TaskStatus.RanToCompletion, the appropriate exception
                // is thrown
                TaskStatus.RanToCompletion
                or TaskStatus.Canceled
                or TaskStatus.Faulted => mapper.Invoke(t.Result),

                _ => throw new InvalidOperationException(
                    $"Invalid antecedent task status: {t.Status}")
            });
        }

        internal static Task<TOut> Map<TIn, TOut>(this
            Task<TIn> task,
            Func<TIn, Task<TOut>> mapper)
        {
            ArgumentNullException.ThrowIfNull(task);
            ArgumentNullException.ThrowIfNull(mapper);

            return task
                .ContinueWith(t => t.Status switch
                {
                    // note that if the status is not TaskStatus.RanToCompletion, the appropriate exception
                    // is thrown
                    TaskStatus.RanToCompletion
                    or TaskStatus.Canceled
                    or TaskStatus.Faulted => mapper.Invoke(t.Result),

                    _ => throw new InvalidOperationException(
                        $"Invalid antecedent task status: {t.Status}")
                })
                .Unwrap();
        }

        internal static IEnumerable<T> SelectMany<T>(this IEnumerable<IEnumerable<T>> sequenceOfSequence)
        {
            ArgumentNullException.ThrowIfNull(sequenceOfSequence);

            return sequenceOfSequence.SelectMany(t => t);
        }

        internal static Effect Combine(this IEnumerable<Effect> effects)
        {
            foreach(var effect in effects)
            {
                if(effect == Effect.Deny)
                    return Effect.Deny;
            }

            return Effect.Grant;
        }

        public static Task<TOut> OnGrant<TOut>(this
            Task<Effect> effectTask,
            Func<TOut> func)
        {
            ArgumentNullException.ThrowIfNull(effectTask);
            ArgumentNullException.ThrowIfNull(func);

            return effectTask.ContinueWith(t => t.Status switch
            {
                TaskStatus.RanToCompletion => t.Result switch
                {
                    Effect.Grant => func.Invoke(),
                    _ => throw new AccessDeniedException()
                },
                _ => t.Exception!.InnerExceptions.Count == 1
                    ? t.Exception.InnerException.Throw<TOut>()
                    : t.Exception.Throw<TOut>()
            });
        }

        public static Task OnGrant(this
            Task<Effect> effectTask,
            Action action)
        {
            ArgumentNullException.ThrowIfNull(effectTask);
            ArgumentNullException.ThrowIfNull(action);

            return effectTask.ContinueWith(t =>
            {
                if (t.Status == TaskStatus.RanToCompletion)
                {
                    if (t.Result == Effect.Grant)
                        action.Invoke();

                    else throw new AccessDeniedException();
                }
                else if (t.Exception!.InnerExceptions.Count == 1)
                    t.Exception.InnerException.Throw();

                else t.Exception.Throw();
            });
        }
    }
}
