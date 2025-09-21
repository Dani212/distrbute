using App.Distrbute.Api.Common.Dtos.Post;
using App.Distrbute.Api.Common.Services.Interfaces;
using App.Distrbute.Common.Models;
using Persistence.Sdk.Core.Interfaces;
using Socials.Sdk.Dtos;

namespace App.Distrbute.Api.Common.Services.Providers;

public class PostValuationWriter : IPostValuationWriter
{
    private readonly IDbRepository _dbRepository;

    public PostValuationWriter(IDbRepository dbRepository)
    {
        _dbRepository = dbRepository;
    }

    public async Task<SocialProfile> WriteAsync(ValuedPost req)
    {
        var valuation = req.Valuation;
        var socialPost = req.SocialPost;
        var pseudoPost = CreatePseudoPost(req, socialPost);
        
        await using var optimisticLock = await _dbRepository.OptimisticTransactionAsync();
        try
        {
            var savedValuation = await _dbRepository.AddAsync(valuation);
            
            pseudoPost.PostValuation = savedValuation;
            
            await _dbRepository.UpsertManyAsync([pseudoPost], conflictKeySelector: p => p.ExternalPostId);

            await optimisticLock.CommitAsync();

            return req.SocialProfile;
        }
        catch (Exception)
        {
            await optimisticLock.RollbackAsync();
            throw;
        }
    }

    public async Task<SocialProfile> WriteManyAsync(ValuedPosts req)
    {
        if (!req.SocialPostsWithValuations.Any())
        {
            return req.SocialProfile;
        }
        
        var valuations = new List<PostValuation>();
        var pseudoPosts = new List<Post>();
        
        foreach (var postWithValuation in req.SocialPostsWithValuations)
        {
            var socialPost = postWithValuation.Key;
            var valuation = postWithValuation.Value;
            valuation.Id = Guid.NewGuid().ToString(); // provide own id so we don't have to call db
            
            var pseudoPost = CreatePseudoPost(req, socialPost);
            pseudoPost.PostValuation = valuation;
            
            valuations.Add(valuation);
            pseudoPosts.Add(pseudoPost);
        }
        
        await using var optimisticLock = await _dbRepository.OptimisticTransactionAsync();
        try
        {
            await _dbRepository.AddManyAsync(valuations);
            
            await _dbRepository.UpsertManyAsync(pseudoPosts, conflictKeySelector: p => p.ExternalPostId);

            await optimisticLock.CommitAsync();

            return req.SocialProfile;
        }
        catch (Exception)
        {
            await optimisticLock.RollbackAsync();
            throw;
        }
    }
    
    private static Post CreatePseudoPost(TrackPostBase req, SocialPost socialPost)
    {
        var pseudoPost = new Post();
        pseudoPost.DistributorSocialAccount = req.DistributorSocialAccount;
        pseudoPost.BrandSocialAccount = req.BrandSocialAccount;
        pseudoPost.ExternalPostId = socialPost.Id;
        pseudoPost.Embedding = socialPost.Embedding;
        pseudoPost.Link = socialPost.Link;
        pseudoPost.ContentType = socialPost.ContentType;
        pseudoPost.PostedAt = socialPost.CreatedAt;
        
        return pseudoPost;
    }
}