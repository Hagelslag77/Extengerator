// using Zenject;
//
// namespace Api.OGame.ResponseFilters
// {
//     public static class ResponseFilterReceiverExtensionsXXX
//     {
//         public static void AddFilters(this IResponseFilterReceiver extend, DiContainer container)
//         {
//             extend.AddResponseFilterLast(container.Instantiate<RateLimitedFilter>());
//             extend.AddResponseFilterLast(container.Instantiate<UserBannedFilter>());
//         }
//     }
// }