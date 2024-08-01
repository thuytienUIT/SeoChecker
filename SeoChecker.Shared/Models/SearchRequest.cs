using System.ComponentModel.DataAnnotations;

namespace SeoChecker.Shared.Models
{
	public class SearchRequest
	{
        [Required(ErrorMessage = "Keywords are required.")]
        [StringLength(100, ErrorMessage = "Keywords length can't be more than 100.")]
        public string Keywords { get; set; }

        [Required(ErrorMessage = "URL is required.")]
        public string Url { get; set; }

        public SearchRequest(string keywords, string url)
        {
            Keywords = keywords;
            Url = url;
        }
	}
}

