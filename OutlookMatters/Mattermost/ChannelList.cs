﻿using System.Collections.Generic;
using Newtonsoft.Json;
namespace OutlookMatters.Mattermost
{
    public class ChannelList
    {
        [JsonProperty("channels")]
        public List<Channel> Channels { get; set; }
    }

    public class Channel
    {
        [JsonProperty("id")]
        public string ChannelId { get; set; }

        [JsonProperty("display_name")]
        public string ChannelName { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}