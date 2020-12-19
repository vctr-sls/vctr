using DatabaseAccessLayer.Models;

namespace Gateway.Controllers
{
    public interface ILinkController
    {
        public LinkModel ProcessedLink { get; set; }
    }
}
