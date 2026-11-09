using Abp.Dependency;
using Abp.ObjectMapping;
using AutoMapper;
using Eaf.Middleware.Chat;
using Eaf.Middleware.Chat.Dto;
using Eaf.Middleware.Friendships;
using Eaf.Middleware.Friendships.Cache;
using Eaf.Middleware.Friendships.Dto;
using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Eaf.Middleware.Mapping
{
    /// <summary>
    /// Source-generated Mapperly mapper for Middleware.Core DTOs.
    /// Replaces the runtime reflection-based AutoMapper profile for this module.
    /// </summary>
    [Mapper]
    public partial class MiddlewareCoreMapperly
    {
        /// <summary>Maps a chat message to its DTO. App-layer-only members are left default.</summary>
        [MapperIgnoreTarget(nameof(ChatMessageDto.TargetUserName))]
        public partial ChatMessageDto ToDto(ChatMessage source);

        /// <summary>Maps a friendship to its DTO. Enriched members are left default.</summary>
        [MapperIgnoreSource(nameof(Friendship.Id))]
        [MapperIgnoreSource(nameof(Friendship.CreationTime))]
        [MapperIgnoreSource(nameof(Friendship.TenantId))]
        [MapperIgnoreSource(nameof(Friendship.UserId))]
        [MapperIgnoreTarget(nameof(FriendshipDto.Name))]
        [MapperIgnoreTarget(nameof(FriendshipDto.Surname))]
        [MapperIgnoreTarget(nameof(FriendshipDto.Email))]
        [MapperIgnoreTarget(nameof(FriendshipDto.IsOnline))]
        [MapperIgnoreTarget(nameof(FriendshipDto.UnreadMessageCount))]
        [MapperIgnoreTarget(nameof(FriendshipDto.GroupId))]
        public partial FriendshipDto ToDto(Friendship source);

        /// <summary>Maps a friendship to its cache item.</summary>
        [MapperIgnoreSource(nameof(Friendship.Id))]
        [MapperIgnoreSource(nameof(Friendship.CreationTime))]
        [MapperIgnoreSource(nameof(Friendship.TenantId))]
        [MapperIgnoreSource(nameof(Friendship.UserId))]
        [MapperIgnoreTarget(nameof(FriendCacheItem.Name))]
        [MapperIgnoreTarget(nameof(FriendCacheItem.Surname))]
        [MapperIgnoreTarget(nameof(FriendCacheItem.Email))]
        [MapperIgnoreTarget(nameof(FriendCacheItem.UnreadMessageCount))]
        public partial FriendCacheItem ToCacheItem(Friendship source);

        /// <summary>Maps a friend cache item to its DTO.</summary>
        [MapperIgnoreTarget(nameof(FriendshipDto.IsOnline))]
        [MapperIgnoreTarget(nameof(FriendshipDto.GroupId))]
        public partial FriendshipDto ToDto(FriendCacheItem source);

        /// <summary>Maps a list of chat messages.</summary>
        public partial List<ChatMessageDto> ToDtoList(List<ChatMessage> source);

        /// <summary>Maps a list of friendships.</summary>
        public partial List<FriendshipDto> ToDtoList(List<Friendship> source);

        /// <summary>Maps a list of friend cache items.</summary>
        public partial List<FriendshipDto> ToDtoList(List<FriendCacheItem> source);
    }

    /// <summary>
    /// <see cref="IObjectMapper"/> adapter that dispatches mapped pairs to the
    /// source-generated Mapperly mappers and falls back to the registered AutoMapper
    /// <see cref="IMapper"/> for every other pair during the migration.
    /// Modules contribute pairs via <see cref="Register{TSource,TDestination}"/>.
    /// </summary>
    public class EafMapperlyObjectMapper : IObjectMapper
    {
        private static readonly Dictionary<(Type Source, Type Destination), Func<object, object>> Dispatch = new();

        /// <summary>
        /// Registers a generated mapping function for a source/destination pair.
        /// Called by modules during <c>Initialize()</c>; thread-safe for startup use.
        /// </summary>
        public static void Register<TSource, TDestination>(Func<TSource, TDestination> map)
        {
            lock (Dispatch)
            {
                Dispatch[(typeof(TSource), typeof(TDestination))] = s => map((TSource)s);
            }
        }

        private readonly IMapper _autoMapper;

        /// <summary>
        /// EafMapperlyObjectMapper.
        /// </summary>
        public EafMapperlyObjectMapper(IMapper autoMapper)
        {
            _autoMapper = autoMapper;
        }

        /// <inheritdoc />
        public TDestination Map<TDestination>(object source)
        {
            if (source != null
                && Dispatch.TryGetValue((source.GetType(), typeof(TDestination)), out var map))
            {
                return (TDestination)map(source);
            }
            return _autoMapper.Map<TDestination>(source);
        }

        /// <inheritdoc />
        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            return _autoMapper.Map(source, destination);
        }

        /// <inheritdoc />
        public IQueryable<TDestination> ProjectTo<TDestination>(IQueryable source)
        {
            return _autoMapper.ProjectTo<TDestination>(source);
        }
    }
}
