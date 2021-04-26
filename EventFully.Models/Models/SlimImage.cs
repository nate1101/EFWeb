using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFully.Models
{
    public partial class SlimImage
    {
        [JsonProperty("server")]
        public string Server { get; set; }
        [JsonProperty("input")]
        public ImageInput Input { get; set; }
        [JsonProperty("output")]
        public ImageOutput Output { get; set; }
        [JsonProperty("actions")]
        public ImageAction Actions { get; set; }
    }

    public partial class ImageInput
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("size")]
        public int Size { get; set; }
        [JsonProperty("width")]
        public int Width { get; set; }
        [JsonProperty("height")]
        public int Height { get; set; }
        [JsonProperty("field")]
        public string Field { get; set; }
    }

    public partial class ImageOutput
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("size")]
        public int Size { get; set; }
        [JsonProperty("width")]
        public decimal Width { get; set; }
        [JsonProperty("height")]
        public decimal Height { get; set; }
        [JsonProperty("image")]
        public string Image { get; set; }
    }

    public partial class ImageAction
    {
        [JsonProperty("rotation")]
        public int? Rotation { get; set; }
        [JsonProperty("crop")]
        public Crop Crop { get; set; }
        [JsonProperty("size")]
        public Size Size { get; set; }
        [JsonProperty("filters")]
        public Filters Filters { get; set; }
        [JsonProperty("minSize")]
        public MinSize MinSize { get; set; }
    }

    public partial class Crop
    {
        [JsonProperty("x")]
        public decimal X { get; set; }
        [JsonProperty("y")]
        public decimal Y { get; set; }
        [JsonProperty("height")]
        public decimal Height { get; set; }
        [JsonProperty("width")]
        public decimal Width { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public partial class Size
    {
        [JsonProperty("height")]
        public decimal Height { get; set; }
        [JsonProperty("width")]
        public decimal Width { get; set; }
    }

    public partial class Filters
    {
        [JsonProperty("sharpen")]
        public int Sharpen { get; set; }
    }

    public partial class MinSize
    {
        [JsonProperty("height")]
        public int Height { get; set; }
        [JsonProperty("width")]
        public int Width { get; set; }
    }
}
