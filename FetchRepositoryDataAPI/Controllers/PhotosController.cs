using FetchRepositoryDataAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using HtmlAgilityPack;

namespace FetchRepositoryDataAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PhotosController : ControllerBase
    {
        [HttpGet(Name = "GetTitles")]
        public async Task<List<Item>> Get()
        {
            List<Item> items = await GetData();
            return items;
        }

        private async Task<List<Item>> GetData()
        {
            string repoUrl = "https://github.com/SimonaRa20/art";
            string rawUrlPrefix = "https://raw.githubusercontent.com/SimonaRa20/art/main/";

            List<Item> images = await GetImageUrlsFromRepo(repoUrl, rawUrlPrefix);
            return images;
        }

        private async Task<List<Item>> GetImageUrlsFromRepo(string repoUrl, string rawUrlPrefix)
        {
            var images = new List<Item>();
            var titlesSet = new HashSet<string>();
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(repoUrl);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var nodes = htmlDoc.DocumentNode.SelectNodes("//a[@class='Link--primary']");
            foreach (var node in nodes)
            {
                if (node.Attributes["title"] != null && node.Attributes["href"] != null)
                {
                    var title = node.Attributes["title"].Value;
                    if ((title.EndsWith(".jpg") || title.EndsWith(".png") || title.EndsWith(".jpeg")) && !titlesSet.Contains(title))
                    {
                        Item item = new Item();
                        item.Title = title;
                        item.Link = rawUrlPrefix + title;
                        images.Add(item);
                        titlesSet.Add(title);
                    }
                }
            }

            return images;
        }
    }
}
