﻿using SiraUtil.Logging;

namespace SaberFactory.Configuration
{
    public class TrailConfig : ConfigBase
    {
        public int Granularity { get; set; } = 70;

        public int SamplingFrequency { get; set; } = 90;

        public bool OnlyUseVertexColor { get; set; } = true;

        public TrailConfig(PluginDirectories pluginDirs, SiraLog logger) : base(pluginDirs, logger, "TrailConfig.json")
        { }
    }
}