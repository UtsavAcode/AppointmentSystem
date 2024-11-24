using AppointmentSystem.Models.ViewModel;
using AppointmentSystem.Service.Implementation;
using AppointmentSystem.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentSystem.Controllers
{
    public class PostController : Controller
    {
        private readonly IPostService _postService;
        private readonly IOfficerService _officerService;

        public PostController(IPostService postService, IOfficerService officerService)
        {
            _postService = postService;
            _officerService = officerService;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var posts = await _postService.GetAllPostsAsync();
            return View(posts);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PostViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Post model is null");
            }

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                ModelState.AddModelError(nameof(model.Name), "Name is required");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }


            model.Status = true;

            await _postService.CreatePostAsync(model);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id, bool status)
        {
            try
            {
                var post = await _postService.GetPostByIdAsync(id);
                if (post == null)
                {
                    return NotFound("Post not found.");
                }

                var activeOfficers = await _officerService.GetActiveOfficersByPostIdAsync(post.Id);
                if (activeOfficers.Any() && !status)
                {
                    return BadRequest("Cannot deactivate the post. There are active officers assigned to it.");
                }

                post.Status = status;
                await _postService.UpdatePostAsync(post);
                return Ok("Post status updated successfully.");
            }
            catch (Exception ex)
            {

                return StatusCode(500, "An error occurred while updating the status.");
            }
        }




        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _postService.GetPostByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PostViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {

                var existingPost = await _postService.GetPostByIdAsync(model.Id);
                if (existingPost == null)
                {
                    return NotFound();
                }


                existingPost.Name = model.Name;


                await _postService.UpdatePostAsync(existingPost);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating the post.");
                return View(model);
            }
        }





        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var post = await _postService.GetPostByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _postService.GetPostByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _postService.DeletePostAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", "An error occurred while deleting the post.");
                return RedirectToAction(nameof(Index));
            }
        }


        public async Task<IActionResult> DeactivatePost(int postId)
        {
            var result = await _postService.DeactivatePostAsync(postId);

            if (result.success)
            {

                return RedirectToAction(nameof(Index));
            }
            else
            {

                ViewData["Error"] = result.message;
                return View("Details", await _postService.GetPostByIdAsync(postId));
            }
        }

    }
}
