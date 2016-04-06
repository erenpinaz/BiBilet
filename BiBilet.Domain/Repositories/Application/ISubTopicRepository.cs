using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BiBilet.Domain.Entities.Application;

namespace BiBilet.Domain.Repositories.Application
{
    /// <summary>
    /// Repository interface for <see cref="SubTopic" />
    /// </summary>
    public interface ISubTopicRepository : IRepository<SubTopic>
    {
        /// <summary>
        /// Returns a list of sub topics
        /// </summary>
        /// <param name="topicId"></param>
        /// <returns>A list of <see cref="SubTopic" /></returns>
        List<SubTopic> GetSubTopics(Guid topicId);

        /// <summary>
        /// Asynchronously returns a list of sub topics
        /// </summary>
        /// <param name="topicId"></param>
        /// <returns>A list of <see cref="SubTopic" /></returns>
        Task<List<SubTopic>> GetSubTopicsAsync(Guid topicId);

        /// <summary>
        /// Asynchronously returns a list of sub topics
        /// with cancellation support
        /// </summary>
        /// <param name="topicId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>A list of <see cref="SubTopic" /></returns>
        Task<List<SubTopic>> GetSubTopicsAsync(Guid topicId, CancellationToken cancellationToken);
    }
}