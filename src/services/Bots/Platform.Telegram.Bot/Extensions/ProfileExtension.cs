using System;
using System.Collections.Generic;
using System.Linq;
using Platform.Contract;
using Platform.Contract.Abstractions;
using Platform.Primitive;

namespace Platform.Telegram.Bot.Extensions
{
    public static class ProfileExtension
    {
        public static IEnumerable<TProfile> MakeProfiles<TProfile>(this TargetModel targetModel, TraceContext sessionContext)
            where TProfile : ITargetProfile, IToolProfile, new() =>
            targetModel.Targets.Select(target => new TProfile { Target = target, TraceContext = sessionContext, Tools = Array.Empty<string>() });
    }
}