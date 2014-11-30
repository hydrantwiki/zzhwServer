using HydrantWiki.Library.Helpers;

namespace Site.JsonObjects
{
    public class Tag
    {
        public string Location { get; set; }
        public string TagDateTime { get; set; }
        public string Thumbnail { get; set; }
        public string Status { get; set; }

        public Tag(HydrantWiki.Library.Objects.Tag _tag,
                   bool _showViewMapButton = true,
                   bool _allowViewFullImage = true)
        {
            TagDateTime = _tag.DeviceDateTime.ToString("G");
            Status = _tag.Status;

            if (_allowViewFullImage)
            {
                Thumbnail = string.Format("<img src=\"{0}\" class=\"tagimg\" onclick=\"ShowImage('{1}');\">", _tag.GetUrl(true), _tag.GetUrl(false));
            }
            else
            {
                Thumbnail = string.Format("<img src=\"{0}\" class=\"tagimg\" >", _tag.GetUrl(true));
            }

            if (_tag.Position != null)
            {
                if (_showViewMapButton)
                {
                    Location = string.Format("Latitude: {0}<br>Longitude: {1}<br><button type=\"button\" class=\"btn btn-info\" onclick=\"TagMap('{2}')\">View</button>",
                        _tag.Position.Y.ToString("###.######"), 
                        _tag.Position.X.ToString("###.######"),
                        _tag.Guid);
                }
                else
                {
                    Location = string.Format("Latitude: {0}<br>Longitude: {1}",
                        _tag.Position.Y.ToString("###.######"),
                        _tag.Position.X.ToString("###.######"));
                }
            }
        }
    }
}