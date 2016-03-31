﻿using System;
using Newtonsoft.Json;
using OutlookMatters.Http;
using OutlookMatters.Mattermost.DataObjects;

namespace OutlookMatters.Mattermost.Session
{
    public class UserSession : ISession
    {
        private readonly Uri _baseUri;
        private readonly IHttpClient _httpClient;
        private readonly string _token;
        private readonly string _userId;

        public UserSession(Uri baseUri, string token, string userId, IHttpClient httpClient)
        {
            _baseUri = baseUri;
            _token = token;
            _userId = userId;
            _httpClient = httpClient;
        }

        public void CreatePost(string channelId, string message, string rootId = "")
        {
            try
            {
                var post = new Post {channel_id = channelId, message = message, user_id = _userId, root_id = rootId};
                var postUrl = PostUrl(channelId);
                using (_httpClient.Request(postUrl)
                    .WithContentType("text/json")
                    .WithHeader("Authorization", "Bearer " + _token)
                    .Post(JsonConvert.SerializeObject(post)))
                {
                }
            }
            catch (HttpException hex)
            {
                throw TranslateException(hex);
            }
        }

        private static MattermostException TranslateException(HttpException hex)
        {
            var error = JsonConvert.DeserializeObject<DataObjects.Error>(hex.Response.GetPayload());
            var exception = new MattermostException(error);
            return exception;
        }

        public Post GetPostById(string postId)
        {
            try
            {
                var postUrl = "api/v1/posts/" + postId;
                var url = new Uri(_baseUri, postUrl);
                using (var response = _httpClient.Request(url)
                    .WithHeader("Authorization", "Bearer " + _token)
                    .Get())
                {
                    var thread = JsonConvert.DeserializeObject<Thread>(response.GetPayload());
                    return thread.posts[postId];
                }
            }
            catch (HttpException hex)
            {
                throw TranslateException(hex);
            }
        }

        private Uri PostUrl(string channelId)
        {
            var postUrl = "api/v1/channels/" + channelId + "/create";

            var url = new Uri(_baseUri, postUrl);
            return url;
        }
    }
}