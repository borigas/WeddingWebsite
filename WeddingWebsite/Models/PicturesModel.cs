using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace WeddingWebsite.Models
{
    public class PicturesModel
    {
        public List<PictureModel> PictureModels { get; set; }
    }

    public class PictureModel
    {
        public List<PictureMetadata> PictureMetadatas { get; set; }

        public string DefaultPath
        {
            get { return PictureMetadatas.First().RelativePath; }
        }

        public string SrcSetPaths
        {
            get
            {
                var pathsAndSizes = PictureMetadatas.Select(p => p.RelativePath + " " + p.Width + "w");
                return string.Join(", ", pathsAndSizes);
            }
        }
    }

    public class PictureMetadata
    {
        public PictureMetadata(string fullPath, string relativePath)
        {
            FullPath = fullPath;
            RelativePath = relativePath;
            using (var image = Image.FromFile(fullPath))
            {
                Width = image.Width;
                Height = image.Height;
            }
        }

        public float Width { get; set; }
        public float Height { get; set; }
        public string FullPath { get; set; }
        public string RelativePath { get; set; }
    }
}