using AppointmentSystem.Models.ViewModel;
using AppointmentSystem.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentSystem.Controllers
{
    public class PostController : Controller
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        // GET: /Post
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var posts = await _postService.GetAllPostsAsync();  // This returns IEnumerable<PostViewModel>
            return View(posts);  // Pass the collection to the view
        }

        // GET: /Post/Create
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

            // Force status to be true (active)
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
                    return NotFound(); // Post not found, return 404
                }

                post.Status = status;
                await _postService.UpdatePostAsync(post); // Ensure this method works properly
                return Ok(); // Status updated successfully
            }
            catch (Exception ex)
            {
                // Log the exception here if needed for debugging
                return StatusCode(500, "An error occurred while updating the status. " + ex.Message);
            }
        }



        // GET: /Post/Edit/{id}
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

        // POST: /Post/Edit
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
                // Get the existing post
                var existingPost = await _postService.GetPostByIdAsync(model.Id);
                if (existingPost == null)
                {
                    return NotFound();
                }

                // Update the model with the new name but keep the existing status
                existingPost.Name = model.Name;
                // Status remains unchanged as it's coming from the hidden field

                await _postService.UpdatePostAsync(existingPost);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating the post.");
                return View(model);
            }
        }




        // GET: /Post/Details/{id}
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

        // GET: /Post/Delete/{id}
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
                return RedirectToAction(nameof(Index)); // Redirect back to the post list
            }
            catch (Exception ex)
            {
                // Handle error (e.g., log it and return an error message to the user)
                ModelState.AddModelError("", "An error occurred while deleting the post.");
                return RedirectToAction(nameof(Index));
            }
        }


    }
}
