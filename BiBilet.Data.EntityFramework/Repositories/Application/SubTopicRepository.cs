using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BiBilet.Domain.Entities.Application;
using BiBilet.Domain.Repositories.Application;

namespace BiBilet.Data.EntityFramework.Repositories.Application
{
    /// <summary>
    /// Entity framework implementation of <see cref="ISubTopicRepository" />
    /// </summary>
    public class SubTopicRepository : Repository<SubTopic>, ISubTopicRepository
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public SubTopicRepository(BiBiletContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Returns a list of sub topics
        /// </summary>
        /// <param name="topicId"></param>
        /// <returns>A list of <see cref="SubTopic"/></returns>
        public List<SubTopic> GetSubTopics(Guid topicId)
        {
            return Set.Where(s => s.TopicId == topicId).ToList();
        }

        /// <summary>
        /// Asynchronously returns a list of sub topics
        /// </summary>
        /// <param name="topicId"></param>
        /// <returns>A list of <see cref="SubTopic"/></returns>
        public Task<List<SubTopic>> GetSubTopicsAsync(Guid topicId)
        {
            return Set.Where(s => s.TopicId == topicId).ToListAsync();
        }

        /// <summary>
        /// Asynchronously returns a list of sub topics
        /// with cancellation support
        /// </summary>
        /// <param name="topicId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>A list of <see cref="SubTopic"/></returns>
        public Task<List<SubTopic>> GetSubTopicsAsync(Guid topicId, CancellationToken cancellationToken)
        {
            return Set.Where(s => s.TopicId == topicId).ToListAsync(cancellationToken);
        }
    }
}