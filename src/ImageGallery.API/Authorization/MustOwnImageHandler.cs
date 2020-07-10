using ImageGallery.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageGallery.API.Authorization
{
    public class MustOwnImageHandler : AuthorizationHandler<MustOwnImageRequirement>
    {
        private IGalleryRepository _galleryRepository;
        private HttpContextAccessor _httpContextAccessor;

        public MustOwnImageHandler(HttpContextAccessor httpContextAccessor, IGalleryRepository galleryRepository)
        {
            _galleryRepository = galleryRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MustOwnImageRequirement requirement)
        {
            var image = _httpContextAccessor.HttpContext.GetRouteValue("id").ToString();
            if (!Guid.TryParse(image, out Guid imageAsGuid))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var ownerID = context.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;

            if(!_galleryRepository.IsImageOwner(imageAsGuid, ownerID))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;

        }
    }
}
