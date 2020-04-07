using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using RapidCMS.Core.Abstractions.Data;

namespace SharpNinja.UI.PersonalCms.Data
{
    public class Video : IEntity, ICloneable
    {
        public string Id { get; set; }

        public string VideoName { get; set; }
        public string VideoUrl { get; set; }

        public object Clone()
        {
            return new Video
            {
                Id = Id,
                VideoName = VideoName,
                VideoUrl = VideoUrl
            };
        }
    }
}
